# Backpack Slot 底色功能指南

## 🎨 功能概述

現在每個背包格子都有底色功能，提供更好的視覺效果：

- **未收集狀態**：深灰色底色 + 半透明圖示
- **已收集狀態**：淺灰色底色 + 全彩圖示
- **高亮狀態**：藍色底色（用於特殊提示）

## 🔧 新增的設定選項

### BackpackSlotUI 組件設定

**Background Settings**：
- `emptyBackgroundColor`：未收集格子的底色（預設：深灰色）
- `collectedBackgroundColor`：已收集格子的底色（預設：淺灰色）
- `highlightBackgroundColor`：高亮格子的底色（預設：藍色）

**預設顏色**：
```csharp
emptyBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);     // 深灰色
collectedBackgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.8f); // 淺灰色
highlightBackgroundColor = new Color(0.4f, 0.4f, 0.6f, 0.9f); // 藍色
```

## 🎯 視覺效果

### 格子結構

```
BackpackSlot
├── Background (底色)
└── Icon (道具圖示)
```

**層級關係**：
- Background 在底層，提供底色
- Icon 在上層，顯示道具圖示
- Button 使用 Background 作為目標圖形

### 狀態變化

**未收集狀態**：
- 底色：深灰色 `(0.2, 0.2, 0.2, 0.8)`
- 圖示：半透明灰色 `(0.5, 0.5, 0.5, 0.3)`
- 效果：暗淡的格子，只顯示道具外框

**已收集狀態**：
- 底色：淺灰色 `(0.3, 0.3, 0.3, 0.8)`
- 圖示：全彩白色 `(1, 1, 1, 1)`
- 效果：明亮的格子，顯示完整道具圖像

**高亮狀態**：
- 底色：藍色 `(0.4, 0.4, 0.6, 0.9)`
- 圖示：保持原有顏色
- 效果：特殊提示，如可使用狀態

## 🚀 使用方法

### 自動創建（推薦）

使用 Backpack UI Creator 工具會自動創建帶有底色的格子：

1. 選擇 `Tools > Inventory > Create Backpack UI`
2. 點擊 "Create Backpack UI"
3. 工具會自動創建帶有背景的格子結構

### 手動設定

如果您想手動調整底色：

1. 選中 BackpackSlot GameObject
2. 在 BackpackSlotUI 組件中調整顏色
3. 運行遊戲查看效果

### 程式碼控制

**設定高亮**：
```csharp
BackpackSlotUI slotUI = GetComponent<BackpackSlotUI>();
slotUI.SetHighlight(true);  // 啟用高亮
slotUI.SetHighlight(false); // 關閉高亮
```

**更新顯示**：
```csharp
slotUI.UpdateDisplay(); // 根據道具狀態更新底色
```

## 🎨 自訂顏色

### 改變預設底色

**深色主題**：
```csharp
emptyBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);     // 更深的灰色
collectedBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f); // 深灰色
highlightBackgroundColor = new Color(0.3f, 0.3f, 0.5f, 1f);   // 深藍色
```

**彩色主題**：
```csharp
emptyBackgroundColor = new Color(0.3f, 0.2f, 0.2f, 0.8f);     // 深紅色
collectedBackgroundColor = new Color(0.2f, 0.3f, 0.2f, 0.8f); // 深綠色
highlightBackgroundColor = new Color(0.3f, 0.3f, 0.2f, 0.9f); // 深黃色
```

**透明主題**：
```csharp
emptyBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.3f);     // 半透明
collectedBackgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // 半透明
highlightBackgroundColor = new Color(0.4f, 0.4f, 0.6f, 0.7f); // 半透明
```

### 在 BackpackUICreator 中設定

您可以在 BackpackUICreator 工具中添加顏色設定選項：

```csharp
[Header("Background Settings")]
private Color emptyBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
private Color collectedBackgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
private Color highlightBackgroundColor = new Color(0.4f, 0.4f, 0.6f, 0.9f);
```

## 🔍 故障排除

### 常見問題

**1. 格子沒有底色**
- 檢查 BackpackSlotUI 的 `backgroundImage` 是否設定
- 確認 Background GameObject 存在
- 檢查 Background Image 組件是否正常

**2. 底色顏色不正確**
- 檢查 BackpackSlotUI 的顏色設定
- 確認 `UpdateDisplay()` 被正確調用
- 檢查是否有其他腳本覆蓋顏色

**3. 高亮功能不工作**
- 確認 `SetHighlight()` 方法被調用
- 檢查 `highlightBackgroundColor` 設定
- 確認 `backgroundImage` 不為 null

### Debug 模式

啟用 Debug 模式查看詳細信息：
```csharp
BackpackSlotUI slotUI = GetComponent<BackpackSlotUI>();
slotUI.showDebugInfo = true;
```

## 💡 最佳實踐

1. **顏色對比**：確保底色與圖示有足夠對比
2. **一致性**：所有格子使用相同的顏色方案
3. **可讀性**：避免過於鮮豔或過於暗淡的顏色
4. **主題統一**：底色應該與遊戲整體風格一致
5. **測試驗證**：在不同背景下測試顏色效果

## 🎯 進階功能

### 動態顏色變化

您可以根據道具類型設定不同的底色：

```csharp
public void UpdateDisplay()
{
    // 根據道具類別設定不同底色
    Color bgColor = GetBackgroundColorByCategory(itemData.category);
    backgroundImage.color = bgColor;
}

private Color GetBackgroundColorByCategory(ItemCategory category)
{
    switch (category)
    {
        case ItemCategory.Key:
            return new Color(0.3f, 0.2f, 0.2f, 0.8f); // 紅色系
        case ItemCategory.Tool:
            return new Color(0.2f, 0.3f, 0.2f, 0.8f); // 綠色系
        case ItemCategory.Material:
            return new Color(0.2f, 0.2f, 0.3f, 0.8f); // 藍色系
        default:
            return emptyBackgroundColor;
    }
}
```

### 動畫效果

您可以添加顏色過渡動畫：

```csharp
public void SetHighlight(bool highlight)
{
    if (backgroundImage == null) return;
    
    Color targetColor = highlight ? highlightBackgroundColor : GetNormalBackgroundColor();
    
    // 使用 DOTween 或其他動畫系統
    backgroundImage.DOColor(targetColor, 0.3f);
}
```

## 🎉 完成！

現在您的背包格子具有：
- ✅ 美觀的底色效果
- ✅ 清晰的狀態區分
- ✅ 高亮提示功能
- ✅ 完全可自訂的顏色
- ✅ 自動創建和設定

享受您的新背包 UI 視覺效果吧！



