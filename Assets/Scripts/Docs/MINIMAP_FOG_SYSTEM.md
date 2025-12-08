# 小地圖迷霧系統說明文件

## 概述

小地圖迷霧系統（Fog of War）讓未探索的區域在小地圖上被黑色迷霧遮蔽，玩家移動時會自動揭開周圍的迷霧，已探索的區域會保持半透明狀態。

## 系統特性

### 核心功能

1. **動態迷霧**：根據玩家位置動態更新迷霧
2. **探索記憶**：已探索的區域會保持揭開狀態
3. **平滑過渡**：迷霧邊緣有平滑的漸變效果
4. **性能優化**：可調整更新頻率和解析度
5. **觸發區域**：可設置特殊區域自動揭開迷霧

### 視覺效果

- **未探索區域**：完全黑色不透明
- **已探索區域**：半透明（可調整透明度）
- **當前視野**：完全透明

## 系統組成

### 1. MinimapFogOfWar.cs

核心迷霧系統，負責：
- 管理迷霧 Texture2D 數據
- 追蹤玩家位置並揭開迷霧
- 提供 API 供其他系統使用

### 2. MinimapFogRenderer.cs

UI 渲染組件，負責：
- 在小地圖 UI 上顯示迷霧
- 同步迷霧 Texture 更新

### 3. FogRevealTrigger.cs（可選）

觸發器組件，負責：
- 玩家進入區域時自動揭開迷霧
- 用於重要地點或特殊區域

## 設置步驟

### 步驟 1：創建迷霧系統

1. 在 Hierarchy 中創建一個空物件，命名為 `MinimapFogSystem`
2. 添加 `MinimapFogOfWar` 組件
3. 設置參數：

#### Fog Settings（迷霧設置）
- **Fog Resolution**: `512`（越高越精細，建議 256-1024）
- **Vision Radius**: `15`（玩家視野範圍，世界單位）
- **Fog Color**: `黑色 (0, 0, 0, 0.8)`
- **Explored Alpha**: `0.3`（已探索區域的透明度）
- **Smoothness**: `3`（迷霧邊緣平滑度）

#### World Bounds（世界邊界）
- **World Min**: 您地圖的最小座標（例如 `-50, -50`）
- **World Max**: 您地圖的最大座標（例如 `50, 50`）

⚠️ **重要**：World Bounds 必須覆蓋整個遊戲地圖！

#### References（參考）
- **Player**: 拖入 Player GameObject
- **Fog Material**: 留空（稍後創建）

#### Performance（性能）
- **Update Interval**: `0.1`（更新頻率，秒）
- **Enable Fog**: ✓ 勾選

### 步驟 2：創建迷霧材質

1. 在 `Assets/Materials/` 創建新材質，命名為 `MinimapFogMaterial`
2. 設置材質：
   - **Shader**: `Unlit/Transparent`
   - **Rendering Mode**: `Transparent`
   - **Main Texture**: 會由系統自動設置

3. 將此材質拖入 `MinimapFogOfWar` 的 `Fog Material` 欄位

### 步驟 3：在小地圖 UI 上添加迷霧圖層

1. 找到您的小地圖 UI（`MiniMapDisplay`）
2. 在小地圖 Image 下創建子物件，命名為 `FogOverlay`
3. 添加 `RawImage` 組件：
   - **Texture**: 留空（會自動設置）
   - **Color**: 白色 `(255, 255, 255, 255)`
   - **Material**: 拖入剛才創建的 `MinimapFogMaterial`

4. 設置 RectTransform：
   - **Anchors**: Stretch 全滿（左上右下都是 0, 0）
   - **Position**: `(0, 0, 0)`
   - **Scale**: `(1, 1, 1)`

5. 添加 `MinimapFogRenderer` 組件：
   - **Fog System**: 會自動找到
   - **Fog Image**: 會自動取得 RawImage
   - **Fog Material**: 拖入 `MinimapFogMaterial`

