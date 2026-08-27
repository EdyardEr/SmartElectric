# Agent context map

Nav: [docs index](../README.md) · [conventions](conventions.md)

**Lost?** Open `docs/README.md` first.

| Task | Open |
|------|------|
| Orient / tree | `docs/README.md` |
| Install Unity / deploy phone | `docs/setup/install-and-deploy.md` |
| Phase 1 Editor wire-up | `docs/setup/phase1-wire-up.md` |
| Cut AR Mobile demo / own scenes | `docs/setup/from-template-to-product.md` |
| Scope / phase | `AGENTS.md`, `docs/product/roadmap.md` |
| Product / UX (RU OK) | `docs/product/vision.md`, `modes.md` |
| Stack / packages | `docs/architecture/stack.md`, `docs/decisions/001-*.md` |
| Unity C# practices | `docs/architecture/unity-coding.md`, `.cursor/rules/unity-csharp.mdc` |
| Schema / save | `docs/architecture/room-model.md` |
| Agent rules | `.cursor/rules/core.mdc`, `unity-csharp.mdc`, `unity-ar.mdc`, `room-model.mdc` |
| Ignore / secrets | `.gitignore`, `.cursorignore` |

## Language

| Layer | Lang |
|-------|------|
| `AGENTS.md`, `.cursor/rules/` | EN, minimal |
| `docs/architecture/`, ADR | EN |
| `docs/product/` | RU OK |
| Code | EN |

## Sync

Behavior / schema / stack / phase change → matching `docs/` **and** rules/`AGENTS.md` same change. New decision → `docs/decisions/00N-….md`.

New caches, builds, secrets, generated Unity files, or large binaries → extend `.gitignore` (and `.cursorignore` if needed) in the same change.
