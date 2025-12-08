# System Diagrams - 系統架構視覺化

> 這份文件用視覺化圖表展示遊戲系統的架構和資料流

---

## 整體系統架構

```
                        🎮 Game Architecture
                                |
                ┌───────────────┴───────────────┐
                |                               |
           🎯 Core Layer                   🎲 Game Layer
                |                               |
    ┌───────────┴───────────┐          ┌───────┴───────┐
    |                       |          |               |
GameManager          Scene System   Player        Interaction
 (Singleton)                         System         System
    |                                  |               |
    ├─ SpawnPointManager              ├─ Movement     ├─ IInteractable
    ├─ SceneTransitionManager         ├─ StateMachine |    - chair
    └─ PersistenceManager             ├─ Animation    |    - door
                                      ├─ Energy       |    - cable
                                      └─ Interaction  └─ IInteractor
                                           Handler
```

---

## GameManager 架構

```
┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃                     GameManager                       ┃
┃                    (Singleton)                        ┃
┃━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┃
┃                                                       ┃
┃  ┌─────────────────────────────────────────────┐    ┃
┃  │         Sub-Managers (組件)                  │    ┃
┃  │                                              │    ┃
┃  │  ┌──────────────────────────────────┐       │    ┃
┃  │  │   SpawnPointManager              │       │    ┃
┃  │  │   - 管理重生點（椅子）            │       │    ┃
┃  │  │   - 追蹤已發現的椅子              │       │    ┃
┃  │  │   - 記錄當前重生點                │       │    ┃
┃  │  └──────────────────────────────────┘       │    ┃
┃  │                                              │    ┃
┃  │  ┌──────────────────────────────────┐       │    ┃
┃  │  │   SceneTransitionManager         │       │    ┃
┃  │  │   - 處理場景載入和切換            │       │    ┃
┃  │  │   - 記錄場景中的玩家位置          │       │    ┃
┃  │  │   - 決定玩家出現位置              │       │    ┃
┃  │  └──────────────────────────────────┘       │    ┃
┃  │                                              │    ┃
┃  │  ┌──────────────────────────────────┐       │    ┃
┃  │  │   PersistenceManager             │       │    ┃
┃  │  │   - 管理 DontDestroyOnLoad       │       │    ┃
┃  │  │   - 處理跨場景持久化              │       │    ┃
┃  │  └──────────────────────────────────┘       │    ┃
┃  └─────────────────────────────────────────────┘    ┃
┃                                                       ┃
┃  ┌─────────────────────────────────────────────┐    ┃
┃  │         Unified API (統一介面)                │    ┃
┃  │                                              │    ┃
┃  │  • LoadScene(...)                           │    ┃
┃  │  • RegisterSpawnPoint(...)                  │    ┃
┃  │  • TeleportToSpawnPoint(...)                │    ┃
┃  │  • RespawnPlayer()                          │    ┃
┃  │  • SavePlayerPositionForScene(...)          │    ┃
┃  └─────────────────────────────────────────────┘    ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
```

---

## PlayerController 組件架構

```
┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃              PlayerController                    ┃
┃          (實作 IInteractor)                      ┃
┃━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┃
┃                                                   ┃
┃  Input Layer (輸入層)                             ┃
┃  ┌────────────────────────────────────────┐     ┃
┃  │ ← → Space  U  D  R  M                  │     ┃
┃  │ 移動 跳躍 互動 離開 重生 快速旅行        │     ┃
┃  └────────────────────────────────────────┘     ┃
┃              ↓                                    ┃
┃  Control Layer (控制層)                           ┃
┃  ┌────────────────────────────────────────┐     ┃
┃  │ HandleInput()                          │     ┃
┃  │ - 收集輸入                              │     ┃
┃  │ - 檢查狀態鎖定                          │     ┃
┃  │ - 分發指令到子系統                      │     ┃
┃  └────────────────────────────────────────┘     ┃
┃              ↓                                    ┃
┃  Component Layer (組件層)                         ┃
┃  ┌──────────────┬──────────────┬─────────────┐  ┃
┃  │              │              │             │  ┃
┃  │ Movement     │ StateMachine │ Animation   │  ┃
┃  │              │              │             │  ┃
┃  │ • Walk       │ • Idle       │ • Animator  │  ┃
┃  │ • Jump       │ • Walking    │ • Blend     │  ┃
┃  │ • Physics    │ • Jumping    │   Trees     │  ┃
┃  │ • Ground     │ • Falling    │             │  ┃
┃  │   Check      │ • Sitting    │             │  ┃
┃  │              │ • Rappelling │             │  ┃
┃  └──────────────┴──────────────┴─────────────┘  ┃
┃                                                   ┃
┃  ┌──────────────┬──────────────────────────┐    ┃
┃  │              │                          │    ┃
┃  │ Energy       │ InteractionHandler       │    ┃
┃  │              │                          │    ┃
┃  │ • Max: 5     │ • Zone Detection         │    ┃
┃  │ • Current    │ • IInteractable          │    ┃
┃  │ • Consume    │   Management             │    ┃
┃  │ • Restore    │ • Trigger Handling       │    ┃
┃  └──────────────┴──────────────────────────┘    ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
```

