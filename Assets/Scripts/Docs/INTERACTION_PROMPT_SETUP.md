# 互動提示系統設置指南

## 概述

互動提示系統會在玩家靠近可互動物件（如 spawnpoint、道具等）時，自動在該物件上方顯示閃動的按鍵圖片，提示玩家可以互動。

## 核心組件

### 1. InteractionPrompt

核心組件，負責顯示閃動的按鍵圖片。

#### 使用方法

**方法 A：手動添加（推薦）**

1. 選擇要添加提示的可互動物件（如 SavePoint、CollectableItem 等）
2. 在 Inspector 中點擊 "Add Component"
3. 搜索並添加 `InteractionPrompt` 組件
4. 設置以下參數：
   - **Button Sprite**: 拖入 `Assets/Image/UI/zbutton.png`（或留空讓系統自動載入）
   - **World Offset**: 按鍵圖片相對於物件的位置（預設：0, 1.5, 0）
   - **Button Size**: 按鍵圖片的尺寸（預設：0.8, 0.8）
   - **Pulse Speed**: 閃動速度（預設：2）
   - **Min Alpha / Max Alpha**: 閃動的透明度範圍（預設：0.5 - 1.0）

**方法 B：使用 AutoInteractionPrompt（自動添加）**

1. 選擇可互動物件
2. 添加 `AutoInteractionPrompt` 組件
3. 組件會自動檢查物件是否為可互動物件，並自動添加 `InteractionPrompt`

## 設置步驟

### 為 SavePoint 添加互動提示

1. 選擇場景中的 SavePoint 物件
2. 確保 SavePoint 有 `Collider2D` 組件（設為 Trigger）
3. 添加 `InteractionPrompt` 組件
4. 在 Inspector 中設置按鍵圖片（或留空讓系統自動載入）

### 為 CollectableItem 添加互動提示

1. 選擇場景中的 CollectableItem 物件
2. 確保 CollectableItem 有 `Collider2D` 組件（設為 Trigger）
3. 添加 `InteractionPrompt` 組件
4. 設置提示位置和動畫參數

### 為其他 IInteractable 物件添加互動提示

任何實現了 `IInteractable` 接口的物件都可以添加 `InteractionPrompt`：

1. 確保物件有 `Collider2D` 組件（設為 Trigger）
2. 添加 `InteractionPrompt` 組件
3. 設置參數

## 組件參數說明

### InteractionPrompt 參數

| 參數 | 說明 | 預設值 |
|------|------|--------|
| Button Sprite | 要顯示的按鍵圖片 | 自動載入 zbutton.png |
| World Offset | 按鍵圖片相對於物件的位置 | (0, 1.5, 0) |
| Button Size | 按鍵圖片的尺寸 | (0.8, 0.8) |
| Pulse Speed | 閃動動畫的速度（每秒閃動次數） | 2 |
| Min Alpha | 閃動動畫的最小透明度 | 0.5 |
| Max Alpha | 閃動動畫的最大透明度 | 1.0 |
| Enable Float Animation | 是否啟用上下浮動動畫 | true |
| Float Distance | 上下浮動的距離 | 0.1 |
| Float Speed | 上下浮動的速度 | 2 |
| Detection Radius | 檢測範圍（如果使用距離檢測） | 2 |
| Use Distance Detection | 使用距離檢測而非 Trigger | false |

### AutoInteractionPrompt 參數

| 參數 | 說明 | 預設值 |
|------|------|--------|
| Auto Add On Start | 是否在 Start 時自動添加 | true |
| Override Existing | 是否覆蓋已存在的 InteractionPrompt | false |
| Button Sprite | 按鍵圖片（留空則使用預設） | null |
| World Offset | 提示位置偏移 | (0, 1.5, 0) |
| Button Size | 提示尺寸 | (0.8, 0.8) |
| Detection Radius | 檢測範圍 | 2 |
| Use Distance Detection | 使用距離檢測 | false |

## 檢測模式

### Trigger 模式（預設）

- 使用 `Collider2D` 的 Trigger 功能
- 當玩家進入 Trigger 範圍時顯示提示
- 當玩家離開 Trigger 範圍時隱藏提示
- **需要**：`Collider2D.isTrigger = true`

### 距離檢測模式

- 使用距離計算來檢測玩家
- 每幀檢查玩家距離
- 當玩家距離小於 `Detection Radius` 時顯示提示
- **需要**：`Collider2D.isTrigger = false` 或 `Use Distance Detection = true`

## 動畫效果

### 閃動動畫

按鍵圖片會以設定的速度閃動，透明度在 `Min Alpha` 和 `Max Alpha` 之間變化。

### 上下浮動動畫

如果啟用，按鍵圖片會上下浮動，增加視覺吸引力。

## 程式碼範例

### 手動控制提示顯示/隱藏

```csharp
using UnityEngine;

public class MyInteractable : MonoBehaviour, IInteractable
{
    private InteractionPrompt prompt;
    
    void Start()
    {
        prompt = GetComponent<InteractionPrompt>();
    }
    
    public void OnInteractorEnterZone(IInteractor interactor)
    {
        // 顯示提示
        if (prompt != null)
        {
            prompt.ShowPrompt(true);
        }
    }
    
    public void OnInteractorExitZone(IInteractor interactor)
    {
        // 隱藏提示
        if (prompt != null)
        {
            prompt.ShowPrompt(false);
        }
    }
    
    public bool Interact(IInteractor interactor)
    {
        // 執行互動
        return true;
    }
    
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
```

### 動態設置提示參數

```csharp
InteractionPrompt prompt = GetComponent<InteractionPrompt>();

// 設置按鍵圖片
prompt.SetButtonSprite(mySprite);

// 設置偏移位置
prompt.SetWorldOffset(new Vector3(0, 2, 0));

// 顯示/隱藏提示
prompt.ShowPrompt(true);
```

## 注意事項

1. **Collider2D 要求**：`InteractionPrompt` 需要 `Collider2D` 組件來檢測玩家
2. **按鍵圖片**：如果未在 Inspector 中設置，系統會嘗試自動載入 `Assets/Image/UI/zbutton.png`
3. **排序層**：提示圖片使用 `sortingOrder = 100`，確保顯示在最上層
4. **效能**：距離檢測模式每幀都會計算距離，如果場景中有很多物件，建議使用 Trigger 模式

## 疑難排解

### 提示不顯示

1. 檢查物件是否有 `Collider2D` 組件
2. 檢查 `Collider2D.isTrigger` 是否正確設置
3. 檢查按鍵圖片是否正確載入
4. 檢查玩家是否正確標記為 "Player" Tag 或有 `PlayerController` 組件

### 提示位置不對

1. 調整 `World Offset` 參數
2. 檢查物件的中心點位置

### 動畫效果不明顯

1. 調整 `Pulse Speed` 增加閃動速度
2. 調整 `Min Alpha` 和 `Max Alpha` 增加對比度
3. 啟用 `Enable Float Animation` 增加浮動效果

## 支援的物件類型

- ✅ SavePoint（重生點）
- ✅ CollectableItem（可收集道具）
- ✅ 任何實現 `IInteractable` 的物件
- ✅ PhysicalDoor（實體門）
- ✅ Chair（椅子）
- ✅ 其他可互動物件

## 更新日誌

- **v1.0** (2024): 初始版本
  - 基本互動提示功能
  - 閃動和浮動動畫
  - Trigger 和距離檢測模式
  - 自動資源載入

