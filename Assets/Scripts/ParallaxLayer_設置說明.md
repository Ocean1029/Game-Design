# ParallaxLayer 視差背景設置說明

## 📋 概述
`ParallaxLayer.cs` 是一個用於創建視差滾動效果的 Unity 腳本。它能讓背景根據相機移動以不同速度移動，創造出深度感和立體效果。

---

## 🎮 基本設置步驟

### 1. 準備背景圖片
- 確保背景圖片比相機視野**稍大**
- 建議尺寸：
  - **遠景層（Layer 1）**: 相機尺寸的 **1.1 倍**
  - **中景層（Layer 2）**: 相機尺寸的 **1.5 倍**

#### 計算範例（假設相機視野為 1920x1080）：
- 遠景層圖片尺寸：`1920 × 1.1 = 2304` × `1080 × 1.1 = 1296`
- 中景層圖片尺寸：`1920 × 1.5 = 2880` × `1080 × 1.5 = 1620`

---

### 2. 在場景中設置背景層

#### 步驟 A：建立背景物件
1. 在 Hierarchy 中右鍵 → `Create Empty`
2. 命名為 `Backgrounds`（父物件）
3. 在 `Backgrounds` 下建立子物件：
   - `Background_Far` （遠景層）
   - `Background_Mid` （中景層）

#### 步驟 B：添加精靈渲染器
1. 選擇 `Background_Far`
2. 添加組件：`Sprite Renderer`
3. 設置：
   - **Sprite**: 拖入您的遠景背景圖片
   - **Sorting Layer**: 建議建立 `Background` 層
   - **Order in Layer**: `-2`（確保在最後方）

4. 對 `Background_Mid` 重複步驟，但：
   - **Order in Layer**: `-1`（在遠景前面）

#### 步驟 C：設置 Z 軸深度
- `Background_Far`: Z = `10`
- `Background_Mid`: Z = `5`
- 玩家和前景：Z = `0` 或更小

---

### 3. 添加 ParallaxLayer 腳本

#### 遠景層設置（Background_Far）：
1. 選擇 `Background_Far`
2. 添加組件：`ParallaxLayer`
3. 設置參數：
   ```
   Parallax Multiplier: 0.02 - 0.08
   Enable Clamp: ✓
   Min Offset: (-10, -10)
   Max Offset: (10, 10)
   Target Camera: Main Camera（或留空自動偵測）
   ```

**建議值**：
- 極遠景：`0.02 - 0.04`
- 遠景：`0.05 - 0.08`

#### 中景層設置（Background_Mid）：
1. 選擇 `Background_Mid`
2. 添加組件：`ParallaxLayer`
3. 設置參數：
   ```
   Parallax Multiplier: 0.10 - 0.25
   Enable Clamp: ✓
   Min Offset: (-20, -20)
   Max Offset: (20, 20)
   Target Camera: Main Camera
   ```

**建議值**：
- 中景：`0.10 - 0.18`
- 近景：`0.20 - 0.25`

---

## 🔧 參數詳解

### Parallax Multiplier（視差倍率）
- **數值範圍**: 0.0 - 1.0
- **效果說明**:
  - `0.0`: 背景完全靜止
  - `0.05`: 背景移動為相機移動的 5%（看起來很遠）
  - `0.5`: 背景移動為相機移動的 50%
  - `1.0`: 背景與相機同步移動（無視差效果）

### Enable Clamp（啟用限制）
- **建議**: 永遠啟用 ✓
- **作用**: 防止背景移動過度而露出空白區域

### Min/Max Offset（最小/最大偏移量）
- **單位**: Unity 世界座標單位
- **設置技巧**:
  1. 計算背景圖片與相機視野的差距
  2. 例如：如果背景寬度比相機寬 10 單位，則：
     - `minOffset.x = -5`
     - `maxOffset.x = 5`

#### 計算公式：
```
允許偏移量 = (背景尺寸 - 相機視野尺寸) / 2
```

---

## 📐 實際範例設置

### 範例場景配置

假設您的遊戲：
- 相機視野高度：`10 單位`
- 遊戲向右和向下滾動

#### 遠景層（雲朵、山脈）
```
Parallax Multiplier: 0.03
Enable Clamp: ✓
Min Offset: (-8, -6)
Max Offset: (8, 6)
```
- 背景圖片應為相機視野的 **1.2 倍**
- 移動速度極慢，創造極遠效果