---

## 場景載入流程圖

```
開始
  │
  ├─ 使用者觸發載入
  │  (進入觸發區 / 點擊快速旅行 / 重生)
  │
  ↓
┌───────────────────────────────────┐
│ GameManager.LoadScene()           │
│ or TeleportToSpawnPoint()         │
└───────────────────────────────────┘
  │
  ↓
┌───────────────────────────────────┐
│ SceneTransitionManager            │
│ - SaveCurrentPlayerPosition()     │  ← 保存當前場景的玩家位置
└───────────────────────────────────┘
  │
  ↓
┌───────────────────────────────────┐
│ Unity SceneManager                │
│ - SceneManager.LoadScene()        │  ← Unity 引擎載入場景
└───────────────────────────────────┘
  │
  ↓
┌───────────────────────────────────┐
│ OnSceneLoaded Event               │  ← 場景載入完成
└───────────────────────────────────┘
  │
  ↓
┌───────────────────────────────────┐
│ GameManager.OnSceneTransitioned() │
└───────────────────────────────────┘
  │
  ↓
┌───────────────────────────────────────────────────┐
│ SceneTransitionManager                            │
│ .MovePlayerToScenePosition()                      │
│                                                   │
│ 優先順序：                                         │
│ 1. Scene Saved Position (場景保存位置)             │
│    ├─ Yes → 移動到保存位置                         │
│    └─ No  → 繼續                                  │
│                                                   │
│ 2. Last Spawn Point (最後重生點)                  │
│    ├─ Yes & Same Scene → 移動到 Spawn Point       │
│    └─ No → 繼續                                   │
│                                                   │
│ 3. PlayerSpawn Tag (預設生成點)                   │
│    ├─ Found → 移動到 Tag 位置                     │
│    └─ Not Found → 繼續                            │
│                                                   │
│ 4. Keep Current Position (保持位置)              │
│    └─ 不移動玩家                                  │
└───────────────────────────────────────────────────┘
  │
  ↓
完成
```

---

## 重生流程圖

```
玩家按 R 鍵 或 死亡
  │
  ↓
┌──────────────────────────────┐
│ GameManager.RespawnPlayer()  │
└──────────────────────────────┘
  │
  ↓
┌──────────────────────────────┐
│ 取得當前 Spawn Point          │
│ GetCurrentSpawnPoint()       │
└──────────────────────────────┘
  │
  ├─ Spawn Point 存在？
  │  ├─ No → ❌ 顯示警告訊息
  │  └─ Yes → 繼續
  │
  ↓
┌────────────────────────────────────┐
│ 清除目標場景的保存位置               │
│ ClearScenePosition(sceneName)      │
│                                    │
│ 🎯 關鍵：確保使用 Spawn Point       │
│         而不是場景保存位置          │
└────────────────────────────────────┘
  │
  ↓
┌──────────────────────────────┐
│ TeleportToSpawnPoint(id)     │
└──────────────────────────────┘
  │
  ├─ Spawn Point 在不同場景？
  │  ├─ Yes → LoadScene(sceneName)
  │  │         ↓
  │  │        等待場景載入
  │  │         ↓
  │  │        移動到 Spawn Point
  │  │
  │  └─ No  → 直接移動到 Spawn Point
  │
  ↓
┌──────────────────────────────┐
│ 玩家出現在 Spawn Point        │
│ - 恢復能量                    │
│ - 恢復控制                    │
└──────────────────────────────┘
  │
  ↓
完成
```

---

## 互動系統架構

