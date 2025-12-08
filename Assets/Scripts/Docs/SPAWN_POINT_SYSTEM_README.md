# Spawn Point System - 使用說明

## 系統概述

這個系統實現了自動記錄玩家最後一個 spawn point（椅子）的功能。當場景重新載入時，玩家會從最後坐過的椅子重生，確保遊戲進度的連續性。

## 核心功能

### 1. 自動記錄最後的 Spawn Point
- 玩家每次坐在椅子上時，系統會自動記錄該椅子作為當前的 spawn point
- 記錄內容包括：椅子位置、場景名稱、椅子 ID、顯示名稱

### 2. 重生位置優先順序
當玩家進入場景時，系統會按以下優先順序決定玩家的出現位置：

1. **Scene Saved Position（場景保存位置）** - 用於場景間的自然切換
2. **Last Spawn Point（最後的重生點）** - 用於重生或重新載入場景 ⭐ **新增功能**
3. **Default PlayerSpawn Tag** - 場景中預設的 PlayerSpawn tag 物件
4. **Current Position（維持當前位置）** - 最後的備選方案

### 3. 場景切換 vs. 重生的區別

#### 場景切換（Scene Transition）
```csharp
// 玩家從場景 A 走到場景 B
GameManager.GetInstance().LoadScene("SceneB");
// 結果：玩家會出現在場景 B 中上次離開時的位置
```

#### 重生（Respawn）
```csharp
// 玩家按 R 鍵重生，或者死亡後重生
GameManager.GetInstance().RespawnPlayer();
// 結果：玩家會出現在最後一個 spawn point（椅子）的位置
```

## 使用範例

### 設置椅子作為 Spawn Point

在場景中添加椅子物件：

1. 在場景中創建一個椅子 GameObject
2. 添加 `chair.cs` 腳本
3. 設置以下參數：
   - **Chair Id**: 唯一識別碼（例如：`chair_village_01`）
   - **Chair Name**: 顯示名稱（例如：`村莊入口的椅子`）
   - **Location Description**: 位置描述（例如：`位於村莊廣場東側`）
   - **Sit Point**: 玩家坐下時的位置 Transform

### 玩家互動流程

```
1. 玩家靠近椅子
   ↓
2. 顯示 "按 U 坐下" 提示
   ↓
3. 玩家按 U 鍵
   ↓
4. 椅子自動呼叫 SaveProgress()
   ↓
5. GameManager 記錄這個椅子為當前 spawn point
   ↓
6. 玩家下次重生時會從這裡出現
```

## API 使用

### GameManager API

```csharp
// 取得 GameManager 實例
GameManager gm = GameManager.GetInstance();

// 設置當前 spawn point
gm.SetCurrentSpawnPoint("chair_village_01", position, "VillageScene");

// 取得當前 spawn point
SpawnPointData currentSpawn = gm.GetCurrentSpawnPoint();

// 重生玩家（會清除當前場景的保存位置，強制使用 spawn point）
gm.RespawnPlayer();

// 清除特定場景的保存位置（讓玩家重新從 spawn point 出現）
gm.ClearScenePosition("VillageScene");
```

### SceneTransitionManager API

```csharp
SceneTransitionManager stm = GameManager.GetInstance().GetSceneTransitionManager();

// 載入場景（會保留場景位置記錄）
stm.LoadScene("NewScene");

// 移動玩家到特定位置
stm.MovePlayerToPosition(newPosition, "description");

// 清除場景位置記錄
stm.ClearScenePosition("SceneName");
```

## 實作細節

### 位置記錄的生命週期

#### Scene Position（場景位置）
- **記錄時機**：每次離開場景時自動記錄
- **使用時機**：重新進入該場景時
- **清除時機**：呼叫 `RespawnPlayer()` 或 `ClearScenePosition()` 時
- **用途**：保持場景探索的連續性

#### Spawn Point（重生點）
- **記錄時機**：玩家坐在椅子上時
- **使用時機**：重生、或進入場景但沒有保存位置時
- **清除時機**：不會自動清除（持久保存）
- **用途**：提供安全的重生位置

### 修改的檔案

1. **SceneTransitionManager.cs**
   - 修改 `MovePlayerToScenePosition()` - 新增 spawn point 優先級
   - 新增 `ClearScenePosition()` - 清除特定場景的保存位置

2. **GameManager.cs**
   - 修改 `RespawnPlayer()` - 重生時清除場景位置
   - 新增 `ClearScenePosition()` - 公開 API

3. **TagManager.asset**
   - 新增 "PlayerSpawn" tag

## 測試流程

### 測試 Spawn Point 記憶功能

1. 開始遊戲，玩家出現在場景 A
2. 走到椅子 1，坐下（這會設置 spawn point）
3. 探索場景，走到其他地方
4. 按 R 鍵重生
5. ✅ 玩家應該出現在椅子 1 的位置

### 測試場景切換功能

1. 在場景 A 的椅子 1 坐下
2. 走到場景 B
3. 在場景 B 探索
4. 走回場景 A
5. ✅ 玩家應該出現在離開場景 A 時的位置（不是椅子 1）

### 測試跨場景重生

1. 在場景 A 的椅子 1 坐下
2. 走到場景 B
3. 在場景 B 按 R 鍵重生
4. ✅ 玩家應該被傳送回場景 A 的椅子 1

## Debug 訊息

系統會在 Console 中輸出詳細的 debug 訊息：

```
SceneTransitionManager: Player moved to last spawn point 'chair_village_01' in 'VillageScene': (10, 5, 0)
GameManager: Progress saved at '村莊入口的椅子' (chair_village_01)
SceneTransitionManager: Cleared saved position for scene 'VillageScene'
```

## 常見問題

### Q: 為什麼重生時玩家不在 spawn point？
A: 檢查是否有 scene saved position。呼叫 `ClearScenePosition()` 來清除。

### Q: 如何設置遊戲開始時的預設 spawn point？
A: 在場景中創建一個帶有 "PlayerSpawn" tag 的空物件。

### Q: 場景位置和 spawn point 有什麼區別？
A: Scene position 用於場景探索的連續性；Spawn point 用於重生和快速旅行。

### Q: 如何實現玩家死亡後重生？
A: 在死亡處理程式碼中呼叫 `GameManager.GetInstance().RespawnPlayer()`

## 未來擴展

可能的擴展方向：

1. **持久化存檔** - 將 spawn point 保存到檔案
2. **多個備份點** - 記錄最近的 N 個 spawn point
3. **Spawn Point 解鎖** - 某些椅子需要特定條件才能使用
4. **視覺效果** - 重生時的淡入淡出效果
5. **音效系統** - 重生時的音效提示

