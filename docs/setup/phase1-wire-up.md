# Phase 1 — wire SmartElectric into the AR scene

Nav: [docs index](../README.md) · [template exit](from-template-to-product.md) · [install](install-and-deploy.md)

After Domain/AR scripts imported in Unity:

1. Open `Assets/Scenes/SampleScene.unity` (template AR scene).
2. Menu **SmartElectric → Setup Phase1 On Open Scene**.
3. Confirm Hierarchy has `SmartElectric_Runtime` with `ProjectSession`, `ArDevicePlacer`, `ProjectDebugHud`.
4. Ensure scene has `ARRaycastManager` (template XR Origin usually does).
5. Play or Build And Run:
   - Wait for planes
   - HUD: **Outlet** / **Panel**
   - Tap plane to place
   - **Save** / **Load** (JSON under `persistentDataPath/projects/`)

Still **T1 Parallel** — product code in `_Project`, template scene reused. Next: own scene (**T2**).
