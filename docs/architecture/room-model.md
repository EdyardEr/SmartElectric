# RoomModel

Nav: [docs index](../README.md) · [stack](stack.md) · [modes](../product/modes.md)

Canonical shared room format. All scan adapters write this; Live AR, routing, and estimate only read this.

`schemaVersion` must be updated when breaking fields change.

## Conceptual shape

```text
RoomModel
  schemaVersion: number
  id: string
  name: string
  scanMode: Lidar | Planes | Manual
  confidence: High | Medium | Low
  units: Meters
  floor: { transform, polygon? }
  ceiling?: { height, transform? }
  walls[]: Wall
  openings[]: Opening
  devices[]: Device
  routes[]: Route
  meta: { createdAt, updatedAt, appVersion }
```

### Wall

```text
Wall
  id: string
  width: number          # meters along wall
  height: number         # meters
  transform: { position, rotation }  # room space
  polygon?: Vec3[]       # optional non-rect footprint edge
```

### Opening

```text
Opening
  id: string
  type: Door | Window
  wallId: string
  localPosition: { x, y }  # on wall plane
  width: number
  height: number
```

### Device

```text
Device
  id: string
  type: Outlet | Panel | Switch | Other
  wallId: string
  localPosition: { x, y }  # on wall; y ≈ height from floor
  rotation?: number
  catalogId?: string       # links to price catalog later
```

### Route

```text
Route
  id: string
  fromDeviceId: string
  toDeviceId: string
  path: Vec3[]             # world/room polyline
  lengthMeters: number
  channel: Wall | Ceiling | Floor | Conduit
```

## Sample JSON (illustrative)

```json
{
  "schemaVersion": 1,
  "id": "room_demo_001",
  "name": "Kitchen",
  "scanMode": "Planes",
  "confidence": "Medium",
  "units": "Meters",
  "floor": {
    "transform": { "position": [0, 0, 0], "rotation": [0, 0, 0, 1] }
  },
  "ceiling": { "height": 2.7 },
  "walls": [
    {
      "id": "wall_n",
      "width": 4.2,
      "height": 2.7,
      "transform": {
        "position": [0, 1.35, 2.1],
        "rotation": [0, 0, 0, 1]
      }
    }
  ],
  "openings": [],
  "devices": [
    {
      "id": "dev_panel",
      "type": "Panel",
      "wallId": "wall_n",
      "localPosition": { "x": 0.5, "y": 1.5 }
    },
    {
      "id": "dev_out_1",
      "type": "Outlet",
      "wallId": "wall_n",
      "localPosition": { "x": 2.0, "y": 0.3 }
    }
  ],
  "routes": [],
  "meta": {
    "createdAt": "2026-08-27T00:00:00Z",
    "updatedAt": "2026-08-27T00:00:00Z",
    "appVersion": "0.1.0"
  }
}
```

## Rules

- No LiDAR-only fields in the core schema; extras go in `meta` or adapter-side cache.
- Confidence: LiDAR default High; Planes Medium; Manual Medium/Low depending on edits.
- Routing consumes walls + devices; must ignore `scanMode` except for UI badges.
- When schema changes: update this doc, sample JSON, and serializers together.
