using SmartElectric.Domain;
using UnityEngine;

namespace SmartElectric.AR
{
    /// <summary>Shared primitive visuals for outlets/panels (Phase 1).</summary>
    public static class DeviceVisualFactory
    {
        public static GameObject Spawn(GameObject prefab, Pose pose, ElectricalDeviceType type, Transform parent = null)
        {
            GameObject instance;
            if (prefab != null)
            {
                instance = parent != null
                    ? Object.Instantiate(prefab, pose.position, pose.rotation, parent)
                    : Object.Instantiate(prefab, pose.position, pose.rotation);
            }
            else
            {
                instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                instance.name = type == ElectricalDeviceType.Panel ? "Panel" : "Outlet";
                instance.transform.SetPositionAndRotation(pose.position, pose.rotation);
                if (parent != null)
                    instance.transform.SetParent(parent, true);
                ApplyPrimitiveStyle(instance, type);
            }

            return instance;
        }

        public static void ApplyPrimitiveStyle(GameObject go, ElectricalDeviceType type)
        {
            go.transform.localScale = type == ElectricalDeviceType.Panel
                ? new Vector3(0.35f, 0.5f, 0.08f)
                : new Vector3(0.12f, 0.08f, 0.04f);

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = type == ElectricalDeviceType.Panel
                    ? new Color(0.2f, 0.45f, 0.85f)
                    : new Color(0.95f, 0.85f, 0.2f);
            }
        }

        public static Pose GetPose(DeviceData device)
        {
            if (device != null && device.hasWorldPose)
            {
                var p = device.worldPosition;
                return new Pose(
                    new Vector3(p.x, p.y, p.z),
                    Quaternion.Euler(0f, device.worldEulerY, 0f));
            }

            if (device != null)
            {
                return new Pose(
                    new Vector3(device.localPosition.x, device.localPosition.y, 0f),
                    Quaternion.identity);
            }

            return Pose.identity;
        }
    }
}