### 步驟 4：設置 UI 層級順序

確保 `FogOverlay` 在小地圖的**最上層**：

```
MiniMapDisplay (Image - 地圖背景)
├─ MapContent (RawImage - 地圖內容)
└─ FogOverlay (RawImage - 迷霧層) ← 最上層
```

### 步驟 5：測試

1. 運行遊戲
2. 小地圖應該完全被黑色迷霧覆蓋
3. 移動玩家，周圍的迷霧應該會逐漸揭開
4. 已探索的區域應該保持半透明狀態

## 進階功能

### 添加觸發區域（可選）

如果您想讓玩家進入特定區域時自動揭開大範圍迷霧：

1. 在地圖上創建空物件（例如：`FogReveal_Cave`）
2. 添加 `BoxCollider2D` 或 `CircleCollider2D`
3. 設置為 **Is Trigger**: ✓
4. 添加 `FogRevealTrigger` 組件：
   - **Reveal Center**: 留空（使用此物件位置）
   - **Reveal Radius**: `20`（揭開的半徑）
   - **Trigger Once**: ✓（只觸發一次）
   - **Reveal Delay**: `0`（立即揭開）

5. 玩家進入此區域時，迷霧會自動揭開

### 程式碼 API

#### 揭開指定位置的迷霧

```csharp
MinimapFogOfWar fogSystem = MinimapFogOfWar.GetInstance();
if (fogSystem != null)
{
    // 揭開座標 (10, 20) 周圍半徑 15 的區域
    fogSystem.RevealArea(new Vector2(10f, 20f), 15f);
}
```

#### 重置所有迷霧

```csharp
MinimapFogOfWar fogSystem = MinimapFogOfWar.GetInstance();
if (fogSystem != null)
{
    fogSystem.ResetFog();
}
```

#### 揭開所有迷霧（作弊/測試）

```csharp
MinimapFogOfWar fogSystem = MinimapFogOfWar.GetInstance();
if (fogSystem != null)
{
    fogSystem.RevealAllFog();
}
```

#### 啟用/禁用迷霧系統

```csharp
MinimapFogOfWar fogSystem = MinimapFogOfWar.GetInstance();
if (fogSystem != null)
{
    fogSystem.SetFogEnabled(false); // 禁用迷霧
}
```

### 存檔/讀檔整合

如果您想保存玩家探索的區域，可以在存檔時保存迷霧數據：

```csharp
// 存檔時
MinimapFogOfWar fogSystem = MinimapFogOfWar.GetInstance();
Texture2D fogTexture = fogSystem.GetFogTexture();
byte[] fogData = fogTexture.EncodeToPNG();
// 將 fogData 保存到檔案

// 讀檔時
// 從檔案讀取 fogData
Texture2D loadedTexture = new Texture2D(512, 512);
loadedTexture.LoadImage(fogData);
// 將 loadedTexture 應用到迷霧系統
```

## 性能優化

### 調整解析度

- **低性能設備**: `256x256`
- **中等設備**: `512x512`（推薦）
- **高性能設備**: `1024x1024`

### 調整更新頻率

- **Update Interval**:
  - `0.05` - 非常流暢，較耗性能
  - `0.1` - 流暢，平衡性能（推薦）
  - `0.2` - 稍有延遲，省性能

### 減少揭開半徑

- 較小的 **Vision Radius** 可以減少每次更新需要計算的像素數量

## 參數說明

### MinimapFogOfWar 參數

| 參數 | 說明 | 推薦值 |
|------|------|--------|
| Fog Resolution | 迷霧 Texture 解析度 | 512 |
| Vision Radius | 玩家視野半徑（世界單位） | 15 |
| Fog Color | 迷霧顏色 | 黑色 (0,0,0,0.8) |
| Explored Alpha | 已探索區域透明度 | 0.3 |
| Smoothness | 邊緣平滑度 | 3 |
| World Min | 地圖最小座標 | (-50, -50) |
| World Max | 地圖最大座標 | (50, 50) |
| Update Interval | 更新頻率（秒） | 0.1 |
| Enable Fog | 啟用迷霧 | ✓ |