```
                    ┌──────────────────┐
                    │   IInteractor    │
                    │   (介面)         │
                    └────────┬─────────┘
                             │
                     ┌───────▽────────┐
                     │ PlayerController│
                     │   (實作者)      │
                     └────────┬────────┘
                              │
                ┌─────────────┼─────────────┐
                │             │             │
                │    InteractionHandler     │
                │    - OnTriggerEnter2D     │
                │    - OnTriggerExit2D      │
                │    - 管理互動區域          │
                └─────────────┬─────────────┘
                              │
                    檢測到 IInteractable
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ↓                     ↓                     ↓
┌───────────────┐     ┌───────────────┐     ┌───────────────┐
│    chair      │     │     door      │     │    cable      │
│  (IInteractable)│     │ (IInteractable)│     │ (IInteractable)│
├───────────────┤     ├───────────────┤     ├───────────────┤
│ • 顯示提示    │     │ • 檢查鑰匙    │     │ • 開始垂降    │
│ • 坐下        │     │ • 打開門      │     │ • 鎖定輸入    │
│ • 保存進度    │     │ • 轉換場景    │     │ • 移動玩家    │
│ • 恢復能量    │     └───────────────┘     │ • 播放動畫    │
└───────────────┘                           └───────────────┘
        │
        ↓
┌─────────────────────────────┐
│ 保存為 Spawn Point           │
│                             │
│ GameManager                 │
│  .RegisterSpawnPoint()      │
│  .SetCurrentSpawnPoint()    │
└─────────────────────────────┘
```

---

## 狀態機轉換圖

```
                    PlayerStateMachine

     ┌────────────────────────────────────────────┐
     │                                            │
     │         ┌──────────┐                      │
     │    ┌───▷   Idle   ◁───┐                  │
     │    │    └─┬─────┬─┘   │                  │
     │    │      │     │      │                  │
     │    │  walk│jump │land  │stand             │
     │    │      │     │      │                  │
     │    │    ┌─▽─────▽─┐   │                  │
     │    │    │ Walking  │   │                  │
     │    │    └─────┬───┘   │                  │
     │    │          │jump    │                  │
     │    │          ↓        │                  │
     │    │    ┌─────────┐   │                  │
     │    └────│ Jumping │   │                  │
     │         └────┬────┘   │                  │
     │              │fall     │                  │
     │              ↓         │                  │
     │         ┌─────────┐   │                  │
     │         │ Falling ├───┘                  │
     │         └────┬────┘                       │
     │              │land                        │
     │              └─────────┐                  │
     │                        │                  │
     │    ┌───────────┐      │                  │
     │    │  Sitting  │      │                  │
     │    └─────▲─────┘      │                  │
     │          │sit          │                  │
     │          └─────────────┘                  │
     │                                            │
     │    ┌─────────────┐                        │
     │    │ Rappelling  │                        │
     │    └──────┬──────┘                        │
     │           │complete                        │
     │           └───────→ Idle                  │
     │                                            │
     │    ┌──────────┐                           │
     │    │   Dead   │                           │
     │    └────┬─────┘                           │
     │         │respawn                           │
     │         └────────→ Idle                   │
     └────────────────────────────────────────────┘

狀態轉換規則：
• Idle    → Walking, Jumping, Sitting, Rappelling
• Walking → Idle, Jumping
• Jumping → Falling
• Falling → Idle, Walking
• Sitting → Idle
• Rappelling → Idle
• Dead    → Idle (重生後)
```

---

## UI 系統架構

```
                        UI System
                            │
        ┌───────────────────┼───────────────────┐
        │                   │                   │
        ↓                   ↓                   ↓
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│ FastTravelUI │    │ EnergyUIDisplay│    │ InventoryUI │
└──────┬───────┘    └──────┬───────┘    └──────┬───────┘
       │                   │                   │
       │                   │                   │
       ├─ 取得 Spawn       ├─ 監聽能量變化     ├─ 顯示物品
       │  Points           │                   │
       │  from             ├─ 更新色塊顯示     ├─ 物品管理
       │  GameManager      │                   │
       │                   │                   │
       ├─ 生成按鈕         └─ UI Blocks        └─ Grid Layout
       │                     (Color)
       │
       ├─ 處理點擊
       │   ↓
       └─ GameManager
          .TeleportToSpawnPoint()


快速旅行 UI 流程：

按 M 鍵
  ↓
FastTravelUI.OpenMenu()
  ↓
取得已發現的 Spawn Points
  ↓
為每個 Spawn Point 生成按鈕
  ├─ 設定按鈕文字 (椅子名稱)
  ├─ 設定位置描述
  ├─ 標記當前 Spawn Point (★)
  └─ 綁定點擊事件
  ↓
玩家點擊按鈕
  ↓
OnTravelButtonClicked(spawnPointId)
  ↓
GameManager.TeleportToSpawnPoint(id)
  ↓
關閉選單
```

