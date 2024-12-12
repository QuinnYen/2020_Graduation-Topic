# 中華大學校園網頁資訊整合系統

**[ 此專案建立於2020年並執行完成，之後便無任何維護計畫 ]**  
這是一個專門為中華大學開發的校園網站資訊整合系統，透過網路爬蟲技術自動擷取和分析校園網站內容，提供更有效率的資訊搜尋服務。

## 專案簡介

本專案旨在解決學校網站資訊量龐大、使用者難以快速找到所需資訊的問題。系統使用網路爬蟲技術擷取網站內容，並透過自然語言處理進行關鍵字分析，提供更精準的搜尋結果。

### 主要功能

1. **校園網頁搜尋**
   - 跨網站整合搜尋
   - 關鍵字智能分析
   - 搜尋結果即時預覽

2. **快速單位連結**
   - 常用單位快速導覽
   - 學術單位分類瀏覽
   - 行政單位分類瀏覽

3. **熱門網站推薦**
   - 依據使用頻率排序
   - 即時更新熱門清單
   - 使用者行為分析

## 技術架構

### 後端技術
- Python 3.x
- BeautifulSoup4（網頁解析）
- Jieba（中文分詞）
- SQLite（資料儲存）
- Requests（網頁爬蟲）

### 前端技術
- ASP.NET
- HTML/CSS
- JavaScript
- Bootstrap

## 安裝說明

1. 安裝必要的 Python 套件：
```bash
pip install -r requirements.txt
```

2. 設置環境：
- 確保已安裝 Python 3.x
- 安裝 Visual Studio（ASP.NET 開發環境）
- 設置 SQLite 資料庫

3. 配置檔案：
- 確認 `resource` 資料夾中的所有必要檔案
- 設定資料庫連接字串
- 配置爬蟲參數

## 使用說明

1. 啟動爬蟲程式：
```bash
python cra_with_sqllite.py
```

2. 執行網站應用程式：
- 使用 Visual Studio 開啟專案
- 執行 ASP.NET 應用程式

## 開發團隊
- 顏寬（程式設計）
- 徐世鈞（文件撰寫、美工設計）
- 徐聖凱（資料分析）
- 高詠傑（資料分析）

## 授權資訊
本專案為中華大學資訊工程學系專題，版權所有。

## 更新日誌
- Version 2.0 (2021/08)
  - 改善搜尋演算法
  - 優化使用者介面
  - 新增熱門網站推薦功能

## 參考資源
- [Python 官方文件](https://docs.python.org/3/)
- [ASP.NET 文件](https://docs.microsoft.com/zh-tw/aspnet/core/?view=aspnetcore-5.0)
- [BeautifulSoup 文件](https://www.crummy.com/software/BeautifulSoup/bs4/doc/)

## 注意事項  
- [專案為2020年創立並執行完成，之後便無任何維護計畫]
- 本系統需要穩定的網路連接
- 建議使用現代瀏覽器（Chrome, Firefox, Edge 等）
- 初次執行爬蟲可能需要較長時間
