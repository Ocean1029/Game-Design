# Cable & Rope 系統說明文件

## 概述

Cable（繩索下降）系統已更新，現在 cable 預設為壞掉狀態，玩家需要使用 rope（繩索）道具來修復才能使用。

## 系統特性

### Cable 狀態

1. **壞掉狀態（預設）**：
   - Cable 不可穿透（solid collider）
   - 玩家無法使用 rappelling 功能
   - 視覺上顯示壞掉的樣子（如果設置了 brokenVisual）

2. **修復狀態**：
   - Cable 可穿透（trigger collider）
   - 玩家可以按 Z 鍵使用 rappelling 下降
   - 視覺上顯示修復的樣子（如果設置了 repairedVisual）

### 修復流程

1. 玩家撿到 rope（繩索）道具
2. 靠近壞掉的 cable
3. **背包中的 rope 物件上方會顯示閃動的 Z 鍵提示**
4. 按下 **Z 鍵**消耗 rope 修復 cable
5. Cable 變成可使用狀態（可穿透）
6. 再次按 Z 鍵可以使用 rappelling 下降

## 設置步驟

### 1. 在場景中設置 Cable

1. 選擇場景中的 Cable GameObject
2. 在 `cable` 組件的 Inspector 中：

#### Cable State（狀態設定）
- **Start Broken**: ✓ 勾選（預設為壞掉）

#### Rope Requirement（繩索需求）
- **Required Rope Item Id**: `rope`
- **Required Rope Tag**: `rope`

#### Visual Feedback（視覺回饋，可選）
- **Broken Visual**: 拖入壞掉狀態的視覺物件（例如：斷裂的繩索圖片）
- **Repaired Visual**: 拖入修復狀態的視覺物件（例如：完整的繩索）

### 2. 將 Rope 添加到遊戲

#### 方法 A：創建 Rope 收集物

1. 在場景中創建一個 GameObject（例如：`Rope_Pickup`）
2. 添加 `SpriteRenderer` 組件，設置 rope 的圖片
3. 添加 `BoxCollider2D` 組件，設為 `Is Trigger`
4. 添加 `CollectableItem` 組件
5. 在 `CollectableItem` 組件中：
   - **Item Data**: 拖入 `Assets/ScriptableObjects/Items/rope.asset`
   - **Quantity**: `1`

#### 方法 B：測試用（直接添加到背包）

在 PlayerController 的 `Start()` 方法中臨時添加：

```csharp
void Start()
{
    // ... 現有程式碼 ...
    
    // 測試用：直接添加 rope 到背包
    InventorySystem inventory = GetComponent<InventorySystem>();
    if (inventory != null)
    {
        ItemData ropeItem = Resources.Load<ItemData>("ScriptableObjects/Items/rope");
        if (ropeItem != null)
        {
            inventory.AddItem(ropeItem, 1);
            Debug.Log("Added rope to inventory for testing");
        }
    }
}
```

### 3. 將 Rope 添加到 AllGameItems 列表

1. 選擇場景中的 Player GameObject
2. 找到 `InventorySystem` 組件
3. 在 `All Game Items` 列表中：
   - 點擊 `+` 新增一個槽位
   - 將 `Assets/ScriptableObjects/Items/rope.asset` 拖入新槽位

這樣背包 UI 才會顯示 rope 的格子。

## 使用流程

### 玩家視角

1. **撿到 rope**：
   - 觸碰場景中的 rope 收集物
   - rope 自動加入背包
   - 浮動文字顯示 "+ Rope"

2. **靠近壞掉的 cable**：
   - 如果身上有 rope：
     - 背包中的 rope 物件上方會出現**閃動的 Z 鍵提示**
   - 如果身上沒有 rope：
     - 顯示紅色浮動文字："需要繩索修復！"

3. **按 Z 鍵修復**：
   - 消耗 1 個 rope
   - rope 圖示播放消耗動畫（放大→閃爍→縮小→變暗）
   - Z 鍵提示消失
   - 顯示綠色浮動文字："Cable 已修復！"
   - Cable 變成可穿透狀態

4. **使用修復後的 cable**：
   - 靠近修復後的 cable
   - 顯示原有的使用提示（usePrompt）
   - 按 Z 鍵開始 rappelling 下降

## 程式碼結構

### 修改的檔案

1. **cable.cs** - 主要邏輯
   - 添加壞掉/修復狀態管理
   - 整合背包提示系統
   - 實現 rope 消耗功能