---

## 資料流向圖

### Spawn Point 註冊流程

```
玩家坐在椅子上
        │
        ↓
┌─────────────────────────┐
│ chair.Interact()        │
│ - 觸發互動               │
└────────┬────────────────┘
         │
         ↓
┌─────────────────────────┐
│ PlayerController        │
│ .SitOnChair(chair)      │
│ - 設定 Sitting 狀態     │
│ - 鎖定輸入              │
└────────┬────────────────┘
         │
         ↓
┌─────────────────────────┐
│ chair.SaveProgress()    │
└────────┬────────────────┘
         │
         ├──────────────┐
         ↓              ↓
┌─────────────────┐  ┌─────────────────────┐
│ GameManager     │  │ 播放音效和視覺效果  │
│ .RegisterSpawn  │  └─────────────────────┘
│  Point()        │
└─────┬───────────┘
      │
      ↓
┌─────────────────────────────┐
│ SpawnPointManager           │
│ - 檢查是否已存在            │
│ - 註冊或更新 Spawn Point    │
└─────┬───────────────────────┘
      │
      ↓
┌─────────────────────────────┐
│ GameManager                 │
│ .SetCurrentSpawnPoint(id)   │
└─────┬───────────────────────┘
      │
      ↓
┌─────────────────────────────┐
│ SpawnPointManager           │
│ - 設定為當前重生點          │
│ - currentSpawnPoint = ...   │
└─────────────────────────────┘
      │
      ↓
┌─────────────────────────────┐
│ 更新 UI                     │
│ - FastTravelUI 可以顯示     │
│ - 標記 (★) 當前重生點      │
└─────────────────────────────┘
```

### 快速旅行資料流

```
FastTravelUI
      │
      ↓ 開啟選單
┌─────────────────────────────────────┐
│ GameManager.GetDiscoveredSpawnPoints()│
└─────────┬───────────────────────────┘
          │
          ↓
┌─────────────────────────────────────┐
│ SpawnPointManager                   │
│ .GetDiscoveredSpawnPoints()         │
│                                     │
│ Returns: List<SpawnPointData>       │
└─────────┬───────────────────────────┘
          │
          ↓
┌─────────────────────────────────────┐
│ FastTravelUI                        │
│ - 為每個 SpawnPoint 生成 Button     │
│ - 顯示名稱、位置、標記當前          │
└─────────┬───────────────────────────┘
          │
          ↓ 玩家點擊
┌─────────────────────────────────────┐
│ OnTravelButtonClicked(id)           │
└─────────┬───────────────────────────┘
          │
          ↓
┌─────────────────────────────────────┐
│ GameManager                         │
│ .TeleportToSpawnPoint(id)           │
└─────────┬───────────────────────────┘
          │
          ↓
┌─────────────────────────────────────┐
│ 1. 取得 SpawnPointData              │
│ 2. 設定為當前 Spawn Point           │
│ 3. 保存當前位置                     │
│ 4. 檢查是否需要載入場景             │
│    ├─ 同場景 → 直接傳送            │
│    └─ 不同場景 → 載入場景後傳送    │
└─────────────────────────────────────┘
```

---

## 依賴關係圖

```
Layer 1: Foundation (基礎層)
┌─────────────────────────────────────────────┐
│ Unity Engine                                │
│ - MonoBehaviour                             │
│ - SceneManager                              │
│ - Physics2D                                 │
└──────────────────┬──────────────────────────┘
                   │
Layer 2: Core Management (核心管理層)
┌──────────────────▼──────────────────────────┐
│ GameManager (Singleton)                     │
│  ├─ SpawnPointManager                       │
│  ├─ SceneTransitionManager                  │
│  └─ PersistenceManager                      │
└──────────┬───────────────┬──────────────────┘
           │               │
           │               │
Layer 3: Game Systems (遊戲系統層)
┌──────────▼──────┐  ┌────▼─────────────────┐
│ Player System   │  │ Interaction System   │
│ - Controller    │  │ - IInteractable      │
│ - Movement      │  │ - IInteractor        │
│ - StateMachine  │  │ - chair / door       │
│ - Animation     │  └──────────────────────┘
│ - Energy        │
└─────────┬───────┘
          │
Layer 4: Presentation (呈現層)
┌─────────▼───────────────────────────────────┐
│ UI System          Camera System            │
│ - FastTravelUI     - CameraFollow           │
│ - EnergyDisplay                             │
│ - InventoryUI                               │
└─────────────────────────────────────────────┘

依賴方向：上層依賴下層，下層不依賴上層
```

