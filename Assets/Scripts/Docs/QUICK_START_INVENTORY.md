# 背包系統快速開始指南

## 🎯 使用 Editor Tools 快速設置

新的背包系統提供了三個強大的 Editor Tools，讓你可以在幾分鐘內完成設置！

---

## 🛠️ Editor Tools 介紹

### 1. Inventory Setup Tool（一鍵設置工具）
**路徑**：`Window → Inventory → Setup Tool`

**功能**：
- ✅ 自動添加 InventorySystem 到 Player
- ✅ 自動創建 FloatingText Prefab
- ✅ 自動創建 InventorySlot Prefab
- ✅ 自動創建完整背包 UI
- ✅ 創建測試用鑰匙 ItemData

### 2. Item Creator Tool（物品創建工具）
**路徑**：
- 創建可收集物品：`Hierarchy 右鍵 → Inventory → Create Collectable Item`
- 創建 ItemData：`Hierarchy 右鍵 → Inventory → Create ItemData Asset`

**功能**：
- ✅ 快速創建可收集物品 GameObject
- ✅ 自動設置 Collider 和 CollectableItem 組件
- ✅ 快速創建 ItemData ScriptableObject

### 3. InventorySystem Inspector（增強 Inspector）
**路徑**：選擇 Player → Inspector 查看 InventorySystem

**功能**：
- ✅ 自動查找所有 ItemData 並添加到列表
- ✅ 運行時顯示背包內容
- ✅ 測試用清空背包按鈕

---

## 🚀 3 分鐘快速設置流程

### 步驟 1：打開設置工具（5 秒）
```
Unity 頂部選單 → Window → Inventory → Setup Tool
```

### 步驟 2：使用一鍵設置（1 分鐘）

1. **拖曳 Player 到 "Player" 欄位**
2. **點擊 "自動查找 Player"**（如果找不到就手動拖曳）
3. **點擊 "添加 InventorySystem 到 Player"**
4. **拖曳 Canvas 到 "Canvas" 欄位**（或點擊 "自動查找 Canvas"）
5. **點擊 "創建 FloatingText Prefab"**
6. **點擊 "創建 InventorySlot Prefab"**
7. **點擊 "創建完整背包 UI"**（綠色大按鈕）
8. **完成！**

### 步驟 3：創建測試物品（1 分鐘）

1. **在 Setup Tool 中點擊 "創建測試用紅色鑰匙 ItemData"**
2. **設置鑰匙圖示**：
   - 在 Project 視窗找到剛創建的 Key_Red
   - 拖曳鑰匙圖片到 Icon 欄位
3. **添加到 Player**：
   - 選擇 Player
   - 在 InventorySystem Inspector 中點擊 "自動查找所有 ItemData"

### 步驟 4：創建可收集鑰匙（30 秒）

1. **Hierarchy 右鍵 → Inventory → Create Collectable Item**
2. **設置物品**：
   - 拖曳鑰匙圖片到 Sprite Renderer 的 Sprite 欄位
   - 在 CollectableItem 組件中拖曳 Key_Red 到 Item Data
3. **放置到場景中想要的位置**

### 步驟 5：測試！（30 秒）

1. **按下 Play**
2. **走向鑰匙**
3. **看到 "獲得 紅色鑰匙"**
4. **按 Tab 打開背包**
5. **看到鑰匙圖示亮起**
6. **完成！**

---

## 📋 詳細使用說明

### Inventory Setup Tool 使用指南

#### 視窗布局

```
┌─────────────────────────────────────┐
│ 背包系統快速設置工具                  │
├─────────────────────────────────────┤
│ 步驟 1: 設置 Player                  │
│   Player: [拖曳或自動查找]            │
│   [添加 InventorySystem 到 Player]   │
├─────────────────────────────────────┤
│ 步驟 2: 設置 Canvas                  │
│   Canvas: [拖曳或自動查找]            │
├─────────────────────────────────────┤
│ 步驟 3: 創建 UI Prefabs              │
│   [創建 FloatingText Prefab]         │
│   [創建 InventorySlot Prefab]        │
├─────────────────────────────────────┤
│ 步驟 4: 設置場景 UI                  │
│   [創建完整背包 UI] (大按鈕)          │
├─────────────────────────────────────┤
│ 步驟 5: 創建測試物品                 │
│   [創建測試用紅色鑰匙 ItemData]       │
├─────────────────────────────────────┤
│ 快速操作                             │
│   [自動查找 Player] [自動查找 Canvas] │
└─────────────────────────────────────┘
```

