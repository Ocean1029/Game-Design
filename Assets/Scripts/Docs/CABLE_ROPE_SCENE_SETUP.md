# Cable & Rope 系統場景設置指南

## 概述

本文件列出在 Unity 場景中啟用 cable 和 rope 互動所需的所有設置步驟。

## 前置需求

確認已有以下組件/系統：
- ✅ `InventorySystem` - 管理玩家物品
- ✅ `PlayerController` - 玩家控制器
- ✅ `BackpackUIController` - 背包 UI 控制器
- ✅ `InventoryPromptManager` - 物品提示管理器（Singleton）
- ✅ `FloatingTextManager` - 浮動文字管理器（Singleton）

## 場景設置步驟

### 步驟 1：確保 Singleton 管理器存在

#### A. InventoryPromptManager（物品提示管理器）

1. 在 Hierarchy 中查找或創建 `InventoryPromptManager` GameObject
2. 確認它有 `InventoryPromptManager` 組件
3. **重要**：此 GameObject 必須在場景根層級或 UI Canvas 下
4. 設置 Inspector 參數：
   - **Show Debug Info**: 勾選（開發中方便調試）

```
Hierarchy 結構：
Canvas
├─ InventoryPromptManager (InventoryPromptManager 組件)
├─ BackpackUI
├─ MiniMapUI
└─ ...
```

或者直接在任何 GameObject 上添加：
- 右鍵 → Add Component → InventoryPromptManager

#### B. FloatingTextManager（浮動文字管理器）

1. 在 Hierarchy 中查找或創建 `FloatingTextManager` GameObject
2. 確認它有 `FloatingTextManager` 組件
3. 設置 Inspector 參數：
   - **Text Prefab**: 拖入浮動文字預製件（或保持空，會自動創建）
   - **Canvas**: 拖入場景中的主 Canvas

### 步驟 2：在場景中設置 Rope 道具

#### 方案 A：創建可收集的 Rope（推薦）

1. 在場景中創建新 GameObject（例如：`Rope_Item_01`）
2. 添加以下組件：

**SpriteRenderer**
- 設置 rope 的圖片精靈

**BoxCollider2D 或 CircleCollider2D**
- **Is Trigger**: ✅ 勾選
- 調整大小以包圍圖片

**CollectableItem**（如果有此組件）
- **Item Data**: 拖入 `Assets/ScriptableObjects/Items/rope.asset`
- **Quantity**: `1`

#### 方案 B：直接添加到背包（用於測試）

編輯 `PlayerController.cs` 的 `Start()` 方法：

```csharp
void Start()
{
    // ... 現有程式碼 ...
    
    // 測試用：添加 rope 到背包
    InventorySystem inventory = GetComponent<InventorySystem>();
    if (inventory != null)
    {
        ItemData ropeItem = Resources.Load<ItemData>("ScriptableObjects/Items/rope");
        if (ropeItem != null)
        {
            inventory.AddItem(ropeItem, 1);
            Debug.Log("✓ Added rope to inventory for testing");
        }
    }
}
```

### 步驟 3：在 InventorySystem 中註冊 Rope

1. 選擇場景中的 **Player** GameObject
2. 找到 `InventorySystem` 組件
3. 在 Inspector 中找到 **All Game Items** 列表
4. 點擊 `+` 新增一個槽位
5. 將 `Assets/ScriptableObjects/Items/rope.asset` 拖入新槽位

**結果**：背包 UI 會自動為 rope 創建一個 slot

### 步驟 4：設置 Cable

#### A. Cable GameObject 基本設置

1. 選擇場景中的 **Cable** GameObject
2. 確保它有以下組件：
   - ✅ `cable` 腳本
   - ✅ `Collider2D`（BoxCollider2D 或 CircleCollider2D）
   - ✅ `Animator`（如果要播放動畫）

#### B. 在 cable 檢查器中配置

選擇 cable GameObject，在 Inspector 中找到 `cable` 組件：

**Cable State（狀態設定）**
- **Start Broken**: ✅ 勾選（預設壞掉）

**Rope Requirement（繩索需求）**
- **Required Rope Item Id**: `rope`
- **Required Rope Tag**: `rope`

**Rappelling Configuration（下降設置）**
- **Target Floor Point**: 拖入玩家著陸的目標位置 Transform
- **Animation Trigger Name**: `player_enter`（或改成你的動畫觸發器名稱）

**UI Prompts（提示設置）**
- **Use Prompt**: 拖入修復後才顯示的提示 UI（可選，用於 rappelling）

**Visual Feedback（視覺反饋）**
- **Broken Visual**: 拖入壞掉狀態的視覺物件（可選，例如：斷裂的繩索圖片）
- **Repaired Visual**: 拖入修復狀態的視覺物件（可選，例如：完整的繩索）

#### C. Cable Collider 設置

1. 選擇 cable 的 Collider2D 組件
2. **Is Trigger**: ❌ 不勾選（因為預設是壞掉且不可穿透）
3. 調整 Collider 大小以符合 cable 的外形

#### D. Cable 動畫設置（可選）

