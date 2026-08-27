# ADR 001 — Unity + AR Foundation as client

Nav: [docs index](../README.md) · [stack](../architecture/stack.md)

- **Status:** Accepted
- **Date:** 2026-08-27

## Context

SmartElectric needs live AR on iOS and Android, LiDAR room scan on Apple Pro devices, assisted/manual room capture elsewhere, plus a shared routing and estimate core.

## Decision

Use **Unity 6 + AR Foundation** for the client and live AR session.

- **RoomPlan** only as an iOS LiDAR scan adapter mapping into RoomModel.
- **Domain** (RoomModel, routing, estimate) as pure C#.
- No backend for MVP; local JSON persistence.

## Alternatives considered

| Option | Why not (MVP) |
|--------|----------------|
| Swift + Kotlin dual native | Two AR codebases; slower for small team |
| Flutter / React Native | Weaker fit for 3D snap + route viz as core |
| iOS-first RealityKit only | Delays Android; product needs No-LiDAR reach |

## Consequences

- Mac required for iOS builds and RoomPlan integration.
- RoomPlan needs a bridge or third-party Unity kit; not fully covered by AR Foundation alone.
- Shared C# domain is testable in Editor without devices.
