# ADR 002 — AR Mobile template as temporary scaffolding

- **Status:** Accepted
- **Date:** 2026-08-27

Nav: [docs index](../README.md) · [exit plan](../setup/from-template-to-product.md)

## Context

The repo was bootstrapped with Unity **AR Mobile (Core)**. That template provides working URP + AR Foundation + demo placement UX, but not SmartElectric domain architecture.

## Decision

Keep template and product in **one** Unity project. Put product code under `Assets/_Project/`. Treat template as scaffolding with staged exit **T0→T4** (see `docs/setup/from-template-to-product.md`). Purge demo assets after Phase 1 is stable on our own scene, before RoomPlan work.

## Consequences

- Faster Hello AR on device.
- Must not implement product features inside `MobileARTemplateAssets` / `Samples`.
- Explicit purge gate + git tag `pre-template-purge` before deletion.
