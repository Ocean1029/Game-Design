# TextMeshPro 中文字體設置指南

## 問題說明

TextMeshPro 預設的字體（如 Asap-Variable SDF）不包含中文字符，導致中文顯示為空白。

錯誤訊息：
```
The character with Unicode value \u7372 was not found in the [Asap-Variable SDF] font asset
```

## 解決方案

### 方案 1：使用支援中文的 TextMeshPro 字體（推薦）

#### 步驟 1：準備中文字體檔案

推薦使用以下免費中文字體：
- **Noto Sans TC** (Google)
- **思源黑體** (Adobe)
- **Noto Sans CJK**
- **或任何 .ttf/.otf 中文字體**

下載後放到 `Assets/Fonts/` 資料夾

#### 步驟 2：創建 TextMeshPro Font Asset

1. **打開 Font Asset Creator**：
   ```
   Window → TextMeshPro → Font Asset Creator
   ```

2. **設置參數**：
   ```
   Source Font File: [選擇你的中文字體]
   Sampling Point Size: Auto Sizing
   Padding: 5
   Packing Method: Optimum
   Atlas Resolution: 4096 x 4096  (中文字多，需要大)
   ```

3. **字符集設置（重要！）**：
   
   選擇 `Character Set`: **Unicode Range (Hex)**
   
   在 `Character Sequence (Hex)` 輸入：
   ```
   20-7E,
   4E00-9FFF,
   3000-303F,
   FF00-FFEF
   ```
   
   這包含：
   - `20-7E`: 基本 ASCII
   - `4E00-9FFF`: 中日韓統一表意文字（CJK）
   - `3000-303F`: CJK 符號和標點
   - `FF00-FFEF`: 全形 ASCII

4. **生成字體**：
   - 點擊 **"Generate Font Atlas"**
   - 等待處理（可能需要幾分鐘）

5. **保存字體資產**：
   - 點擊 **"Save"** 或 **"Save As"**
   - 保存到 `Assets/Fonts/`
   - 命名如 `NotoSansTC_SDF`

#### 步驟 3：設置 FloatingText 使用新字體

1. **選擇 FloatingText Prefab**：
   - `Assets/Prefabs/UI/FloatingText.prefab`

2. **修改 TextMeshProUGUI 組件**：
   - Font Asset: 選擇剛創建的中文字體（如 `NotoSansTC_SDF`）

3. **保存 Prefab**

#### 步驟 4：測試

1. Play 遊戲
2. 撿取物品
3. 應該看到正確的中文："獲得 紅色鑰匙"

### 方案 2：使用英文訊息（已實施）

如果暫時不想設置中文字體，系統已改為使用英文：

```
獲得 紅色鑰匙  →  + Red Key
使用了 紅色鑰匙  →  Used Red Key
按 E 使用鑰匙  →  Press E to use Key
這裡無法使用  →  Cannot use here
```

### 方案 3：使用圖示代替文字

另一個選擇是使用圖示+簡短英文：
```
+ 🔑  →  使用 emoji
+ ✓   →  使用符號
```

## 常見問題

### Q: 字體生成失敗？
A: 降低 Atlas Resolution（如 2048 x 2048）或減少字符範圍

### Q: 只需要部分常用字？
A: 使用 **Characters from File** 選項，創建一個文字檔包含你需要的所有字

範例 `chinese_chars.txt`:
```
獲得紅色鑰匙藍
使用按這裡無法
背包物品工具材料
```

### Q: 字體太模糊？
A: 增加 Atlas Resolution 或調整 Sampling Point Size

### Q: 生成時間太長？
A: 減少字符範圍，只包含遊戲中會用到的字

## 推薦設置

### 遊戲常用字最小集合

如果只需要基本中文支持，使用這個最小字符集：

```
Character Set: Custom Characters

在文字框中輸入遊戲中所有會用到的中文字：
獲得紅色鑰匙藍綠黃
使用按這裡無法背包
物品工具材料消耗品
任務道具開門修理
```

這樣生成速度快，字體檔案小！

## 其他 UI 文字

如果背包 UI、提示文字也需要中文：

1. **BackpackPanel 的 Title**：
   - 改用中文字體或改為英文 "Inventory"

2. **UseHintPanel 的文字**：
   - 改用中文字體或改為英文 "Press E to use..."

3. **InventorySlotUI 的 QuantityText**：
   - 數字不受影響，可以繼續使用

## 字體資產大小建議

| 字符範圍 | Atlas Size | 檔案大小 | 適用 |
|---------|-----------|---------|------|
| 遊戲常用字 (~100字) | 512x512 | ~500KB | 小型遊戲 |
| 基本中文 (~3000字) | 2048x2048 | ~5MB | 一般遊戲 |
| 完整CJK (~20000字) | 4096x4096 | ~20MB | 完整支持 |

**建議**：先用遊戲常用字，需要時再擴充！

## 快速測試

創建字體後，測試這些字是否正常顯示：

```
獲得紅色鑰匙
使用藍色工具
背包已滿
```

全部正常顯示即可！

