# 計算器 Web API 應用程式

這是一個簡單的全端 Web 應用程式，展示了前端 HTML/JavaScript 與後端 ASP.NET Core Web API 的互動。

## 架構概述

```
前端 (HTML/JS) ←→ HTTP API ←→ 後端 (.NET Core)
    index.html              CalculatorController
```

## 前端部分 (index.html)

### 1. 使用者介面
- **Bootstrap 5** 提供響應式設計
- 兩個數字輸入框
- 四個運算按鈕（加減乘除）
- Loading 狀態顯示
- 結果顯示區域

### 2. JavaScript 核心功能

#### 資料收集與驗證
```javascript
const firstNumber = parseFloat(document.getElementById('firstNumber').value);
const secondNumber = parseFloat(document.getElementById('secondNumber').value);

// 驗證輸入
if (isNaN(firstNumber) || isNaN(secondNumber)) {
    showError('請輸入有效的數字！');
    return;
}
```

#### HTTP API 呼叫
```javascript
const response = await fetch(`/api/calculator/${operation}`, {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({
        firstNumber: firstNumber,
        secondNumber: secondNumber
    })
});
```

#### 非同步處理
- 使用 `async/await` 處理 HTTP 請求
- `try/catch` 錯誤處理
- Loading 狀態管理

## 後端部分 (ASP.NET Core)

### 1. 程式進入點 (Program.cs)

#### 服務註冊
```csharp
builder.Services.AddControllers();        // 控制器支援
builder.Services.AddEndpointsApiExplorer(); // API 探索
builder.Services.AddSwaggerGen();         // API 文件
```

#### 中介軟體配置 (重要順序)
```csharp
app.UseDefaultFiles();    // 1. 先設定預設檔案 (index.html)
app.UseStaticFiles();     // 2. 後設定靜態檔案支援
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();     // 路由對應
```

### 2. API 控制器 (CalculatorController.cs)

#### 路由設計
- **基礎路由**: `/api/calculator`
- **端點**:
  - `POST /api/calculator/add` - 加法
  - `POST /api/calculator/subtract` - 減法
  - `POST /api/calculator/multiply` - 乘法
  - `POST /api/calculator/divide` - 除法

#### 資料模型
```csharp
// 請求模型
public class CalculationRequest
{
    public decimal FirstNumber { get; set; }
    public decimal SecondNumber { get; set; }
}

// 回應模型
public class CalculationResponse
{
    public decimal Result { get; set; }
    public string Operation { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}
```

#### 錯誤處理
```csharp
try
{
    // 計算邏輯
    var result = request.FirstNumber + request.SecondNumber;
    return Ok(new CalculationResponse { ... });
}
catch (Exception ex)
{
    return BadRequest(new CalculationResponse { 
        Success = false,
        ErrorMessage = "計算發生錯誤：" + ex.Message 
    });
}
```

## 完整的請求流程

### 1. 使用者操作
1. 使用者在前端輸入兩個數字
2. 點擊運算按鈕（例如：加法）

### 2. 前端處理
1. JavaScript 收集輸入資料
2. 驗證數字格式
3. 顯示 Loading 狀態
4. 發送 HTTP POST 請求到 `/api/calculator/add`

### 3. HTTP 請求
```http
POST https://localhost:7068/api/calculator/add
Content-Type: application/json

{
    "firstNumber": 10,
    "secondNumber": 5
}
```

### 4. 後端處理
1. ASP.NET Core 接收請求
2. 路由到 `CalculatorController.Add()` 方法
3. 模型綁定：JSON → `CalculationRequest` 物件
4. 執行計算邏輯
5. 回傳 JSON 回應

### 5. HTTP 回應
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
    "result": 15,
    "operation": "加法",
    "success": true,
    "errorMessage": ""
}
```

### 6. 前端顯示結果
1. 接收並解析 JSON 回應
2. 隱藏 Loading 狀態
3. 顯示計算結果或錯誤訊息

## 關鍵技術特點

### 前端
- **響應式設計**: Bootstrap 5
- **非同步程式設計**: async/await, fetch API
- **DOM 操作**: 動態顯示/隱藏元素
- **事件處理**: 按鈕點擊, Enter 鍵

### 後端
- **RESTful API**: HTTP POST 方法
- **模型綁定**: `[FromBody]` 自動解析 JSON
- **錯誤處理**: try/catch + HTTP 狀態碼
- **中介軟體順序**: 靜態檔案服務的正確配置

### 通訊
- **JSON**: 資料序列化/反序列化
- **HTTP**: 標準 Web 協定
- **CORS**: 同源請求（前後端同一網域）

## 部署注意事項

1. **靜態檔案**: 確保 `UseDefaultFiles()` 在 `UseStaticFiles()` 之前
2. **端口設定**: launchSettings.json 中的端口配置
3. **HTTPS**: 開發環境支援 SSL
4. **錯誤處理**: 前後端都有適當的錯誤處理機制