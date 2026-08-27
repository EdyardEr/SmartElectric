# Install & deploy — detailed plan

Nav: [docs index](../README.md) · [stack](../architecture/stack.md) · [roadmap](../product/roadmap.md)

Пошаговый план: что поставить, как создать Unity-проект в этом репо, как собрать и поставить на телефон.  
**Сейчас цель — Phase 1** (No-LiDAR live AR). RoomPlan / TestFlight production — позже.

---

## 0. Что получите в конце Phase 1 setup

- Unity 6 проект в корне `SmartElectric/` (рядом с `docs/`)
- AR Foundation: плоскости + касание → «розетка» на стене/плоскости
- Сборка на **Android** и/или **iOS** (без LiDAR)
- Сохранение черновика `RoomModel` в JSON (когда дойдёте до кода Domain)

---

## 1. Железо, аккаунты, роли машин

| Нужно | Зачем |
|-------|--------|
| ПК (Windows OK) | Unity Editor, Android-сборки, разработка Domain |
| **Mac** (обязателен для iOS) | Xcode, подпись, RoomPlan (фаза 2), TestFlight |
| Android-телефон с **ARCore** | Основной QA Phase 1 |
| iPhone без LiDAR | QA No-LiDAR на iOS |
| iPhone/iPad **с LiDAR** | Только с фазы 2 |
| Apple Developer Account | Установка на iPhone / TestFlight |
| Google Play / side-load | APK/AAB на Android (для MVP достаточно USB/`adb`) |
| Git + доступ к [репо](https://github.com/EdyardEr/SmartElectric.git) | Версии |

**Рекомендация:** дневная разработка на Windows → Android; раз в цикл — Mac для iOS smoke-test.

---

## 2. Установка инструментов (Windows)

### 2.1 Git

1. Установить [Git for Windows](https://git-scm.com/download/win).
2. Клонировать репо (если ещё не локально):

```bash
git clone https://github.com/EdyardEr/SmartElectric.git
cd SmartElectric
```

Репо уже содержит `docs/`, `AGENTS.md`, `.gitignore` — **не** создавайте Unity-проект в другой папке: создайте **в этом корне**.

### 2.2 Unity Hub + Unity 6

1. Установить [Unity Hub](https://unity.com/download).
2. Войти в Unity ID (Personal OK на старте).
3. Installs → **Install Editor** → **Unity 6** (LTS, если доступен; иначе актуальный 6000.x).
4. В модулях редактора отметить:

| Module | Нужен |
|--------|--------|
| **Android Build Support** | да (+ OpenJDK, Android SDK & NDK Tools — галочки Hub) |
| **iOS Build Support** | да (генерация Xcode-проекта; **сборка .ipa только на Mac**) |
| Visual Studio / VS Code support | по желанию |
| Documentation | по желанию |

5. Проверить, что путь Editor известен Hub.

### 2.3 Android: телефон

1. На телефоне: Developer options → **USB debugging**.
2. Установить [platform-tools](https://developer.android.com/tools/releases/platform-tools) (`adb`) или использовать SDK из Unity Hub.
3. Проверить: `adb devices` — устройство `device`, не `unauthorized`.
4. Убедиться, что модель в [списке ARCore](https://developers.google.com/ar/devices) (или Google Play Services for AR ставится из магазина).

### 2.4 IDE

- Rider / Visual Studio / VS Code + C# Dev Kit — на выбор.
- Cursor уже используется для агента/доков.

---

## 3. Установка инструментов (Mac) — для iOS

Делать, когда нужна установка на iPhone (можно отложить на 1–2 недели после Android).

1. Mac с актуальным **macOS** + [Xcode](https://developer.apple.com/xcode/) из App Store.
2. Xcode → Settings → Platforms: iOS SDK.
3. Открыть Xcode один раз, принять лицензии: `sudo xcodebuild -license`.
4. Unity Hub на Mac + **тот же major** Unity 6, что на Windows (меньше боли с Library).
5. Модуль **iOS Build Support**.
6. Apple Developer: создать App ID, сертификат Development, provisioning profile (или Automatic Signing в Xcode).
7. iPhone: Developer Mode (iOS 16+), доверить компьютеру.

**RoomPlan (фаза 2):** Xcode + устройство с LiDAR + iOS 17+ (уточнять по доке Apple/пакета на момент внедрения).

---

## 4. Создать Unity-проект в этом репозитории

### 4.1 Создание

1. Unity Hub → **New project**.
2. Template: **3D (URP)** или **Mobile 3D** (URP предпочтителен для мобильного AR).
3. Project name: `SmartElectric` (или оставить имя папки).
4. Location: родительская папка так, чтобы корень проекта Unity = корень git:

```text
E:\Projects\SmartElectric\     ← сюда (уже есть docs/, .git/)
```

Hub иногда хочет пустую папку. Если ругается на непустой каталог:

- Вариант A: создать проект во временной папке → перенести `Assets/`, `Packages/`, `ProjectSettings/` в git-корень (не переносить `Library/`).
- Вариант B: создать Unity-проект, затем перенести наши `docs/`, `AGENTS.md`, `.cursor/` в него и сделать эту папку remote origin.

**Важно:** в корневой `.gitignore` уже игнорятся Unity-кэш и бинарники (`Library/`, `Temp/`, `Logs/`, `UserSettings/`, `*.csproj`/`*.sln`, APK/AAB, Addressables temp, …). Не коммитьте `Library/`.

### 4.2 Первый коммит после создания (когда будете готовы)

В индексе должны появиться примерно:

- `Assets/`
- `Packages/manifest.json`
- `ProjectSettings/`

Не коммитить: `Library/`, `Logs/`, `Temp/`, `UserSettings/` (уже в `.gitignore`).

### 4.3 Целевая структура `Assets/` (создать папки сразу)

```text
Assets/
  _Project/
    Domain/           # pure C#: RoomModel, serialize — no MonoBehaviour
    Adapters/         # PlaneScan, ManualPlan (RoomPlan later)
    AR/               # session, placement, re-align
    UI/
    Prefabs/          # Outlet, Panel
    Scenes/
      Bootstrap.unity
      ARPlacement.unity
    Resources/        # or Addressables later
  XR/                 # may be created by XR Plug-in Management
```

Имена можно уточнить, принцип: **Domain без AR API**.

---

## 5. Пакеты и XR (ядро стека Phase 1)

Открыть **Window → Package Manager**.

### 5.1 Обязательные (Phase 1)

| Package | Откуда | Зачем |
|---------|--------|--------|
| **XR Plug-in Management** | Unity Registry | Включение ARKit/ARCore |
| **AR Foundation** | Unity Registry | Общий AR API |
| **Google ARCore XR Plugin** | Unity Registry | Android |
| **Apple ARKit XR Plugin** | Unity Registry | iOS |
| **Input System** | Unity Registry | Ввод (согласовать Active Input Handling) |

Опционально сразу или чуть позже:

| Package | Зачем |
|---------|--------|
| Addressables | префабы устройств |
| Unity UI / UI Toolkit | меню режимов |

**Не ставить сейчас (фаза 2+):** RoomPlan kits, Firebase, PDF-библиотеки.

Версии: брать совместимый набор под ваш Unity 6 (Package Manager покажет зависимости). Зафиксировать фактические версии в `Packages/manifest.json` и коротко обновить `docs/architecture/stack.md` после установки.

### 5.2 XR Plug-in Management

1. **Edit → Project Settings → XR Plug-in Management**.
2. Вкладка **Android**: ☑ Google ARCore.
3. Вкладка **iOS**: ☑ Apple ARKit.
4. При необходимости инициализировать XR settings (кнопка Create / load defaults).

### 5.3 Player Settings (минимум)

**Android**

- Minimum API Level: **24+** (лучше 26+; свериться с текущей докой AR Foundation / ARCore).
- Scripting Backend: **IL2CPP**.
- Target Architectures: **ARM64**.
- Graphics: Vulkan/OpenGLES3 по требованиям ARCore для вашей версии.
- Internet / Camera: Camera permission (AR).

**iOS**

- Target minimum: по AR Foundation (часто iOS 13+; RoomPlan позже выше).
- Camera Usage Description: строка вроде `AR room scanning and outlet placement`.
- Architecture: ARM64.
- Requires ARKit: YES (когда включите).

**Both**

- Color Space: **Linear** (URP default).
- Active Input Handling: **Input System Package** или Both (если ещё старый input).

### 5.4 Первая AR-сцена (smoke)

1. Scene: `Assets/_Project/Scenes/ARPlacement.unity`.
2. Hierarchy (типичный минимум AF):

   - `AR Session`
   - `XR Origin` (AR) / `AR Session Origin` (имя зависит от версии AF)
   - `AR Camera` (дочерняя)
   - `AR Plane Manager` (horizontal + vertical)
   - `AR Raycast Manager`
   - `AR Anchor Manager` (желательно)

3. Prefab плоскости — из Samples AR Foundation (Package Manager → AR Foundation → Samples → import **Simple AR** / Plane detection) — можно скопировать материалы индикатора плоскости.
4. Скрипт-заглушка: tap → raycast → instantiate «Outlet» prefab (куб).
5. File → Build Settings: добавить сцену, Switch Platform Android или iOS.

Пока **не** нужен полный RoomModel — цель шага: «камера + плоскости + объект в мире».

---

## 6. Развёртывание на Android

1. Телефон по USB, `adb devices` OK.
2. Build Settings → Android → **Switch Platform**.
3. Player Settings заполнены (п. 5.3).
4. **Build And Run** (или Build APK → `adb install -r app.apk`).
5. На устройстве разрешить камеру.
6. Ожидание: плоскости на полу/стенах, тап ставит объект, трекинг держится при медленном движении.

### Чеклист проблем Android

| Симптом | Что проверить |
|---------|----------------|
| Чёрный экран | ARCore установлен; устройство в списке; лог `adb logcat` |
| Нет плоскостей | Свет / текстура пола; vertical+horizontal в Plane Manager |
| Build fail NDK/SDK | External Tools в Unity: пути JDK/SDK/NDK из Hub |
| IL2CPP error | ARM64 only; правильный NDK модуля Hub |

---

## 7. Развёртывание на iOS (Mac)

1. На Windows можно **Build** iOS → папка Xcode-проекта → перенести на Mac **или** сразу билдить на Mac.
2. Mac: открыть `.xcodeproj` / `.xcworkspace`.
3. Signing & Capabilities: Team, Bundle ID, Automatic Signing.
4. Device → Run.
5. Доверие разработчику на iPhone при первой установке.
6. Камера / World Sensing permissions — если OS запросит.

Симулятор iOS **не** заменяет AR на устройстве для этого продукта — только реальное устройство.

---

## 8. Порядок работ после «Hello AR» (код Phase 1)

Не путать с установкой софта — это следующий этап разработки (см. [roadmap](../product/roadmap.md)):

1. `Domain/RoomModel` + JSON save/load по [room-model.md](../architecture/room-model.md)
2. Placement → запись `devices[]` с `wallId` / временной привязкой к plane
3. UI: новый проект / сохранить / загрузить
4. Manual plan (прямоугольник) + align — Phase 3 можно частично раньше, если нужно
5. Только потом Phase 2: RoomPlan adapter

---

## 9. Phase 2 (позже) — RoomPlan на iOS

Краткий задел, **не делать при первом setup**:

1. Mac + LiDAR device + актуальный iOS.
2. Выбор: Swift bridge / [RoomPlan for Unity Kit](https://assetstore.unity.com/packages/tools/utilities/roomplan-for-unity-kit-255058) / возможности AF 6.x (bounding boxes ≠ полный план).
3. Adapter: `CapturedRoom` → тот же `RoomModel`.
4. UI switcher режимов.
5. Обновить `stack.md` + ADR при выборе конкретного kit/bridge.

---

## 10. Git, CI, магазины (по мере роста)

| Этап | Действие |
|------|----------|
| Сейчас | Локальные билды; remote GitHub |
| Когда есть сцена | Не коммитить `Library/`; крупные USDZ — LFS или вне репо (см. `.gitignore`) |
| CI позже | GitHub Actions / Unity Cloud Build: Android job; iOS — Mac runner |
| Store | Google Play internal testing; TestFlight — после стабильного Phase 1–2 |

Backend / Firebase / PDF — **не** ставить до Phase 4.

---

## 11. Чеклист «setup готов»

- [ ] Unity 6 + Android (и при необходимости iOS) modules в Hub  
- [ ] Репо = корень Unity (`Assets/`, `Packages/`, `ProjectSettings/` на месте)  
- [ ] AR Foundation + ARCore (+ ARKit) в manifest  
- [ ] XR Plug-in Management: платформы включены  
- [ ] Сцена с session + plane + raycast + тестовый объект  
- [ ] Build And Run на физическом Android и/или iPhone  
- [ ] Версии пакетов записаны / обновлён [stack.md](../architecture/stack.md)  
- [ ] `docs/README.md` → **Code** обновлён («Unity project exists»)

---

## 12. Типичные ошибки

1. Unity-проект создали **рядом**, а не в git-корне → два корня, агент и docs «не видят» код.  
2. Закоммитили `Library/` → огромный дифф, боли у команды.  
3. Сразу тянут RoomPlan на Windows-only машине → блокер; Phase 1 на Android.  
4. Тест только в Editor без устройства → ложное чувство готовности AR.  
5. Minimum API / ARCore не совпали → чёрный экран на телефоне.

---

## 13. Следующий документ после выполнения setup

Когда Unity-проект лежит в репо: обновить «Now» в [docs/README.md](../README.md) и при необходимости добавить `docs/setup/first-ar-scene.md` с точными именами компонентов вашей версии AF (они чуть плавают между 5.x/6.x).
