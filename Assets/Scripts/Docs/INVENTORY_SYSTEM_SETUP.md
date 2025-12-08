# 背包系統設置指南

## 系統概述

新的背包系統提供完整的物品管理功能：

1. ✅ **顯示所有可能的道具框框**（空/實心狀態）
2. ✅ **自動收集功能**（碰觸物品自動加入背包）
3. ✅ **手動使用 & 自動觸發**（兩種使用模式）
4. ✅ **使用提示浮窗**（靠近可用地點顯示提示）
5. ✅ **浮現文字通知**（獲得物品時顯示）

---

## 新增的腳本

### 核心系統

1. **ItemData.cs** - 物品數據定義（ScriptableObject）
2. **InventoryItem.cs** - 背包物品實例
3. **InventorySystem.cs** - 背包系統管理器
4. **IItemUsable.cs** - 可使用物品的接口

### 物品組件

5. **CollectableItem.cs** - 可收集的物品

### UI 組件

6. **InventoryUI.cs** - 背包 UI 管理器
7. **InventorySlotUI.cs** - 單個物品格子 UI

---

## Unity Editor 設置步驟

### 第一步：創建 ItemData（ScriptableObject）

1. **在 Project 視窗右鍵**：
   - Create → Inventory → Item Data

2. **設置紅色鑰匙範例**：
   ```
   Item ID: "key_red"
   Item Name: "紅色鑰匙"
   Description: "可以打開紅色的門"
   Icon: [鑰匙圖示]
   
   Item Type: Key
   Use Type: Auto Use  （靠近門自動使用）
   Is Consumable: true  （使用後消失）
   Max Stack Size: 1
   
   Interactable Tag: "door_red"
   Use Message: "使用了 {itemName}"
   Can Use Hint Message: "按 E 使用 {itemName}"
   
   Collect Sound: [收集音效]
   Use Sound: [使用音效]
   ```

3. **創建更多物品**：
   - 藍色鑰匙（door_blue）
   - 工具（tool）
   - 材料（material）
   - 等等...

4. **保存到**：`Assets/ScriptableObjects/Items/`

### 第二步：設置 Player

1. **選擇 Player GameObject**

2. **添加 InventorySystem 組件**：
   - Add Component → InventorySystem
   - Max Inventory Slots: 20
   - All Game Items: 拖曳所有 ItemData 到列表中

### 第三步：創建背包 UI

#### 3.1 創建 InventorySlot Prefab

1. **在 Canvas 下創建 Image**：
   - 命名為 "InventorySlot"
   - Size: (64, 64)

2. **添加子物件**：
   
   **Background（背景框）**：
   - Image Component
   - Color: White
   - Sprite: 框框圖片
   
   **Icon（物品圖示）**：
   - Image Component  
   - RaycastTarget: false
   - Preserve Aspect: true
   
   **QuantityText（數量文字）**：
   - TextMeshProUGUI
   - Alignment: Bottom-Right
   - Font Size: 16
   - Color: White
   - Outline: 黑色
   
   **EmptyOverlay（空狀態遮罩）**：
   - Image Component
   - Color: (0, 0, 0, 0.7) - 半透明黑色
   - RaycastTarget: false

3. **添加組件到 InventorySlot**：
   - Button Component
   - InventorySlotUI Component:
     - Background Image: Background
     - Icon Image: Icon
     - Quantity Text: QuantityText
     - Empty Overlay: EmptyOverlay
     - Collected Color: (1, 1, 1, 1)
     - Empty Color: (1, 1, 1, 0.3)
     - Can Use Highlight: (1, 1, 0, 0.5)

4. **製作成 Prefab**：
   - 拖曳到 `Assets/Prefabs/UI/InventorySlot.prefab`
   - 刪除場景中的實例

#### 3.2 創建背包主 UI

1. **在 Canvas 下創建 Panel**：
   - 命名為 "InventoryPanel"
   - Anchor: Stretch (全螢幕)
   - Color: (0, 0, 0, 0.8) - 半透明黑色背景

2. **在 InventoryPanel 下創建 Grid Layout**：
   - 命名為 "SlotContainer"
   - Grid Layout Group:
     - Cell Size: (70, 70)
     - Spacing: (10, 10)
     - Child Alignment: Upper Center
     - Constraint: Fixed Column Count = 5

3. **在 InventoryPanel 下創建標題**：
   - TextMeshProUGUI: "背包"
   - Font Size: 32
   - Alignment: Top Center

4. **在 InventoryPanel 下創建關閉按鈕**：
   - Button: "關閉 (Tab)"

5. **創建 Use Hint Panel**：
   - 在 Canvas 下創建 Panel
   - 命名為 "UseHintPanel"
   - Anchor: Bottom Center
   - Position: (0, 100)
   - Size: (300, 60)
   - Color: (0, 0, 0, 0.7)
   
