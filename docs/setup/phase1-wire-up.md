# Phase 1 — wire-up & T2 product scene

Nav: [docs index](../README.md) · [template exit](from-template-to-product.md) · [install](install-and-deploy.md)

## Product scene (already in repo)

`Assets/_Project/Scenes/ARPlacement.unity` is the **main build scene** (Build Settings index 0).

It includes XR from the template, disabled demo UI, and `SmartElectric_Runtime`.

### Open in Unity

1. If Unity was open during git pull — **Assets → Refresh** (or restart Editor).
2. **Project** → `Assets/_Project/Scenes` → double-click **ARPlacement**.
3. **File → Build And Run** (or Play on device).

### On device

1. Wait for planes (status shows `Walls: N`).
2. HUD (top-left): **Outlet** / **Panel** → tap wall (not on HUD).
3. Devices parent to AR anchors when possible.
4. **Save** / **Load** — JSON in `persistentDataPath/projects/current_room.json`.

## Recreate scene (optional)

If scene breaks: close Unity, then either:

- Menu **SmartElectric → Create ARPlacement Scene (T2)** (overwrite), or  
- Re-copy from git / run Unity batchmode when Editor is **closed**.

## Alternate — patch any open scene

**SmartElectric → Setup Phase1 On Open Scene** — adds `SmartElectric_Runtime` only.

## Next

- Verify on phone → close **T2** gate in [from-template-to-product.md](from-template-to-product.md)
- Later **T3**: remove `MobileARTemplateAssets` / `Samples`
