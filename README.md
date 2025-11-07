# 遊戲專案 - 類銀河惡魔城系統

**類型**: 2D 類銀河惡魔城遊戲  
**Unity 版本**: 6000.2.2f1  
**最後更新**: 2025-10-20

---

## 🎮 遊戲特色

- **存檔點系統**：椅子作為存檔點，支援快速旅行和重生
- **一次性存檔點**：遊戲初始出生點等特殊機制
- **可傳送性控制**：靈活的快速旅行限制
- **場景傳送系統**：1對1對應的場景切換
- **能量系統**：跳躍消耗能量，椅子恢復能量

---

## 🚀 快速開始

### Unity 編輯器設置

1. **打開 Unity 專案**

2. **設置新系統**
   ```
   GameObject → Setup New System
   ```
   
   這會自動創建所有需要的資料庫和管理器。

3. **設置預設出生點**
   - Hierarchy 右鍵 → Create Empty
   - 命名為 `PlayerSpawn`
   - Tag 設為 `PlayerSpawn`
   - 移動到初始位置

4. **添加快速旅行 UI**
   - Hierarchy 右鍵 → Create Empty
   - 命名為 `FastTravelSystem`
   - 添加 `SimpleFastTravelUI` 組件

5. **測試遊戲**
   - 按 Play
   - 走到椅子按 U 鍵坐下
   - 按 R 鍵測試重生
   - 按 M 鍵測試快速旅行

---

## 📋 控制鍵位

| 按鍵 | 功能 |
|------|------|
| **WASD / 方向鍵** | 移動 |
| **Space** | 跳躍 |
| **U** | 互動（椅子、傳送點等） |
| **D**（坐著時）| 站起來 |
| **R** | 重生到最後的存檔點 |
| **M** | 打開/關閉快速旅行選單 |
| **1-9**（選單中）| 選擇快速旅行目的地 |
| **ESC**（選單中）| 關閉選單 |

---

## 🗂️ 專案結構

```
Assets/
├── Scripts/
│   ├── Data/                          # 資料結構
│   │   ├── SpawnPointData.cs         # 存檔點資料
│   │   ├── SpawnPointDatabase.cs     # 存檔點資料庫
│   │   ├── TransitionPointData.cs    # 傳送點資料
│   │   └── TransitionPointDatabase.cs # 傳送點資料庫
│   ├── Manager/                       # 管理器
│   │   ├── NewGameManager.cs         # 核心遊戲管理器
│   │   ├── SpawnPointSystem.cs       # 存檔點系統
│   │   └── TransitionPointSystem.cs  # 傳送點系統
│   ├── Interaction/                   # 互動組件
│   │   ├── chair.cs                  # 椅子（存檔點）
│   │   ├── SavePoint.cs              # 新的存檔點組件
│   │   └── TransitionPoint.cs        # 傳送點組件
│   ├── Player/                        # 玩家相關
│   │   ├── PlayerController.cs       # 玩家控制器
│   │   ├── PlayerEnergy.cs           # 能量系統
│   │   ├── PlayerMovement.cs         # 移動系統
│   │   └── PlayerStateMachine.cs     # 狀態機
│   ├── UI/                            # UI 系統
│   │   └── SimpleFastTravelUI.cs     # 快速旅行 UI
│   └── Editor/                        # 編輯器工具
│       ├── QuickSetup.cs             # 快速設置工具
│       └── AutoSetupOnLoad.cs        # 自動設置工具
├── ScriptableObjects/                 # ScriptableObject 資料
│   ├── SpawnPointDatabase.asset      # 存檔點資料庫
│   └── TransitionPointDatabase.asset # 傳送點資料庫
├── Scenes/                            # 遊戲場景
│   ├── MainScene.unity               # 主場景
│   └── SampleScene.unity             # 範例場景
└── Prefabs/                           # 預製件
    └── NewGameManager.prefab         # 遊戲管理器預製件
```

---

## 📚 文件說明

| 文件 | 說明 |
|------|------|
| **SETUP_AND_USAGE_GUIDE.md** | 設置與使用指南（必讀）|
| **NEW_SYSTEM_README.md** | 新系統完整功能說明 |
| **SETUP_DEFAULT_SPAWN.md** | 預設出生點設置指南 |
| **MANUAL_SETUP_GUIDE.md** | 手動設置詳細步驟 |
| **QUICK_SETUP.md** | 快速設置指南 |

---

## 🔧 Unity 編輯器工具

### 設置工具

```
GameObject → Setup New System        # 快速設置
GameObject → Validate Setup          # 驗證設置
Assets → Setup New System            # 快速設置（替代位置）
```

### 資料庫創建

```
Assets → Create → Game Data → Spawn Point Database
Assets → Create → Game Data → Transition Point Database
```

---

## 🎯 核心系統

### 存檔點系統

**椅子（chair.cs）**：
- 玩家可以坐在椅子上存檔
- 自動註冊為存檔點
- 支援快速旅行
- 恢復玩家能量

**SavePoint 組件**：
- 更靈活的存檔點組件
- 支援一次性存檔點
- 支援可傳送性控制

### 傳送點系統

**TransitionPoint 組件**：
- 場景間的傳送
- 1對1對應關係
- 支援雙向和單向傳送
- 支援鑰匙系統

### 重生系統

**優先順序**：
1. 最後坐過的椅子
2. PlayerSpawn tag 物件
3. 保持當前位置（顯示警告）

### 快速旅行

**SimpleFastTravelUI**：
- 按 M 鍵開啟
- Console 顯示可用存檔點
- 按數字鍵選擇目的地
- 自動處理場景切換

---

## 🛠️ 開發注意事項

### 重要提醒

1. **場景中必須有 NewGameManager**
   - 只需要一個
   - 會自動 DontDestroyOnLoad

2. **每個場景都要有 PlayerSpawn**
   - Tag 設為 `PlayerSpawn`
   - 作為預設出生點

3. **椅子需要設定 ID**
   - 每個椅子需要唯一的 Chair ID
   - 建議格式：`chair_[場景]_[編號]`

4. **傳送點需要雙向設置**
   - 兩個場景都要有對應的傳送點
   - ID 要正確對應

### 除錯技巧

- 開啟 `Show Debug Info` 查看詳細日誌
- 使用 `GameObject → Validate Setup` 驗證設置
- 檢查 Console 的錯誤訊息
- 使用 Unity Debugger 追蹤問題

---

## 📖 詳細文件

請參考以下文件獲取更多資訊：

1. **SETUP_AND_USAGE_GUIDE.md** - 完整設置與使用指南
2. **NEW_SYSTEM_README.md** - 系統功能詳細說明
3. **SETUP_DEFAULT_SPAWN.md** - 預設出生點設置

---

## 🎊 開始開發！

現在所有系統都已設置完成，你可以：

- ✅ 添加更多椅子到場景
- ✅ 創建多個場景並設置傳送點
- ✅ 設計存檔點的位置和屬性
- ✅ 測試快速旅行功能
- ✅ 繼續開發遊戲內容

祝你開發順利！ 🚀