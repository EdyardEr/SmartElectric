# Docs & agent conventions

Nav: [docs index](../README.md) · [context map](context-map.md)

## Audience

| Path | Role |
|------|------|
| `docs/README.md` | Start here — map of the project |
| `docs/` | Full source of truth |
| `AGENTS.md` + `.cursor/rules/` | Short EN agent instructions |

## Language

- **EN:** agents, architecture, ADR, schema, code
- **RU OK:** product vision, UX copy
- No long RU text in always-on rules — link to `docs/`

## Sync matrix

| Change | Also update |
|--------|-------------|
| Phase / scope | `roadmap.md`, `AGENTS.md`, `core.mdc`, `docs/README.md` (Now) |
| Product / modes | `docs/product/*`, `core.mdc` if invariant shifts |
| Stack / packages | `stack.md`, ADR, `unity-ar.mdc`, `setup/install-and-deploy.md` if install steps change |
| Unity bootstrap / deploy | `setup/install-and-deploy.md`, `docs/README.md` (Code/Now), `stack.md` versions |
| RoomModel | `room-model.md`, `room-model.mdc`, serializers |
| Navigation / process | `docs/README.md`, `context-map.md`, this file |

Keep rules terse; explanations stay in `docs/`.
