# Lab02_EmployeeAPI - 員工管理系統

> **學習目標：** ASP.NET Core Web API + Entity Framework Core 完整 CRUD 實作練習

## 📚 練習重點

- **Web API 開發**：建立 RESTful API 端點
- **Entity Framework Core**：Code First 開發模式
- **前後端分離**：HTML/JS 呼叫 API 
- **資料庫操作**：完整 CRUD 與軟刪除
- **前端整合**：Bootstrap + JavaScript Fetch API

## 🛠️ 技術棧

**後端**
- ASP.NET Core 8.0 Web API
- Entity Framework Core 9.0
- SQL Server LocalDB
- Swagger/OpenAPI

**前端**
- HTML5 + JavaScript (ES6+)
- Bootstrap 5.3
- Fetch API

## 🎯 實作功能

### API 端點
| HTTP | 端點 | 功能 |
|------|------|------|
| GET | `/api/employees` | 取得所有員工 |
| GET | `/api/employees/{id}` | 取得單一員工 |
| POST | `/api/employees` | 新增員工 |
| PUT | `/api/employees/{id}` | 更新員工 |
| DELETE | `/api/employees/{id}` | 軟刪除員工 |
| GET | `/api/employees/search?keyword={keyword}` | 搜尋員工 |
| GET | `/api/employees/departments` | 取得部門清單 |

### 前端功能
- ✅ 員工清單展示
- ✅ 新增/編輯員工表單
- ✅ 刪除確認對話框
- ✅ 即時搜尋功能
- ✅ 載入狀態與錯誤處理
- ✅ 響應式 RWD 設計

## 🗂️ 資料模型

```csharp
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }           // 必填
    public string Email { get; set; }          // 必填，唯一
    public string Department { get; set; }     // 必填
    public string? Position { get; set; }      // 選填
    public decimal? Salary { get; set; }       // 選填
    public DateTime HireDate { get; set; }     // 必填
    public bool IsActive { get; set; }         // 軟刪除標記
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

## 🚀 執行方式


使用套件：

```bash
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
```

```bash
# 還原套件
dotnet restore

# 更新資料庫
dotnet ef database update

# 執行專案
dotnet run
```

執行後瀏覽：
- **前端介面**：終端顯示的 URL (通常是 https://localhost:7xxx)
- **API 文件**：上述 URL + `/swagger`

## 📝 API 使用範例

**新增員工**
```json
POST /api/employees
{
    "name": "張小明",
    "email": "ming@company.com", 
    "department": "資訊部",
    "position": "軟體工程師",
    "salary": 60000,
    "hireDate": "2024-01-15"
}
```

**搜尋員工**
```
GET /api/employees/search?keyword=張
```

## 🎓 學習筆記

### 練習到的技術點
1. **ASP.NET Core Web API** - 建立控制器與路由
2. **Entity Framework Code First** - 模型定義與資料庫遷移
3. **資料驗證** - Data Annotations 與模型驗證
4. **軟刪除模式** - IsActive 欄位實作
5. **前後端分離** - API 與前端分離開發
6. **JavaScript 非同步** - async/await 與 Fetch API
7. **錯誤處理** - 前後端錯誤訊息處理
8. **響應式設計** - Bootstrap RWD 實作

### 重要概念理解

- **Code First vs Database First** - 程式碼驅動資料庫設計
- **Migration 機制** - 資料庫版本控制和同步
- **RESTful 設計** - HTTP 動詞的正確使用
- **軟刪除** - 業務邏輯刪除 vs 物理刪除
- **CORS 設定** - 跨域請求處理
- **Profile 設定** - 不同啟動模式管理

### 資料庫指令
```bash
# 新增 Migration
dotnet ef migrations add InitialCreate

# 更新資料庫
dotnet ef database update
```

### 開發工作流程

1. **定義模型** → **設定 DbContext** → **建立 Migration** → **更新資料庫**
2. **實作 API** → **測試 Swagger** → **開發前端** → **整合測試**

---

## 常見問題與解決方案

### Migration 動態值警告

**錯誤：** `The model for context changes each time it is built`

**原因：** 種子資料中使用 `DateTime.Now` 動態值

**解決：** 使用固定時間值

```csharp
// ❌ 錯誤寫法
CreatedAt = DateTime.Now

// ✅ 正確寫法  
CreatedAt = new DateTime(2023, 1, 15, 9, 0, 0)
```

###  Identity 欄位插入錯誤

**錯誤：** `Cannot insert explicit value for identity column`

**原因：** POST 請求包含了 Id 值，但 Id 是自動遞增欄位

**解決：** 前端不傳送 Id 欄位

### 預設開啟 Swagger 而非前端頁面

**問題：** 執行專案時自動開啟 `/swagger` 而不是主頁面

**解決：** 修改 `Properties/launchSettings.json`

```json
"launchUrl": "",  // 改為空字串，而不是 "swagger"
```

並在 `Program.cs` 設定預設頁面：

```csharp
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});
```

### EF Core 主鍵慣例

**問題：** 自訂 ID 名稱（如 UId, MembersId）無法自動設為主鍵

**說明：** EF Core 主鍵慣例：

- ✅ `Id` - 自動成為主鍵
- ✅ `{ClassName}Id` - 自動成為主鍵
- ❌ `UId`, `MembersId` - 需手動設定

**解決：** 使用 `[Key]` 或 Fluent API

```csharp
[Key]
[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
public int UId { get; set; }	
```
