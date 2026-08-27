# Scan modes

Nav: [docs index](../README.md) · [vision](vision.md) · [roadmap](roadmap.md)

## Mode A — With LiDAR

**Stack:** ARKit + RoomPlan (+ AR Foundation for placement session)

**Flow:**

1. «Сканировать комнату» → RoomCapture
2. Walls, doors, windows, dimensions
3. Optional quick edit
4. Live AR: snap objects to RoomModel walls (not random planes)

**Pros:** Fast full plan, better corners and meters.  
**Cons:** Pro devices only (mainly Apple at start).

## Mode B — Without LiDAR

**Stack:** ARKit / ARCore plane detection + assisted UI

Without LiDAR, a “magic apartment plan” is unstable → **assisted scan**.

### B1 — Planes + room assembly (MVP-friendly “wow”)

1. Point at floor → lock floor
2. Point at each wall → add wall
3. App closes walls into a contour (rect / N-gon)
4. Height: gesture / input / ceiling if detected
5. Doors/windows: manual cutouts or skip in v1

### B2 — Manual plan + AR alignment (reliable for estimates)

1. Draw room (length × width × height)
2. In AR, align plan to floor / corner
3. Same live AR as other modes

Often ship both: quick rectangle + refine with walls.

**Pros:** Almost any AR phone.  
**Cons:** Slower, needs UX discipline, lower accuracy → mandatory size edits.

## Mode selection in app

```
Start project
├─ Auto: LiDAR capable? → suggest LiDAR (default on Pro)
├─ Always: switcher «Скан без LiDAR» / «Ручной план»
└─ Android: No-LiDAR only (no RoomPlan; Depth API later as enhancement)
```

Do not hide No-LiDAR: Pro users need it in dark/empty rooms where RoomPlan fails.

### LiDAR detect (orienting)

- iOS: scene depth / LiDAR-capable device
- Unity: native plugin or AR Foundation capabilities

## Live AR (both modes)

| | LiDAR | No-LiDAR |
|---|--------|----------|
| Camera tracking | ARKit | ARKit / ARCore |
| Outlet snap | RoomModel wall | RoomModel wall or vertical plane |
| Anchor stability | Higher | Lower → more re-align |
| Re-enter room | Better | Short «align plan to room» (floor + one wall) |

## Shared downstream

Routing engine, PDF, and estimate do not know whether LiDAR was used. A later LiDAR rescan can upgrade a draft RoomModel.
