# 實體門設置指南

## 概述

`PhysicalDoor` 腳本實現了一個有碰撞的實體門，當玩家碰撞到門時：
- 如果玩家有正確的鑰匙，會消耗鑰匙並將門變成透明
- 如果玩家沒有鑰匙，會顯示提示訊息
- 門開啟後，玩家可以穿過（碰撞器被禁用）

## 功能特點

- ✅ **實體碰撞**：門有真實的碰撞，玩家無法穿過
- ✅ **鑰匙檢測**：支援 InventorySystem 和 KeyInventory 兩種鑰匙系統
- ✅ **鑰匙消耗**：開啟門時會消耗一把鑰匙
- ✅ **平滑動畫**：門開啟時會有透明度動畫效果
- ✅ **音效支援**：支援開啟和關閉音效
- ✅ **粒子效果**：支援開啟時的視覺效果
- ✅ **浮動文字**：沒有鑰匙時會顯示提示

## Unity 設置步驟

### 第一步：創建門物件

1. **創建空的 GameObject**：
   - 右鍵 Hierarchy → Create Empty
   - 命名為 "PhysicalDoor"

2. **添加 SpriteRenderer**：
   - 選擇 PhysicalDoor
   - Add Component → Sprite Renderer
   - 設置門的圖片

3. **添加 Collider2D**：
   - Add Component → Collider 2D
   - 選擇合適的碰撞器形狀（Box Collider 2D 或 Polygon Collider 2D）
   - **重要**：確保 `Is Trigger` 是 **取消勾選** 的

4. **添加 PhysicalDoor 腳本**：
   - Add Component → Scripts → Physical Door

### 第二步：配置門的參數

在 PhysicalDoor 組件中設置：

```
門的配置：
- Required Key Tag: "key1" (或你想要的鑰匙標籤)
- Open Transparency: 0.5 (50% 透明度)
- Animation Duration: 1.0 (動畫持續時間)

音效：
- Open Sound: 門開啟音效
- Close Sound: 門關閉音效

視覺效果：
- Open Effect: 門開啟時的粒子效果
```

### 第三步：設置玩家

確保玩家有以下組件之一：

**選項 1：使用 InventorySystem**
- 玩家需要有 `InventorySystem` 組件
- 鑰匙需要設置為 `ItemData` 並添加到物品欄

**選項 2：使用 KeyInventory**
- 玩家需要有 `KeyInventory` 組件
- 鑰匙使用 `Key.cs` 腳本收集

### 第四步：測試

1. **運行遊戲**
2. **沒有鑰匙時**：嘗試撞門，應該會顯示 "需要鑰匙！" 的浮動文字
3. **有鑰匙時**：撞門，鑰匙會被消耗，門會變成透明並可以穿過

## 進階設置

### 自定義動畫

你可以修改以下參數來調整動畫效果：

```csharp
// 在 PhysicalDoor 腳本中
public float openTransparency = 0.5f;  // 開啟時的透明度
public float animationDuration = 1f;   // 動畫持續時間
```

### 添加音效

1. **準備音效檔案**：
   - 將音效檔案拖到 Project 視窗
   - 設置為 AudioClip

2. **設置音效**：
   - 在 PhysicalDoor 組件中
   - 拖曳音效到 `Open Sound` 和 `Close Sound` 欄位

### 添加粒子效果

1. **創建粒子系統**：
   - 創建 Particle System
   - 設置你想要的視覺效果
   - 製作成 Prefab

2. **設置效果**：
   - 在 PhysicalDoor 組件中
   - 拖曳粒子 Prefab 到 `Open Effect` 欄位

## 腳本 API

### 公共方法

```csharp
// 檢查門是否已開啟
bool IsOpened()

// 關閉門（可選功能）
void CloseDoor()

// 重置門的狀態
void ResetDoor()
```

### 事件

腳本會在 Console 中輸出以下訊息：
- `門被開啟！消耗鑰匙: [keyTag]`
- `需要鑰匙才能開啟這扇門: [keyTag]`
- `門已完全開啟！`
- `門已關閉！`
- `門已重置！`

## 故障排除

### 常見問題

1. **門沒有碰撞**：
   - 檢查 Collider2D 的 `Is Trigger` 是否取消勾選
   - 確保 Collider2D 的大小正確

2. **鑰匙沒有被消耗**：
   - 檢查 `Required Key Tag` 是否與鑰匙的標籤一致
   - 確保玩家有對應的鑰匙系統組件

3. **動畫沒有播放**：
   - 檢查 `Animation Duration` 是否大於 0
   - 確保 SpriteRenderer 存在

4. **音效沒有播放**：
   - 檢查 AudioSource 組件是否存在
   - 確保音效檔案已正確設置

### 調試技巧

1. **查看 Console**：
   - 所有重要事件都會在 Console 中顯示
   - 檢查是否有錯誤訊息

2. **檢查組件**：
   - 確保所有必要組件都已添加
   - 檢查組件的設置是否正確

3. **測試鑰匙系統**：
   - 使用 `SaveSystem.GetDebugInfo()` 查看存檔狀態
   - 檢查鑰匙是否正確添加到玩家身上

## 與其他系統的整合

### 與存檔系統整合

門的狀態不會自動保存，如果需要保存門的狀態，可以：

1. **在門開啟時保存狀態**：
```csharp
// 在 OpenDoorAnimation 的最後添加
SaveSystem.MarkItemCollected($"door_{gameObject.name}");
```

2. **在遊戲開始時檢查狀態**：
```csharp
// 在 Start() 方法中添加
if (SaveSystem.IsItemCollected($"door_{gameObject.name}"))
{
    // 門已經開啟，直接設置為開啟狀態
    isOpened = true;
    spriteRenderer.color = targetColor;
    doorCollider.enabled = false;
}
```

### 與 UI 系統整合

可以添加 UI 提示來改善用戶體驗：

1. **創建 UI 提示**：
   - 創建 Canvas 和 Text 元件
   - 設置為顯示 "需要鑰匙！" 的訊息

2. **在腳本中引用**：
```csharp
public GameObject keyRequiredUI;
```

3. **在 ShowKeyRequiredMessage 中顯示**：
```csharp
if (keyRequiredUI != null)
{
    keyRequiredUI.SetActive(true);
}
```