6. **在 UseHintPanel 下創建文字**：
   - TextMeshProUGUI: "按 E 使用鑰匙"
   - Font Size: 20
   - Alignment: Center
   - Color: Yellow

7. **在 Canvas 下創建 InventoryUI GameObject**：
   - 添加 InventoryUI 組件:
     - Slot Container: SlotContainer
     - Slot Prefab: InventorySlot prefab
     - Inventory Panel: InventoryPanel
     - Use Hint Text: UseHintPanel 的 Text
     - Use Hint Panel: UseHintPanel

8. **初始狀態**：
   - InventoryPanel: Inactive
   - UseHintPanel: Inactive

### 第四步：創建可收集物品

1. **創建物品 GameObject**：
   - 添加 Sprite Renderer（物品圖示）
   - 添加 Collider2D（Is Trigger = true）
   - 添加 Rigidbody2D（如果需要物理）

2. **添加 CollectableItem 組件**：
   - Item Data: 拖曳對應的 ItemData
   - Quantity: 1
   - Collect Effect: 可選，收集特效
   - Collect Sound: 可選，收集音效

3. **Layer 設置**：
   - 確保能與 Player 碰撞

### 第五步：更新 Door

**不需要手動修改！** Door 已自動支持新系統。

只需確保：
- Door 的 `Required Key Tag` = ItemData 的 `Interactable Tag`

---

## 使用方式

### 玩家操作

1. **撿取物品**：
   - 走到物品上自動收集
   - 顯示浮現文字："獲得 [物品名稱]"

2. **打開背包**：
   - 按 Tab 或 I 鍵
   - 顯示所有物品格子（空/實心）

3. **使用物品**：
   
   **自動使用（Auto Use）**：
   - 靠近可用地點自動觸發
   - 例如：拿著鑰匙靠近門會自動開門
   
   **手動使用（Manual）**：
   - 靠近可用地點時顯示提示："按 E 使用 [物品名稱]"
   - 按 E 鍵使用
   - 可以在背包中點擊物品查看描述

### 兩種使用模式比較

| 特性 | Auto Use | Manual |
|------|----------|--------|
| 觸發方式 | 靠近自動 | 按 E 手動 |
| 適用場景 | 鑰匙開門 | 工具使用、消耗品 |
| UI 提示 | 無需提示 | "按 E 使用..." |
| 範例 | 鑰匙、通行證 | 工具、藥水、材料 |

---

## ItemData 設定範例

### 範例 1：自動使用鑰匙

```
Item ID: "key_red"
Item Name: "紅色鑰匙"
Item Type: Key
Use Type: Auto Use
Is Consumable: true
Interactable Tag: "door_red"
```

### 範例 2：手動使用工具

```
Item ID: "hammer"
Item Name: "鐵鎚"
Item Type: Tool
Use Type: Manual
Is Consumable: false
Interactable Tag: "repair_spot"
Can Use Hint Message: "按 E 使用鐵鎚修理"
```

### 範例 3：任務道具

```
Item ID: "quest_letter"
Item Name: "神秘信件"
Item Type: QuestItem
Use Type: Passive
Is Consumable: false
Interactable Tag: ""
Description: "這是一封神秘的信件..."
```

---

## 創建新的可使用物件

如果你想創建新的可使用物件（如修理點、合成台等）：

1. **實現 IItemUsable 接口**：

```csharp
public class RepairStation : MonoBehaviour, IItemUsable
{
    public bool UseItem(ItemData item, InventorySystem inventory)
    {
        if (item.itemId == "hammer")
        {
            // 執行修理邏輯
            Debug.Log("修理完成！");
            return true;
        }
        return false;
    }
    
    public string GetUsableTag()
    {
        return "repair_spot";
    }
}
```

2. **註冊/取消註冊**（在 trigger 中）：

```csharp
void OnTriggerEnter2D(Collider2D collision)
{
    InventorySystem inventory = collision.GetComponent<InventorySystem>();
    if (inventory != null)
    {
        inventory.RegisterNearbyUsable(GetUsableTag(), gameObject);
    }
}

void OnTriggerExit2D(Collider2D collision)
{
    InventorySystem inventory = collision.GetComponent<InventorySystem>();
    if (inventory != null)
    {
        inventory.UnregisterNearbyUsable(GetUsableTag());
    }
}
```

---

## 系統流程圖

### 收集物品流程

```
玩家觸碰物品
    ↓
CollectableItem.OnTriggerEnter2D()
    ↓
InventorySystem.AddItem()
    ↓
OnItemCollected 事件觸發
    ↓
InventoryUI 更新顯示
InventorySlotUI 顯示物品圖示
    ↓
FloatingTextManager 顯示："獲得 [物品名稱]"
    ↓
物品被銷毀
```

### 自動使用流程（Auto Use）

