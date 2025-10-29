# 鑰匙系統完整測試指南

## 測試目標

確保鑰匙系統的完整流程正常運作：
1. ✅ 收集鑰匙時 UI 亮起
2. ✅ 門檢測到玩家有鑰匙
3. ✅ 碰撞門時消耗鑰匙並開門
4. ✅ UI 上的鑰匙圖示變暗

## 測試步驟

### 第一步：檢查設置

1. **檢查玩家設置**：
   - 玩家有 `KeyInventory` 組件
   - 玩家有 `PlayerController` 組件

2. **檢查鑰匙設置**：
   - 鑰匙物件有 `Key` 組件
   - `Key Tag` 設置為 "key1"（或其他標籤）
   - 鑰匙有 `Collider2D` 且 `Is Trigger = true`

3. **檢查門設置**：
   - 門物件有 `Physical Door` 組件
   - `Required Key Tag` 設置為 "key1"（與鑰匙標籤一致）
   - 門有 `Collider2D` 且 `Is Trigger = false`

4. **檢查 UI 設置**：
   - KeyUI 物件有 `KeyUI` 組件
   - `Key Tag` 設置為 "key1"（與鑰匙標籤一致）
   - 設置了 `Key Outline Image` 和 `Key Filled Image`

### 第二步：運行測試

1. **開始遊戲**
2. **收集鑰匙**：
   - 走到鑰匙旁邊
   - 應該看到 Console 訊息：`KeyInventory: Collected key 'key1' - [鑰匙名稱]`
   - 應該看到 UI 上的鑰匙圖示亮起（淡入動畫）

3. **測試門檢測**：
   - 走到門旁邊（不要碰撞）
   - 應該沒有訊息（因為還沒有碰撞）

4. **測試開門**：
   - 直接撞向門
   - 應該看到 Console 訊息：
     - `門被開啟！消耗鑰匙: key1`
     - `從鑰匙欄消耗鑰匙: key1`
     - `KeyInventory: Removed key 'key1'`
   - 門應該變成透明（50% 透明度）
   - UI 上的鑰匙圖示應該變暗（淡出動畫）
   - 玩家應該可以穿過門

### 第三步：驗證結果

**成功的標誌**：
- ✅ 收集鑰匙時 UI 亮起
- ✅ 門檢測到玩家有鑰匙
- ✅ 碰撞門時消耗鑰匙並開門
- ✅ UI 上的鑰匙圖示變暗
- ✅ 玩家可以穿過門

## 故障排除

### 問題 1：鑰匙收集後 UI 沒有亮起

**可能原因**：
- KeyUI 的 `Key Tag` 與鑰匙的 `Key Tag` 不一致
- 玩家沒有 `KeyInventory` 組件
- KeyUI 沒有正確設置 `Key Outline Image` 和 `Key Filled Image`

**解決方法**：
1. 檢查所有標籤是否一致
2. 確保玩家有 `KeyInventory` 組件
3. 檢查 KeyUI 的設置

### 問題 2：門檢測不到鑰匙

**可能原因**：
- 門的 `Required Key Tag` 與鑰匙的 `Key Tag` 不一致
- 玩家沒有 `KeyInventory` 組件

**解決方法**：
1. 檢查標籤是否一致
2. 確保玩家有 `KeyInventory` 組件

### 問題 3：鑰匙消耗後 UI 沒有變暗

**可能原因**：
- KeyUI 沒有監聽 `OnKeyConsumed` 事件
- 鑰匙沒有被正確消耗

**解決方法**：
1. 檢查 KeyUI 是否正確訂閱了事件
2. 檢查 Console 是否有消耗鑰匙的訊息

### 問題 4：門沒有變成透明

**可能原因**：
- 門沒有 `SpriteRenderer` 組件
- 動畫時間設置為 0

**解決方法**：
1. 確保門有 `SpriteRenderer` 組件
2. 檢查 `Animation Duration` 是否大於 0

## 調試技巧

### 1. 使用 Console 訊息

所有重要事件都會在 Console 中顯示：
- 鑰匙收集：`KeyInventory: Collected key 'key1' - [名稱]`
- 鑰匙消耗：`KeyInventory: Removed key 'key1'`
- 門開啟：`門被開啟！消耗鑰匙: key1`

### 2. 檢查組件設置

確保所有必要組件都已正確設置：
- 玩家：`PlayerController` + `KeyInventory`
- 鑰匙：`Key` + `Collider2D` (Is Trigger = true)
- 門：`Physical Door` + `Collider2D` (Is Trigger = false)
- UI：`KeyUI` + 正確的 Image 設置

### 3. 測試標籤一致性

確保所有標籤都一致：
- 鑰匙的 `Key Tag` = "key1"
- 門的 `Required Key Tag` = "key1"
- KeyUI 的 `Key Tag` = "key1"

## 進階測試

### 測試多把鑰匙

1. 創建多把鑰匙（key1, key2, key3）
2. 創建多個 KeyUI（每個對應一把鑰匙）
3. 創建多扇門（每扇門需要不同的鑰匙）
4. 測試每把鑰匙的收集和消耗

### 測試存檔系統

1. 收集鑰匙
2. 保存遊戲
3. 重新載入
4. 檢查鑰匙是否還在
5. 檢查門是否還記得已開啟

### 測試錯誤情況

1. 沒有鑰匙時撞門
2. 有錯誤鑰匙時撞門
3. 鑰匙被消耗後再次撞門

## 預期結果

**完整的成功流程**：
1. 玩家走到鑰匙旁 → 鑰匙被收集 → UI 亮起
2. 玩家撞向門 → 門檢測到鑰匙 → 消耗鑰匙 → 門變透明 → UI 變暗
3. 玩家可以穿過門

如果所有步驟都正常，說明鑰匙系統已經完全正常運作！