#### 使用順序

1. ✅ 自動查找或手動拖曳 Player 和 Canvas
2. ✅ 點擊 "添加 InventorySystem 到 Player"
3. ✅ 點擊 "創建 FloatingText Prefab"
4. ✅ 點擊 "創建 InventorySlot Prefab"
5. ✅ 點擊 "創建完整背包 UI"
6. ✅ 點擊 "創建測試用紅色鑰匙 ItemData"
7. ✅ 完成！

### Item Creator Tool 使用指南

#### 創建 ItemData

1. **打開工具**：
   - `Hierarchy 右鍵 → Inventory → Create ItemData Asset`
   - 或 `Assets 右鍵 → Create → Inventory → Item Data`

2. **快速範本**：
   - 點擊 "紅色鑰匙" → 自動填寫鑰匙設定
   - 點擊 "藍色鑰匙" → 自動填寫鑰匙設定
   - 點擊 "工具" → 自動填寫工具設定

3. **自訂設定**：
   - 修改任何欄位
   - 點擊 "創建 ItemData"

4. **完成**：
   - ItemData 已保存到 `Assets/ScriptableObjects/Items/`
   - 自動選中並高亮顯示

#### 創建可收集物品

1. **右鍵 Hierarchy**：
   - `Inventory → Create Collectable Item`

2. **自動創建**：
   - ✅ GameObject
   - ✅ SpriteRenderer
   - ✅ CircleCollider2D（Is Trigger）
   - ✅ CollectableItem 組件

3. **設置**：
   - 拖曳圖片到 SpriteRenderer
   - 拖曳 ItemData 到 CollectableItem
   - 放置到場景中

### InventorySystem Inspector 功能

選擇 Player 後，在 Inspector 中：

#### 標準設置
```
Max Inventory Slots: 20
All Game Items: [物品列表]
```

#### 快速操作按鈕

**自動查找所有 ItemData**：
- 掃描整個專案
- 找到所有 ItemData 資產
- 自動添加到 All Game Items 列表

**清除列表**：
- 一鍵清空 All Game Items

#### 運行時資訊（Play Mode）

顯示：
- 當前物品數量
- 每個物品的名稱和數量
- 清空背包按鈕（測試用）

---

## 🎮 完整工作流程範例

### 範例：創建一個需要鑰匙的關卡

#### 1. 設置系統（使用 Setup Tool）
```
1. Window → Inventory → Setup Tool
2. 自動查找 Player 和 Canvas
3. 一鍵創建所有組件
4. 完成！
```

#### 2. 創建紅色鑰匙
```
1. Hierarchy 右鍵 → Inventory → Create ItemData Asset
2. 點擊 "紅色鑰匙" 範本
3. 設置圖示
4. 點擊 "創建 ItemData"
```

#### 3. 創建可收集鑰匙
```
1. Hierarchy 右鍵 → Inventory → Create Collectable Item
2. 設置 Sprite 和 ItemData
3. 放置在場景中
```

#### 4. 設置門
```
1. 選擇門 GameObject
2. 確保有 door 組件
3. Required Key Tag = "door_red"
```

#### 5. 測試
```
1. Play
2. 收集鑰匙
3. 打開背包（Tab）
4. 靠近門自動開啟
```

#### 總時間：**不到 5 分鐘！**

---

## 💡 進階技巧

### 技巧 1：批量創建物品

1. 創建第一個 ItemData（例如 Key_Red）
2. 在 Project 視窗中複製（Ctrl+D）
3. 修改 Item ID 和名稱
4. 選擇 Player → 點擊 "自動查找所有 ItemData"
5. 所有物品自動添加！

### 技巧 2：快速更新圖示

1. 選擇多個 ItemData
2. 在 Inspector 中拖曳圖片到 Icon
3. 批量更新！

### 技巧 3：運行時測試

1. Play Mode 中選擇 Player
2. 在 InventorySystem Inspector 查看背包內容
3. 使用 "清空背包" 測試重複收集

### 技巧 4：複製可收集物品

1. 創建一個 CollectableItem
2. 在 Hierarchy 中複製（Ctrl+D）
3. 只需修改位置，ItemData 自動保留

