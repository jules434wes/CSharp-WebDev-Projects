namespace GuessNumberGame.Modal
{
    // 遊戲請求模型 - 用來接收玩家的猜測
    public class GuessRequest
    {
        public string Guess { get; set; } = string.Empty;
        public int GameId { get; set; }
    }

    // 遊戲回應模型 - 用來回傳遊戲結果
    public class GuessResponse
    {
        public int A { get; set; }          // 位置對數字對的數量
        public int B { get; set; }          // 位置錯數字對的數量
        public bool IsWin { get; set; }     // 是否獲勝
        public string Message { get; set; } = string.Empty;
        public int AttemptCount { get; set; } // 嘗試次數
        public bool Success { get; set; }   // 操作是否成功
        public string ErrorMessage { get; set; } = string.Empty;
    }

    // 新遊戲回應模型
    public class NewGameResponse
    {
        public int GameId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    // 遊戲歷史記錄
    public class GuessHistory
    {
        public string Guess { get; set; } = string.Empty;
        public int A { get; set; }
        public int B { get; set; }
        public DateTime Time { get; set; }
    }

    // 遊戲狀態 - 儲存進行中的遊戲資訊
    public class GameState
    {
        public int GameId { get; set; }
        public string Answer { get; set; } = string.Empty;  // 正確答案
        public List<GuessHistory> History { get; set; } = new List<GuessHistory>();
        public DateTime StartTime { get; set; }
        public bool IsCompleted { get; set; }
    }
}
