# 背包物件提示系統設置指南

## 概述

背包物件提示系統會在玩家可以使用特定物品時，在背包UI的對應物件上方顯示閃動的按鍵圖片提示。

## 已整合的功能

### 1. **PhysicalDoor（門與鑰匙）**
- 當玩家身上有鑰匙並碰到門時
- 在背包鑰匙物件上方顯示按鍵提示
- 按下 **Z 鍵**可以使用鑰匙開門

### 2. **Stone & Bomb（石頭與炸彈）**
- 當玩家靠近石頭且身上有炸彈時
- 在背包炸彈物件上方顯示按鍵提示
- 按下 **Z 鍵**可以使用炸彈炸石頭

### 3. **Chair（椅子 / SpawnPoint）**
- 按下 **Z 鍵**坐下
- 在坐著的狀態下，按下 **Z 鍵**起立
- 不再使用 U 鍵和 D 鍵

## 核心組件

### 1. InventorySlotPrompt

在背包 slot 上方顯示閃動按鍵提示的組件。

**位置**: `Assets/Scripts/UI/InventorySlotPrompt.cs`

**主要功能**:
- 顯示/隱藏按鍵圖片
- 閃動動畫效果
- 上下浮動動畫

### 2. InventoryPromptManager

管理所有背包物件提示的中央系統（Singleton）。

**位置**: `Assets/Scripts/UI/InventoryPromptManager.cs`

**主要功能**:
- 註冊/取消註冊 slot 提示
- 根據 itemId 或 tag 顯示/隱藏提示
- 管理所有提示的狀態

## 設置步驟

### 步驟 1：在場景中添加 InventoryPromptManager

1. 在場景中找到 Canvas 或 UI 管理物件
2. 添加 `InventoryPromptManager` 組件
3. 確保場景只有一個 InventoryPromptManager（Singleton）

### 步驟 2：為背包 UI 的 Slot 添加 InventorySlotPrompt

需要修改背包 UI 系統，為每個 slot 添加 `InventorySlotPrompt` 組件。

#### 方法 A：修改 SimpleBackpackUI（推薦）

在 `SimpleBackpackUI.cs` 的 `CreateSlot` 方法中：

```csharp
private void CreateSlot(ItemData itemData, int index)
{
    // Instantiate slot
    GameObject slotObj = Instantiate(slotPrefab, backpackContainer.transform);
    slotObj.name = $"Slot_{itemData.itemId}";
    
    // Get slot UI component
    SimpleSlotUI slotUI = slotObj.GetComponent<SimpleSlotUI>();
    if (slotUI == null)
    {
        Debug.LogError($"SimpleBackpackUI: SimpleSlotUI component not found on slot prefab!");
        return;
    }
    
    // Setup slot
    slotUI.Setup(itemData, inventorySystem);
    
    // === 新增：添加 InventorySlotPrompt ===
    InventorySlotPrompt prompt = slotObj.AddComponent<InventorySlotPrompt>();
    
    // === 新增：註冊到 InventoryPromptManager ===
    InventoryPromptManager promptManager = InventoryPromptManager.GetInstance();
    if (promptManager != null)
    {
        promptManager.RegisterSlotPrompt(itemData.itemId, prompt);
    }
    // ====================================
    
    // Position slot using simple grid layout
    PositionSlot(slotObj, index);
    
    // Add to map
    slotMap[itemData.itemId] = slotUI;
}
```

#### 方法 B：修改 BackpackUI

在 `BackpackUI.cs` 的 `CreateInventorySlots` 方法中：

```csharp
foreach (ItemData itemData in allItems)
{
    GameObject slotObj = Instantiate(slotPrefab, slotContainer);
    InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
    
    if (slotUI != null)
    {
        slotUI.Setup(itemData, inventorySystem);
        slotUIMap[itemData.itemId] = slotUI;
        
        // === 新增：添加 InventorySlotPrompt ===
        InventorySlotPrompt prompt = slotObj.AddComponent<InventorySlotPrompt>();
        
        // === 新增：註冊到 InventoryPromptManager ===
        InventoryPromptManager promptManager = InventoryPromptManager.GetInstance();
        if (promptManager != null)
        {
            promptManager.RegisterSlotPrompt(itemData.itemId, prompt);
        }
        // ====================================
    }
}
```

### 步驟 3：確認按鍵圖片資源

確保 `Assets/Image/UI/zbutton.png` 存在。

如果圖片在其他位置，需要：
1. 移動到 `Assets/Resources/UI/zbutton.png`，或
2. 在 `InventorySlotPrompt` 的 Inspector 中手動指定按鍵圖片

