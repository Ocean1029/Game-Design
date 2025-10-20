# Game Architecture - 遊戲架構文件

> **版本**: 1.0  
> **最後更新**: 2025-10-20  
> **Unity 版本**: 6000.2.2f1

---

## 目錄

1. [架構總覽](#架構總覽)
2. [系統層級結構](#系統層級結構)
3. [核心模組](#核心模組)
4. [資料流向](#資料流向)
5. [設計模式](#設計模式)
6. [檔案組織](#檔案組織)
7. [擴展指南](#擴展指南)

---

## 架構總覽

這是一個 2D 平台動作遊戲，採用模組化架構設計，核心系統包括玩家控制、互動系統、場景管理、UI 系統和持久化存檔。

### 系統架構圖

```
┌─────────────────────────────────────────────────────────────────┐
│                         GameManager                              │
│                     (Singleton - 核心協調器)                      │
│  ┌────────────────┐  ┌──────────────────┐  ┌─────────────────┐ │
│  │ SpawnPoint     │  │ SceneTransition  │  │  Persistence    │ │
│  │ Manager        │  │ Manager          │  │  Manager        │ │
│  └────────────────┘  └──────────────────┘  └─────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                            ↓ ↑
        ┌───────────────────┼───────────────────┐
        ↓                   ↓                   ↓
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│   Player     │←──→│ Interaction  │    │   Camera     │
│   System     │    │   System     │    │   System     │
└──────────────┘    └──────────────┘    └──────────────┘
        ↓                   ↓                   ↓
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│  UI System   │    │  World       │    │   Scene      │
│              │    │  Objects     │    │   Objects    │
└──────────────┘    └──────────────┘    └──────────────┘
```

### 核心設計原則

1. **單一職責原則 (SRP)**: 每個類別只負責一項功能
2. **開放封閉原則 (OCP)**: 透過介面擴展功能，而非修改既有程式碼
3. **依賴反轉原則 (DIP)**: 透過介面降低耦合度
4. **模組化設計**: 各系統可獨立開發和測試

---

## 系統層級結構

### Layer 1: Core Management (核心管理層)

這一層負責全局管理和系統協調。

#### GameManager (遊戲管理器)
**位置**: `Scripts/Manager/GameManager.cs`  
**職責**:
- 作為遊戲的核心協調器（Singleton）
- 管理所有子管理器的生命週期
- 提供統一的 API 給其他系統使用
- 處理跨場景的持久化

**關鍵 API**:
```csharp
// Singleton access
GameManager.GetInstance()

// Scene management
LoadScene(string sceneName)
LoadScene(int sceneIndex)

// Spawn point management
RegisterSpawnPoint(...)
SetCurrentSpawnPoint(...)
TeleportToSpawnPoint(string spawnPointId)
RespawnPlayer()

// Position management
SavePlayerPositionForScene(...)
ClearScenePosition(string sceneName)
```

#### SpawnPointManager (生成點管理器)
**位置**: `Scripts/Manager/SpawnPointManager.cs`  
**職責**:
- 管理所有已發現的 spawn point (椅子)
- 追蹤當前的重生點
- 提供 spawn point 查詢功能

**資料結構**:
```csharp
SpawnPointData {
    string spawnPointId;
    string displayName;
    string locationDescription;
    Vector3 position;
    string sceneName;
}
```

#### SceneTransitionManager (場景轉換管理器)
**位置**: `Scripts/Manager/SceneTransitionManager.cs`  
**職責**:
- 處理場景載入和切換
- 管理玩家在各場景的位置記錄
- 決定玩家進入場景時的出現位置

**位置優先順序**:
1. Scene Saved Position（場景切換時使用）
2. Last Spawn Point（重生時使用）
3. Default PlayerSpawn Tag（預設生成點）
4. Current Position（保持當前位置）

#### PersistenceManager (持久化管理器)
**位置**: `Scripts/Manager/PersistenceManager.cs`  
**職責**:
- 管理 DontDestroyOnLoad 物件
- 處理跨場景的物件持久化

---

### Layer 2: Player System (玩家系統層)

這一層處理所有玩家相關的功能。

#### PlayerController (玩家控制器)
**位置**: `Scripts/Player/PlayerController.cs`  
**職責**: 
- 統一的玩家控制入口
- 協調所有玩家子系統
- 處理玩家輸入
- 實作 IInteractor 介面

**組成部分**:
```
PlayerController
├── PlayerMovement          (物理和移動)
├── PlayerStateMachine      (狀態管理)
├── PlayerAnimationController (動畫控制)
├── InteractionHandler      (互動處理)
└── PlayerEnergy           (能量系統)
```

#### PlayerMovement (移動控制)
**位置**: `Scripts/Player/PlayerMovement.cs`  
**職責**:
- 處理角色物理移動
- 實作跳躍機制
- 地面檢測
- 速度控制

**關鍵參數**:
- Walk Speed: 行走速度
- Jump Force: 跳躍力度
- Ground Check: 地面檢測設定

#### PlayerStateMachine (狀態機)
**位置**: `Scripts/Player/PlayerStateMachine.cs`  
**職責**:
- 管理玩家狀態轉換
- 控制輸入鎖定
- 提供狀態查詢

**狀態定義** (`PlayerState.cs`):
```csharp
enum PlayerState {
    Idle,
    Walking,
    Jumping,
    Falling,
    Sitting,
    Rappelling,
    Dead
}
```

#### PlayerEnergy (能量系統)
**位置**: `Scripts/Player/PlayerEnergy.cs`  
**職責**:
- 管理玩家能量值
- 限制跳躍次數
- 提供能量恢復機制

#### InteractionHandler (互動處理器)
**位置**: `Scripts/Player/InteractionHandler.cs`  
**職責**:
- 處理與世界物件的互動
- 管理互動觸發器
- 協調 IInteractor 和 IInteractable

---

### Layer 3: Interaction System (互動系統層)

這一層定義了遊戲中的互動機制。

#### IInteractable Interface (可互動介面)
**位置**: `Scripts/Interaction/IInteractable.cs`  
**定義**:
```csharp
interface IInteractable {
    void OnInteractorEnterZone(IInteractor interactor);
    void OnInteractorExitZone(IInteractor interactor);
    bool Interact(IInteractor interactor);
    GameObject GetGameObject();
}
```

#### IInteractor Interface (互動者介面)
**位置**: `Scripts/Interaction/IInteractor.cs`  
**定義**:
```csharp
interface IInteractor {
    GameObject GetGameObject();
    Vector3 GetPosition();
}
```

#### 互動物件實作

##### chair (椅子)
**位置**: `Scripts/Interaction/chair.cs`  
**功能**:
- 玩家可以坐下休息
- 作為 spawn point 保存進度
- 恢復玩家能量
- 提供快速旅行功能

**工作流程**:
```
玩家靠近椅子 → 顯示提示 → 按 U 坐下 → 保存 Spawn Point → 恢復能量
```

##### door (門)
**位置**: `Scripts/Interaction/door.cs`  
**功能**:
- 需要鑰匙才能開啟
- 觸發場景轉換
- 可配置目標場景

##### cable (繩索)
**位置**: `Scripts/Interaction/cable.cs`  
**功能**:
- 垂降到下方區域
- 播放垂降動畫
- 暫時鎖定玩家輸入

---

### Layer 4: UI System (UI 系統層)

這一層處理所有使用者介面。

#### FastTravelUI (快速旅行界面)
**位置**: `Scripts/UI/FastTravelUI.cs`  
**功能**:
- 顯示已發現的椅子列表
- 提供快速旅行選項
- 標示當前 spawn point
- 可選擇暫停遊戲時間

**使用方式**:
```
按 M 鍵 → 打開快速旅行選單 → 選擇椅子 → 傳送
```

#### EnergyUIDisplay (能量顯示)
**位置**: `Scripts/UI/EnergyUIDisplay.cs`  
**功能**:
- 以色塊顯示當前能量
- 自動更新顯示
- 視覺化能量消耗

#### InventoryUI (物品欄界面)
**位置**: `Scripts/UI/Item/InventoryUI.cs`  
**功能**:
- 顯示玩家持有的物品
- 管理物品欄 UI

#### JumpUI (跳躍界面)
**位置**: `Scripts/UI/UI/JumpUI.cs`  
**功能**:
- 顯示跳躍相關資訊
- 開發工具用途

#### RetryButton (重試按鈕)
**位置**: `Scripts/UI/UI/RetryButton.cs`  
**功能**:
- 提供重生功能
- UI 按鈕互動

---

### Layer 5: Support Systems (支援系統層)

#### CameraFollow (相機跟隨)
**位置**: `Scripts/Camera/CameraFollow.cs`  
**功能**:
- 平滑跟隨玩家
- 可設定跟隨偏移
- 邊界限制

#### SceneTransition (場景轉換觸發器)
**位置**: `Scripts/Manager/SceneTransition.cs`  
**功能**:
- 在場景中放置觸發區域
- 當玩家進入時載入指定場景
- 可配置目標場景和生成點

---

## 資料流向

### 場景載入流程

```
1. 玩家觸發場景切換
   ↓
2. SceneTransition 或 FastTravelUI 呼叫 GameManager.LoadScene()
   ↓
3. GameManager 委託給 SceneTransitionManager
   ↓
4. SceneTransitionManager 保存當前場景的玩家位置
   ↓
5. Unity SceneManager 載入新場景
   ↓
6. SceneTransitionManager.OnSceneLoaded 被觸發
   ↓
7. 通知 GameManager 場景已載入
   ↓
8. GameManager.OnSceneTransitioned 被呼叫
   ↓
9. SceneTransitionManager.MovePlayerToScenePosition 決定玩家位置
   ↓
10. 完成場景轉換
```

### 重生流程

```
1. 玩家按 R 鍵（或死亡）
   ↓
2. PlayerController 呼叫 GameManager.RespawnPlayer()
   ↓
3. GameManager 取得當前 spawn point
   ↓
4. 清除目標場景的保存位置（確保使用 spawn point）
   ↓
5. 呼叫 TeleportToSpawnPoint()
   ↓
6. 如果 spawn point 在不同場景，載入該場景
   ↓
7. 玩家移動到 spawn point 位置
   ↓
8. 完成重生
```

### 存檔進度流程

```
1. 玩家坐在椅子上
   ↓
2. chair.Interact() 被呼叫
   ↓
3. PlayerController.SitOnChair() 執行
   ↓
4. chair.SaveProgress() 執行
   ↓
5. 呼叫 GameManager.RegisterSpawnPoint()
   ↓
6. SpawnPointManager 註冊或更新 spawn point
   ↓
7. 呼叫 GameManager.SetCurrentSpawnPoint()
   ↓
8. SpawnPointManager 設定當前重生點
   ↓
9. 播放音效和視覺效果
   ↓
10. 恢復玩家能量
```

### 快速旅行流程

```
1. 玩家按 M 鍵打開快速旅行選單
   ↓
2. FastTravelUI.OpenMenu() 執行
   ↓
3. 從 GameManager 取得所有已發現的 spawn points
   ↓
4. 為每個 spawn point 生成 UI 按鈕
   ↓
5. 玩家點擊某個椅子按鈕
   ↓
6. FastTravelUI.OnTravelButtonClicked() 執行
   ↓
7. 呼叫 GameManager.TeleportToSpawnPoint()
   ↓
8. 設定為當前 spawn point
   ↓
9. 如果在不同場景，載入該場景
   ↓
10. 移動玩家到 spawn point 位置
```

---

## 設計模式

### 1. Singleton Pattern (單例模式)

**使用位置**: GameManager

**目的**: 確保只有一個遊戲管理器實例存在，並提供全局訪問點。

**實作**:
```csharp
private static GameManager instance;

void Awake() {
    if (instance != null && instance != this) {
        Destroy(gameObject);
        return;
    }
    instance = this;
    DontDestroyOnLoad(gameObject);
}

public static GameManager GetInstance() {
    return instance;
}
```

### 2. Component Pattern (組件模式)

**使用位置**: PlayerController 和其子系統

**目的**: 將複雜系統分解成多個獨立的組件，每個組件負責單一功能。

**實作**:
```csharp
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayerAnimationController))]
public class PlayerController : MonoBehaviour {
    private PlayerMovement movement;
    private PlayerStateMachine stateMachine;
    // ...
}
```

### 3. Observer Pattern (觀察者模式)

**使用位置**: SceneTransitionManager 事件系統

**目的**: 當場景載入完成時，通知相關系統進行處理。

**實作**:
```csharp
// Event definition
public event SceneLoadedHandler OnSceneTransitionComplete;

// Subscription
sceneTransitionManager.OnSceneTransitionComplete += OnSceneTransitioned;

// Notification
OnSceneTransitionComplete?.Invoke(scene.name);
```

### 4. State Pattern (狀態模式)

**使用位置**: PlayerStateMachine

**目的**: 管理玩家的不同狀態和狀態轉換。

**實作**:
```csharp
public enum PlayerState {
    Idle, Walking, Jumping, Falling, Sitting, Rappelling, Dead
}

public void SetState(PlayerState newState) {
    currentState = newState;
    OnStateChanged?.Invoke(newState);
}
```

### 5. Strategy Pattern (策略模式)

**使用位置**: IInteractable 介面

**目的**: 定義可互換的互動行為，讓不同物件有不同的互動方式。

**實作**:
```csharp
interface IInteractable {
    bool Interact(IInteractor interactor);
}

// Different strategies
class chair : IInteractable { /* sit behavior */ }
class door : IInteractable { /* open behavior */ }
class cable : IInteractable { /* rappel behavior */ }
```

### 6. Manager Pattern (管理器模式)

**使用位置**: 所有 Manager 類別

**目的**: 集中管理特定領域的功能和資料。

**結構**:
```
GameManager (Master Manager)
├── SpawnPointManager
├── SceneTransitionManager
└── PersistenceManager
```

---

## 檔案組織

### 資料夾結構

```
Assets/Scripts/
├── Camera/                    # 相機系統
│   └── CameraFollow.cs
│
├── Interaction/               # 互動系統
│   ├── IInteractable.cs      # 可互動介面
│   ├── IInteractor.cs        # 互動者介面
│   ├── chair.cs              # 椅子（存檔點）
│   ├── door.cs               # 門
│   └── cable.cs              # 繩索
│
├── Manager/                   # 管理系統
│   ├── GameManager.cs        # 核心管理器
│   ├── SpawnPointManager.cs  # 生成點管理
│   ├── SceneTransitionManager.cs  # 場景轉換
│   ├── PersistenceManager.cs     # 持久化管理
│   ├── SceneTransition.cs    # 場景觸發器
│   ├── REFACTORING_GUIDE.md  # 重構指南
│   └── SPAWN_POINT_SYSTEM_README.md  # Spawn Point 系統說明
│
├── Player/                    # 玩家系統
│   ├── PlayerController.cs   # 玩家控制器（協調者）
│   ├── PlayerMovement.cs     # 移動控制
│   ├── PlayerState.cs        # 狀態定義
│   ├── PlayerStateMachine.cs # 狀態機
│   ├── PlayerAnimationController.cs  # 動畫控制
│   ├── PlayerEnergy.cs       # 能量系統
│   ├── PlayerInteractionController.cs  # 互動控制（舊版，待整合）
│   └── InteractionHandler.cs # 互動處理器
│
├── UI/                        # UI 系統
│   ├── FastTravelUI.cs       # 快速旅行界面
│   ├── EnergyUIDisplay.cs    # 能量顯示
│   ├── Item/                 # 物品系統 UI
│   │   ├── InventoryUI.cs
│   │   ├── InventoryUITester.cs
│   │   └── ItemType.cs
│   └── UI/                   # 其他 UI
│       ├── JumpUI.cs
│       ├── JumpUITester.cs
│       └── RetryButton.cs
│
├── Tools/                     # 開發工具
│   └── JumpMeasureTool.cs    # 跳躍測量工具
│
├── README.md                  # 專案說明
└── GAME_ARCHITECTURE.md      # 本文件
```

### 命名規範

#### 檔案命名
- **Class 檔案**: `ClassName.cs` (PascalCase)
- **Interface 檔案**: `IInterfaceName.cs` (以 I 開頭)
- **文件檔案**: `DOCUMENT_NAME.md` (大寫加底線)

#### 程式碼命名
```csharp
// Classes: PascalCase
public class GameManager { }

// Interfaces: I + PascalCase
public interface IInteractable { }

// Public methods: PascalCase
public void LoadScene() { }

// Private fields: camelCase
private GameObject player;

// Serialized fields: camelCase with [SerializeField]
[SerializeField] private float jumpForce;

// Constants: UPPER_SNAKE_CASE
private const int MAX_ENERGY = 5;

// Events: Pascal Case (often start with "On")
public event Action OnSceneLoaded;
```

---

## 擴展指南

### 添加新的互動物件

1. **創建新的類別**，實作 `IInteractable` 介面：

```csharp
using UnityEngine;

public class NewInteractable : MonoBehaviour, IInteractable
{
    public void OnInteractorEnterZone(IInteractor interactor)
    {
        // 玩家進入互動區域時的邏輯
    }

    public void OnInteractorExitZone(IInteractor interactor)
    {
        // 玩家離開互動區域時的邏輯
    }

    public bool Interact(IInteractor interactor)
    {
        // 互動發生時的邏輯
        return true; // 回傳互動是否成功
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
```

2. **將檔案放置在** `Scripts/Interaction/` 資料夾

3. **在 Unity 中設置**:
   - 為物件添加 Collider2D (設為 Trigger)
   - 添加你的新腳本

### 添加新的玩家狀態

1. **在 `PlayerState.cs` 中添加新狀態**:

```csharp
public enum PlayerState
{
    // ... existing states
    YourNewState
}
```

2. **在 `PlayerStateMachine.cs` 中處理狀態轉換**:

```csharp
public bool CanTransitionTo(PlayerState targetState)
{
    switch (currentState)
    {
        case PlayerState.YourNewState:
            // 定義從新狀態可以轉換到哪些狀態
            return targetState == PlayerState.Idle;
        // ...
    }
}
```

3. **在 `PlayerController.cs` 中添加狀態邏輯**:

```csharp
private void HandleYourNewState()
{
    // 新狀態的更新邏輯
}

void Update()
{
    switch (stateMachine.GetCurrentState())
    {
        case PlayerState.YourNewState:
            HandleYourNewState();
            break;
        // ...
    }
}
```

### 添加新的管理器

1. **創建新的 Manager 類別**:

```csharp
using UnityEngine;

public class NewManager : MonoBehaviour
{
    // Manager 邏輯
    public void Initialize()
    {
        Debug.Log("NewManager initialized");
    }
}
```

2. **在 `GameManager.cs` 中註冊**:

```csharp
private NewManager newManager;

private void InitializeManagers()
{
    // ... existing managers
    newManager = GetOrCreateManager<NewManager>();
    newManager.Initialize();
}

public NewManager GetNewManager()
{
    return newManager;
}
```

3. **將檔案放置在** `Scripts/Manager/` 資料夾

### 添加新的 UI 系統

1. **創建 UI 腳本**:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class NewUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button button;

    void Start()
    {
        // 初始化 UI
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        // 按鈕點擊邏輯
    }

    public void Show()
    {
        if (panel != null) panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }
}
```

2. **將檔案放置在** `Scripts/UI/` 資料夾

3. **在 Unity 中設置 UI 物件並綁定引用**

---

## 系統依賴圖

### 核心依賴關係

```
GameManager (核心)
    ├─→ SpawnPointManager
    ├─→ SceneTransitionManager
    └─→ PersistenceManager

PlayerController
    ├─→ PlayerMovement
    ├─→ PlayerStateMachine
    ├─→ PlayerAnimationController
    ├─→ InteractionHandler
    └─→ PlayerEnergy

FastTravelUI
    └─→ GameManager (讀取 spawn points)

chair (互動物件)
    ├─→ GameManager (保存 spawn point)
    └─→ PlayerController (坐下互動)

CameraFollow
    └─→ PlayerController (跟隨玩家)
```

### 資料依賴

```
SpawnPointData
    ← SpawnPointManager (管理)
    ← GameManager (訪問)
    ← FastTravelUI (顯示)
    ← chair (創建)

PlayerState
    ← PlayerStateMachine (管理)
    ← PlayerController (使用)
    ← PlayerAnimationController (讀取)
```

---

## 最佳實踐

### 1. 使用 Manager 進行系統間通訊

**好的做法** ✅:
```csharp
// 透過 GameManager 存取其他系統
GameManager.GetInstance().RespawnPlayer();
```

**不好的做法** ❌:
```csharp
// 直接存取其他系統
FindObjectOfType<SpawnPointManager>().SetCurrentSpawnPoint(...);
```

### 2. 使用介面降低耦合

**好的做法** ✅:
```csharp
// 使用介面
public void ProcessInteraction(IInteractable interactable) { }
```

**不好的做法** ❌:
```csharp
// 直接依賴具體類別
public void ProcessChairInteraction(chair chair) { }
```

### 3. 使用事件進行異步通知

**好的做法** ✅:
```csharp
// 訂閱事件
sceneManager.OnSceneLoaded += HandleSceneLoaded;

// 發布事件
OnSceneLoaded?.Invoke(sceneName);
```

**不好的做法** ❌:
```csharp
// 直接呼叫其他系統的方法
otherSystem.SceneHasLoaded(sceneName);
```

### 4. 保持組件單一職責

**好的做法** ✅:
```csharp
// PlayerMovement 只處理移動
// PlayerAnimationController 只處理動畫
// PlayerStateMachine 只處理狀態
```

**不好的做法** ❌:
```csharp
// PlayerController 處理所有事情
```

### 5. 使用 SerializeField 而非 public

**好的做法** ✅:
```csharp
[SerializeField] private float jumpForce;
```

**不好的做法** ❌:
```csharp
public float jumpForce;
```

---

## 效能考量

### 1. Object Pooling
目前系統尚未實作 Object Pooling，但建議在以下情況考慮：
- UI 按鈕生成（FastTravelUI）
- 粒子效果
- 音效物件

### 2. Caching
已實作的 Caching：
- Component references（在 Awake 中取得）
- Manager references（在 GameManager 中維護）

### 3. 避免頻繁的 GameObject.Find
- 使用 SerializeField 直接綁定
- 在 Awake 或 Start 中取得並快取引用
- 透過 Manager 存取系統

---

## 除錯指南

### Debug Log 規範

所有重要系統操作都應該有 Debug.Log：

```csharp
Debug.Log($"SystemName: Operation description - {details}");
Debug.LogWarning($"SystemName: Warning message");
Debug.LogError($"SystemName: Error message");
```

範例：
```csharp
Debug.Log($"GameManager: Scene transition to '{sceneName}' complete");
Debug.LogWarning("PlayerController: PlayerEnergy component not found!");
```

### 常見問題排查

#### 玩家無法互動
1. 檢查 InteractionHandler 的 Collider 設定
2. 確認物件有實作 IInteractable
3. 檢查 Layer 和 Collision Matrix

#### 場景切換後玩家位置錯誤
1. 檢查 PlayerSpawn tag 是否正確設定
2. 確認 spawn point 是否已註冊
3. 查看 Console 的 SceneTransitionManager 訊息

#### UI 不更新
1. 確認 UI 腳本有正確綁定 UI 元素
2. 檢查事件訂閱是否成功
3. 確認 Canvas 設定正確

---

## 版本歷史

### v1.0 (2025-10-20)
- 初始架構文件
- 記錄當前系統設計
- 完整的 Spawn Point 系統
- 模組化的 Manager 架構

---

## 參考資源

### 內部文件
- `README.md` - 腳本資料夾結構說明
- `REFACTORING_GUIDE.md` - 重構指南
- `SPAWN_POINT_SYSTEM_README.md` - Spawn Point 系統詳細說明

### Unity 文件
- [SceneManager API](https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html)
- [MonoBehaviour Lifecycle](https://docs.unity3d.com/Manual/ExecutionOrder.html)
- [Unity Best Practices](https://unity.com/how-to/unity-best-practices)

### 設計模式參考
- [Game Programming Patterns](https://gameprogrammingpatterns.com/)
- [Unity Design Patterns](https://github.com/QianMo/Unity-Design-Pattern)

---

**文件維護者**: AI Assistant  
**最後更新**: 2025-10-20

如有任何問題或建議，請更新本文件並記錄在版本歷史中。

