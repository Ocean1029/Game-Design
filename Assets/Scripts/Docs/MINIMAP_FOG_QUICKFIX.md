# 小地圖迷霧快速修復指南

## 🔴 問題：迷霧是紅色的且完全遮蔽小地圖

### 原因
迷霧系統使用了錯誤的混合模式，需要專門的 Shader 來正確混合迷霧和小地圖內容。

### 解決方案（3 步驟）

---

## 步驟 1：創建迷霧材質

1. 在 Unity 頂部選單點擊：
   ```
   Tools → Minimap Fog Diagnostic
   ```

2. 點擊 **"創建/更新迷霧材質"** 按鈕

3. 查看 Console，應該顯示：
   ```
   ✓ 創建新材質: Assets/Materials/MinimapFogMaterial.mat
   ✓ 材質創建成功！
   ✓ 已將材質應用到 FogOverlay
   ```

✅ **完成！材質已自動創建並應用。**

---

## 步驟 2：修復 FogOverlay 設置

還在診斷工具窗口中：

1. 點擊 **"修復 FogOverlay 設置"** 按鈕

2. 查看 Console 確認：
   ```
   ✓ 設置 UV Rect 為 (0, 0, 1, 1)
   ✓ 設置顏色為白色
   ✓ 設置 RectTransform 為 Stretch 全滿
   ✓ 設置迷霧 Texture (512x512)
   ```

---

## 步驟 3：調整 World Bounds（匹配小地圖比例）

選擇 `MinimapFogSystem` → `MinimapFogOfWar` 組件：

### 如果您的小地圖是 200x100（2:1 比例）

設置 World Bounds 為 **2:1 比例**：
```
World Min: (-100, -50)
World Max: (100, 50)
```

### 如果您的小地圖是正方形（1:1 比例）

設置 World Bounds 為 **1:1 比例**：
```
World Min: (-50, -50)
World Max: (50, 50)
```

### 如何確定您的小地圖比例？

1. 選擇 Hierarchy 中的 `MiniMapDisplay`
2. 查看 Inspector 的 **RectTransform** → **Width** 和 **Height**
3. 計算比例：`Width / Height`
   - 例如：200 / 100 = 2.0（2:1 比例）
   - 例如：150 / 150 = 1.0（1:1 比例）

4. World Bounds 應該使用**相同的比例**

---

## ✅ 測試

1. **運行遊戲**
2. 觀察小地圖：
   - ✅ 未探索區域顯示**黑色迷霧**
   - ✅ 玩家周圍的迷霧會**逐漸揭開**
   - ✅ 已探索區域可以**看到地圖內容**（半透明黑色覆蓋）
   - ✅ 當前視野內**完全透明**

3. 在診斷工具中點擊 **"測試：揭開所有迷霧"**
   - 如果小地圖完全顯示，說明系統正常

---

## 🔧 進階調整

### 調整迷霧濃度

選擇 `MinimapFogSystem` → `MinimapFogOfWar`：

**更暗的迷霧**（未探索區域完全黑）：
```
Fog Color: (0, 0, 0, 1.0)
Explored Alpha: 0.5
```

**更淡的迷霧**（可以稍微看到未探索區域）：
```
Fog Color: (0, 0, 0, 0.7)
Explored Alpha: 0.2
```

**已探索區域幾乎看不出來**：
```
Explored Alpha: 0.05
```

### 調整探索範圍

**更大的視野**：
```
Vision Radius: 25
```

**更小的視野**：
```
Vision Radius: 10
```

---

## ❌ 常見問題

### Q1: 還是顯示紅色/其他顏色

**檢查**：
1. 選擇 FogOverlay
2. Inspector → RawImage → **Material**
3. 應該顯示 `MinimapFogMaterial`
4. 如果是 `None` 或其他材質，重新點擊 "創建/更新迷霧材質"

### Q2: 迷霧範圍和小地圖不匹配

**原因**：World Bounds 比例與小地圖比例不同

**解決**：
1. 確認小地圖的寬高比（例如 200x100 = 2:1）
2. 設置 World Bounds 為相同比例
3. 例如 2:1 比例：`(-100, -50)` 到 `(100, 50)`

### Q3: 探索後的區域還是完全黑

**檢查**：
1. 選擇 MinimapFogSystem
2. 確認 **Explored Alpha** < 1.0（建議 0.2-0.3）
3. 如果是 1.0，已探索區域會和未探索一樣黑

### Q4: 看不到迷霧效果（完全透明）

**檢查**：
1. 確認 **Enable Fog** 已勾選
2. 確認 **Fog Color** 的 Alpha > 0（建議 0.8-1.0）
3. 運行遊戲時檢查 Console 是否有錯誤

### Q5: 迷霧不會隨玩家移動

**檢查**：
1. 確認 **Player** 參考已設置
2. 確認玩家在 **World Bounds** 範圍內
3. 在 Scene 視圖中查看黃色框（Gizmos）確認範圍

---

## 📝 完整檢查清單

使用診斷工具後，確認以下項目：

### MinimapFogSystem 設置
- [ ] Enable Fog: ✓ 勾選
- [ ] Fog Resolution: 512
- [ ] Vision Radius: 15
- [ ] Fog Color: (0, 0, 0, 0.8)
- [ ] Explored Alpha: 0.3
- [ ] World Min/Max: 符合小地圖比例
- [ ] Player: 已設置

### FogOverlay 設置
- [ ] RawImage 存在
- [ ] Texture: 顯示 512x512 的 Texture
- [ ] UV Rect: (0, 0, 1, 1)
- [ ] Color: 白色 (255, 255, 255, 255)
- [ ] Material: MinimapFogMaterial
- [ ] Material Shader: UI/MinimapFog
- [ ] RectTransform: Stretch 全滿

### 運行測試
- [ ] 運行遊戲
- [ ] 小地圖顯示黑色迷霧
- [ ] 玩家移動時迷霧揭開
- [ ] 已探索區域可見
- [ ] 迷霧範圍匹配小地圖

---

## 🎯 一鍵修復（推薦）

如果以上步驟太複雜，使用這個簡化流程：

1. `Tools` → `Minimap Fog Diagnostic`
2. 點擊 **"創建/更新迷霧材質"**
3. 點擊 **"修復 FogOverlay 設置"**
4. 調整 World Bounds 匹配小地圖比例
5. 運行遊戲測試

✅ **完成！**

