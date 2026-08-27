# Unity coding practices

Nav: [docs index](../README.md) · [stack](stack.md) · [template exit](../setup/from-template-to-product.md)

Agent rule (short): `.cursor/rules/unity-csharp.mdc` + `.cursor/rules/unity-ar.mdc`.

## Goals

Mobile AR app: readable structure, testable Domain, no per-frame garbage, clear boundary vs AR Mobile template.

## Layout

| Path | Responsibility |
|------|----------------|
| `Assets/_Project/Domain` | RoomModel, routing, estimate — pure C# |
| `Assets/_Project/Adapters` | Scan → RoomModel |
| `Assets/_Project/AR` | AR Foundation session, placement, re-align |
| `Assets/_Project/UI` | Menus, HUD |
| `Assets/_Project/Prefabs`, `Scenes` | Product prefabs/scenes |
| `MobileARTemplateAssets`, `Samples` | Scaffolding only — do not extend with product features |

Namespaces: `SmartElectric.Domain`, `SmartElectric.AR`, `SmartElectric.Adapters`, `SmartElectric.UI`.

## MonoBehaviour vs Domain

- **Domain / services:** plain classes, unit-testable in Editor without Play mode when possible.
- **MonoBehaviours:** serialize references, lifecycle, call into services. Keep methods short.
- Avoid God-objects (`GameManager` that owns model + UI + AR + I/O). Prefer small components + one composition root if needed.

## Inspector & references

- `[SerializeField] private` + clear tooltips when non-obvious.
- Assign in Inspector or inject from a bootstrap; don’t hide hard dependencies behind silent `Find`.
- Missing critical ref → log error once and disable behaviour.

## Performance

- Hot paths (`Update`, plane callbacks at high rate): no LINQ, no string concat for logs every frame, reuse buffers.
- Cache `Transform`, managers, cameras.
- AR: prefer event-driven plane/trackables updates over tight polling.

## Async & teardown

- Coroutines: stop on disable; don’t assume object still alive after `yield`.
- Unsubscribe events / Input actions in `OnDisable`.
- File I/O (JSON save): not on the AR render critical path; offload or gate behind UI action.

## Assets & VCS

- Always commit `.meta` with scripts/prefabs/scenes.
- Prefabs for reusable devices (outlet, panel); scenes for composition.
- Extend `.gitignore` when new generated folders appear.

## Do / don’t

| Do | Don’t |
|----|--------|
| New product scripts under `_Project` | Patch product logic into template scripts |
| Explicit null checks for serialized deps | Empty catch / swallow AR failures |
| Input System | Legacy `Input` for new code |
| Small PRs matching existing style | Drive-by renames across Samples |