### 新增的檔案

1. **rope.asset** - Rope ItemData
   - 位置：`Assets/ScriptableObjects/Items/rope.asset`
   - Item ID: `rope`
   - Tag: `rope`
   - 可堆疊最多 5 個

2. **CABLE_ROPE_SYSTEM.md** - 本說明文件

## Inspector 參數說明

### Cable 組件參數

| 參數 | 說明 | 預設值 |
|------|------|--------|
| Start Broken | 是否預設為壞掉狀態 | true |
| Required Rope Item Id | 修復所需的 rope ItemData ID | "rope" |
| Required Rope Tag | 修復所需的 rope tag | "rope" |
| Target Floor Point | Rappelling 的目標位置 | - |
| Animation Trigger Name | 動畫觸發器名稱 | "player_enter" |
| Use Prompt | 使用時的 UI 提示（修復後顯示） | - |
| Broken Visual | 壞掉狀態的視覺物件 | - |
| Repaired Visual | 修復狀態的視覺物件 | - |

## 動畫效果

### Rope 消耗動畫

當玩家按 Z 修復 cable 時：

1. **提示消失**：Z 鍵提示立即隱藏
2. **消耗動畫**（約 1 秒）：
   - Rope 圖示放大到 1.2 倍
   - 閃爍橙色 3 次
   - 縮小回原大小
3. **最終狀態**：Rope 變成半透明（如果用完）

### Cable 修復效果

- 顯示綠色浮動文字："Cable 已修復！"
- Cable collider 從 solid 變成 trigger
- 切換視覺效果（如果有設置）

## 測試清單

- [ ] Cable 預設為壞掉狀態（solid collider）
- [ ] 玩家無法穿透壞掉的 cable
- [ ] 撿到 rope 後，背包顯示 rope 圖示
- [ ] 靠近壞掉的 cable 時，rope 上方顯示 Z 鍵提示
- [ ] 離開 cable 時，Z 鍵提示消失
- [ ] 按 Z 鍵成功修復 cable
- [ ] Rope 播放消耗動畫
- [ ] Rope 消耗後變成半透明（如果用完）
- [ ] 修復後的 cable 變成可穿透（trigger）
- [ ] 修復後的 cable 可以正常使用 rappelling
- [ ] 如果沒有 rope，顯示"需要繩索修復！"訊息

## 進階功能

### 手動設置 Cable 狀態

在程式碼中可以手動控制 cable 狀態：

```csharp
cable myCable = GameObject.Find("MyCable").GetComponent<cable>();

// 檢查狀態
if (myCable.IsBroken())
{
    Debug.Log("Cable is broken!");
}

// 手動設置為壞掉
myCable.SetBroken(true);

// 手動設置為修復
myCable.SetBroken(false);
```

### 設置多個 Cable

如果場景中有多個 cable：

1. 每個 cable 獨立管理狀態
2. 可以設置不同的 `startBroken` 值
3. 每個 cable 都需要單獨修復

### 自訂 Rope 需求

可以在 Inspector 中修改：

- **Required Rope Item Id**: 改成其他道具 ID
- **Required Rope Tag**: 改成其他 tag

例如：使用不同等級的 rope 修復不同的 cable。

## 疑難排解

### Cable 修復後還是不能穿透

1. 檢查 Console 是否有 "Set to REPAIRED state" 訊息
2. 檢查 cable 的 Collider2D.isTrigger 是否為 true
3. 檢查 `cableCollider` 是否正確獲取

### 背包沒有顯示 Rope

1. 確認 rope.asset 已加入到 InventorySystem 的 `All Game Items` 列表
2. 檢查 BackpackUIController 是否正常創建 slot
3. 查看 Console 是否有建立 slot 的訊息

### Z 鍵提示不顯示

1. 確認場景中有 InventoryPromptManager
2. 檢查 rope 的 itemId 是否為 "rope"
3. 查看 Console 是否有 "Showing rope prompt" 訊息
4. 確認 BackpackUIController 有正確註冊 slot prompt

### 按 Z 鍵沒反應

1. 確認玩家身上有 rope（檢查背包）
2. 確認玩家在 cable 的碰撞範圍內
3. 查看 Console 的 Debug 訊息
4. 確認 PlayerController 的 interactKey 設為 Z

## 相關文件

- `INVENTORY_PROMPT_SETUP.md` - 背包提示系統設置指南
- `CHANGES_SUMMARY.md` - 系統更新摘要