## 使用 API

### 顯示提示

```csharp
InventoryPromptManager promptManager = InventoryPromptManager.GetInstance();

// 根據 itemId 顯示提示
promptManager.ShowPromptForItem("key1");

// 根據 tag 顯示提示（自動查找對應的 itemId）
InventorySystem inventory = player.GetComponent<InventorySystem>();
promptManager.ShowPromptForTag("key1", inventory);
```

### 隱藏提示

```csharp
// 根據 itemId 隱藏提示
promptManager.HidePromptForItem("key1");

// 根據 tag 隱藏提示
promptManager.HidePromptForTag("key1", inventory);

// 隱藏所有提示
promptManager.HideAllPrompts();
```

### 檢查提示狀態

```csharp
bool isVisible = promptManager.IsPromptVisibleForItem("key1");
```

## 整合範例

### 範例 1：在自訂互動物件中使用

```csharp
using UnityEngine;

public class CustomInteractable : MonoBehaviour
{
    [SerializeField] private string requiredItemTag = "special_key";
    private InventoryPromptManager promptManager;
    private PlayerController nearbyPlayer;
    
    void Start()
    {
        promptManager = InventoryPromptManager.GetInstance();
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            nearbyPlayer = player;
            
            // 檢查玩家是否有所需物品
            InventorySystem inventory = player.GetComponent<InventorySystem>();
            if (inventory != null && inventory.GetItemForTag(requiredItemTag) != null)
            {
                // 顯示背包物件提示
                promptManager.ShowPromptForTag(requiredItemTag, inventory);
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null && player == nearbyPlayer)
        {
            // 隱藏提示
            InventorySystem inventory = player.GetComponent<InventorySystem>();
            if (inventory != null)
            {
                promptManager.HidePromptForTag(requiredItemTag, inventory);
            }
            nearbyPlayer = null;
        }
    }
    
    void Update()
    {
        // 玩家按下 Z 鍵時執行互動
        if (nearbyPlayer != null && Input.GetKeyDown(KeyCode.Z))
        {
            InventorySystem inventory = nearbyPlayer.GetComponent<InventorySystem>();
            ItemData item = inventory.GetItemForTag(requiredItemTag);
            if (item != null)
            {
                // 使用物品並執行動作
                inventory.RemoveItem(item, 1);
                promptManager.HidePromptForTag(requiredItemTag, inventory);
                
                // 執行自訂動作...
                Debug.Log("物品已使用！");
            }
        }
    }
}
```

## 參數設定

### InventorySlotPrompt 參數

| 參數 | 說明 | 預設值 |
|------|------|--------|
| Button Sprite | 按鍵圖片 | 自動載入 zbutton.png |
| Offset | 提示位置偏移（相對於 slot） | (0, 60) |
| Button Size | 按鍵圖片尺寸 | (40, 40) |
| Pulse Speed | 閃動速度 | 2 |
| Min Alpha | 最小透明度 | 0.5 |
| Max Alpha | 最大透明度 | 1.0 |
| Enable Float Animation | 啟用浮動動畫 | true |
| Float Distance | 浮動距離 | 5 |
| Float Speed | 浮動速度 | 2 |

## 注意事項

1. **單例模式**：確保場景中只有一個 InventoryPromptManager
2. **註冊順序**：slot 必須在使用前註冊到 InventoryPromptManager
3. **itemId vs tag**：
   - `itemId`: ItemData 的唯一 ID（例如：`key1`、`bomb_for_rock`）
   - `tag`: ItemData 的 tag 欄位（可能多個物品共用同一 tag）
4. **Canvas 層級**：提示圖片會顯示在 slot 的子物件中，確保 Canvas 設置正確

## 疑難排解

### 提示不顯示

1. 檢查 InventoryPromptManager 是否存在於場景中
2. 檢查 slot 是否正確註冊（查看 Console Debug 訊息）
3. 檢查 itemId 是否正確（區分大小寫）
4. 檢查按鍵圖片資源是否正確載入

### 提示位置不對

1. 調整 `InventorySlotPrompt` 的 `Offset` 參數
2. 檢查 Canvas 的 Render Mode 設置
3. 檢查 slot 的 RectTransform 設置

### 提示無法隱藏

1. 確保呼叫了正確的 Hide 方法
2. 檢查是否有多個地方同時控制提示
3. 查看 Console 中的 Debug 訊息

## 更新日誌

- **v1.0** (2024): 初始版本
  - 背包物件提示系統
  - 整合 PhysicalDoor、Stone/Bomb、Chair
  - 統一改用 Z 鍵互動

