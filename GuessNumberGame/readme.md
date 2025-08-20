# 1A2B 猜數字遊戲

## 🎯 專案概述

這是一個經典的 1A2B 猜數字遊戲，透過實作這個專案學習了完整的前後端開發流程。

**遊戲規則：**
- 系統隨機產生 4 位數字（不可重複）
- 玩家猜測 4 位數字
- A：位置對且數字對的數量
- B：位置錯但數字存在於答案中的數量
- 例如：答案 `1234`，猜測 `1325` → 結果 `1A2B`

---

## 🛠️ 技術棧與學習重點

### 後端技術
- **ASP.NET Core 8.0 Web API** - RESTful API 設計
- **C# 程式語言** - 物件導向程式設計
- **JSON 資料格式** - 前後端資料交換
- **正則表達式 (Regex)** - 輸入驗證
- **記憶體儲存** - 遊戲狀態管理（Dictionary）

### 前端技術
- **HTML5 + CSS3** - 網頁結構與樣式
- **JavaScript ES6+** - 非同步程式設計 (async/await)
- **Bootstrap 5** - 響應式 UI 框架
- **Fetch API** - Ajax 非同步通訊

### 開發工具
- **Visual Studio 2022** - IDE 開發環境
- **Git** - 版本控制

---

## 📚 核心學習成果

### 1. 前後端分離架構理解
```
前端 (HTML/JS) ←→ HTTP API ←→ 後端 (.NET Core)
     客戶端              通訊協定        伺服器端
```

**學到的概念：**
- 前端負責使用者介面和使用者體驗
- 後端負責業務邏輯和資料處理
- 透過 HTTP API 進行資料交換

### 2. RESTful API 設計實務

| HTTP 方法 | 端點 | 功能說明 |
|-----------|------|----------|
| `POST` | `/api/game/newgame` | 開始新遊戲 |
| `POST` | `/api/game/guess` | 提交猜測 |
| `GET` | `/api/game/{id}/history` | 查看遊戲歷史 |

**學習重點：**
- HTTP 動詞的正確使用
- JSON 請求/回應格式設計
- 錯誤處理與狀態碼

### 3. C# 核心概念應用

#### 物件導向設計
```csharp
// 遊戲狀態封裝
public class GameState
{
    public int GameId { get; set; }
    public string Answer { get; set; }
    public List<GuessHistory> History { get; set; }
    public bool IsCompleted { get; set; }
}
```

#### 演算法實作 - 1A2B 計算邏輯
```csharp
private (int A, int B) CalculateAB(string answer, string guess)
{
    // Step 1: 計算 A（位置對且數字對）
    int A = 0;
    for (int i = 0; i < 4; i++)
    {
        if (answer[i] == guess[i]) A++;
    }
    
    // Step 2: 計算 B（位置錯但數字存在）
    // 使用 Dictionary 統計數字出現次數
    // ...
}
```

- 集合類別使用 (Dictionary, List)
- 字串處理技巧
- 演算法思維與實作

### 4. 輸入驗證與錯誤處理

#### 後端驗證 - 正則表達式
```csharp
// 驗證必須是 4 位數字
if (!Regex.IsMatch(request.Guess, @"^\d{4}$"))
{
    return BadRequest("請輸入正確的 4 位數字");
}
```

#### 前端驗證 - JavaScript
```javascript
// 限制只能輸入數字
input.addEventListener('input', function(e) {
    e.target.value = e.target.value.replace(/[^0-9]/g, '');
});
```

### 5. 非同步程式設計

#### 前端 Ajax 呼叫
```javascript
async function submitGuess() {
    try {
        const response = await fetch('/api/game/guess', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestData)
        });
        
        const result = await response.json();
        // 處理回應...
    } catch (error) {
        // 錯誤處理...
    }
}
```

- Promise 與 async/await 語法
- 錯誤處理最佳實務
- JSON 序列化/反序列化

## 隨機數字產生與 1A2B 計算邏輯

**不重複數字產生演算法：** 從 0-9 這十個數字中隨機選擇四個不重複的數字。使用 List 儲存可用數字，每次隨機選擇一個後就從清單中移除，確保不會重複選到相同數字。這種方法保證產生的答案永遠不會有重複數字。

**1A2B 計算邏輯：** 計算過程分為兩個階段，且這兩個階段是互斥的（同一位置的數字不能同時算入 A 和 B）：

1. **計算 A（位置對且數字對）：** 逐一比較答案和猜測的每個位置，如果位置相同且數字也相同，A 就加一。
2. **計算 B（位置錯但數字存在）：** 只考慮那些位置不對的數字。由於數字不可重複，每個數字在答案和猜測中最多只會出現一次，所以計算相對簡單：檢查猜測中每個位置錯誤的數字是否存在於答案的其他位置，如果存在就貢獻 1 個 B。

**為什麼選擇 Dictionary 作為統計工具：** 雖然在「數字不可重複」的版本中，每個數字最多只出現一次，但使用 Dictionary 仍有其優勢：提供 O(1) 的查詢效率來檢查數字是否存在，比使用 List 進行線性搜尋更有效率。同時，這個設計也為未來可能的功能擴展（如支援可重複數字模式）保留了彈性。