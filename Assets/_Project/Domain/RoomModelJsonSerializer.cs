using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartElectric.Domain
{
    /// <summary>
    /// JSON for schemaVersion 1. JsonUtility DTOs use flat arrays (no jagged float[][]).
    /// </summary>
    public static class RoomModelJsonSerializer
    {
        public static string ToJson(RoomModel model, bool prettyPrint = true)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            return JsonUtility.ToJson(RoomModelDto.FromModel(model), prettyPrint);
        }

        public static RoomModel FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON is empty.", nameof(json));

            var dto = JsonUtility.FromJson<RoomModelDto>(json);
            if (dto == null)
                throw new InvalidOperationException("Failed to parse RoomModel JSON.");
            return dto.ToModel();
        }
    }

    [Serializable]
    class RoomModelDto
    {
        public int schemaVersion;
        public string id;
        public string name;
        public string scanMode;
        public string confidence;
        public string units;
        public FloorDto floor = new FloorDto();
        public CeilingDto ceiling = new CeilingDto();
        public WallDto[] walls = Array.Empty<WallDto>();
        public OpeningDto[] openings = Array.Empty<OpeningDto>();
        public DeviceDto[] devices = Array.Empty<DeviceDto>();
        public RouteDto[] routes = Array.Empty<RouteDto>();
        public MetaDto meta = new MetaDto();

        public static RoomModelDto FromModel(RoomModel m)
        {
            return new RoomModelDto
            {
                schemaVersion = m.schemaVersion,
                id = m.id,
                name = m.name,
                scanMode = m.scanMode.ToString(),
                confidence = m.confidence.ToString(),
                units = m.units.ToString(),
                floor = FloorDto.From(m.floor),
                ceiling = CeilingDto.From(m.ceiling),
                walls = Map(m.walls, WallDto.From),
                openings = Map(m.openings, OpeningDto.From),
                devices = Map(m.devices, DeviceDto.From),
                routes = Map(m.routes, RouteDto.From),
                meta = MetaDto.From(m.meta)
            };
        }

        public RoomModel ToModel()
        {
            return new RoomModel
            {
                schemaVersion = schemaVersion > 0 ? schemaVersion : RoomModel.CurrentSchemaVersion,
                id = id,
                name = name,
                scanMode = Parse(scanMode, ScanMode.Planes),
                confidence = Parse(confidence, Confidence.Medium),
                units = Parse(units, LengthUnits.Meters),
                floor = floor != null ? floor.ToModel() : new FloorData(),
                ceiling = ceiling != null ? ceiling.ToModel() : new CeilingData(),
                walls = MapList(walls, w => w.ToModel()),
                openings = MapList(openings, o => o.ToModel()),
                devices = MapList(devices, d => d.ToModel()),
                routes = MapList(routes, r => r.ToModel()),
                meta = meta != null ? meta.ToModel() : new RoomMeta()
            };
        }

        static TEnum Parse<TEnum>(string value, TEnum fallback) where TEnum : struct
        {
            if (string.IsNullOrEmpty(value))
                return fallback;
            return Enum.TryParse(value, true, out TEnum parsed) ? parsed : fallback;
        }

        static TOut[] Map<TIn, TOut>(List<TIn> list, Func<TIn, TOut> map)
        {
            if (list == null || list.Count == 0)
                return Array.Empty<TOut>();
            var arr = new TOut[list.Count];
            for (var i = 0; i < list.Count; i++)
                arr[i] = map(list[i]);
            return arr;
        }

        static List<TOut> MapList<TIn, TOut>(TIn[] arr, Func<TIn, TOut> map) where TIn : class
        {
            var list = new List<TOut>();
            if (arr == null)
                return list;
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] != null)
                    list.Add(map(arr[i]));
            }
            return list;
        }
    }

    [Serializable]
    class MetaDto
    {
        public string createdAt;
        public string updatedAt;
        public string appVersion;

        public static MetaDto From(RoomMeta m) => m == null
            ? new MetaDto()
            : new MetaDto { createdAt = m.createdAt, updatedAt = m.updatedAt, appVersion = m.appVersion };

        public RoomMeta ToModel() => new RoomMeta
        {
            createdAt = createdAt,
            updatedAt = updatedAt,
            appVersion = appVersion
        };
    }

    [Serializable]
    class Vec3Dto
    {
        public float x;
        public float y;
        public float z;

        public static Vec3Dto From(Vec3Data v) => new Vec3Dto { x = v.x, y = v.y, z = v.z };
        public Vec3Data ToModel() => new Vec3Data(x, y, z);
    }

    [Serializable]
    class PoseDto
    {
        public float[] position = { 0f, 0f, 0f };
        public float[] rotation = { 0f, 0f, 0f, 1f };

        public static PoseDto Identity => new PoseDto();

        public static PoseDto From(PoseData p) => new PoseDto
        {
            position = new[] { p.position.x, p.position.y, p.position.z },
            rotation = new[] { p.qx, p.qy, p.qz, p.qw }
        };

        public PoseData ToPose()
        {
            var pose = PoseData.Identity;
            if (position != null && position.Length >= 3)
                pose.position = new Vec3Data(position[0], position[1], position[2]);
            if (rotation != null && rotation.Length >= 4)
            {
                pose.qx = rotation[0];
                pose.qy = rotation[1];
                pose.qz = rotation[2];
                pose.qw = rotation[3];
            }
            return pose;
        }
    }

    [Serializable]
    class FloorDto
    {
        public PoseDto transform = PoseDto.Identity;
        public Vec3Dto[] polygon = Array.Empty<Vec3Dto>();

        public static FloorDto From(FloorData f)
        {
            if (f == null) return new FloorDto();
            return new FloorDto
            {
                transform = PoseDto.From(f.transform),
                polygon = MapVecs(f.polygon)
            };
        }

        public FloorData ToModel() => new FloorData
        {
            transform = transform != null ? transform.ToPose() : PoseData.Identity,
            polygon = MapVecList(polygon)
        };

        static Vec3Dto[] MapVecs(List<Vec3Data> list)
        {
            if (list == null || list.Count == 0) return Array.Empty<Vec3Dto>();
            var arr = new Vec3Dto[list.Count];
            for (var i = 0; i < list.Count; i++)
                arr[i] = Vec3Dto.From(list[i]);
            return arr;
        }

        static List<Vec3Data> MapVecList(Vec3Dto[] arr)
        {
            var list = new List<Vec3Data>();
            if (arr == null) return list;
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] != null)
                    list.Add(arr[i].ToModel());
            }
            return list;
        }
    }

    [Serializable]
    class CeilingDto
    {
        public float height = 2.7f;
        public PoseDto transform = PoseDto.Identity;

        public static CeilingDto From(CeilingData c)
        {
            if (c == null) return new CeilingDto();
            return new CeilingDto { height = c.height, transform = PoseDto.From(c.transform) };
        }

        public CeilingData ToModel() => new CeilingData
        {
            height = height > 0f ? height : 2.7f,
            transform = transform != null ? transform.ToPose() : PoseData.Identity
        };
    }

    [Serializable]
    class WallDto
    {
        public string id;
        public float width;
        public float height;
        public PoseDto transform = PoseDto.Identity;
        public Vec3Dto[] polygon = Array.Empty<Vec3Dto>();

        public static WallDto From(WallData w) => new WallDto
        {
            id = w.id,
            width = w.width,
            height = w.height,
            transform = PoseDto.From(w.transform),
            polygon = FloorDtoFromPolygon(w.polygon)
        };

        public WallData ToModel() => new WallData
        {
            id = id,
            width = width,
            height = height,
            transform = transform != null ? transform.ToPose() : PoseData.Identity,
            polygon = FloorDtoToPolygon(polygon)
        };

        static Vec3Dto[] FloorDtoFromPolygon(List<Vec3Data> list)
        {
            if (list == null || list.Count == 0) return Array.Empty<Vec3Dto>();
            var arr = new Vec3Dto[list.Count];
            for (var i = 0; i < list.Count; i++)
                arr[i] = Vec3Dto.From(list[i]);
            return arr;
        }

        static List<Vec3Data> FloorDtoToPolygon(Vec3Dto[] arr)
        {
            var list = new List<Vec3Data>();
            if (arr == null) return list;
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] != null)
                    list.Add(arr[i].ToModel());
            }
            return list;
        }
    }

    [Serializable]
    class LocalPosDto
    {
        public float x;
        public float y;

        public static LocalPosDto From(Vec2Data v) => new LocalPosDto { x = v.x, y = v.y };
        public Vec2Data ToModel() => new Vec2Data(x, y);
    }

    [Serializable]
    class OpeningDto
    {
        public string id;
        public string type;
        public string wallId;
        public LocalPosDto localPosition = new LocalPosDto();
        public float width;
        public float height;

        public static OpeningDto From(OpeningData o) => new OpeningDto
        {
            id = o.id,
            type = o.type.ToString(),
            wallId = o.wallId,
            localPosition = LocalPosDto.From(o.localPosition),
            width = o.width,
            height = o.height
        };

        public OpeningData ToModel() => new OpeningData
        {
            id = id,
            type = Enum.TryParse(type, true, out OpeningType t) ? t : OpeningType.Door,
            wallId = wallId,
            localPosition = localPosition != null ? localPosition.ToModel() : new Vec2Data(0f, 0f),
            width = width,
            height = height
        };
    }

    [Serializable]
    class DeviceDto
    {
        public string id;
        public string type;
        public string wallId;
        public LocalPosDto localPosition = new LocalPosDto();
        public float rotation;
        public string catalogId;

        public static DeviceDto From(DeviceData d) => new DeviceDto
        {
            id = d.id,
            type = d.type.ToString(),
            wallId = d.wallId,
            localPosition = LocalPosDto.From(d.localPosition),
            rotation = d.rotation,
            catalogId = d.catalogId
        };

        public DeviceData ToModel() => new DeviceData
        {
            id = id,
            type = Enum.TryParse(type, true, out DeviceType t) ? t : DeviceType.Other,
            wallId = wallId,
            localPosition = localPosition != null ? localPosition.ToModel() : new Vec2Data(0f, 0f),
            rotation = rotation,
            catalogId = catalogId
        };
    }

    [Serializable]
    class RouteDto
    {
        public string id;
        public string fromDeviceId;
        public string toDeviceId;
        public Vec3Dto[] path = Array.Empty<Vec3Dto>();
        public float lengthMeters;
        public string channel;

        public static RouteDto From(RouteData r)
        {
            Vec3Dto[] pathArr = Array.Empty<Vec3Dto>();
            if (r.path != null && r.path.Count > 0)
            {
                pathArr = new Vec3Dto[r.path.Count];
                for (var i = 0; i < r.path.Count; i++)
                    pathArr[i] = Vec3Dto.From(r.path[i]);
            }

            return new RouteDto
            {
                id = r.id,
                fromDeviceId = r.fromDeviceId,
                toDeviceId = r.toDeviceId,
                path = pathArr,
                lengthMeters = r.lengthMeters,
                channel = r.channel.ToString()
            };
        }

        public RouteData ToModel()
        {
            var list = new List<Vec3Data>();
            if (path != null)
            {
                for (var i = 0; i < path.Length; i++)
                {
                    if (path[i] != null)
                        list.Add(path[i].ToModel());
                }
            }

            return new RouteData
            {
                id = id,
                fromDeviceId = fromDeviceId,
                toDeviceId = toDeviceId,
                path = list,
                lengthMeters = lengthMeters,
                channel = Enum.TryParse(channel, true, out RouteChannel c) ? c : RouteChannel.Wall
            };
        }
    }
}
