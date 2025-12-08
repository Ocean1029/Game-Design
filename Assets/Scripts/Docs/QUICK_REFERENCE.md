# Quick Reference Guide - 快速參考指南

> 快速查找常用功能和 API 的指南

---

## 目錄

- [常用 API](#常用-api)
- [系統快速查找](#系統快速查找)
- [常見任務](#常見任務)
- [檔案位置](#檔案位置)

---

## 常用 API

### GameManager - 遊戲核心管理

```csharp
// 取得 GameManager 實例
GameManager gm = GameManager.GetInstance();

// === 場景管理 ===
gm.LoadScene("SceneName");              // 載入場景（名稱）
gm.LoadScene(1);                        // 載入場景（索引）

// === Spawn Point 管理 ===
gm.RegisterSpawnPoint(id, name, desc, pos, scene);  // 註冊新的重生點
gm.SetCurrentSpawnPoint(id);                         // 設定當前重生點
gm.TeleportToSpawnPoint(id);                         // 傳送到指定重生點
gm.RespawnPlayer();                                  // 重生玩家

// === 查詢 ===
SpawnPointData spawn = gm.GetCurrentSpawnPoint();              // 取得當前重生點
List<SpawnPointData> spawns = gm.GetDiscoveredSpawnPoints();  // 取得所有已發現重生點
bool discovered = gm.IsSpawnPointDiscovered(id);              // 檢查是否已發現

// === 位置管理 ===
gm.SavePlayerPositionForScene(sceneName, position);  // 保存玩家位置
Vector3? pos = gm.GetPlayerPositionForScene(sceneName);  // 取得保存的位置
gm.ClearScenePosition(sceneName);                    // 清除場景位置（強制使用 spawn point）
```

### PlayerController - 玩家控制

```csharp
PlayerController player = GetComponent<PlayerController>();

// === 狀態查詢 ===
bool sitting = player.IsSitting();      // 是否正在坐著
bool rappelling = player.IsRappelling();  // 是否正在垂降

// === 動作控制 ===
player.SitOnChair(chairObject);         // 坐在椅子上
player.StandUp();                       // 站起來
player.Rappel(duration);                // 垂降

// === 能量系統 ===
if (player.HasEnergy()) {
    player.ConsumeEnergy();
}
player.RestoreEnergy();                 // 恢復能量
```

### PlayerMovement - 移動控制

```csharp
PlayerMovement movement = GetComponent<PlayerMovement>();

// === 移動 ===
movement.Move(1f);   // 向右移動
movement.Move(-1f);  // 向左移動

// === 跳躍 ===
if (movement.IsGrounded()) {
    movement.Jump();
}

// === 停止 ===
movement.Stop();     // 停止移動
```

### 互動系統 - IInteractable

```csharp
// 實作可互動物件
public class MyObject : MonoBehaviour, IInteractable
{
    public void OnInteractorEnterZone(IInteractor interactor) {
        // 玩家進入範圍
    }

    public void OnInteractorExitZone(IInteractor interactor) {
        // 玩家離開範圍
    }

    public bool Interact(IInteractor interactor) {
        // 互動邏輯
        return true;  // 成功
    }

    public GameObject GetGameObject() {
        return gameObject;
    }
}
```

---

## 系統快速查找

### 我想要...

| 功能需求 | 使用的系統 | 程式碼位置 |
|---------|-----------|-----------|
| 管理遊戲全局狀態 | GameManager | `Manager/GameManager.cs` |
| 處理場景切換 | SceneTransitionManager | `Manager/SceneTransitionManager.cs` |
| 管理重生點 | SpawnPointManager | `Manager/SpawnPointManager.cs` |
| 控制玩家移動 | PlayerMovement | `Player/PlayerMovement.cs` |
| 管理玩家狀態 | PlayerStateMachine | `Player/PlayerStateMachine.cs` |
| 處理玩家互動 | InteractionHandler | `Player/InteractionHandler.cs` |
| 控制相機跟隨 | CameraFollow | `Camera/CameraFollow.cs` |
| 建立互動物件 | IInteractable | `Interaction/IInteractable.cs` |
| 顯示快速旅行 UI | FastTravelUI | `UI/FastTravelUI.cs` |
| 顯示能量 UI | EnergyUIDisplay | `UI/EnergyUIDisplay.cs` |

---

## 常見任務

### 1. 創建新的重生點（椅子）

```csharp
// 方法 A: 在場景中放置 chair prefab
// 1. 拖曳 chair prefab 到場景
// 2. 設定 Inspector 中的參數：
//    - Chair Id: "chair_village_01"
//    - Chair Name: "村莊入口"
//    - Location Description: "位於村莊廣場"

// 方法 B: 程式碼註冊
GameManager.GetInstance().RegisterSpawnPoint(
    "chair_village_01",          // ID
    "村莊入口",                   // 顯示名稱
    "位於村莊廣場",               // 位置描述
    new Vector3(10f, 5f, 0f),   // 位置
    "VillageScene"              // 場景名稱
);
```

### 2. 實作場景切換觸發器

```csharp
// 方法 A: 使用 SceneTransition 元件
// 1. 在場景中創建空物件
// 2. 添加 BoxCollider2D（設為 Trigger）
// 3. 添加 SceneTransition 腳本
// 4. 在 Inspector 設定 targetSceneName

// 方法 B: 程式碼觸發
void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("Player")) {
        GameManager.GetInstance().LoadScene("TargetScene");
    }
}
```

### 3. 創建新的互動物件

```csharp
using UnityEngine;

public class MyInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactPrompt;

    public void OnInteractorEnterZone(IInteractor interactor)
    {
        // 顯示提示
        if (interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    public void OnInteractorExitZone(IInteractor interactor)
    {
        // 隱藏提示
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    public bool Interact(IInteractor interactor)
    {
        // 執行互動
        Debug.Log("互動執行！");
        
        // 檢查是否為玩家
        PlayerController player = interactor.GetGameObject()
            .GetComponent<PlayerController>();
        
        if (player != null) {
            // 對玩家執行動作
        }
        
        return true;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}

// Unity 設定：
// 1. 添加此腳本到物件
// 2. 添加 Collider2D（Trigger）
// 3. 綁定 interactPrompt UI 物件
```

### 4. 添加新的玩家狀態

```csharp
// 步驟 1: 在 PlayerState.cs 添加新狀態
public enum PlayerState
{
    Idle,
    Walking,
    Jumping,
    Falling,
    Sitting,
    Rappelling,
    Dead,
    YourNewState  // ← 新增
}

// 步驟 2: 在 PlayerStateMachine.cs 定義轉換規則
public bool CanTransitionTo(PlayerState targetState)
{
    switch (currentState)
    {
        case PlayerState.YourNewState:
            // 定義可以轉換到哪些狀態
            return targetState == PlayerState.Idle || 
                   targetState == PlayerState.Walking;
        
        case PlayerState.Idle:
            // 閒置狀態可以轉換到新狀態
            return targetState == PlayerState.YourNewState || ...;
    }
}

// 步驟 3: 在 PlayerController.cs 處理狀態邏輯
void Update()
{
    switch (stateMachine.GetCurrentState())
    {
        case PlayerState.YourNewState:
            HandleYourNewState();
            break;
    }
}

private void HandleYourNewState()
{
    // 新狀態的更新邏輯
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        stateMachine.SetState(PlayerState.Idle);
    }
}
```

### 5. 實作玩家死亡和重生

```csharp
public class PlayerHealth : MonoBehaviour
{
    private PlayerController controller;
    
    void Start()
    {
        controller = GetComponent<PlayerController>();
    }
    
    public void Die()
    {
        // 設定死亡狀態
        PlayerStateMachine stateMachine = 
            GetComponent<PlayerStateMachine>();
        stateMachine.SetState(PlayerState.Dead);
        
        // 禁用玩家輸入
        controller.enabled = false;
        
        // 延遲後重生
        StartCoroutine(RespawnAfterDelay(2f));
    }
    
    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // 重生到最後的 spawn point
        GameManager.GetInstance().RespawnPlayer();
        
        // 恢復玩家控制
        controller.enabled = true;
        
        // 恢復能量
        GetComponent<PlayerEnergy>()?.RestoreAllEnergy();
    }
}
```

### 6. 創建快速旅行 UI

```csharp
// FastTravelUI 已經實作，只需要設定：
// 1. 在場景中創建 Canvas
// 2. 添加 FastTravelUI 腳本
// 3. 在 Inspector 綁定：
//    - Menu Panel
//    - Button Container (ScrollView Content)
//    - Chair Button Prefab
//    - No Chairs Text
// 4. 玩家按 M 鍵打開選單
```

### 7. 保存和載入遊戲進度

```csharp
// 目前系統在記憶體中保存進度，重啟後會清空
// 如需持久化存檔，擴展 PersistenceManager：

public class SaveData
{
    public string currentSpawnPointId;
    public List<string> discoveredSpawnPoints;
    public Dictionary<string, Vector3> scenePositions;
}

// 保存
public void SaveGame()
{
    SaveData data = new SaveData();
    
    SpawnPointData current = GameManager.GetInstance()
        .GetCurrentSpawnPoint();
    data.currentSpawnPointId = current?.spawnPointId;
    
    data.discoveredSpawnPoints = GameManager.GetInstance()
        .GetDiscoveredSpawnPoints()
        .Select(sp => sp.spawnPointId)
        .ToList();
    
    string json = JsonUtility.ToJson(data);
    PlayerPrefs.SetString("SaveData", json);
    PlayerPrefs.Save();
}

// 載入
public void LoadGame()
{
    if (!PlayerPrefs.HasKey("SaveData")) return;
    
    string json = PlayerPrefs.GetString("SaveData");
    SaveData data = JsonUtility.FromJson<SaveData>(json);
    
    // 恢復 spawn points 和當前重生點
    // 恢復場景位置
    // ...
}
```

---

## 檔案位置

### 核心系統檔案

```
Scripts/
├── GAME_ARCHITECTURE.md          ← 完整架構文件
├── QUICK_REFERENCE.md            ← 本文件
└── README.md                     ← 資料夾結構說明
```

### Manager 系統

```
Scripts/Manager/
├── GameManager.cs                ← 核心管理器（Singleton）
├── SpawnPointManager.cs          ← 重生點管理
├── SceneTransitionManager.cs     ← 場景轉換管理
├── PersistenceManager.cs         ← 持久化管理
├── SceneTransition.cs            ← 場景觸發器元件
├── REFACTORING_GUIDE.md          ← 重構指南
└── SPAWN_POINT_SYSTEM_README.md  ← Spawn Point 詳細說明
```

### Player 系統

```
Scripts/Player/
├── PlayerController.cs           ← 玩家主控制器
├── PlayerMovement.cs             ← 移動物理
├── PlayerStateMachine.cs         ← 狀態機
├── PlayerState.cs                ← 狀態定義
├── PlayerAnimationController.cs  ← 動畫控制
├── PlayerEnergy.cs               ← 能量系統
└── InteractionHandler.cs         ← 互動處理
```

### 互動系統

```
Scripts/Interaction/
├── IInteractable.cs              ← 可互動介面
├── IInteractor.cs                ← 互動者介面
├── chair.cs                      ← 椅子（重生點）
├── door.cs                       ← 門
└── cable.cs                      ← 繩索
```

### UI 系統

```
Scripts/UI/
├── FastTravelUI.cs               ← 快速旅行選單
├── EnergyUIDisplay.cs            ← 能量顯示
├── Item/
│   ├── InventoryUI.cs
│   └── ItemType.cs
└── UI/
    ├── JumpUI.cs
    └── RetryButton.cs
```

---

## 調試技巧

### 查看當前狀態

```csharp
// 查看玩家狀態
PlayerStateMachine sm = player.GetComponent<PlayerStateMachine>();
Debug.Log($"Current State: {sm.GetCurrentState()}");

// 查看當前 spawn point
SpawnPointData spawn = GameManager.GetInstance().GetCurrentSpawnPoint();
if (spawn != null) {
    Debug.Log($"Current Spawn: {spawn.displayName} in {spawn.sceneName}");
}

// 查看所有已發現的 spawn points
List<SpawnPointData> spawns = GameManager.GetInstance()
    .GetDiscoveredSpawnPoints();
Debug.Log($"Discovered Spawns: {spawns.Count}");
foreach (var sp in spawns) {
    Debug.Log($"  - {sp.displayName} ({sp.spawnPointId})");
}
```

### 測試用快捷方式

```csharp
// 在 PlayerController 的 Update() 中添加測試按鍵
#if UNITY_EDITOR
void Update()
{
    // 測試傳送
    if (Input.GetKeyDown(KeyCode.T))
    {
        transform.position = new Vector3(10, 5, 0);
        Debug.Log("Teleported to test position");
    }
    
    // 測試重生
    if (Input.GetKeyDown(KeyCode.R))
    {
        GameManager.GetInstance().RespawnPlayer();
    }
    
    // 測試能量恢復
    if (Input.GetKeyDown(KeyCode.E))
    {
        GetComponent<PlayerEnergy>()?.RestoreAllEnergy();
        Debug.Log("Energy restored");
    }
}
#endif
```

### Console 訊息過濾

在 Console 視窗中輸入以下文字來過濾特定系統的訊息：
- `GameManager:` - 只顯示 GameManager 的訊息
- `SceneTransitionManager:` - 場景轉換訊息
- `SpawnPointManager:` - Spawn point 相關訊息
- `Chair:` - 椅子互動訊息

---

## 常見問題

### Q: 如何讓物件在場景切換後保留？

```csharp
// 在物件的 Awake 或 Start 中：
DontDestroyOnLoad(gameObject);

// 或使用 PersistenceManager
GameManager.GetInstance().GetPersistenceManager()
    .RegisterPersistentObject(gameObject);
```

### Q: 如何檢測玩家是否在地面上？

```csharp
PlayerMovement movement = player.GetComponent<PlayerMovement>();
if (movement.IsGrounded()) {
    // 玩家在地面上
}
```

### Q: 如何取得當前場景名稱？

```csharp
string sceneName = UnityEngine.SceneManagement.SceneManager
    .GetActiveScene().name;

// 或透過 SceneTransitionManager
string sceneName = GameManager.GetInstance()
    .GetSceneTransitionManager()
    .GetCurrentSceneName();
```

### Q: 如何暫停遊戲？

```csharp
// 暫停
Time.timeScale = 0f;

// 恢復
Time.timeScale = 1f;

// FastTravelUI 已實作自動暫停功能
```

---

## 按鍵配置

### 預設按鍵（可在 PlayerController Inspector 中修改）

| 按鍵 | 功能 |
|-----|------|
| ← → | 左右移動 |
| Space | 跳躍 |
| U | 互動（坐下、開門等） |
| D | 離開互動（站起來） |
| R | 重生到最後的 spawn point |
| M | 打開快速旅行選單 |

---

## 效能優化建議

### 1. 避免頻繁的 GetComponent

```csharp
// ❌ 不好
void Update() {
    GetComponent<PlayerMovement>().Move(input);
}

// ✅ 好
private PlayerMovement movement;
void Awake() {
    movement = GetComponent<PlayerMovement>();
}
void Update() {
    movement.Move(input);
}
```

### 2. 快取 Manager 引用

```csharp
// ❌ 不好
void Update() {
    GameManager.GetInstance().DoSomething();
}

// ✅ 好
private GameManager gameManager;
void Start() {
    gameManager = GameManager.GetInstance();
}
void Update() {
    gameManager.DoSomething();
}
```

### 3. 使用事件而非輪詢

```csharp
// ❌ 不好
void Update() {
    if (playerHealth.IsDead()) {
        HandleDeath();
    }
}

// ✅ 好
void Start() {
    playerHealth.OnDeath += HandleDeath;
}
```

---

**最後更新**: 2025-10-20  
**維護者**: AI Assistant

需要更多詳細資訊，請參閱 [`GAME_ARCHITECTURE.md`](./GAME_ARCHITECTURE.md)

