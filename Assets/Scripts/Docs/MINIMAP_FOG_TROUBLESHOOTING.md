# 小地圖迷霧系統疑難排解指南

## 常見問題與解決方案

### 問題 1：白色正方形遮住小地圖

**症狀**：小地圖被白色方塊遮住（通常是左半邊或其他部分），沒有迷霧效果。

**原因**：
1. RawImage 的 UV Rect 設置不正確
2. RectTransform 沒有完全覆蓋小地圖
3. 迷霧 Texture 沒有正確應用

**解決方案**：

#### 自動修復（推薦）

1. 在 Unity 頂部選單點擊 `Tools` → `Minimap Fog Diagnostic`
2. 點擊 **"修復 FogOverlay 設置"**
3. 查看 Console 的修復報告
4. 運行遊戲測試

#### 手動修復

1. 選擇 Hierarchy 中的 `FogOverlay`
2. 檢查 **RawImage** 組件：
   - **Texture**: 應該顯示迷霧 Texture
   - **UV Rect**: 必須是 `X:0, Y:0, W:1, H:1`
   - **Color**: 必須是白色 `(255, 255, 255, 255)`
   
3. 檢查 **RectTransform**：
   - **Anchors**: 
     - Min: `(0, 0)`
     - Max: `(1, 1)`
   - **Left**: `0`
   - **Right**: `0`
   - **Top**: `0`
   - **Bottom**: `0`

4. 確保 **MinimapFogRenderer** 組件存在並啟用

---

### 問題 2：迷霧完全沒有顯示

**症狀**：小地圖完全正常顯示，沒有任何迷霧遮蔽。

**原因**：
1. MinimapFogOfWar 系統沒有創建或啟用
2. FogOverlay 沒有正確設置
3. 迷霧 Texture 沒有初始化

**解決方案**：

1. 使用診斷工具：`Tools` → `Minimap Fog Diagnostic` → **"檢查迷霧系統設置"**
2. 查看 Console 輸出，確認：
   - ✓ 找到 MinimapFogOfWar 組件
   - ✓ 迷霧 Texture 已創建
   - ✓ 找到 MinimapFogRenderer 組件

3. 如果缺少組件，參考 `MINIMAP_FOG_SYSTEM.md` 重新設置

---

### 問題 3：迷霧不會隨玩家移動揭開

**症狀**：迷霧顯示正常，但玩家移動時迷霧不會揭開。

**原因**：
1. Player 參考沒有正確設置
2. World Bounds 設置錯誤
3. 玩家在 World Bounds 範圍外

**解決方案**：

1. 選擇 `MinimapFogSystem`
2. 檢查 **MinimapFogOfWar** 組件：
   - **Player**: 必須拖入 Player GameObject
   - **World Min**: 地圖左下角座標（例如 `-50, -50`）
   - **World Max**: 地圖右上角座標（例如 `50, 50`）
   - **Enable Fog**: 必須勾選

3. 在 Scene 視圖中查看黃色 Gizmos 方框（World Bounds）：
   - 選擇 MinimapFogSystem
   - 確保黃色方框覆蓋整個遊戲地圖
   - 確保玩家在方框內

4. 檢查 Player Tag：
   - 選擇 Player GameObject
   - 確認 Tag 設為 `Player`

---

### 問題 4：迷霧顯示但是是正方形的（不符合小地圖形狀）

**症狀**：迷霧 Texture 是正方形的，但小地圖是長方形。

**原因**：Fog Resolution 生成的是正方形 Texture，需要調整 UV 或使用不同的解析度。

**解決方案**：

1. 調整 FogOverlay 的 **RawImage** UV Rect：
   - 如果小地圖是 2:1 長方形，設置 `UV Rect: (0, 0, 1, 0.5)` 或 `(0, 0, 0.5, 1)`
   
2. 或者，調整 World Bounds 使其符合小地圖比例：
   - 如果小地圖是 200x100（2:1），World Bounds 也應該是 2:1 比例
   - 例如：World Min `(-100, -50)`, World Max `(100, 50)`

---

### 問題 5：迷霧太暗或太亮

**症狀**：迷霧顏色不理想。

**解決方案**：

調整 `MinimapFogSystem` → `MinimapFogOfWar` 組件：

**更暗的迷霧**：
```
Fog Color: (0, 0, 0, 1.0)  // Alpha = 1.0
Explored Alpha: 0.5        // 較高的值
```

**更亮/更淡的迷霧**：
```
Fog Color: (0, 0, 0, 0.6)  // Alpha = 0.6
Explored Alpha: 0.1        // 較低的值
```

**彩色迷霧**：
```
Fog Color: (0.1, 0.1, 0.3, 0.8)  // 深藍色
```

---

### 問題 6：迷霧邊緣很鋸齒

**症狀**：迷霧揭開的邊緣不平滑。

**解決方案**：

1. 增加 **Fog Resolution**：
   - 從 `256` 增加到 `512` 或 `1024`

2. 增加 **Smoothness**：
   - 從 `2` 增加到 `4` 或 `5`

