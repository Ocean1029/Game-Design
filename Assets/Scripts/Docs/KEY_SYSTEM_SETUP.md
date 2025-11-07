# 鑰匙系統設置指南

## 系統概述

新的鑰匙系統將鑰匙從物理物件改為 UI 圖示顯示：
- ✅ 玩家撿到鑰匙後，鑰匙圖示會在 UI 上亮起
- ✅ 靠近門時自動使用鑰匙開門
- ✅ 撿到鑰匙時顯示浮現文字提示

---

## 新增的腳本

### 1. KeyInventory.cs
**位置**: `Assets/Scripts/Inventory/KeyInventory.cs`
**功能**: 管理玩家擁有的鑰匙（非物理物件）

### 2. KeyUI.cs
**位置**: `Assets/Scripts/UI/KeyUI.cs`
**功能**: 顯示單個鑰匙的 UI 圖示（輪廓 + 填充圖片）

### 3. FloatingText.cs
**位置**: `Assets/Scripts/UI/FloatingText.cs`
**功能**: 顯示浮現並淡出的文字

### 4. FloatingTextManager.cs
**位置**: `Assets/Scripts/UI/FloatingTextManager.cs`
**功能**: 管理浮現文字的創建和顯示

### 5. Key.cs
**位置**: `Assets/Scripts/Items/Key.cs`
**功能**: 可收集的鑰匙物件（自動收集）

---

## Unity Editor 設置步驟

### 第一步：設置 Player

1. **選擇 Player GameObject**
2. **添加 KeyInventory 組件**：
   - Add Component → KeyInventory

### 第二步：創建 FloatingText Prefab

1. **創建新的 Canvas**（如果還沒有用於 UI 的 Canvas）：
   - Hierarchy → Create → UI → Canvas
   - Canvas Scaler → UI Scale Mode → Scale With Screen Size

2. **在 Canvas 下創建 TextMeshPro 物件**：
   - 右鍵 Canvas → UI → Text - TextMeshPro
   - 命名為 "FloatingText"

3. **設置 FloatingText**：
   - 添加 FloatingText 組件
   - 設置：
     - Lifetime: 2
     - Float Speed: 1
     - Float Distance: 2
   - TextMeshPro 設置：
     - Font Size: 24
     - Alignment: Center
     - Color: White
     - Outline: 啟用（黑色，寬度 0.2）

4. **將 FloatingText 製作成 Prefab**：
   - 拖曳到 `Assets/Prefabs/UI/` 資料夾
   - 刪除場景中的實例

### 第三步：創建 FloatingTextManager

1. **在 Canvas 下創建空 GameObject**：
   - 命名為 "FloatingTextManager"

2. **添加 FloatingTextManager 組件**：
   - 將 FloatingText prefab 拖曳到 `Floating Text Prefab` 欄位
   - 設置：
     - Default Font Size: 24
     - Default Color: White
     - World Offset: (0, 1, 0)

### 第四步：創建 KeyUI

1. **在 Canvas 下創建 Image**：
   - 命名為 "KeyUI_Key1"
   - 這是鑰匙的容器

2. **設置 KeyUI 容器**：
   - RectTransform:
     - Anchors: Top-Left
     - Position: (100, -100)
     - Size: (64, 64)

3. **在 KeyUI_Key1 下創建兩個 Image 子物件**：
   
   **a. KeyOutline（輪廓）**：
   - Image Component:
     - Source Image: 鑰匙輪廓圖片
     - Color: (1, 1, 1, 0.3) - 半透明白色
   - RectTransform: Stretch to fill parent
   
   **b. KeyFilled（填充）**：
   - Image Component:
     - Source Image: 鑰匙填充圖片
     - Color: (1, 1, 1, 1) - 完全不透明
   - RectTransform: Stretch to fill parent
   - **初始設為 Inactive**

4. **添加 KeyUI 組件到 KeyUI_Key1**：
   - Key Outline Image: KeyOutline
   - Key Filled Image: KeyFilled
   - Key Tag: "key1"
   - Outline Color Empty: (1, 1, 1, 0.3)
   - Outline Color Filled: (1, 1, 1, 1)
   - Fade In Duration: 0.5

5. **如果有多把鑰匙，重複步驟 1-4**：
   - KeyUI_Key2, KeyUI_Key3...
   - 修改 Key Tag 為 "key2", "key3"...

### 第五步：設置 Key 物件

