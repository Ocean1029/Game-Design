# Backpack UI Creator 使用指南

## 🎯 功能概述

Backpack UI Creator 是一個 Unity Editor 工具，可以快速創建一個完整的背包 UI 系統：

### ✨ 主要特點

- **固定位置**：背包容器自動定位在畫面右上角
- **自動布局**：格子自動排列成網格狀
- **狀態顯示**：
  - 未拾取：顯示物件外框（半透明灰色）
  - 已拾取：顯示完整圖像（全彩）
- **事件驅動**：自動響應道具收集、使用、移除事件
- **完全自訂**：可調整格子大小、間距、顏色等

## 🚀 使用方法

### 步驟 1：打開工具

1. 在 Unity 中選擇 `Tools > Inventory > Create Backpack UI`
2. 工具窗口會打開

### 步驟 2：調整設置（可選）

**Layout Settings（布局設置）**：
- **Slots Per Row**：每行格子數量（預設：4）
- **Slot Size**：格子大小（預設：60）
- **Slot Spacing**：格子間距（預設：10）
- **Container Margin**：容器邊距（預設：20）

**Visual Settings（視覺設置）**：
- **Empty Slot Color**：未拾取格子的顏色（預設：半透明灰色）
- **Collected Slot Color**：已拾取格子的顏色（預設：白色）
- **Container Background**：容器背景顏色（預設：半透明黑色）

### 步驟 3：創建背包 UI

1. 點擊 "Create Backpack UI" 按鈕
2. 工具會自動：
   - 檢查 Canvas 和 InventorySystem
   - 創建 BackpackContainer GameObject
   - 為所有遊戲道具創建格子
   - 設置所有必要的引用
   - 定位在右上角

## 📁 創建的檔案結構

```
Hierarchy:
Canvas
└── BackpackContainer
    ├── Slot_item1
    ├── Slot_item2
    ├── Slot_item3
    └── ...

Components:
BackpackContainer
├── RectTransform
├── Image (背景)
└── BackpackUIController

Each Slot
├── RectTransform
├── Image (道具圖示)
├── BackpackSlotUI
└── Button (互動)
```

## 🔧 系統運作原理

### 初始化流程

1. **BackpackUIController.Start()** 被調用
2. **驗證引用**：檢查所有必要的組件和引用
3. **訂閱事件**：監聽 InventorySystem 的道具事件
4. **創建格子**：為每個 ItemData 創建對應的格子
5. **計算布局**：自動計算容器大小和格子位置
6. **定位容器**：將容器放置在右上角

### 事件處理

**道具收集時**：
```csharp
OnItemCollected(ItemData itemData, int quantity)
{
    // 找到對應的格子
    // 更新顯示狀態（從外框變為完整圖像）
}
```

**道具使用時**：
```csharp
OnItemUsed(ItemData itemData)
{
    // 更新格子顯示
    // 如果數量歸零，變回外框狀態
}
```

**道具移除時**：
```csharp
OnItemRemoved(ItemData itemData)
{
    // 更新格子顯示
    // 變回外框狀態
}
```

### 視覺狀態切換

**未拾取狀態**：
- 圖像顏色：`emptyColor`（半透明灰色）
- 視覺效果：只顯示物件外框

**已拾取狀態**：
- 圖像顏色：`collectedColor`（全彩）
- 視覺效果：顯示完整圖像

## 🎨 自訂選項

### 改變布局

**3x3 網格**：
```
Slots Per Row: 3
Slot Size: 70
Slot Spacing: 15
```

**單列水平排列**：
```
Slots Per Row: 10 (或你的道具總數)
Slot Size: 50
Slot Spacing: 5
```

**緊湊排列**：
```
Slots Per Row: 6
Slot Size: 45
Slot Spacing: 3
```

### 改變視覺樣式

**深色主題**：
```
Empty Slot Color: (0.2, 0.2, 0.2, 0.5)
Collected Slot Color: (1, 1, 1, 1)
Container Background: (0, 0, 0, 0.3)
```