3. 確認 Texture Filter Mode：
   - 迷霧 Texture 應該使用 `Bilinear` 過濾

---

### 問題 7：性能問題（卡頓）

**症狀**：啟用迷霧後遊戲變卡。

**解決方案**：

1. 降低 **Fog Resolution**：
   - 從 `1024` 降到 `512` 或 `256`

2. 增加 **Update Interval**：
   - 從 `0.1` 增加到 `0.2` 或更高

3. 減少 **Vision Radius**：
   - 從 `20` 減少到 `10` 或 `15`

---

## 使用診斷工具

Unity 頂部選單：`Tools` → `Minimap Fog Diagnostic`

### 功能說明

1. **檢查迷霧系統設置**
   - 診斷所有組件是否正確設置
   - 在 Console 輸出詳細報告
   - 建議：設置完成後先運行此檢查

2. **修復 FogOverlay 設置**
   - 自動修復 RawImage 的常見問題
   - 設置正確的 UV Rect、顏色、RectTransform
   - 建議：遇到顯示問題時使用

3. **測試：揭開所有迷霧**
   - 立即揭開所有迷霧（遊戲運行時）
   - 用於測試迷霧系統是否正常工作

4. **測試：重置所有迷霧**
   - 重置所有迷霧到初始狀態（遊戲運行時）
   - 用於重新測試探索效果

---

## 調試檢查清單

依序檢查以下項目：

### 1. 基礎設置
- [ ] `MinimapFogSystem` GameObject 存在
- [ ] `MinimapFogOfWar` 組件已添加並啟用
- [ ] `FogOverlay` GameObject 存在（在小地圖下）
- [ ] `MinimapFogRenderer` 組件已添加

### 2. MinimapFogOfWar 設置
- [ ] Player 參考已設置
- [ ] World Bounds 覆蓋整個地圖
- [ ] Enable Fog 已勾選
- [ ] Fog Resolution 設為 512
- [ ] Vision Radius 設為 15

### 3. FogOverlay 設置
- [ ] RawImage 組件存在
- [ ] UV Rect 是 (0, 0, 1, 1)
- [ ] Color 是白色
- [ ] RectTransform 是 Stretch 全滿
- [ ] MinimapFogRenderer 組件已添加

### 4. 運行時檢查
- [ ] Console 顯示 "MinimapFogOfWar: Initialized"
- [ ] Console 顯示 "MinimapFogRenderer: Initialized"
- [ ] 沒有錯誤訊息
- [ ] 小地圖顯示黑色迷霧
- [ ] 玩家移動時迷霧會揭開

---

## Console 輸出解讀

### 正常輸出（無問題）

```
MinimapFogOfWar: Auto-found player
MinimapFogOfWar: Initialized with resolution 512x512
World bounds: (-50, -50) to (50, 50), size: (100, 100)
Pixels per unit: 5.12
MinimapFogOfWar: Fog texture created and initialized
MinimapFogRenderer: Initialized with fog texture 512x512
MinimapFogRenderer: RawImage size: (200, 100)
MinimapFogRenderer: UV Rect: (0.0, 0.0, 1.0, 1.0)
```

### 錯誤輸出

**❌ "MinimapFogOfWar: No player found!"**
- 解決：設置 Player 參考或給 Player 添加 "Player" Tag

**❌ "MinimapFogRenderer: MinimapFogOfWar system not found!"**
- 解決：確保場景中有 MinimapFogSystem 並運行了 Start()

**❌ "MinimapFogRenderer: Fog texture is null!"**
- 解決：等待一幀或確保 MinimapFogOfWar 先初始化

**❌ "MinimapFogRenderer: RawImage component not found!"**
- 解決：在 FogOverlay 上添加 RawImage 組件

---

## 快速修復流程

遇到問題時，按此順序操作：

1. **運行診斷工具**
   ```
   Tools → Minimap Fog Diagnostic → 檢查迷霧系統設置
   ```
   查看 Console 輸出

2. **嘗試自動修復**
   ```
   Tools → Minimap Fog Diagnostic → 修復 FogOverlay 設置
   ```

3. **測試迷霧揭開**
   - 運行遊戲
   - 點擊 "測試：揭開所有迷霧"
   - 如果小地圖完全顯示，說明迷霧系統正常運作

4. **重置並測試探索**
   - 點擊 "測試：重置所有迷霧"
   - 移動玩家，觀察迷霧是否揭開

5. **如果還有問題**
   - 查看本文檔的常見問題
   - 檢查 Console 的錯誤訊息
   - 參考 `MINIMAP_FOG_SYSTEM.md` 重新設置

---

## 仍然無法解決？

如果以上方法都無效：

1. 啟用調試模式：
   - MinimapFogOfWar: `Show Debug Info` ✓
   - MinimapFogRenderer: `Show Debug Info` ✓

2. 運行遊戲，查看詳細的 Console 輸出

3. 截圖以下內容：
   - FogOverlay 的 Inspector
   - MinimapFogSystem 的 Inspector
   - Console 的輸出
   - Game 視圖中的小地圖

4. 檢查是否有其他 UI 元素遮擋 FogOverlay