```
玩家靠近門
    ↓
door.OnInteractorEnterZone()
    ↓
檢查 InventorySystem.GetItemForTag()
    ↓
有對應物品 + Auto Use
    ↓
door.OpenDoor() 自動開門
    ↓
移除物品（如果 isConsumable）
```

### 手動使用流程（Manual）

```
玩家靠近修理點
    ↓
RepairStation.OnTriggerEnter()
    ↓
InventorySystem.RegisterNearbyUsable()
    ↓
檢查背包是否有對應物品
    ↓
有物品 → OnCanUseItemChanged 事件
    ↓
InventoryUI 顯示："按 E 使用 [物品]"
    ↓
玩家按 E
    ↓
InventorySystem.UseItem()
    ↓
RepairStation.UseItem() 執行邏輯
    ↓
移除物品（如果 isConsumable）
```

---

## 測試步驟

1. **創建測試物品**：
   - 創建 ItemData（紅色鑰匙）
   - 創建 GameObject + CollectableItem

2. **創建測試門**：
   - Door 的 Required Key Tag = "door_red"

3. **測試自動使用**：
   - 撿取鑰匙
   - 背包顯示鑰匙圖示
   - 靠近門自動開啟

4. **測試手動使用**：
   - 創建 Manual 類型物品
   - 靠近可用地點
   - 看到提示："按 E 使用..."
   - 按 E 使用

5. **測試背包 UI**：
   - 按 Tab 打開
   - 看到所有物品格子
   - 已收集的亮起，未收集的暗淡

---

## 擴展功能建議

### 1. 物品堆疊

已支持！在 ItemData 中設置：
- Max Stack Size: 99（可堆疊）
- Max Stack Size: 1（不可堆疊）

### 2. 物品分類

使用 ItemType 枚舉：
- Key, Tool, Material, Consumable, QuestItem

可以在 UI 添加分類標籤

### 3. 物品排序

在 InventoryUI 添加排序按鈕：
```csharp
inventory.GetAllItems().OrderBy(i => i.itemData.itemType);
```

### 4. 物品詳情彈窗

點擊物品格子時顯示詳細資訊：
- 名稱、描述
- 數量、類型
- 使用方式

### 5. 持久化保存

整合到 SaveSystem：
```csharp
// 保存
string json = JsonUtility.ToJson(inventory.GetAllItems());
PlayerPrefs.SetString("Inventory", json);

// 載入
string json = PlayerPrefs.GetString("Inventory");
// 反序列化並添加到背包
```

### 6. 物品掉落

創建 DropItem 方法：
```csharp
public void DropItem(ItemData item)
{
    // 在玩家位置生成 CollectableItem
    GameObject dropped = Instantiate(collectableItemPrefab, player.position, Quaternion.identity);
    dropped.GetComponent<CollectableItem>().SetItemData(item);
    RemoveItem(item, 1);
}
```

---

## 常見問題

### Q: 物品沒有被收集？
A: 檢查：
- Player 是否有 InventorySystem 組件
- CollectableItem 是否有正確的 ItemData
- Collider2D 是否設為 Is Trigger
- Layer 碰撞設定

### Q: 背包 UI 沒有顯示物品？
A: 檢查：
- InventorySystem 的 All Game Items 是否包含該 ItemData
- InventorySlotUI 是否正確設置
- 事件是否正常觸發

### Q: 手動使用沒有顯示提示？
A: 檢查：
- ItemData 的 Use Type 是否為 Manual
- Interactable Tag 是否與目標物件匹配
- UseHintPanel 是否正確設置
- 目標物件是否有實現 IItemUsable 並註冊

### Q: 門沒有自動開啟？
A: 檢查：
- ItemData 的 Use Type 是否為 Auto Use
- Interactable Tag 是否與 Door 的 Required Key Tag 一致
- Door 是否有實現 IItemUsable
- Player 是否有 InventorySystem

---

## vs 舊鑰匙系統的優勢

| 特性 | 舊系統 | 新系統 |
|------|--------|--------|
| 物品類型 | 只有鑰匙 | 所有物品類型 |
| 顯示方式 | 單一鑰匙 UI | 完整背包 UI |
| 使用方式 | 只有自動 | 自動 + 手動 |
| 擴展性 | 困難 | 容易 |
| 提示系統 | 無 | 有使用提示 |
| 管理方式 | KeyInventory | 統一 InventorySystem |

新系統完全向後兼容，可以逐步遷移！

---

## 遷移指南（從舊系統）

1. **保留舊的 KeyInventory.cs** - 暫時不要刪除

2. **在 Player 添加 InventorySystem**

3. **創建對應的 ItemData** - 為每把鑰匙創建

4. **更新場景中的鑰匙**：
   - 移除 Key 組件
   - 添加 CollectableItem 組件

5. **測試兩個系統共存** - 確保都能工作

6. **逐步移除舊系統** - 當新系統穩定後

建議：先用新系統添加新物品，舊鑰匙暫時保留舊系統，逐步遷移。

