# Roadmap

Nav: [docs index](../README.md) · [install](../setup/install-and-deploy.md) · [vision](vision.md) · [modes](modes.md)

## Now

**Phase 1** — Live AR + No-LiDAR (planes → place devices → save RoomModel JSON).

Order: ship live AR before LiDAR. LiDAR only upgrades scan quality.

## Phase 1 — Live AR + No-LiDAR (current)

- [x] Domain `RoomModel` + JSON save/load (`Assets/_Project/Domain`)
- [x] Tap-to-place Outlet/Panel on AR planes + debug HUD
- [x] World pose in JSON (`hasWorldPose`) for Save/Load on device
- [x] Editor menu **Create ARPlacement Scene (T2)** + Build Settings helper
- [x] `ARPlacement.unity` committed (demo UI off, SmartElectric_Runtime wired)
- [x] Plane → `RoomModel` walls (`PlaneWallSync` adapter) + AR anchors on place
- [x] HUD no longer triggers placement on tap
- [ ] Verify on phone: planes + place + save/load ([phase1-wire-up](../setup/phase1-wire-up.md))
- Vertical / horizontal planes (template)
- Save project as RoomModel JSON
- Covers most devices early; proves the core feature

## Phase 2 — LiDAR / RoomPlan

- Pro scan branch (iOS)
- Map `CapturedRoom` → same RoomModel
- Mode switcher in UI

## Phase 3 — Strengthen No-LiDAR

- Wizard: floor → walls → height
- Manual rectangle room
- Alignment on project reopen

## Phase 4 — Routes / estimate / PDF

- On shared RoomModel — no branching by LiDAR
- Catalog of materials × route lengths
- Export PDF / shareable summary

## Later (optional)

- Android Depth API as quality boost (not a second RoomPlan)
- Cloud sync / auth
- Multi-room projects
- Price catalog CMS

## Phase gate

Implement against **Now** above / `AGENTS.md`. Cross-phase only if the user explicitly asks.

## Template exit (parallel track)

AR Mobile demo is scaffolding. When to cut it: [docs/setup/from-template-to-product.md](../setup/from-template-to-product.md).

- Ideal purge window: **after Phase 1 stable, before Phase 2 RoomPlan** (stage **T3**).
- Do not implement product features inside `MobileARTemplateAssets` / `Samples`.

