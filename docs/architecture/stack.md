# Tech stack

Nav: [docs index](../README.md) · [RoomModel](room-model.md) · [ADR 001](../decisions/001-unity-ar-foundation.md)

## Verdict

| Layer | Choice | Why |
|-------|--------|-----|
| AR client | Unity 6 + AR Foundation 6.x | One live AR session on iOS + Android |
| LiDAR scan | RoomPlan (Swift bridge or RoomPlan for Unity Kit) | Full CapturedRoom; AF RoomPlan bounding boxes alone are not enough |
| No-LiDAR | AR Foundation planes + custom Room Builder | Shared API for ARKit/ARCore |
| Domain | Pure C# in Unity (no MonoBehaviour) | Testable without device; shared by all modes |
| Persistence (MVP) | JSON RoomModel on disk (± SQLite) | Simple save/load |
| Backend | None for MVP → Supabase/Firebase later | Sync + price catalog later |
| PDF | Deferred to phase 4 | Local or cloud function |
| iOS builds | Mac + Xcode required | RoomPlan / TestFlight |

## Architecture

```
Presentation (UI Toolkit / UGUI)
        │
        ├─ AR Session (AR Foundation: planes, anchors, raycasts)
        ├─ Scan adapters (RoomPlan | Planes | Manual) → RoomModel
        └─ Domain core: RoomModel, Placement, Routing, Estimate, Pdf
```

Adapters write RoomModel; routing/estimate never call AR APIs.

## Why not other stacks (MVP)

- **Native Swift + Kotlin only:** best platform APIs, but two live AR codebases.
- **Flutter / RN:** weaker for 3D snap + route visualization as the core feature.
- **Polycam / Matterport / full BIM:** overkill for single-room electrical MVP.

## Packages (expected)

- AR Foundation, XR Plug-in Management
- Apple ARKit XR Plugin, Google ARCore XR Plugin
- Input System, Addressables (device prefabs)
- RoomPlan integration only on the LiDAR iOS branch

## Devices for QA

- 1× iPhone/iPad with LiDAR
- 1× non-LiDAR iPhone
- 1–2 mid-range Android with ARCore
