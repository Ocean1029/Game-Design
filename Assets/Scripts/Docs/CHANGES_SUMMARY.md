# 遊戲系統更新摘要

## 更新日期
2024

## 主要更新內容

### 1. 背包物件提示系統

新增了在背包 UI 物件上方顯示閃動按鍵提示的功能。

**新增檔案**:
- `Assets/Scripts/UI/InventorySlotPrompt.cs` - 單個 slot 的提示組件
- `Assets/Scripts/UI/InventoryPromptManager.cs` - 提示管理器（Singleton）
- `Assets/Scripts/Docs/INVENTORY_PROMPT_SETUP.md` - 設置說明文件

**功能**:
- 在背包物件上方顯示閃動的按鍵圖片
- 支援透明度閃動動畫
- 支援上下浮動動畫
- 統一管理所有提示的顯示/隱藏

### 2. PhysicalDoor（門與鑰匙系統）修改

**修改檔案**: `Assets/Scripts/Interaction/PhysicalDoor.cs`

**變更內容**:
- ❌ 舊：玩家碰到門時，如果有鑰匙會自動消耗並開門
- ✅ 新：玩家碰到門時，如果有鑰匙會在背包鑰匙物件上方顯示按鍵提示
- ✅ 玩家需要按下 **Z 鍵**才會使用鑰匙開門

**新增功能**:
- `ShowKeyPrompt()` - 顯示鑰匙提示
- `HideKeyPrompt()` - 隱藏鑰匙提示
- 按 Z 鍵開門的 Update 邏輯

### 3. 石頭與炸彈系統修改

**修改檔案**: `Assets/Scripts/Player/PlayerUseBomb.cs`

**變更內容**:
- ✅ 保持原有的 Z 鍵使用炸彈功能
- ✅ 新增：靠近石頭時在背包炸彈物件上方顯示按鍵提示
- ✅ 離開石頭時自動隱藏提示

**新增功能**:
- `HasBomb()` - 檢查玩家是否有炸彈
- `ShowBombPrompt()` - 顯示炸彈提示
- `HideBombPrompt()` - 隱藏炸彈提示

### 4. 椅子（SpawnPoint）系統修改

**修改檔案**:
- `Assets/Scripts/Interaction/chair.cs`
- `Assets/Scripts/Player/PlayerController.cs`

**變更內容**:
- ❌ 舊：按 **U 鍵**坐下，按 **D 鍵**起立
- ✅ 新：按 **Z 鍵**坐下/起立（toggle）

**UI 變更**:
- 移除 `pressAPrompt` (Press U)
- 移除 `pressDPrompt` (Press D)  
- 新增 `pressZPrompt` (Press Z)

**方法變更**:
- 移除 `ShowPromptA()` 和 `ShowPromptD()`
- 新增 `ShowPromptZ()`
- 修改 `Interact()` 邏輯支援 toggle 行為

### 5. PlayerController 修改

**修改檔案**: `Assets/Scripts/Player/PlayerController.cs`

**變更內容**:
- ✅ 將 `interactKey` 從 `KeyCode.U` 改為 `KeyCode.Z`
- ❌ 移除 `exitInteractionKey` (原本的 KeyCode.D)
- ✅ 移除 exitInteractionKey 相關的輸入處理邏輯

**按鍵配置**:
```
左移: ← (Left Arrow)
右移: → (Right Arrow)
跳躍: Space
互動: Z (坐下/起立/使用鑰匙/使用炸彈等)
重生: R
快速旅行: M
```

## 統一互動按鍵

所有互動功能現在都使用 **Z 鍵**：

1. **椅子互動**：按 Z 坐下，再按 Z 起立
2. **使用鑰匙開門**：靠近門時按 Z
3. **使用炸彈炸石頭**：靠近石頭時按 Z
4. **其他 IInteractable 物件**：按 Z 互動

## 設置需求

### 必須完成的設置

1. **添加 InventoryPromptManager 到場景**
   - 在 Canvas 或 UI Manager 上添加 `InventoryPromptManager` 組件

2. **修改背包 UI 系統**
   - 在創建 slot 時添加 `InventorySlotPrompt` 組件
   - 將 slot 註冊到 `InventoryPromptManager`
   - 參考 `INVENTORY_PROMPT_SETUP.md` 的詳細說明

3. **更新椅子 UI**
   - 將場景中所有椅子的 `pressAPrompt` 和 `pressDPrompt` 改為 `pressZPrompt`
   - 更新 UI 文字為 "Press Z"

4. **確認按鍵圖片資源**
   - 確保 `Assets/Image/UI/zbutton.png` 存在
   - 或將圖片放到 `Assets/Resources/UI/zbutton.png`

## 向後兼容性

### 可能的破壞性變更

1. **椅子 Prefab**
   - 需要更新所有椅子 prefab，將 UI prompt 參考從兩個改為一個
   - 舊的 `pressAPrompt` 和 `pressDPrompt` 欄位已移除

2. **輸入系統**
   - U 鍵和 D 鍵不再用於椅子互動
   - 如果有其他系統依賴這些按鍵，需要重新配置

3. **自動開門邏輯**
   - PhysicalDoor 不再自動開門
   - 需要玩家主動按 Z 鍵

## 測試清單

- [ ] 測試門與鑰匙：靠近門時背包鑰匙上方顯示提示
- [ ] 測試門與鑰匙：按 Z 鍵成功開門並消耗鑰匙
- [ ] 測試石頭與炸彈：靠近石頭時背包炸彈上方顯示提示
- [ ] 測試石頭與炸彈：按 Z 鍵成功使用炸彈
- [ ] 測試椅子：按 Z 鍵坐下
- [ ] 測試椅子：在坐著狀態按 Z 鍵起立
- [ ] 測試椅子：提示 UI 正確顯示
- [ ] 測試提示動畫：閃動和浮動效果正常
- [ ] 測試提示隱藏：離開範圍時提示消失

## 相關文件

- `INVENTORY_PROMPT_SETUP.md` - 背包提示系統詳細設置指南
- `INTERACTION_PROMPT_SETUP.md` - 世界物件互動提示系統
- `SPAWN_POINT_SYSTEM_README.md` - SpawnPoint/椅子系統說明
- `KEY_SYSTEM_SETUP.md` - 鑰匙與門系統說明

