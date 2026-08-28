using System.Collections.Generic;
using SmartElectric.Domain;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SmartElectric.AR
{
    /// <summary>
    /// Phase 1: tap/click AR plane → spawn device and record world pose in RoomModel.
    /// </summary>
    public sealed class ArDevicePlacer : MonoBehaviour
    {
        const string PlaneWallId = "ar_plane";

        [SerializeField] ARRaycastManager raycastManager;
        [SerializeField] ProjectSession session;
        [SerializeField] GameObject outletPrefab;
        [SerializeField] GameObject panelPrefab;
        [SerializeField] bool preferVerticalPlanes = true;

        readonly List<ARRaycastHit> hits = new List<ARRaycastHit>(8);

        void Awake()
        {
            if (raycastManager == null)
                raycastManager = FindAnyObjectByType<ARRaycastManager>();
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

            var pose = chosen.pose;
            var type = session.ActiveDeviceType;
            var prefab = type == ElectricalDeviceType.Panel ? panelPrefab : outletPrefab;
            var instance = DeviceVisualFactory.Spawn(prefab, pose, type);

            var p = pose.position;
            var local = new Vec2Data(p.x, p.y);
            var world = new Vec3Data(p.x, p.y, p.z);
            var device = session.Room.AddDevice(
                type,
                PlaneWallId,
                local,
                hasWorldPose: true,
                worldPosition: world,
                worldEulerY: pose.rotation.eulerAngles.y);
            session.RegisterSpawned(device.id, instance);
            session.NotifyChanged();
        }
    }
}
