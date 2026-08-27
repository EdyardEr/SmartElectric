using System.Collections.Generic;
using SmartElectric.Domain;
using UnityEngine;

namespace SmartElectric.AR
{
    /// <summary>Holds the active RoomModel and spawned device visuals for Phase 1.</summary>
    public sealed class ProjectSession : MonoBehaviour
    {
        [SerializeField] string saveFileName = "current_room.json";
        [SerializeField] string defaultRoomName = "Room";

        readonly Dictionary<string, GameObject> spawnedByDeviceId = new Dictionary<string, GameObject>();

        public RoomModel Room { get; private set; }
        public DeviceType ActiveDeviceType { get; private set; } = DeviceType.Outlet;
        public string LastStatus { get; private set; } = "Ready";

        void Awake()
        {
            Room = RoomModel.CreateNew(defaultRoomName, ScanMode.Planes);
            LastStatus = $"New room '{Room.name}' ({Room.id})";
        }

        public void SetDeviceType(DeviceType type)
        {
            ActiveDeviceType = type;
            LastStatus = $"Place mode: {type}";
        }

        public void RegisterSpawned(string deviceId, GameObject instance)
        {
            if (string.IsNullOrEmpty(deviceId) || instance == null)
                return;
            spawnedByDeviceId[deviceId] = instance;
        }

        public void NotifyChanged()
        {
            LastStatus = $"Devices: {Room.devices.Count} | mode: {ActiveDeviceType}";
        }

        public void Save()
        {
            try
            {
                RoomModelStore.Save(Room, saveFileName);
                LastStatus = $"Saved ({Room.devices.Count} devices)";
            }
            catch (System.Exception ex)
            {
                LastStatus = "Save failed";
                Debug.LogError($"[SmartElectric] Save failed: {ex.Message}", this);
            }
        }

        public void Load()
        {
            try
            {
                if (!RoomModelStore.TryLoad(saveFileName, out var loaded) || loaded == null)
                {
                    LastStatus = "Nothing to load";
                    return;
                }

                ClearSpawned();
                Room = loaded;
                RespawnFromModel();
                LastStatus = $"Loaded '{Room.name}' ({Room.devices.Count} devices)";
            }
            catch (System.Exception ex)
            {
                LastStatus = "Load failed";
                Debug.LogError($"[SmartElectric] Load failed: {ex.Message}", this);
            }
        }

        public void NewRoom()
        {
            ClearSpawned();
            Room = RoomModel.CreateNew(defaultRoomName, ScanMode.Planes);
            LastStatus = $"New room '{Room.id}'";
        }

        void ClearSpawned()
        {
            foreach (var pair in spawnedByDeviceId)
            {
                if (pair.Value != null)
                    Destroy(pair.Value);
            }
            spawnedByDeviceId.Clear();
        }

        void RespawnFromModel()
        {
            if (Room?.devices == null)
                return;

            for (var i = 0; i < Room.devices.Count; i++)
            {
                var d = Room.devices[i];
                var pos = new Vector3(d.localPosition.x, d.localPosition.y, 0f);
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = $"{d.type}_{d.id}";
                go.transform.position = pos;
                go.transform.localScale = d.type == DeviceType.Panel
                    ? new Vector3(0.35f, 0.5f, 0.08f)
                    : new Vector3(0.12f, 0.08f, 0.04f);
                spawnedByDeviceId[d.id] = go;
            }
        }
    }
}
