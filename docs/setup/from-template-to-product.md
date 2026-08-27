# From AR Mobile template → SmartElectric product

Nav: [docs index](../README.md) · [roadmap](../product/roadmap.md) · [install](install-and-deploy.md) · [stack](../architecture/stack.md)

План: **когда** опираемся на демо-шаблон, **когда** отрезаем его, и что остаётся «конечным» проектом.  
Один Unity-проект на весь путь; шаблон — scaffolding, не вторая архитектура.

---

## Картина цели

| Остаётся навсегда | Уходит после exit |
|-------------------|-------------------|
| URP + ProjectSettings (XR, Player) | `Assets/MobileARTemplateAssets/` |
| AR Foundation / ARCore / ARKit packages | Демо-скрипты (`GoalManager`, `ARTemplateMenuManager`, …) |
| `Assets/XR/`, нужные XRI settings (если ещё используем XRI) | Onboarding «goals», меню клиньев, demo prefabs |
| `Assets/_Project/**` (весь продукт) | `Assets/Samples/**` (если не ссылаемся) |
| Свои сцены в `_Project/Scenes/` | Зависимость Build Settings от `SampleScene` как единственной |

**Конечный проект** = тот же репо, но entry scene и весь UX/код — только `_Project` (+ минимальный XR bootstrap).

---

## Статусы (коротко)

| Статус | Смысл |
|--------|--------|
| **T0 Borrow** | Шаблон = основной способ проверить AR на телефоне |
| **T1 Parallel** | Продукт в `_Project`, сцена шаблона ещё открыта для референса |
| **T2 Own scene** | Build Settings → наша сцена; шаблон в проекте, но не в билде |
| **T3 Soft delete** | Папки шаблона удалены или в архив; репо = продукт |
| **T4 Clean** | Нет ссылок на template GUIDs; Samples вычищены; размер билда ок |

Сейчас ожидаемо: **T0 → T1** (папки `_Project` уже есть).

---

## Этап T0 — Borrow (сейчас / первые дни)

**Цель:** Build And Run, плоскости, тап ставит объект.

**Делаем:**
- Не переписываем `MobileARTemplateAssets`.
- Учимся на `SampleScene`: XR Origin, Plane Manager, raycast/placement.
- Код SmartElectric ещё можно не писать или только заготовки в `_Project/Domain`.

**Не делаем:**
- Не раздуваем логику продукта внутри скриптов шаблона.
- Не удаляем шаблон «чтобы было чисто» — ещё рано.

**Выход из T0:** приложение с шаблона стабильно ставится на Android (или iOS) и держит трекинг.

---

## Этап T1 — Parallel (Phase 1 код начинается)

**Цель:** продукт и демо живут рядом, граница ясная.

**Делаем:**
- `RoomModel` + JSON в `_Project/Domain`.
- Placement / save в `_Project/AR` (можно вызывать те же AF API, что в сцене шаблона).
- Префабы Outlet/Panel в `_Project/Prefabs` — **не** клинья шаблона как финальные ассеты.
- UI продукта в `_Project/UI` (хотя бы Save / Load / «розетка»).

**Шаблон:**
- Остаётся для копирования приёмов (plane fade, occluder, input).
- Можно временно спавнить наш префаб из demo-меню — только как костыль, с пометкой TODO.

**Выход из T1 (gate):**  
«Поставить щиток + розетки и сохранить JSON» работает **без** необходимости проходить onboarding goals шаблона. Допустимо ещё открывать SampleScene для отладки XR.

Связь с [roadmap](../product/roadmap.md): середина **Phase 1**.

---

## Этап T2 — Own scene (главный перелом)

**Цель:** продукт больше не «гость» в SampleScene.

**Делаем:**
1. Создать `Assets/_Project/Scenes/ARPlacement.unity` (или `App.unity`).
2. Скопировать из SampleScene **только инфраструктуру**:
   - AR Session, XR Origin, AR Camera
   - AR Plane Manager (+ visualizer, если нужен)
   - AR Raycast / Anchor (и XRI interactor — только если решите оставить XRI)
   - EventSystem / input, нужный для тачей
3. **Не** копировать: GoalManager, demo create/delete menu, wedge gallery, template onboarding UI.
4. Build Settings: наша сцена = **index 0**; SampleScene убрать из списка (файл пока можно оставить в диске).
5. Документировать в README/setup: «Play / Build → `_Project/Scenes/...`».

**Критерий «своя сцена готова»:**
- [ ] Холодный старт → сразу наш UX (или наш простой HUD)
- [ ] Плоскости + placement наших устройств
- [ ] Save/Load RoomModel
- [ ] На телефоне тот же флоу, что в Editor (насколько AR позволяет)