### FogRevealTrigger 參數

| 參數 | 說明 | 推薦值 |
|------|------|--------|
| Reveal Center | 揭開區域中心 | 留空 |
| Reveal Radius | 揭開半徑 | 20 |
| Trigger Once | 只觸發一次 | ✓ |
| Reveal Delay | 延遲揭開（秒） | 0 |

## 疑難排解

### 迷霧沒有顯示

1. 檢查 `MinimapFogOfWar` 的 `Enable Fog` 是否勾選
2. 確認 `FogOverlay` RawImage 的 `Color` 是白色且 Alpha 是 255
3. 檢查 `Fog Material` 是否正確設置

### 迷霧不會揭開

1. 確認 `Player` Transform 已正確指定
2. 檢查 Player 是否在 `World Bounds` 範圍內
3. 查看 Console 是否有錯誤訊息
4. 啟用 `Show Debug Info` 查看詳細資訊

### 迷霧邊緣很鋸齒

1. 增加 `Fog Resolution`（例如從 256 增加到 512）
2. 增加 `Smoothness` 值（例如從 2 增加到 4）
3. 確認 Texture 的 `Filter Mode` 設為 `Bilinear`

### World Bounds 設置不正確

1. 在場景視圖中啟用 Gizmos
2. 選擇 `MinimapFogSystem` 物件
3. 在 Scene 視圖中會看到黃色方框（World Bounds）
4. 調整 `World Min` 和 `World Max` 直到方框覆蓋整個地圖

### 性能問題

1. 降低 `Fog Resolution`
2. 增加 `Update Interval`
3. 減少 `Vision Radius`
4. 在不需要時禁用迷霧系統

## 視覺效果調整

### 更暗的迷霧

```
Fog Color: (0, 0, 0, 1.0)  // 完全不透明
Explored Alpha: 0.5        // 已探索區域較暗
```

### 更亮的迷霧

```
Fog Color: (0, 0, 0, 0.6)  // 較淡
Explored Alpha: 0.1        // 已探索區域幾乎透明
```

### 彩色迷霧

```
Fog Color: (0.1, 0.1, 0.3, 0.8)  // 深藍色迷霧
```

### 更大的探索範圍

```
Vision Radius: 25          // 增加視野
Reveal Radius: 30          // 觸發器揭開更大範圍
```

## 整合建議

### 與傳送點系統整合

當玩家坐上 Chair（傳送點）時，自動揭開該區域：

```csharp
// 在 chair.cs 的 OnInteractorEnterZone 中
MinimapFogOfWar fogSystem = MinimapFogOfWar.GetInstance();
if (fogSystem != null)
{
    fogSystem.RevealArea(transform.position, 20f);
}
```

### 與收集物品整合

收集重要道具時揭開附近迷霧：

```csharp
// 在 CollectableItem.cs 的 OnCollect 中
MinimapFogOfWar fogSystem = MinimapFogOfWar.GetInstance();
if (fogSystem != null && itemData.isImportant)
{
    fogSystem.RevealArea(transform.position, 15f);
}
```

## 測試清單

- [ ] 迷霧正確顯示在小地圖上
- [ ] 玩家移動時迷霧會揭開
- [ ] 已探索區域保持半透明
- [ ] World Bounds 覆蓋整個地圖
- [ ] 迷霧邊緣平滑無鋸齒
- [ ] FogRevealTrigger 正常工作
- [ ] 性能流暢（無卡頓）
- [ ] Console 無錯誤訊息

## 相關文件

- `MinimapFogOfWar.cs` - 核心迷霧系統
- `MinimapFogRenderer.cs` - UI 渲染組件
- `FogRevealTrigger.cs` - 觸發器組件

