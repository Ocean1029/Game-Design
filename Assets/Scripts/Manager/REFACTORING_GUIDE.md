# GameManager 重構指南

## 🎯 重構概述

GameManager 已經從一個 400+ 行的單體類別重構為模組化的管理器系統。

### 重構前 vs 重構後

**重構前（單體架構）：**
```
GameManager.cs (415 lines)
├── 持久化管理
├── 場景轉換管理
├── 存檔點系統
└── 位置記錄系統
```

**重構後（模組化架構）：**
```
GameManager.cs (核心協調器, ~300 lines)
├── SpawnPointManager.cs (存檔點系統, ~170 lines)
├── SceneTransitionManager.cs (場景轉換, ~200 lines)
└── PersistenceManager.cs (持久化管理, ~160 lines)
```

## 📁 新的文件結構

### 1. **GameManager.cs** - 核心協調器
**職責：**
- 協調所有子管理器
- 提供統一的公共 API
- Singleton 模式管理

**主要功能：**
- 初始化所有子管理器
- 提供向後兼容的 API
- 處理管理器之間的協調

### 2. **SpawnPointManager.cs** - 存檔點管理
**職責：**
- 管理椅子存檔點系統
- 註冊和追蹤已發現的存檔點
- 管理當前存檔點

**主要功能：**
- `RegisterSpawnPoint()` - 註冊新存檔點
- `SetCurrentSpawnPoint()` - 設定當前存檔點
- `GetDiscoveredSpawnPoints()` - 獲取所有存檔點
- `IsSpawnPointDiscovered()` - 檢查存檔點是否已發現

### 3. **SceneTransitionManager.cs** - 場景轉換管理
**職責：**
- 處理場景載入和轉換
- 追蹤玩家在每個場景的位置
- 實現 Hollow Knight 式的探索體驗

**主要功能：**
- `LoadScene()` - 載入新場景
- `MovePlayerToScenePosition()` - 智能決定玩家位置
- `SavePlayerPositionForScene()` - 保存場景位置
- `GetPlayerPositionForScene()` - 獲取場景位置

### 4. **PersistenceManager.cs** - 持久化管理
**職責：**
- 管理跨場景的持久化物件
- 使用 `DontDestroyOnLoad` 保持物件存在

**主要功能：**
- `Initialize()` - 初始化所有持久化物件
- `AddPersistentObject()` - 添加持久化物件
- `RemovePersistentObject()` - 移除持久化物件
- `GetPlayer()` / `GetCanvas()` - 獲取持久化物件

## 🔄 API 兼容性

### ✅ 完全向後兼容

所有原有的 GameManager API 都保持不變，現有代碼無需修改！

**椅子腳本可以繼續使用：**
```csharp
GameManager.GetInstance().RegisterSpawnPoint(...);
GameManager.GetInstance().SetCurrentSpawnPoint(...);
```

**快速傳送 UI 可以繼續使用：**
```csharp
GameManager.GetInstance().GetDiscoveredSpawnPoints();
GameManager.GetInstance().TeleportToSpawnPoint(...);
```

**玩家控制器可以繼續使用：**
```csharp
GameManager.GetInstance().RespawnPlayer();
```

## 🎨 新的使用方式

### 方式 1：透過 GameManager（推薦，向後兼容）
```csharp
GameManager gm = GameManager.GetInstance();
gm.RegisterSpawnPoint("chair_01", "Forest Chair", "", position, sceneName);
gm.LoadScene("NewScene");
```

### 方式 2：直接使用子管理器（更靈活）
```csharp
GameManager gm = GameManager.GetInstance();

// 存檔點相關
SpawnPointManager spm = gm.GetSpawnPointManager();
spm.RegisterSpawnPoint(...);

// 場景轉換相關
SceneTransitionManager stm = gm.GetSceneTransitionManager();
stm.LoadScene("NewScene");

// 持久化相關
PersistenceManager pm = gm.GetPersistenceManager();
pm.AddPersistentObject(myObject);
```

## 📊 重構優勢

### 1. **更好的代碼組織**
- 每個管理器職責單一、清晰
- 代碼分散在多個小文件中，更易閱讀
- 功能邊界明確

### 2. **更容易維護**
- 修改某個功能不會影響其他功能
- 減少代碼衝突的可能性
- 更容易追蹤和修復 bug

### 3. **更好的可測試性**
- 可以單獨測試每個管理器
- 不需要啟動整個遊戲系統
- 更容易模擬和隔離測試

### 4. **更容易擴展**
- 新增功能只需新增新的管理器
- 不會讓 GameManager 變得臃腫
- 符合開放封閉原則

### 5. **更好的團隊協作**
- 不同開發者可以同時修改不同的管理器
- 減少 Git 合併衝突
- 代碼審查更容易

## 🔧 使用建議

### 對於現有代碼
- **無需修改**：所有現有代碼都能正常運作
- **逐步遷移**：可以慢慢改用新的子管理器 API

### 對於新代碼
- **使用 GameManager API**：保持統一性和簡單性
- **需要更多控制時**：直接使用子管理器

### 未來擴展
可以輕鬆添加新的管理器：
- `AudioManager` - 音效和音樂管理
- `SettingsManager` - 遊戲設定管理
- `SaveLoadManager` - 存檔和讀檔管理
- `AchievementManager` - 成就系統管理
- `DialogueManager` - 對話系統管理

## 🚀 遷移步驟（可選）

如果你想逐步採用新架構：

1. **第一階段**：保持使用 GameManager API（當前狀態）
2. **第二階段**：在新功能中使用子管理器
3. **第三階段**：逐步重構舊代碼使用子管理器

## 📝 注意事項

### 重要提醒
1. **GameManager 自動創建子管理器**：無需手動添加
2. **Singleton 模式**：GameManager 確保只有一個實例
3. **場景轉換**：所有管理器都會跨場景保持存在
4. **向後兼容**：所有舊的 API 調用都能正常工作

### 除錯提示
- 所有管理器都有詳細的 Debug.Log 訊息
- 可以在 Inspector 中看到所有管理器組件
- 如果有問題，檢查 Console 的日誌訊息

## 🎉 總結

這次重構大幅提升了代碼質量和可維護性，同時保持了完全的向後兼容性。你可以繼續使用原有的代碼，也可以逐步採用新的架構。

**最重要的是：你的遊戲不需要任何修改就能正常運作！**