**Выход из T2:** день, когда вы перестаёте запускать SampleScene для фич продукта.  
Шаблон ещё в репо = запасной парашют / референс.

Связь с roadmap: **конец Phase 1** (до или сразу после стабильного save JSON).

---

## Этап T3 — Soft delete шаблона (вырезаем демо)

**Когда (не раньше):**
- T2 gate закрыт ≥ нескольких дней стабильной работы на устройстве.
- Нет `#if` / ссылок в коде `_Project` на `UnityEngine.XR.Templates.AR` или prefab GUID из `MobileARTemplateAssets`.
- Никто в команде не открывает SampleScene «как основную».

**Что удалить (порядок безопаснее):**

1. **Поиск зависимостей** в Unity: кто ссылается на материалы/префабы шаблона. Заменить на `_Project` ассеты.
2. Удалить папки (вместе с `.meta`):
   - `Assets/MobileARTemplateAssets/`
   - `Assets/Scenes/SampleScene.unity` (+ meta), если не нужна
   - `Assets/Samples/` — если это только XRI Starter/AR Starter demos и вы не импортируете их заново
3. Проверить **Console** на missing scripts / missing prefabs.
4. Build And Run smoke: плоскости + 2 устройства + save/load.
5. Обновить docs: статус шаблона = removed; tree в `docs/README.md`.

**Что обычно оставить:**
- `Assets/XR/`, `Assets/XRI/Settings` (если XRI ещё в манифесте)
- `Assets/Settings/` (URP)
- Packages: AR Foundation, ARCore, ARKit, Input System (± XRI — решение отдельно)

**Откат:** git revert / ветка `keep-template` перед удалением (обязательный тег/ветка `pre-template-purge`).

Связь с roadmap: после стабилизации Phase 1, **до** тяжёлого Phase 2 (RoomPlan) — чтобы LiDAR не вшивать в демо-сцену. Идеальное окно: **между Phase 1 и Phase 2**.

---

## Этап T4 — Clean (конечный вид репо)

**Делаем:**
- В Build Settings только продуктовые сцены (`Bootstrap` → `ARPlacement` и т.д.).
- Addressables/Resources только из `_Project` (или явного `Assets/Content`).
- Решить судьбу **XR Interaction Toolkit**:
  - оставить, если gesture/grab нужны;
  - или упростить до AR Foundation raycast + свой UI (меньше зависимостей).
- Прогнать размер билда; выкинуть неиспользуемые материалы/текстуры.
- ADR или короткая запись: «template purged on DATE».

**Конечная структура (ориентир):**

```text
Assets/
  _Project/
    Domain/
    Adapters/
    AR/
    UI/
    Prefabs/
    Scenes/          # entry scenes
    Resources/
  Settings/          # URP
  XR/                # XR general settings
  (optional) XRI/    # only if still used
Packages/
ProjectSettings/
docs/
```

После T4 репо **визуально и по смыслу** = SmartElectric, не «AR Mobile demo + наши папки».

---

## Связка с продуктовым roadmap

```text
Phase 1 start     T0 Borrow
    │
    ├─ Domain + placement + JSON     T1 Parallel
    │
    ├─ Own AR scene in build         T2 Own scene   ← главный перелом
    │
Phase 1 stable ──► T3 Soft delete   ← вырезать демо здесь
    │
Phase 2 RoomPlan                   already on product scenes
Phase 3 No-LiDAR wizard
Phase 4 routes / estimate / PDF
                   T4 Clean         ← по мере уборки (можно сразу после T3)
```

**Не ждать Phase 4**, чтобы вырезать шаблон: чем дольше демо в билде, тем больше случайных ссылок.

---

## Правила для агентов / команды

- Новый код продукта → только `Assets/_Project/**` (или явное исключение в ADR).
- Не добавлять фичи SmartElectric в `MobileARTemplateAssets` / `Samples`.
- Перед T3: ветка или тег `pre-template-purge`.
- После удаления шаблона — обновить `docs/README.md` (Now/Tree), этот файл (статус), `.cursorignore` если тяжёлые сэмплы ушли.

**Текущий статус документа:** T0/T1 — шаблон ещё в проекте, `_Project` создан. Обновляйте строку статуса при смене этапа.

| Field | Value |
|-------|--------|
| **Template stage** | T1 Parallel — Domain/AR/UI scripts exist; still on SampleScene until T2 |
| **Next gate** | T2 — own scene as Build Settings index 0 |
