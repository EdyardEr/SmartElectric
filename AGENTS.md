# SmartElectric — agents

AR electrical layout: scan → place outlets/panels → routes + estimate.

**Invariant:** LiDAR | Planes | Manual → one `RoomModel` → shared AR/routing/estimate.

**Stack:** Unity 6 + AR Foundation. RoomPlan = iOS LiDAR adapter only. Domain = pure C#.

**Phase 1:** No-LiDAR live AR + JSON save. No RoomPlan/PDF/cloud unless asked.

**Orient:** `docs/README.md` → `docs/ai/context-map.md`. **Setup:** `docs/setup/install-and-deploy.md`.

**Lang:** Agent EN. Product/UX RU OK. Code EN.

**Sync:** Behavior/schema/stack/phase change → update `docs/` + rules/`AGENTS.md` same change. New caches/builds/secrets/binaries → extend `.gitignore` (and `.cursorignore` if indexed noise).