1. 選擇 cable GameObject
2. 在 Inspector 中找到 **Animator** 組件
3. 確認有對應的 Animation Controller
4. 在 Animation Controller 中創建觸發器參數：
   - 參數名稱：**player_enter**（對應 cable.cs 中的 `animationTriggerName`）
   - 參數類型：**Trigger**

### 步驟 5：檢查背包 UI

確保背包 UI 結構正確：

```
Canvas
├─ BackpackUI (BackpackUIController 組件)
│  └─ BackpackContainer (Horizontal or Grid Layout Group)
│     └─ SlotPrefab (預製件，包含 BackpackSlotUI 組件)
│        ├─ Background (Image)
│        └─ Icon (Image - 用於顯示物品圖片)
└─ ...
```

**重點檢查**：
- ✅ `BackpackUIController` 的 `backpackContainer` 指向正確的 Transform
- ✅ `BackpackUIController` 的 `slotPrefab` 指向正確的預製件
- ✅ Slot 預製件有 `BackpackSlotUI` 組件
- ✅ Slot 的 Icon 是 Image 組件

### 步驟 6：啟用 zbutton 圖片

確保 `zbutton.png` 能被正確讀取：

1. 在 `Assets/Image/UI/` 中確認有 `zbutton.png` 文件
2. 在 Inspector 中選中 zbutton.png，確認：
   - **Texture Type**: `Sprite (2D and UI)`
   - **Sprite Mode**: `Single`
3. 將 zbutton.png 複製一份到 `Assets/Resources/UI/` 目錄（如果 Resources 目錄不存在則創建）
   - **原因**：`InventorySlotPrompt` 會從 Resources 中讀取

### 步驟 7：檢查 FloatingTextManager 的 Canvas

1. 找到場景中的 **Canvas** GameObject
2. 確認 `FloatingTextManager` 組件的 **Canvas** 欄位指向此 Canvas
3. 這樣浮動文字才能正確顯示

## 驗證清單

運行遊戲後檢查以下項目：

- [ ] InventoryPromptManager 在 Hierarchy 中存在
- [ ] FloatingTextManager 在 Hierarchy 中存在
- [ ] Player 有 InventorySystem 組件
- [ ] Player 有足夠的 rope 在背包中
- [ ] Cable GameObject 有 `cable` 組件
- [ ] Cable 有 Collider2D（預設 Is Trigger = false）
- [ ] Cable 有 Animator（可選但推薦）
- [ ] 靠近 cable 時，背包中的 rope 上方出現 Z 鍵提示 ✅
- [ ] 按 Z 鍵時 rope 播放消耗動畫 ✅
- [ ] Cable 觸發動畫 ✅
- [ ] Rope 消耗後消失（如果只有 1 個） ✅
- [ ] Console 無錯誤訊息 ✅

## 常見問題排查

### 問題 1：背包沒有顯示 Rope

**原因**：Rope 沒有添加到 InventorySystem 的 All Game Items 列表

**解決**：
1. 選擇 Player GameObject
2. 找到 InventorySystem 組件
3. 在 All Game Items 中添加 rope.asset

### 問題 2：Z 鍵提示不顯示

**原因**：InventoryPromptManager 未找到或 zbutton.png 未能加載

**解決**：
1. 確認 InventoryPromptManager GameObject 存在
2. 檢查 Console 是否有 "InventoryPromptManager not found" 的警告
3. 確認 zbutton.png 在 Assets/Resources/UI/ 目錄中

### 問題 3：靠近 Cable 沒有反應

**原因**：Cable 的 Collider 可能設置不正確或玩家 tag 不匹配

**解決**：
1. 確認 Cable 有 Collider2D 組件
2. 確認 Cable 在某個圖層上（用於碰撞檢測）
3. 檢查 cable.cs 的 `OnInteractorEnterZone` 是否被調用（查看 Console 的 Debug 訊息）

### 問題 4：按 Z 鍵後沒有消耗 Rope

**原因**：ItemId 或 Tag 不匹配

**解決**：
1. 確認 rope.asset 的 itemId 是 "rope"
2. 確認 cable.cs 中的 `requiredRopeItemId` 也是 "rope"
3. 檢查 Console 中的 Debug 訊息

## 快速檢查列表

如果一切設置正確，以下動作應該能完美運作：

```
1. 遊戲開始
   ✓ Rope 在背包中

2. 走向 Cable
   ✓ Console 顯示 "Interactor entered cable zone"

3. 靠近 Cable（在 Collider 範圍內）
   ✓ 背包中的 Rope 上方出現閃動的 Z 鍵按鈕

4. 按 Z 鍵
   ✓ Rope 圖示播放消耗動畫（放大→閃爍→縮小）
   ✓ Console 顯示 "Using rope to activate cable"
   ✓ Cable 播放動畫
   ✓ 顯示綠色浮動文字 "Cable activated!"

5. 再次靠近 Cable（如果修復了）
   ✓ 可以正常使用 Rappelling
```

## 相關文檔

- `CABLE_ROPE_SYSTEM.md` - 系統詳細說明
- `INVENTORY_PROMPT_SETUP.md` - 背包提示系統指南

