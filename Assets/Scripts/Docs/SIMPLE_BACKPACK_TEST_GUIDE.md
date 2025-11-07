# Simple Backpack UI - Quick Test Guide

## ✅ 編譯錯誤已修正

所有編譯錯誤都已解決：

### 修正的問題

1. **事件簽名匹配**：
   - `OnItemUsed(ItemData itemData)` - 移除了多餘的 `int quantity` 參數
   - `OnItemRemoved(ItemData itemData)` - 移除了多餘的 `int quantity` 參數

2. **屬性名稱修正**：
   - 使用 `inventorySystem.GetAllGameItems()` 而不是 `inventorySystem.AllGameItems`
   - 這是正確的方法調用

3. **方法參數修正**：
   - `GetItemQuantity(ItemData itemData)` 使用正確的 `ItemData` 參數

4. **重新創建 SimpleSlotUI.cs**：
   - 文件被意外刪除，已重新創建

## 🚀 快速測試步驟

### 1. 使用 Editor Tool 創建 UI

1. 在 Unity 中選擇 `Tools > Inventory > Setup Simple Backpack UI`
2. 點擊 "Create Simple Backpack UI"
3. 檢查 Hierarchy 中是否出現 `BackpackContainer`

### 2. 驗證設置

**檢查 BackpackContainer**：
- 位置：右上角
- 組件：`SimpleBackpackUI` + `RectTransform` + `Image`
- 子物件：應該有對應數量的 `Slot_xxx` 物件

**檢查 SimpleBackpackUI 組件**：
- `backpackContainer`：應該指向 BackpackContainer
- `slotPrefab`：應該指向創建的 SimpleSlot prefab
- `slotsPerRow`：預設 4
- `slotSize`：預設 60
- `slotSpacing`：預設 10

### 3. 測試功能

**收集道具測試**：
1. 在場景中放置 `CollectableItem`
2. 確保 `InventorySystem` 的 `All Game Items` 列表包含該道具
3. 運行遊戲，讓玩家收集道具
4. 檢查背包中對應格子是否從暗沉變亮起

**使用道具測試**：
1. 確保玩家持有道具
2. 靠近可互動物品（如門）
3. 使用道具
4. 檢查格子是否正確更新

## 🔧 常見問題排除

### 問題 1：格子沒有出現

**可能原因**：
- `InventorySystem.All Game Items` 列表為空
- `slotPrefab` 沒有正確分配

**解決方法**：
```csharp
// 在 InventorySystem Inspector 中
// 點擊 "Auto Find All ItemData" 按鈕
// 或手動添加 ItemData 到 All Game Items 列表
```

### 問題 2：格子位置不正確

**可能原因**：
- Canvas 設置問題
- RectTransform 錨點設置錯誤

**解決方法**：
```csharp
// 檢查 Canvas 設置
Canvas Scaler:
  UI Scale Mode: Scale With Screen Size
  Reference Resolution: 1920 x 1080
  Screen Match Mode: Match Width Or Height
  Match: 0.5
```

### 問題 3：道具收集後格子沒有更新

**可能原因**：
- 事件訂閱失敗
- `SimpleSlotUI.UpdateDisplay()` 沒有被調用

**檢查方法**：
1. 開啟 Console 查看 Debug 訊息
2. 確認 `showDebugInfo = true`
3. 檢查是否有錯誤訊息

## 📊 預期行為

### 正常運作時應該看到：

**Console 訊息**：
```
SimpleBackpackUI: Start called
SimpleBackpackUI: Found InventorySystem
SimpleBackpackUI: Creating slots for X items
SimpleBackpackUI: Created slot for 'ItemName'
SimpleBackpackUI: Created X slots total
SimpleBackpackUI: Container positioned at top-right, size: WxH
SimpleBackpackUI: Setup complete
```

**視覺效果**：
- 右上角出現容器
- 格子按網格排列
- 未收集的道具：暗沉圖示
- 已收集的道具：亮起圖示

**互動效果**：
- 收集道具時格子立即更新
- 使用道具時格子正確反映數量變化

## 🎯 自訂選項

### 改變格子排列

**3x3 網格**：
```csharp
slotsPerRow = 3;
slotSize = 70f;
slotSpacing = 15f;
```

**單列水平**：
```csharp
slotsPerRow = 10;  // 或你的道具總數
slotSize = 50f;
slotSpacing = 5f;
```

### 改變視覺樣式

**添加背景**：
```csharp
// 在 BackpackContainer 的 Image 組件中
Color: (0, 0, 0, 0.3)  // 半透明黑色
```

**改變格子顏色**：
```csharp
// 在 SimpleSlotUI 組件中
emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);     // 更暗的灰色
collectedColor = new Color(1f, 0.8f, 0f, 1f);        // 金色
```

## 🔄 與現有系統整合

這個簡化版背包 UI 完全相容現有系統：

- ✅ **InventorySystem**：自動偵測所有道具
- ✅ **CollectableItem**：收集時自動更新
- ✅ **SaveSystem**：永久記住收集狀態
- ✅ **ItemData**：使用現有道具定義
- ✅ **door.cs**：道具使用功能正常

**無需修改任何現有程式碼**！

## 📈 效能優勢

相比複雜版 BackpackUI：

| 項目 | 複雜版 | 簡化版 |
|------|--------|--------|
| 層級數 | 4-5 層 | 2 層 |
| 每格子組件 | 4-5 個 | 1-2 個 |
| 記憶體使用 | 較高 | 最少 |
| 更新效能 | 良好 | 極佳 |
| 設置複雜度 | 高 | 低 |

## 🎉 完成！

您的簡化版背包 UI 現在應該可以正常工作了！這是一個極簡、高效的道具顯示系統，完全符合您的需求：右上角容器 + 簡單格子。
