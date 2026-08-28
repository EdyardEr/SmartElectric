using System.Collections.Generic;
using SmartElectric.Adapters;
using SmartElectric.Domain;
using SmartElectric.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SmartElectric.AR
{
    /// <summary>
    /// Phase 1: tap AR plane → anchor + device, linked to RoomModel wall/device.
    /// </summary>
    public sealed class ArDevicePlacer : MonoBehaviour
    {
        const string FallbackWallId = "ar_plane";

        [SerializeField] ARRaycastManager raycastManager;
        [SerializeField] ARAnchorManager anchorManager;
        [SerializeField] ProjectSession session;
        [SerializeField] GameObject outletPrefab;
        [SerializeField] GameObject panelPrefab;
        [SerializeField] bool preferVerticalPlanes = true;

        readonly List<ARRaycastHit> hits = new List<ARRaycastHit>(8);

        void Awake()
        {
            if (raycastManager == null)
                raycastManager = FindAnyObjectByType<ARRaycastManager>();
            if (anchorManager == null)
                anchorManager = FindAnyObjectByType<ARAnchorManager>();
            if (session == null)
                session = FindAnyObjectByType<ProjectSession>();

            if (raycastManager == null)
            {
                Debug.LogError("[SmartElectric] ArDevicePlacer: ARRaycastManager missing.", this);
                enabled = false;
            }
        }

        void Update()
        {
            if (!TryGetPressScreenPosition(out var screenPos))
                return;
            if (ProjectDebugHud.BlocksPlacement(screenPos))
                return;
            TryPlace(screenPos);
        }

        bool TryGetPressScreenPosition(out Vector2 screenPos)
        {
            screenPos = default;
            var touch = Touchscreen.current;
            if (touch != null && touch.primaryTouch.press.wasPressedThisFrame)
            {
                screenPos = touch.primaryTouch.position.ReadValue();
                return true;
            }

            var mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                screenPos = mouse.position.ReadValue();
                return true;
            }

            return false;
        }

        void TryPlace(Vector2 screenPos)
        {
            if (session == null || session.Room == null)
                return;

            hits.Clear();
            if (!raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
                return;

            var chosen = hits[0];
            if (preferVerticalPlanes)
            {
                for (var i = 0; i < hits.Count; i++)
                {
                    var plane = hits[i].trackable as ARPlane;
                    if (plane != null && plane.alignment == PlaneAlignment.Vertical)
                    {
                        chosen = hits[i];
                        break;
                    }
                }
            }

            var hitPlane = chosen.trackable as ARPlane;
            var pose = chosen.pose;
            var type = session.ActiveDeviceType;
            var prefab = type == ElectricalDeviceType.Panel ? panelPrefab : outletPrefab;

            Transform parent = null;
            if (anchorManager != null && hitPlane != null)
            {
                var anchor = anchorManager.AttachAnchor(hitPlane, pose);
                if (anchor != null)
                    parent = anchor.transform;
            }

            var instance = DeviceVisualFactory.Spawn(prefab, pose, type, parent);

            var wallId = hitPlane != null ? PlaneWallSync.WallIdFromPlane(hitPlane) : FallbackWallId;
            var localOnWall = hitPlane != null
                ? PlaneLocalPosition(hitPlane.transform, pose.position)
                : new Vec2Data(pose.position.x, pose.position.y);

            var world = new Vec3Data(pose.position.x, pose.position.y, pose.position.z);
            var device = session.Room.AddDevice(
                type,
                wallId,
                localOnWall,
                hasWorldPose: true,
                worldPosition: world,
                worldEulerY: pose.rotation.eulerAngles.y);

            session.RegisterSpawned(device.id, instance);
            session.NotifyChanged();
        }

        static Vec2Data PlaneLocalPosition(Transform planeTransform, Vector3 worldPosition)
        {
            var local = planeTransform.InverseTransformPoint(worldPosition);
            return new Vec2Data(local.x, local.y);
        }
    }
}
