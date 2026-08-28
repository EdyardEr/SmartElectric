using System.Collections.Generic;
using SmartElectric.Domain;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SmartElectric.AR
{
    /// <summary>
    /// Phase 1: tap/click vertical (or any) AR plane → spawn device prefab and record in RoomModel.
    /// Add to the AR scene next to XR Origin; assign ARRaycastManager.
    /// </summary>
    public sealed class ArDevicePlacer : MonoBehaviour
    {
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

            ARRaycastHit chosen = hits[0];
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
            var prefab = session.ActiveDeviceType == ElectricalDeviceType.Panel ? panelPrefab : outletPrefab;
            var instance = SpawnVisual(prefab, pose, session.ActiveDeviceType);

            var wall = session.Room.EnsureDefaultWall();
            // Approximate wall-local: x along hit, y = height from floor (pose.y).
            var local = new Vec2Data(pose.position.x, pose.position.y);
            var device = session.Room.AddDevice(session.ActiveDeviceType, wall.id, local);
            session.RegisterSpawned(device.id, instance);
            session.NotifyChanged();
        }

        static GameObject SpawnVisual(GameObject prefab, Pose pose, ElectricalDeviceType type)
        {
            if (prefab != null)
            {
                return Instantiate(prefab, pose.position, pose.rotation);
            }

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = type == ElectricalDeviceType.Panel ? "Panel" : "Outlet";
            go.transform.SetPositionAndRotation(pose.position, pose.rotation);
            var scale = type == ElectricalDeviceType.Panel
                ? new Vector3(0.35f, 0.5f, 0.08f)
                : new Vector3(0.12f, 0.08f, 0.04f);
            go.transform.localScale = scale;
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = type == ElectricalDeviceType.Panel
                    ? new Color(0.2f, 0.45f, 0.85f)
                    : new Color(0.95f, 0.85f, 0.2f);
            }
            return go;
        }
    }
}