**彩色主題**：
```
Empty Slot Color: (0.5, 0.5, 0.5, 0.3)
Collected Slot Color: (1, 0.8, 0, 1) // 金色
Container Background: (0.1, 0.1, 0.2, 0.2)
```

### 改變位置

**左下角**：
```csharp
// 在 BackpackUIController.PositionContainer() 中修改
containerRect.anchorMin = new Vector2(0, 0);
containerRect.anchorMax = new Vector2(0, 0);
containerRect.pivot = new Vector2(0, 0);
containerRect.anchoredPosition = new Vector2(containerMargin, containerMargin);
```

**中央頂部**：
```csharp
containerRect.anchorMin = new Vector2(0.5f, 1);
containerRect.anchorMax = new Vector2(0.5f, 1);
containerRect.pivot = new Vector2(0.5f, 1);
containerRect.anchoredPosition = new Vector2(0, -containerMargin);
```

## 🔍 故障排除

### 常見問題

**1. 工具無法創建背包 UI**

**可能原因**：
- 場景中沒有 Canvas
- 場景中沒有 InventorySystem
- InventorySystem 的 All Game Items 列表為空

**解決方法**：
- 確保場景中有 Canvas
- 確保場景中有 InventorySystem
- 在 InventorySystem 中添加 ItemData 到 All Game Items 列表

**2. 格子沒有顯示**

**可能原因**：
- ItemData 沒有設置 icon sprite
- 格子位置計算錯誤
- 容器大小設置錯誤

**解決方法**：
- 檢查 ItemData 的 icon 欄位
- 調整 Slot Size 和 Slot Spacing
- 檢查 Container Margin 設置

**3. 格子狀態不更新**

**可能原因**：
- 事件訂閱失敗
- InventorySystem 引用錯誤
- BackpackSlotUI.UpdateDisplay() 沒有被調用

**解決方法**：
- 檢查 Console 是否有錯誤訊息
- 確認 InventorySystem 正常工作
- 手動調用 RefreshAllSlots()

### Debug 模式

啟用 Debug 模式來查看詳細訊息：
```csharp
// 在 BackpackUIController 中
showDebugInfo = true;

// 在 BackpackSlotUI 中
showDebugInfo = true;
```

## 📊 效能考量

### 記憶體使用

- 每個格子：約 4-5 個組件
- 20 個道具：約 100 個組件
- 記憶體影響：可忽略

### 更新頻率

- 只在事件觸發時更新
- 不會每幀更新
- 效能影響：極小

### 優化建議

**大量道具時**：
- 考慮使用 ScrollRect 讓格子可滾動
- 只在可見範圍內更新格子
- 使用 Object Pooling 重用格子

## 🔄 與現有系統整合

這個背包 UI 系統完全相容現有系統：

- ✅ **InventorySystem**：自動偵測所有道具
- ✅ **CollectableItem**：收集時自動更新
- ✅ **SaveSystem**：永久記住收集狀態
- ✅ **ItemData**：使用現有道具定義
- ✅ **door.cs**：道具使用功能正常

**無需修改任何現有程式碼**！

## 🎯 最佳實踐

1. **使用工具創建**：不要手動創建，使用 Editor Tool
2. **測試不同道具數量**：確保布局適合你的遊戲
3. **調整視覺設置**：讓背包 UI 符合遊戲風格
4. **啟用 Debug 模式**：開發時查看詳細訊息
5. **保存設置**：找到合適的設置後記錄下來

## 🎉 完成！

現在您有一個完整的背包 UI 系統：
- ✅ 固定在右上角
- ✅ 自動顯示所有道具
- ✅ 未拾取顯示外框
- ✅ 已拾取顯示完整圖像
- ✅ 完全自動化運作
- ✅ 易於自訂和擴展

享受您的新背包 UI 系統吧！
