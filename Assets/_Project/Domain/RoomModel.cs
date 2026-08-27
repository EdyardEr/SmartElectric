using System;
using System.Collections.Generic;

namespace SmartElectric.Domain
{
    [Serializable]
    public sealed class FloorData
    {
        public PoseData transform = PoseData.Identity;
        public List<Vec3Data> polygon = new List<Vec3Data>();
    }

    [Serializable]
    public sealed class CeilingData
    {
        public float height = 2.7f;
        public PoseData transform = PoseData.Identity;
    }

    [Serializable]
    public sealed class WallData
    {
        public string id;
        public float width;
        public float height;
        public PoseData transform = PoseData.Identity;
        public List<Vec3Data> polygon = new List<Vec3Data>();
    }

    [Serializable]
    public sealed class OpeningData
    {
        public string id;
        public OpeningType type;
        public string wallId;
        public Vec2Data localPosition;
        public float width;
        public float height;
    }

    [Serializable]
    public sealed class DeviceData
    {
        public string id;
        public DeviceType type;
        public string wallId;
        public Vec2Data localPosition;
        public float rotation;
        public string catalogId;
    }

    [Serializable]
    public sealed class RouteData
    {
        public string id;
        public string fromDeviceId;
        public string toDeviceId;
        public List<Vec3Data> path = new List<Vec3Data>();
        public float lengthMeters;
        public RouteChannel channel;
    }

    [Serializable]
    public sealed class RoomMeta
    {
        public string createdAt;
        public string updatedAt;
        public string appVersion;
    }

    [Serializable]
    public sealed class RoomModel
    {
        public const int CurrentSchemaVersion = 1;
        public const string AppVersion = "0.1.0";

        public int schemaVersion = CurrentSchemaVersion;
        public string id;
        public string name;
        public ScanMode scanMode = ScanMode.Planes;
        public Confidence confidence = Confidence.Medium;
        public LengthUnits units = LengthUnits.Meters;
        public FloorData floor = new FloorData();
        public CeilingData ceiling = new CeilingData();
        public List<WallData> walls = new List<WallData>();
        public List<OpeningData> openings = new List<OpeningData>();
        public List<DeviceData> devices = new List<DeviceData>();
        public List<RouteData> routes = new List<RouteData>();
        public RoomMeta meta = new RoomMeta();

        public static RoomModel CreateNew(string name, ScanMode scanMode = ScanMode.Planes)
        {
            var now = DateTime.UtcNow.ToString("o");
            return new RoomModel
            {
                schemaVersion = CurrentSchemaVersion,
                id = "room_" + Guid.NewGuid().ToString("N").Substring(0, 12),
                name = string.IsNullOrEmpty(name) ? "Untitled room" : name,
                scanMode = scanMode,
                confidence = DefaultConfidence(scanMode),
                units = LengthUnits.Meters,
                floor = new FloorData { transform = PoseData.Identity },
                ceiling = new CeilingData { height = 2.7f, transform = PoseData.Identity },
                meta = new RoomMeta
                {
                    createdAt = now,
                    updatedAt = now,
                    appVersion = AppVersion
                }
            };
        }

        public static Confidence DefaultConfidence(ScanMode mode)
        {
            switch (mode)
            {
                case ScanMode.Lidar:
                    return Confidence.High;
                case ScanMode.Manual:
                    return Confidence.Low;
                default:
                    return Confidence.Medium;
            }
        }

        public void TouchUpdated()
        {
            if (meta == null)
                meta = new RoomMeta();
            meta.updatedAt = DateTime.UtcNow.ToString("o");
            meta.appVersion = AppVersion;
        }

        public DeviceData AddDevice(DeviceType type, string wallId, Vec2Data localPosition)
        {
            if (devices == null)
                devices = new List<DeviceData>();

            var device = new DeviceData
            {
                id = "dev_" + Guid.NewGuid().ToString("N").Substring(0, 10),
                type = type,
                wallId = wallId ?? string.Empty,
                localPosition = localPosition,
                rotation = 0f
            };
            devices.Add(device);
            TouchUpdated();
            return device;
        }

        public WallData EnsureDefaultWall()
        {
            if (walls == null)
                walls = new List<WallData>();

            if (walls.Count > 0)
                return walls[0];

            var wall = new WallData
            {
                id = "wall_default",
                width = 4f,
                height = ceiling != null ? ceiling.height : 2.7f,
                transform = PoseData.Identity
            };
            walls.Add(wall);
            TouchUpdated();
            return wall;
        }
    }
}