---

## 事件系統流程

```
                    Event System
                         │
        ┌────────────────┼────────────────┐
        │                │                │
   Publisher         Subscriber      Subscriber
  (發布者)          (訂閱者 1)      (訂閱者 2)
        │                │                │
        │                │                │
┌───────▼──────────┐     │                │
│ SceneTransition  │     │                │
│ Manager          │     │                │
│                  │     │                │
│ Event:           │     │                │
│ OnSceneTransition│     │                │
│ Complete         │     │                │
└───────┬──────────┘     │                │
        │                │                │
        │   Subscribe    │                │
        │   ◁────────────┘                │
        │                                 │
        │   Subscribe                     │
        │   ◁─────────────────────────────┘
        │
        │ Scene Loaded!
        │
        ├─ Invoke Event
        │
        └─→ OnSceneTransitionComplete?.Invoke(sceneName)
                │
                ├──────────────┬──────────────┐
                ↓              ↓              ↓
        ┌───────────────┐ ┌────────────┐ ┌────────────┐
        │ Subscriber 1  │ │Subscriber 2│ │Subscriber N│
        │               │ │            │ │            │
        │ OnSceneTrans  │ │OnSceneTrans│ │OnSceneTrans│
        │ itioned()     │ │itioned()   │ │itioned()   │
        │ - 處理邏輯    │ │- 處理邏輯  │ │- 處理邏輯  │
        └───────────────┘ └────────────┘ └────────────┘

程式碼範例：

// 定義事件
public delegate void SceneLoadedHandler(string sceneName);
public event SceneLoadedHandler OnSceneTransitionComplete;

// 訂閱事件
sceneManager.OnSceneTransitionComplete += HandleSceneLoaded;

// 發布事件
OnSceneTransitionComplete?.Invoke(sceneName);

// 取消訂閱
sceneManager.OnSceneTransitionComplete -= HandleSceneLoaded;
```

---

## 記憶體管理 - DontDestroyOnLoad

```
Scene A                Scene B                Scene C
────────────────────────────────────────────────────────
   │                      │                      │
   │  ┌────────────────────────────────────┐    │
   │  │   GameManager (Singleton)          │    │
   │  │   DontDestroyOnLoad                │    │
   │  │                                    │    │
   │  │  ┌──────────────────────────┐     │    │
   │  │  │ SpawnPointManager        │     │    │
   │  │  │ - discoveredSpawnPoints  │     │    │
   │  │  │ - currentSpawnPoint      │     │    │
   │  │  └──────────────────────────┘     │    │
   │  │                                    │    │
   │  │  ┌──────────────────────────┐     │    │
   │  │  │ SceneTransitionManager   │     │    │
   │  │  │ - scenePlayerPositions   │     │    │
   │  │  │ - lastSceneName          │     │    │
   │  │  └──────────────────────────┘     │    │
   │  └────────────────────────────────────┘    │
   │             │                   │           │
   │             │ 持久化資料不會消失  │           │
   │             ↓                   ↓           │
   │       Scene A 資料         Scene B 資料      │
   │                                              │
   ↓                          ↓                   ↓
Player              Player              Player
in Scene A          in Scene B          in Scene C
(位置記錄)          (位置記錄)          (位置記錄)

當場景切換時：
1. GameManager 留在記憶體中
2. 儲存當前場景的玩家位置
3. 載入新場景
4. 根據記錄決定玩家位置
```

---

## 設計模式視覺化

### Singleton Pattern - GameManager

```
┌─────────────────────────────────────────────┐
│             Application                     │
│                                             │
│   ┌─────────────────────────────────────┐  │
│   │      Multiple Access Points         │  │
│   │                                     │  │
│   │  System A    System B    System C  │  │
│   │     │            │            │     │  │
│   │     └────────────┼────────────┘     │  │
│   │                  │                   │  │
│   │                  ↓                   │  │
│   │     GameManager.GetInstance()       │  │
│   │                  │                   │  │
│   │                  ↓                   │  │
│   │         ┌────────────────┐           │  │
│   │         │  GameManager   │ ← 唯一實例│  │
│   │         │  (Instance)    │           │  │
│   │         └────────────────┘           │  │
│   └─────────────────────────────────────┘  │
│                                             │
│  特性：                                      │
│  • 全域唯一實例                              │
│  • 跨場景持久化 (DontDestroyOnLoad)         │
│  • 懶加載（第一次訪問時創建）                 │
└─────────────────────────────────────────────┘
```

