# Docs — start here

Quick map of SmartElectric. Open this file first when you are lost.

## Now

| | |
|--|--|
| **Phase** | **1** — No-LiDAR live AR (planes → place devices → save `RoomModel` JSON) |
| **Code** | Unity project not created yet (`Assets/` missing) |
| **Invariant** | All scan modes → one `RoomModel` → shared AR / routes / estimate |

## Where to go

| I need… | Open |
|---------|------|
| **Install Unity / deploy to phone** | [setup/install-and-deploy.md](setup/install-and-deploy.md) |
| What the product is | [product/vision.md](product/vision.md) |
| LiDAR vs planes vs manual | [product/modes.md](product/modes.md) |
| What we build next | [product/roadmap.md](product/roadmap.md) |
| Tech stack | [architecture/stack.md](architecture/stack.md) |
| Data format / JSON | [architecture/room-model.md](architecture/room-model.md) |
| Why Unity | [decisions/001-unity-ar-foundation.md](decisions/001-unity-ar-foundation.md) |
| What agents should open | [ai/context-map.md](ai/context-map.md) |
| Lang + keep docs in sync | [ai/conventions.md](ai/conventions.md) |

## Tree

```text
SmartElectric/
├── README.md                 ← repo overview (humans)
├── AGENTS.md                 ← agent brief (EN, short)
├── .cursor/rules/            ← agent rules (EN, short)
│   ├── core.mdc              always on
│   ├── unity-ar.mdc          Assets/**/*.cs
│   └── room-model.mdc        Domain / RoomModel
├── docs/
│   ├── README.md             ← you are here
│   ├── setup/                install-and-deploy (how to install & run)
│   ├── product/              vision · modes · roadmap
│   ├── architecture/         stack · room-model
│   ├── decisions/            ADRs
│   └── ai/                   context-map · conventions
└── (later) Assets/           Unity
```

## Layers (who reads what)

| Path | Audience | Lang |
|------|----------|------|
| `docs/setup/` | humans (+ agents doing bootstrap) | RU OK |
| `docs/product/` | humans (+ agents when needed) | RU OK |
| `docs/architecture/`, `decisions/` | humans + agents | EN |
| `AGENTS.md`, `.cursor/rules/` | agents | EN, minimal |
| `docs/ai/` | both | EN |

Detail lives in `docs/`. Rules only point here — do not duplicate long specs in rules.