#### 中景層（建築物、樹木）
```
Parallax Multiplier: 0.15
Enable Clamp: ✓
Min Offset: (-15, -12)
Max Offset: (15, 12)
```
- 背景圖片應為相機視野的 **1.5 倍**
- 移動速度較快，創造中等距離效果

#### 近景層（草叢、前景裝飾）（可選）
```
Parallax Multiplier: 0.50
Enable Clamp: ✓
Min Offset: (-25, -20)
Max Offset: (25, 20)
```
- 背景圖片應為相機視野的 **2.0 倍**
- 移動速度接近玩家，創造近距離效果

---

## 🎨 視覺調整技巧

### 1. 測試視差效果
- 執行遊戲，移動相機
- 觀察背景是否平滑移動
- 確認沒有露出空白區域

### 2. 微調 Parallax Multiplier
- 從小數值開始（如 0.05）
- 逐步增加直到達到理想效果
- 不同層次間應有明顯差異（例如：0.03、0.15、0.5）

### 3. 調整 Offset 限制
1. 暫時**取消勾選** `Enable Clamp`
2. 執行遊戲並移動到場景邊緣
3. 觀察背景移動的最大範圍
4. 根據觀察結果設置 `minOffset` 和 `maxOffset`
5. 重新**勾選** `Enable Clamp`

### 4. 使用 Gizmos 視覺化
- 選擇背景物件時，會在 Scene 視圖中顯示綠色矩形
- 綠色矩形代表背景允許移動的範圍
- 確保這個範圍不會讓背景邊緣進入相機視野

---

## ⚠️ 常見問題

### Q1: 背景沒有移動？
**解決方案**:
- 確認相機正在移動
- 檢查 `Parallax Multiplier` 是否大於 0
- 確認腳本已啟用且無錯誤

### Q2: 背景移動太快？
**解決方案**:
- **降低** `Parallax Multiplier` 數值
- 遠景層建議 < 0.1

### Q3: 看到背景邊緣或空白區域？
**解決方案**:
- 確認 `Enable Clamp` 已勾選
- 調整 `minOffset` 和 `maxOffset` 使其更小
- 增加背景圖片尺寸（使其更大於相機視野）

### Q4: 背景完全不動？
**解決方案**:
- 檢查是否設置了過小的 `minOffset/maxOffset`
- 確認相機標籤為 `MainCamera` 或手動指定 `Target Camera`

---

## 🚀 進階技巧

### 多層視差設置建議
創建豐富的深度效果，建議使用 3-5 層背景：

| 層次 | Multiplier | 圖片倍率 | 用途 |
|------|-----------|---------|------|
| 天空層 | 0.01 - 0.02 | 1.1x | 天空、遠山、雲 |
| 遠景層 | 0.05 - 0.08 | 1.2x | 遠處建築、山脈 |
| 中景層 | 0.12 - 0.18 | 1.5x | 樹木、建築物 |
| 近景層 | 0.25 - 0.35 | 2.0x | 草叢、石頭 |
| 前景層 | 0.5 - 0.7 | 3.0x | 前景裝飾 |

### 垂直滾動遊戲調整
如果您的遊戲主要是垂直滾動：
- 可以設置 `minOffset.x = 0` 和 `maxOffset.x = 0`
- 僅允許 Y 軸移動
- 或設置極小的 X 軸範圍以產生輕微搖擺效果

---

## 📝 檢查清單

設置完成後，請確認：
- [ ] 每個背景層都有 `SpriteRenderer` 組件
- [ ] 每個背景層都有 `ParallaxLayer` 組件
- [ ] 背景圖片尺寸大於相機視野
- [ ] `Parallax Multiplier` 數值合理（0-1 之間）
- [ ] `Enable Clamp` 已勾選
- [ ] `minOffset` 和 `maxOffset` 設置正確
- [ ] 各層的 Z 軸位置不同（避免 Z-fighting）
- [ ] 各層的 `Order in Layer` 設置正確
- [ ] 執行遊戲測試無空白區域露出

---

## 🎯 快速開始範例

### 最簡單的雙層視差設置：

1. **遠景層**：
   - Sprite: 您的遠景圖（1.2x 相機尺寸）
   - Parallax Multiplier: `0.05`
   - Z Position: `10`

2. **中景層**：
   - Sprite: 您的中景圖（1.5x 相機尺寸）
   - Parallax Multiplier: `0.15`
   - Z Position: `5`

執行遊戲即可看到視差效果！

---

**祝您製作出美麗的視差背景！** 🎨✨