### Observer Pattern - Event System

```
┌─────────────────────────────────────────────┐
│         Observer Pattern Flow               │
│                                             │
│  ┌────────────────┐                        │
│  │   Subject      │                        │
│  │  (Observable)  │                        │
│  │                │                        │
│  │  OnSceneLoaded │                        │
│  │     Event      │                        │
│  └────────┬───────┘                        │
│           │                                 │
│           │ Subscribe                       │
│           │                                 │
│     ┌─────┼─────┬─────────────┐           │
│     │     │     │             │           │
│     ↓     ↓     ↓             ↓           │
│  ┌────┐┌────┐┌────┐       ┌────┐         │
│  │Obs1││Obs2││Obs3│  ...  │ObsN│         │
│  └────┘└────┘└────┘       └────┘         │
│                                             │
│  當事件發生時：                              │
│     Subject.Invoke()                       │
│         ↓                                   │
│     通知所有 Observers                       │
│         ↓                                   │
│     Observers 執行各自的處理邏輯             │
│                                             │
│  優點：                                      │
│  • 鬆耦合                                   │
│  • 一對多依賴                                │
│  • 動態訂閱/取消訂閱                         │
└─────────────────────────────────────────────┘
```

### Component Pattern - PlayerController

```
┌─────────────────────────────────────────────┐
│       Component Pattern Structure           │
│                                             │
│  ┌─────────────────────────────────────┐   │
│  │      PlayerController (Core)        │   │
│  │         (Coordinator)               │   │
│  └──────────────┬──────────────────────┘   │
│                 │                           │
│        ┌────────┼────────┐                 │
│        │        │        │                 │
│        ↓        ↓        ↓                 │
│   ┌────────┐┌──────┐┌──────────┐          │
│   │Movement││State ││Animation │          │
│   │        ││Machine│          │          │
│   └────────┘└──────┘└──────────┘          │
│                 │                           │
│        ┌────────┼────────┐                 │
│        ↓        ↓        ↓                 │
│   ┌────────┐┌──────┐┌──────────┐          │
│   │Energy  ││Inter ││  ...     │          │
│   │        ││action││          │          │
│   └────────┘└──────┘└──────────┘          │
│                                             │
│  每個組件：                                  │
│  • 獨立的職責                                │
│  • 可獨立測試                                │
│  • 可重複使用                                │
│  • Core 負責協調                             │
└─────────────────────────────────────────────┘
```

---

## 效能考量視覺化

### Good: Component Caching

```
❌ 不好的做法：每幀都查找
─────────────────────────────
void Update() {
    GetComponent<PlayerMovement>().Move();  // 慢！
    GetComponent<StateMachine>().Update();   // 慢！
}

執行時間：
Frame 1: GetComponent × 2 = ~0.02ms
Frame 2: GetComponent × 2 = ~0.02ms
...
60 FPS × 2 calls = 120 次查找/秒

✅ 好的做法：快取引用
─────────────────────────────
private PlayerMovement movement;
private StateMachine stateMachine;

void Awake() {
    movement = GetComponent<PlayerMovement>();
    stateMachine = GetComponent<StateMachine>();
}

void Update() {
    movement.Move();      // 快！
    stateMachine.Update(); // 快！
}

執行時間：
Awake: GetComponent × 2 = ~0.02ms (只執行一次)
Update: 直接訪問 = ~0.0001ms
60 FPS × 2 calls = 120 次直接訪問/秒

效能提升：~200 倍
```

### Object Reference vs Find

```
❌ 避免：
─────────────────────────────
void Update() {
    GameObject player = GameObject.Find("Player");  // 非常慢！
    player.transform.position = ...;
}

GameObject.Find 需要遍歷場景中所有物件

✅ 推薦：
─────────────────────────────
[SerializeField] private GameObject player;

void Update() {
    player.transform.position = ...;  // 快！
}

直接引用，零查找成本
```

---

**最後更新**: 2025-10-20  
**維護者**: AI Assistant

這些圖表可以幫助你快速理解系統架構和資料流向。如需更詳細的說明，請參閱 [`GAME_ARCHITECTURE.md`](./GAME_ARCHITECTURE.md)。

