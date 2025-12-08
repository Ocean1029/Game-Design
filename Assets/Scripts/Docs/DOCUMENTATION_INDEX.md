# Documentation Index - 文件索引

> 所有專案文件的快速導覽索引

---

## 📚 文件清單

### 核心架構文件

| 文件名稱 | 用途 | 適合對象 |
|---------|------|---------|
| **[GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md)** | 完整的遊戲架構設計文件 | 所有開發者 ⭐ 必讀 |
| **[SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md)** | 視覺化系統架構圖表 | 視覺學習者、新成員 |
| **[QUICK_REFERENCE.md](./QUICK_REFERENCE.md)** | 快速參考指南和常用 API | 日常開發 ⭐ 常用 |
| **[README.md](./README.md)** | 腳本資料夾結構說明 | 新成員、尋找檔案 |

### 專題文件

| 文件名稱 | 用途 | 位置 |
|---------|------|-----|
| **[REFACTORING_GUIDE.md](./Manager/REFACTORING_GUIDE.md)** | 程式碼重構指南 | `Manager/` |
| **[SPAWN_POINT_SYSTEM_README.md](./Manager/SPAWN_POINT_SYSTEM_README.md)** | Spawn Point 系統詳細說明 | `Manager/` |

---

## 🎯 根據需求查找文件

### 我是新成員，應該從哪裡開始？

**推薦閱讀順序**:

1. **[README.md](./README.md)** - 了解專案結構（5 分鐘）
2. **[SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md)** - 看圖理解架構（10 分鐘）
3. **[GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md)** - 深入了解設計（30 分鐘）
4. **[QUICK_REFERENCE.md](./QUICK_REFERENCE.md)** - 書籤起來，日常查詢

總計約 45 分鐘，你就能掌握專案架構！

### 我想要快速查找 API 怎麼用

👉 **[QUICK_REFERENCE.md](./QUICK_REFERENCE.md)**

包含：
- 常用 API 範例
- 系統快速查找表
- 常見任務實作方法
- 程式碼範例

### 我想要了解系統是如何設計的

👉 **[GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md)**

包含：
- 完整系統架構
- 設計模式說明
- 資料流向
- 擴展指南
- 最佳實踐

### 我想要看視覺化的架構圖

👉 **[SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md)**

包含：
- 系統架構圖
- 資料流程圖
- 狀態轉換圖
- 組件關係圖
- 設計模式視覺化

### 我想要了解 Spawn Point 系統

👉 **[SPAWN_POINT_SYSTEM_README.md](./Manager/SPAWN_POINT_SYSTEM_README.md)**

包含：
- 系統概述
- 使用範例
- API 說明
- 測試流程
- 常見問題

### 我想要了解重構的歷史

👉 **[REFACTORING_GUIDE.md](./Manager/REFACTORING_GUIDE.md)**

包含：
- 重構動機
- 架構變更
- 遷移指南

---

## 📖 按主題分類

### 架構與設計

- **系統架構**: [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) § 架構總覽
- **層級結構**: [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) § 系統層級結構
- **設計模式**: [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) § 設計模式
- **視覺化圖表**: [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md)

### 核心系統

#### GameManager
- **完整說明**: [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) § GameManager
- **API 參考**: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) § GameManager API
- **架構圖**: [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md) § GameManager 架構

#### Player System
- **完整說明**: [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) § Player System
- **組件架構**: [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md) § PlayerController 組件架構
- **API 參考**: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) § PlayerController API

#### Interaction System
- **完整說明**: [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) § Interaction System
- **實作指南**: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) § 創建新的互動物件
- **架構圖**: [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md) § 互動系統架構

#### Spawn Point System
- **完整說明**: [SPAWN_POINT_SYSTEM_README.md](./Manager/SPAWN_POINT_SYSTEM_README.md)
- **快速開始**: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) § 創建新的重生點
- **資料流**: [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md) § Spawn Point 註冊流程

### 場景管理

- **場景切換**: [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) § SceneTransitionManager
- **載入流程**: [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md) § 場景載入流程圖
- **API 使用**: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) § 場景切換觸發器

### UI 系統

- **UI 架構**: [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) § UI System
- **快速旅行**: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) § 創建快速旅行 UI
- **UI 流程圖**: [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md) § UI 系統架構

### 開發指南

- **擴展指南**: [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) § 擴展指南
- **常見任務**: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) § 常見任務
- **最佳實踐**: [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) § 最佳實踐
- **效能優化**: [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md) § 效能考量視覺化

---

## 🔍 按使用場景查找

### 場景 1: 我要添加新功能

1. **了解現有架構**
   - 閱讀 [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) 相關章節
   - 查看 [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md) 理解資料流