---

## ⚡ 鍵盤快捷鍵

| 按鍵 | 功能 |
|------|------|
| Tab / I | 打開/關閉背包 |
| E | 使用可用物品 |
| Esc | 關閉背包 |

---

## 🎨 自訂設置

### 修改背包 UI 外觀

選擇 BackpackPanel：
- 修改 Color 改變背景透明度
- 修改 SlotContainer 的 Grid Layout 改變排列

### 修改物品格子樣式

編輯 InventorySlot Prefab：
- 修改 Background 改變框框外觀
- 修改 EmptyOverlay 改變空狀態效果
- 修改 QuantityText 改變數量文字樣式

### 修改浮現文字樣式

編輯 FloatingText Prefab：
- 修改 TextMeshProUGUI 設定
- 修改 FloatingText 的 Lifetime, Float Speed 等

---

## ✅ 設置檢查清單

完成以下所有項目即可開始使用：

```
基礎設置：
□ Player 有 InventorySystem 組件
□ InventorySystem 的 All Game Items 有至少一個物品
□ Canvas 存在於場景中

UI 設置：
□ Canvas 下有 FloatingTextManager
□ Canvas 下有 BackpackUI
□ BackpackUI 的所有引用都已設置
□ FloatingText Prefab 已創建
□ InventorySlot Prefab 已創建

測試物品：
□ 至少創建一個 ItemData
□ 場景中有至少一個 CollectableItem
□ CollectableItem 的 ItemData 已設置

Prefabs 路徑：
□ Assets/Prefabs/UI/FloatingText.prefab
□ Assets/Prefabs/UI/InventorySlot.prefab

ItemData 路徑：
□ Assets/ScriptableObjects/Items/[你的物品].asset
```

---

## 🐛 故障排除

### 問題 1：Setup Tool 找不到選單

**解決方案**：
1. 確保 `InventorySetupTool.cs` 在 `Assets/Scripts/Editor/` 資料夾中
2. 等待 Unity 重新編譯
3. 重啟 Unity

### 問題 2：創建 Prefab 失敗

**解決方案**：
1. 手動創建資料夾：`Assets/Prefabs/UI/`
2. 重新點擊創建按鈕

### 問題 3：ItemData 創建後找不到

**解決方案**：
1. 檢查 `Assets/ScriptableObjects/Items/` 資料夾
2. 使用 Project 視窗的搜尋功能
3. 點擊創建視窗中的成功對話框會自動選中

---

## 📚 相關文檔

- **完整設置指南**：`INVENTORY_SYSTEM_SETUP.md`
- **舊鑰匙系統**：`KEY_SYSTEM_SETUP.md`
- **遊戲架構**：`GAME_ARCHITECTURE.md`

---

## 🎯 推薦工作流程

### 第一次使用（完整設置）

1. ✅ 使用 **Inventory Setup Tool** 一鍵設置所有基礎組件
2. ✅ 使用 **Item Creator Tool** 創建你需要的 ItemData
3. ✅ 使用 **InventorySystem Inspector** 的 "自動查找所有 ItemData"
4. ✅ 使用 **Item Creator Tool** 在場景中創建可收集物品
5. ✅ 測試遊戲

### 日常使用（添加新物品）

1. ✅ `Hierarchy 右鍵 → Inventory → Create ItemData Asset`
2. ✅ 選擇範本或自訂設定
3. ✅ Player → InventorySystem → "自動查找所有 ItemData"
4. ✅ `Hierarchy 右鍵 → Inventory → Create Collectable Item`
5. ✅ 設置並測試

---

## 🎁 範本物品

Editor Tool 提供三種快速範本：

### 紅色鑰匙
```
- 自動使用
- 消耗品
- 用於開門
```

### 藍色鑰匙
```
- 自動使用
- 消耗品
- 用於開門
```

### 鐵鎚
```
- 手動使用
- 永久工具
- 用於修理
```

---

## 🚀 開始使用

**現在就打開 Unity Editor**：

1. 選單 → `Window → Inventory → Setup Tool`
2. 跟著視窗指示操作
3. 3 分鐘內完成設置！

**需要幫助？**
- 查看 `INVENTORY_SYSTEM_SETUP.md` 獲取詳細說明
- 檢查 Inspector 的提示訊息
- 查看 Console 的 Debug 訊息

祝你使用愉快！🎉

