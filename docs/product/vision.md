# Product vision

Nav: [docs index](../README.md) · [modes](modes.md) · [roadmap](roadmap.md)

## What

SmartElectric lets a user scan rooms from a phone, see house walls in augmented reality, place outlets and electrical panels in real time, then automatically get an optimal wiring layout and a cost estimate.

## Why

Electric layout planning is slow on paper or generic CAD. AR on-site captures real room geometry and makes placement intuitive for homeowners and electricians.

## One product, one room format

| Mode | Devices | Room construction | Quality |
|------|---------|-------------------|---------|
| With LiDAR | iPhone/iPad Pro (depth) | RoomPlan / scene mesh | High |
| Without LiDAR | Regular iPhone + Android | AR planes + assisted edit, or manual plan + align | Medium; needs assist |

Both modes produce the same **RoomModel**. After that, live AR, devices, routes, and estimate are shared.

```
        ┌─ LiDAR ──── RoomPlan / dense mesh ─┐
Input ──┤                                     ├──► RoomModel ──► Live AR + routes + estimate
        └─ No-LiDAR ─ planes + manual edit ──┘
```

## Non-goals (for now)

- Full apartment multi-room BIM / IFC
- Automatic code-compliant electrical engineering certification
- Cloud collaboration in MVP
- Replacing professional CAD for complex industrial sites

## UX honesty

Show accuracy badge: **high / medium / low** from `scanMode` + `confidence` so installers trust the numbers.

## Naming in UI

- **Точный скан (LiDAR)** — быстро снимет стены и проёмы
- **Обычный скан** — любой телефон, чуть дольше
- **Простой план** — ввести размеры и привязать в AR