1. **選擇現有的鑰匙 GameObject**（或創建新的）：
   - 確保有 Sprite Renderer
   - 確保有 Collider2D（Is Trigger = true）
   - **移除舊的 tag 設定（不再需要 "key1" tag）**

2. **添加 Key 組件**：
   - Key Tag: "key1"（對應 door 的 requiredKeyTag）
   - Key Name: "紅色鑰匙"（顯示名稱）
   - Collect Sound: 可選，收集音效
   - Collect Effect: 可選，收集特效 prefab

3. **Layer 設置**：
   - 確保 Key 的 Layer 能與 Player 碰撞
   - 檢查 Physics2D Matrix

### 第六步：更新 Door 設置

**不需要修改！** Door 腳本已自動更新為使用 KeyInventory 系統。

只需確保：
- Door 的 `Required Key Tag` 與 Key 的 `Key Tag` 一致

---

## 測試步驟

1. **播放遊戲**
2. **檢查 KeyUI**：
   - 應該看到半透明的鑰匙輪廓
3. **走向鑰匙**：
   - 鑰匙應該自動被收集
   - 應該看到浮現文字："獲得 [鑰匙名稱]"
   - UI 上的鑰匙圖示應該亮起（淡入動畫）
4. **走向門**：
   - 門應該自動開啟
   - 鑰匙從 UI 消失（或保持顯示，視需求而定）

---

## 可選設置

### 讓鑰匙保留不消失

如果你希望鑰匙使用後仍保留在 UI：

修改 `door.cs` 的 `OpenDoor` 方法：
```csharp
// 註解掉這一行：
// keyInventory.RemoveKey(requiredKeyTag);
```

### 自訂浮現文字樣式

在 FloatingTextManager 中：
- 修改 `defaultColor` 改變預設顏色
- 修改 `defaultFontSize` 改變預設字體大小
- 在 `ShowKeyCollected` 方法中自訂鑰匙收集的文字樣式

### 添加音效

1. **鑰匙收集音效**：
   - 在 Key 組件的 `Collect Sound` 欄位設置
   
2. **門開啟音效**：
   - 可以在 door.cs 的 `OpenDoor` 方法中添加：
   ```csharp
   if (doorOpenSound != null)
   {
       AudioSource.PlayClipAtPoint(doorOpenSound, transform.position);
   }
   ```

---

## 移除舊系統

### 可以安全移除的內容：

1. **InteractionHandler 的部分功能**：
   - `PickUpItem()` 方法（如果只用於鑰匙）
   - `UseCarriedItem()` 方法（如果只用於鑰匙）
   - `carriedItem` 欄位（如果只用於鑰匙）

2. **PlayerController 的舊鑰匙邏輯**：
   - 已經移除了自動撿取鑰匙的程式碼

### 保留的內容：

- InteractionHandler 的 `RegisterInteractable` 和 `TryInteract` 功能（用於其他互動物件）
- Door 系統（已更新為使用 KeyInventory）

---

## 常見問題

### Q: 鑰匙沒有被收集？
A: 檢查：
- Player 是否有 KeyInventory 組件
- Key 的 Collider2D 是否設為 Is Trigger
- Player 和 Key 的 Layer 碰撞設定

### Q: UI 沒有顯示鑰匙？
A: 檢查：
- KeyUI 的 Key Tag 是否與 Key 的 Key Tag 一致
- KeyFilled Image 是否正確設置
- KeyInventory 的 OnKeyCollected 事件是否正常觸發

### Q: 浮現文字沒有顯示？
A: 檢查：
- FloatingTextManager 是否存在於場景中
- FloatingText Prefab 是否正確設置
- Canvas 是否設為 Screen Space Overlay

### Q: 門沒有自動開啟？
A: 檢查：
- Door 的 Required Key Tag 是否與收集的 Key Tag 一致
- Player 是否有 KeyInventory 組件
- Door 的 Collider2D 是否設為 Is Trigger

---

## 擴展功能建議

### 1. 鑰匙數量顯示
在 KeyUI 旁邊顯示 "x3" 這樣的數量文字。

### 2. 鑰匙音效增強
添加不同鑰匙的特殊音效。

### 3. 鑰匙動畫
鑰匙收集時的旋轉、縮放動畫。

### 4. 鑰匙描述
點擊 UI 上的鑰匙顯示詳細描述。

### 5. 持久化
將鑰匙狀態保存到 SaveSystem，讓玩家重啟遊戲後仍擁有鑰匙。

