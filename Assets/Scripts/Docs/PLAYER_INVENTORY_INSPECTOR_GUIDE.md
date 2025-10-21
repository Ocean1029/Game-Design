# Player Inventory Inspector 使用指南

## 🎯 功能概述

Player Inventory Inspector 是一個強大的 Unity Editor 工具，讓您可以：

- **檢查玩家當前收集的物品**
- **添加/移除物品進行測試**
- **查看已收集的世界道具**
- **管理存檔數據**
- **調試存檔系統**

## 🚀 使用方法

### 步驟 1：打開工具

1. 在 Unity 中選擇 `Tools > Inventory > Player Inventory Inspector`
2. 工具窗口會打開

### 步驟 2：檢查當前背包

**Current Inventory 區域**顯示：
- 玩家當前擁有的所有物品
- 每個物品的圖示、名稱、數量
- 物品 ID
- 移除按鈕

**範例顯示**：
```
• Red Key     x1    (key_red)
• Blue Key    x2    (key_blue)
• Hammer      x1    (hammer)
```

### 步驟 3：使用快速操作

**Quick Actions 區域**提供：

**Clear All Items**：
- 清空整個背包
- 需要確認對話框

**Refresh Display**：
- 刷新背包 UI 顯示
- 同步背包 UI 與實際數據

**Clear Save Data**：
- 清除所有存檔數據
- 重置背包和已收集的世界道具

**Reload from Save**：
- 強制從存檔重新載入背包
- 清除當前背包並重新載入

### 步驟 4：管理物品

**Item Management 區域**顯示：
- 所有可用的遊戲道具
- 每個道具的當前數量
- 添加/移除按鈕

**操作按鈕**：
- **+1**：添加 1 個道具
- **+5**：添加 5 個道具
- **-1**：移除 1 個道具（僅在數量 > 0 時顯示）

### 步驟 5：檢查已收集的世界道具

**Collected World Items 區域**顯示：
- 所有已收集的世界道具 ID
- 每個道具的 Uncollect 按鈕

**Uncollect 功能**：
- 讓已收集的世界道具重新出現
- 需要確認對話框
- 用於測試或重置特定道具

### 步驟 6：查看存檔系統信息

**Save System Info 區域**顯示：
- 存檔系統的詳細信息
- 測試存檔功能
- 調試信息

## 🔧 詳細功能說明

### 檢查玩家收集的物品

**方法 1：使用 Player Inventory Inspector**
1. 打開工具
2. 查看 "Current Inventory" 區域
3. 看到所有當前擁有的物品

**方法 2：使用 InventorySystem Inspector**
1. 選中場景中的 Player GameObject
2. 在 Inspector 中找到 InventorySystem 組件
3. 在 Play Mode 下查看 "Runtime Info" 區域

**方法 3：使用 Console**
```csharp
// 在腳本中
InventorySystem inventory = FindFirstObjectByType<InventorySystem>();
List<InventoryItem> items = inventory.GetAllItems();
foreach (var item in items)
{
    Debug.Log($"{item.itemData.itemName}: {item.quantity}");
}
```

### 調整玩家收集的物品

**添加物品**：
1. 在 Player Inventory Inspector 中
2. 找到要添加的物品
3. 點擊 "+1" 或 "+5" 按鈕

**移除物品**：
1. 在 "Current Inventory" 區域
2. 找到要移除的物品
3. 點擊 "Remove" 按鈕

**清空背包**：
1. 點擊 "Clear All Items" 按鈕
2. 確認對話框

### 管理已收集的世界道具

**查看已收集的世界道具**：
1. 在 Player Inventory Inspector 中
2. 查看 "Collected World Items" 區域
3. 看到所有已收集的世界道具 ID

**讓道具重新出現**：
1. 找到要重新出現的道具 ID
2. 點擊 "Uncollect" 按鈕
3. 確認對話框
4. 重新進入場景，道具會重新出現

### 存檔系統管理

**清除所有存檔**：
1. 點擊 "Clear Save Data" 按鈕
2. 確認對話框
3. 所有存檔數據被清除

**測試存檔功能**：
1. 點擊 "Test Save" 按鈕
2. 當前背包狀態被保存
3. 檢查 Console 確認保存成功

**查看存檔信息**：
1. 點擊 "Show Debug Info" 按鈕
2. Console 顯示詳細的存檔信息

## 🎯 使用場景

### 場景 1：測試遊戲流程

**目標**：測試特定道具組合
**步驟**：
1. 清空背包
2. 添加需要的道具
3. 測試遊戲功能
4. 重複測試不同組合

### 場景 2：調試道具問題

**目標**：解決道具相關問題
**步驟**：
1. 檢查當前背包狀態
2. 添加缺失的道具
3. 測試道具功能
4. 檢查 Console 錯誤

### 場景 3：重置遊戲狀態

**目標**：重置玩家進度
**步驟**：
1. 清空背包
2. 清除已收集的世界道具
3. 重新開始遊戲

### 場景 4：測試存檔系統

**目標**：驗證存檔功能
**步驟**：
1. 添加一些道具
2. 測試保存功能
3. 重新載入遊戲
4. 檢查道具是否正確載入

## 🔍 故障排除

### 常見問題

**1. 工具無法找到 InventorySystem**
- 確保場景中有 Player GameObject
- 確保 Player 有 InventorySystem 組件
- 確保遊戲正在運行（Play Mode）

**2. 背包 UI 沒有更新**
- 點擊 "Refresh Display" 按鈕
- 檢查 BackpackUIController 是否存在
- 確認背包 UI 事件訂閱正常

**3. 存檔數據沒有保存**
- 檢查 Console 是否有錯誤
- 確認 SaveSystem 正常工作
- 使用 "Test Save" 功能測試

**4. 世界道具沒有重新出現**
- 確認道具 ID 正確
- 重新進入場景
- 檢查 CollectableItem 組件

### Debug 模式

啟用 Debug 模式查看詳細信息：
1. 勾選 "Show Debug Info"
2. 查看詳細的存檔信息
3. 使用 "Show Debug Info" 按鈕輸出到 Console

## 💡 最佳實踐

1. **測試前備份**：在測試前先保存當前狀態
2. **逐步測試**：一次測試一個功能
3. **記錄問題**：記錄發現的問題和解決方法
4. **定期檢查**：定期使用工具檢查遊戲狀態
5. **清理數據**：測試完成後清理測試數據

## 🎉 完成！

現在您有一個完整的工具來：
- ✅ 檢查玩家收集的所有物品
- ✅ 添加/移除物品進行測試
- ✅ 管理已收集的世界道具
- ✅ 調試存檔系統
- ✅ 重置遊戲狀態

享受您的新物品管理工具吧！
