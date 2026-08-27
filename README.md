# SmartElectric

Mobile AR: scan a room → place outlets/panels → auto wiring routes + cost estimate.

## Start here

1. **[docs/README.md](docs/README.md)** — project map (phase, tree, where to click)
2. **[Install & deploy](docs/setup/install-and-deploy.md)** — Unity, packages, phone builds
3. [Roadmap](docs/product/roadmap.md) — what we build now
4. [AGENTS.md](AGENTS.md) — short brief for AI

**Now:** Phase 1 (No-LiDAR live AR). Unity in repo; app folders at `Assets/_Project/`.

## Modes (one RoomModel)

| Mode | Devices | How the room is built |
|------|---------|------------------------|
| LiDAR | iPhone/iPad Pro (+ depth) | RoomPlan / dense mesh |
| No-LiDAR | Regular iOS + Android | AR planes + assist, or manual plan + AR align |

## Docs index

| Area | Links |
|------|--------|
| Setup | **[install & deploy](docs/setup/install-and-deploy.md)** · [template → product](docs/setup/from-template-to-product.md) |
| Product | [vision](docs/product/vision.md) · [modes](docs/product/modes.md) · [roadmap](docs/product/roadmap.md) |
| Architecture | [stack](docs/architecture/stack.md) · [RoomModel](docs/architecture/room-model.md) · [unity coding](docs/architecture/unity-coding.md) |
| Decisions | [001 Unity](docs/decisions/001-unity-ar-foundation.md) |
| AI / process | [context map](docs/ai/context-map.md) · [conventions](docs/ai/conventions.md) |

## Repo

https://github.com/EdyardEr/SmartElectric.git
