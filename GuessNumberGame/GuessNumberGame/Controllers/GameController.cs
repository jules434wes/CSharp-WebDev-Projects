using GuessNumberGame.Modal;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace GuessNumberGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        // 使用靜態字典來儲存遊戲狀態（實際專案中應該用資料庫）
        private static Dictionary<int, GameState> _games = new Dictionary<int, GameState>();
        private static int _nextGameId = 1;

        // POST: api/Game/newgame
        // 開始新遊戲
        [HttpPost("newgame")]
        public IActionResult NewGame()
        {
            try
            {
                // 產生新的遊戲 ID
                int gameId = _nextGameId++;

                // 產生 4 位隨機數字（可重複）
                Random random = new Random();
                string answer = "";
                for (int i = 0; i < 4; i++)
                {
                    answer += random.Next(0, 10).ToString();
                }

                // 建立新的遊戲狀態
                var gameState = new GameState
                {
                    GameId = gameId,
                    Answer = answer,
                    StartTime = DateTime.Now,
                    IsCompleted = false
                };

                // 儲存遊戲狀態
                _games[gameId] = gameState;

                // 🔧 開發時可以顯示答案，正式上線時要移除
                Console.WriteLine($"新遊戲開始！遊戲 ID: {gameId}, 答案: {answer}");

                return Ok(new NewGameResponse
                {
                    GameId = gameId,
                    Message = "新遊戲開始！請猜測 4 位數字",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new NewGameResponse
                {
                    Success = false,
                    Message = "開始新遊戲失敗：" + ex.Message
                });
            }
        }

        // POST: api/Game/guess
        // 提交猜測
        [HttpPost("guess")]
        public IActionResult Guess([FromBody] GuessRequest request)
        {
            try
            {
                // 驗證輸入
                if (string.IsNullOrEmpty(request.Guess))
                {
                    return BadRequest(new GuessResponse
                    {
                        Success = false,
                        ErrorMessage = "請輸入猜測的數字"
                    });
                }

                // 檢查是否為 4 位數字
                if (!Regex.IsMatch(request.Guess, @"^\d{4}$"))
                //Regex.IsMatch(...)：這是 C# 的正則表達式方法，用來判斷字串是否符合指定的模式
                {
                    return BadRequest(new GuessResponse
                    {
                        Success = false,
                        ErrorMessage = "請輸入正確的 4 位數字"
                    });
                }

                // 檢查遊戲是否存在
                if (!_games.ContainsKey(request.GameId))
                {
                    return BadRequest(new GuessResponse
                    {
                        Success = false,
                        ErrorMessage = "遊戲不存在，請開始新遊戲"
                    });
                }

                var gameState = _games[request.GameId];

                // 檢查遊戲是否已完成
                if (gameState.IsCompleted)
                {
                    return BadRequest(new GuessResponse
                    {
                        Success = false,
                        ErrorMessage = "此遊戲已結束，請開始新遊戲"
                    });
                }

                // 計算 A 和 B
                var result = CalculateAB(gameState.Answer, request.Guess);

                // 記錄此次猜測
                gameState.History.Add(new GuessHistory
                {
                    Guess = request.Guess,
                    A = result.A,
                    B = result.B,
                    Time = DateTime.Now
                });

                // 檢查是否獲勝
                bool isWin = result.A == 4;
                if (isWin)
                {
                    gameState.IsCompleted = true;
                }

                string message = isWin ?
                    $"🎉 恭喜你猜對了！答案就是 {gameState.Answer}，總共嘗試了 {gameState.History.Count} 次" :
                    $"結果：{result.A}A{result.B}B，繼續加油！";

                return Ok(new GuessResponse
                {
                    A = result.A,
                    B = result.B,
                    IsWin = isWin,
                    Message = message,
                    AttemptCount = gameState.History.Count,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GuessResponse
                {
                    Success = false,
                    ErrorMessage = "處理猜測失敗：" + ex.Message
                });
            }
        }

        // GET: api/Game/{gameId}/history
        // 取得遊戲歷史記錄
        [HttpGet("{gameId}/history")]
        public IActionResult GetHistory(int gameId)
        {
            try
            {
                if (!_games.ContainsKey(gameId))
                {
                    return NotFound(new { message = "遊戲不存在" });
                }

                var gameState = _games[gameId];
                return Ok(new
                {
                    gameId = gameId,
                    history = gameState.History,
                    isCompleted = gameState.IsCompleted,
                    attemptCount = gameState.History.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "取得歷史記錄失敗：" + ex.Message });
            }
        }

        // 核心邏輯：計算 A 和 B
        private (int A, int B) CalculateAB(string answer, string guess)
        {
            int A = 0; // 位置對且數字對
            int B = 0; // 位置錯但數字存在

            // 計算 A（位置對且數字對）
            for (int i = 0; i < 4; i++)
            {
                if (answer[i] == guess[i])
                {
                    A++;
                }
            }

            // 計算 B（位置錯但數字存在）
            // 先統計答案中每個數字的出現次數
            var answerCounts = new Dictionary<char, int>();
            var guessCounts = new Dictionary<char, int>();

            for (int i = 0; i < 4; i++)
            {
                // 只統計位置不對的數字
                if (answer[i] != guess[i])
                {
                    // 統計答案中的數字
                    if (answerCounts.ContainsKey(answer[i]))
                        answerCounts[answer[i]]++;
                    else
                        answerCounts[answer[i]] = 1;

                    // 統計猜測中的數字
                    if (guessCounts.ContainsKey(guess[i]))
                        guessCounts[guess[i]]++;
                    else
                        guessCounts[guess[i]] = 1;
                }
            }

            // 計算 B：取每個數字在答案和猜測中出現次數的最小值
            foreach (var kvp in guessCounts)
            {
                char digit = kvp.Key;
                int guessCount = kvp.Value;

                if (answerCounts.ContainsKey(digit))
                {
                    B += Math.Min(guessCount, answerCounts[digit]);
                }
            }

            return (A, B);
        }
    }
}