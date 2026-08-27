# Roadmap

Nav: [docs index](../README.md) · [vision](vision.md) · [modes](modes.md)

## Now

**Phase 1** — Live AR + No-LiDAR (planes → place devices → save RoomModel JSON).

Order: ship live AR before LiDAR. LiDAR only upgrades scan quality.

## Phase 1 — Live AR + No-LiDAR (current)

- Vertical / horizontal planes
- Place panel + outlets
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
