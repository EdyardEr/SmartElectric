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
        [SerializeField] GameObject outletPrefab;
        [SerializeField] GameObject panelPrefab;

        readonly Dictionary<string, GameObject> spawnedByDeviceId = new Dictionary<string, GameObject>();

        public RoomModel Room { get; private set; }
        public ElectricalDeviceType ActiveDeviceType { get; private set; } = ElectricalDeviceType.Outlet;
        public string LastStatus { get; private set; } = "Ready";

        void Awake()
        {
            Room = RoomModel.CreateNew(defaultRoomName, ScanMode.Planes);
            LastStatus = $"New room '{Room.name}' ({Room.id})";
        }

        public void SetDeviceType(ElectricalDeviceType type)
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
            var wallCount = Room?.walls != null ? Room.walls.Count : 0;
            LastStatus = $"Walls: {wallCount} | devices: {Room.devices.Count} | mode: {ActiveDeviceType}";
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
                var pose = DeviceVisualFactory.GetPose(d);
                var prefab = d.type == ElectricalDeviceType.Panel ? panelPrefab : outletPrefab;
                var go = DeviceVisualFactory.Spawn(prefab, pose, d.type);
                go.name = $"{d.type}_{d.id}";
                spawnedByDeviceId[d.id] = go;
            }
        }
    }
}