2. **參考擴展指南**
   - [GAME_ARCHITECTURE.md § 擴展指南](./GAME_ARCHITECTURE.md#擴展指南)
   - [QUICK_REFERENCE.md § 常見任務](./QUICK_REFERENCE.md#常見任務)

3. **實作功能**
   - 參考 API 文件
   - 遵循最佳實踐

### 場景 2: 我遇到了 Bug

1. **理解系統行為**
   - 查看 [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md) 的相關流程圖
   - 了解預期的資料流向

2. **查找相關 API**
   - [QUICK_REFERENCE.md § 常用 API](./QUICK_REFERENCE.md#常用-api)

3. **使用調試技巧**
   - [QUICK_REFERENCE.md § 調試技巧](./QUICK_REFERENCE.md#調試技巧)
   - [GAME_ARCHITECTURE.md § 除錯指南](./GAME_ARCHITECTURE.md#除錯指南)

### 場景 3: 我要重構程式碼

1. **了解重構歷史**
   - [REFACTORING_GUIDE.md](./Manager/REFACTORING_GUIDE.md)

2. **遵循設計原則**
   - [GAME_ARCHITECTURE.md § 設計模式](./GAME_ARCHITECTURE.md#設計模式)
   - [GAME_ARCHITECTURE.md § 最佳實踐](./GAME_ARCHITECTURE.md#最佳實踐)

3. **保持一致性**
   - 參考現有系統的實作方式
   - 更新相關文件

### 場景 4: 我要優化效能

1. **了解效能考量**
   - [GAME_ARCHITECTURE.md § 效能考量](./GAME_ARCHITECTURE.md#效能考量)
   - [SYSTEM_DIAGRAMS.md § 效能考量視覺化](./SYSTEM_DIAGRAMS.md#效能考量視覺化)

2. **應用最佳實踐**
   - Component Caching
   - 避免頻繁查找
   - 使用事件系統

---

## 📝 文件內容概覽

### [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md)

**總字數**: ~15,000 字  
**預計閱讀時間**: 30-45 分鐘

**主要章節**:
1. 架構總覽
2. 系統層級結構
3. 核心模組（Layer 1-5）
4. 資料流向
5. 設計模式
6. 檔案組織
7. 擴展指南
8. 系統依賴圖
9. 最佳實踐
10. 效能考量
11. 除錯指南

**適合**: 全面了解系統設計、架構決策、擴展方式

---

### [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md)

**總圖表數**: 15+ 個視覺化圖表  
**預計閱讀時間**: 15-20 分鐘

**主要內容**:
- 整體系統架構圖
- GameManager 架構圖
- PlayerController 組件圖
- 場景載入流程圖
- 重生流程圖
- 互動系統架構圖
- 狀態機轉換圖
- UI 系統架構圖
- 資料流向圖
- 依賴關係圖
- 事件系統流程圖
- 設計模式視覺化

**適合**: 視覺學習者、快速理解系統關係

---

### [QUICK_REFERENCE.md](./QUICK_REFERENCE.md)

**總範例數**: 30+ 程式碼範例  
**預計查詢時間**: 1-3 分鐘/查詢

**主要內容**:
- 常用 API 速查
- 系統快速查找表
- 常見任務實作步驟
- 程式碼範例
- 調試技巧
- 常見問題解答

**適合**: 日常開發、快速查詢、解決具體問題

---

### [README.md](./README.md)

**預計閱讀時間**: 5 分鐘

**主要內容**:
- 資料夾結構
- 腳本分類
- 依賴關係
- 添加新腳本的指南

**適合**: 新成員、了解檔案組織

---

### [SPAWN_POINT_SYSTEM_README.md](./Manager/SPAWN_POINT_SYSTEM_README.md)

**預計閱讀時間**: 10-15 分鐘

**主要內容**:
- Spawn Point 系統概述
- 核心功能
- 使用範例
- API 使用
- 測試流程
- 常見問題

**適合**: 理解重生系統、使用 Spawn Point API

---

### [REFACTORING_GUIDE.md](./Manager/REFACTORING_GUIDE.md)

**預計閱讀時間**: 10 分鐘

**主要內容**:
- 重構動機
- 架構變更
- 遷移指南
- 舊系統 vs 新系統對比

**適合**: 了解歷史變更、維護舊程式碼

---

## 🎓 學習路徑建議

### 路徑 A: 快速上手（1 小時）

適合：需要快速開始工作的開發者

1. [README.md](./README.md) - 5 分鐘
2. [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md) - 前 5 個圖表 - 10 分鐘
3. [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) - 瀏覽 - 15 分鐘
4. 實際操作 - 根據 Quick Reference 完成一個簡單任務 - 30 分鐘

**成果**: 能夠開始基本的開發工作

### 路徑 B: 深入理解（3 小時）

適合：需要深入了解系統的開發者

1. [README.md](./README.md) - 5 分鐘
2. [SYSTEM_DIAGRAMS.md](./SYSTEM_DIAGRAMS.md) - 完整閱讀 - 20 分鐘
3. [GAME_ARCHITECTURE.md](./GAME_ARCHITECTURE.md) - 詳細閱讀 - 45 分鐘
4. [SPAWN_POINT_SYSTEM_README.md](./Manager/SPAWN_POINT_SYSTEM_README.md) - 15 分鐘
5. [REFACTORING_GUIDE.md](./Manager/REFACTORING_GUIDE.md) - 10 分鐘
6. 實際操作 - 完成複雜任務 - 1 小時

**成果**: 深入理解系統設計，能夠進行架構級別的開發

### 路徑 C: 特定功能學習（30 分鐘）

適合：只需要了解特定系統的開發者

1. 在本索引中找到相關主題
2. 閱讀對應章節
3. 查看相關圖表
4. 參考 Quick Reference 中的範例
5. 實作

**成果**: 能夠開發特定領域的功能

---

## 📊 文件統計

| 文件 | 行數 | 字數 | 圖表數 | 程式碼範例 |
|------|------|------|--------|-----------|
| GAME_ARCHITECTURE.md | ~1,200 | ~15,000 | 5 | 30+ |
| SYSTEM_DIAGRAMS.md | ~800 | ~8,000 | 15+ | 10+ |
| QUICK_REFERENCE.md | ~700 | ~7,000 | 8 | 40+ |
| README.md | ~100 | ~800 | 1 | 5 |
| SPAWN_POINT_SYSTEM_README.md | ~200 | ~2,500 | 3 | 15 |
| REFACTORING_GUIDE.md | ~200 | ~2,000 | 2 | 20 |
| **總計** | **~3,200** | **~35,300** | **34+** | **120+** |

---

## 🔄 文件更新

### 最後更新時間

| 文件 | 最後更新 | 版本 |
|------|---------|------|
| GAME_ARCHITECTURE.md | 2025-10-20 | v1.0 |
| SYSTEM_DIAGRAMS.md | 2025-10-20 | v1.0 |
| QUICK_REFERENCE.md | 2025-10-20 | v1.0 |
| SPAWN_POINT_SYSTEM_README.md | 2025-10-20 | v1.0 |
| README.md | 2025-10-20 | v2.0 |
| REFACTORING_GUIDE.md | 2025-10-12 | v1.0 |

### 更新頻率

- **核心架構文件**: 重大架構變更時更新
- **API 參考**: 新功能添加時更新
- **常見問題**: 根據實際問題持續更新

---

## ❓ 常見問題

### Q: 我應該從哪份文件開始？

**A**: 取決於你的需求：
- **新成員**: 從 README.md 開始
- **需要快速查詢**: 直接看 QUICK_REFERENCE.md
- **深入理解**: 閱讀 GAME_ARCHITECTURE.md
- **視覺學習者**: 從 SYSTEM_DIAGRAMS.md 開始

### Q: 文件太長了，我沒時間全部看完

**A**: 不用全部看完！使用本索引找到你需要的特定章節，按需閱讀。每個文件都有清晰的目錄結構。

### Q: 我發現文件有錯誤或過時內容

**A**: 請更新文件並在版本歷史中記錄變更。文件應該和程式碼一起維護。

### Q: 我要添加新功能，需要更新哪些文件？

**A**: 通常需要更新：
1. 程式碼檔案（當然）
2. QUICK_REFERENCE.md（如果有新的 API）
3. GAME_ARCHITECTURE.md（如果有架構變更）
4. 相關的專題文件（如果涉及特定系統）

---

## 📞 取得協助

如果你在閱讀文件後還有疑問：

1. **檢查常見問題**: 各文件末尾都有 FAQ 章節
2. **查看範例**: QUICK_REFERENCE.md 有大量程式碼範例
3. **參考現有實作**: 查看類似功能的現有程式碼
4. **詢問團隊成員**: 最直接的方式

---

**維護者**: AI Assistant  
**最後更新**: 2025-10-20  
**索引版本**: 1.0

---

## 快速連結

- 🏠 [返回專案根目錄](../)
- 📖 [完整架構文件](./GAME_ARCHITECTURE.md)
- 🗺️ [系統架構圖](./SYSTEM_DIAGRAMS.md)
- ⚡ [快速參考](./QUICK_REFERENCE.md)
- 🪑 [Spawn Point 系統](./Manager/SPAWN_POINT_SYSTEM_README.md)

