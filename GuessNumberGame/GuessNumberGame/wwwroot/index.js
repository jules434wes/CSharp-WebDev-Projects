// 全域變數
let currentGameId = null;
let gameHistory = [];

// 頁面載入完成
document.addEventListener('DOMContentLoaded', function () {
    setupEventListeners();
});

// 設定事件監聽器
function setupEventListeners() {
    // 新遊戲按鈕
    document.getElementById('newGameBtn').addEventListener('click', startNewGame);

    // 提交猜測按鈕
    document.getElementById('submitGuess').addEventListener('click', submitGuess);

    // 輸入框 Enter 鍵
    document.getElementById('guessInput').addEventListener('keypress', function (e) {
        if (e.key === 'Enter' && !document.getElementById('submitGuess').disabled) {
            submitGuess();
        }
    });

    // 限制只能輸入數字
    document.getElementById('guessInput').addEventListener('input', function (e) {
        e.target.value = e.target.value.replace(/[^0-9]/g, '');
        updateSubmitButton();
    });
}

// 開始新遊戲
async function startNewGame() {
    try {
        showLoading('正在開始新遊戲...');

        const response = await fetch('/api/game/newgame', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        if (result.success) {
            currentGameId = result.gameId;
            gameHistory = [];

            // 更新 UI
            document.getElementById('gameIdDisplay').textContent = currentGameId;
            document.getElementById('attemptCount').textContent = '0';
            document.getElementById('guessInput').disabled = false;
            document.getElementById('guessInput').value = '';
            document.getElementById('guessInput').focus();
            document.getElementById('historyCard').style.display = 'none';

            updateSubmitButton();
            showResult(result.message, 'success');
        } else {
            showResult(result.message, 'error');
        }

    } catch (error) {
        console.error('開始新遊戲失敗:', error);
        showResult('開始新遊戲失敗: ' + error.message, 'error');
    }
}

// 提交猜測
async function submitGuess() {
    const guess = document.getElementById('guessInput').value;

    if (guess.length !== 4) {
        showResult('請輸入完整的 4 位數字！', 'warning');
        return;
    }

    try {
        showLoading('正在處理你的猜測...');

        const response = await fetch('/api/game/guess', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                guess: guess,
                gameId: currentGameId
            })
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        if (result.success) {
            // 添加到歷史記錄
            gameHistory.push({
                guess: guess,
                a: result.a,
                b: result.b,
                time: new Date()
            });

            // 更新嘗試次數
            document.getElementById('attemptCount').textContent = result.attemptCount;

            // 顯示結果
            if (result.isWin) {
                showWinMessage(result.message);
                disableGame();
            } else {
                showResult(`${guess} → ${result.a}A${result.b}B`, 'info');
            }

            // 更新歷史記錄顯示
            updateHistoryDisplay();

            // 清空輸入框
            document.getElementById('guessInput').value = '';
            document.getElementById('guessInput').focus();

        } else {
            showResult(result.errorMessage, 'error');
        }

    } catch (error) {
        console.error('提交猜測失敗:', error);
        showResult('提交猜測失敗: ' + error.message, 'error');
    }
}

// 更新提交按鈕狀態
function updateSubmitButton() {
    const guess = document.getElementById('guessInput').value;
    const submitBtn = document.getElementById('submitGuess');
    const isValid = guess.length === 4 && currentGameId !== null;

    submitBtn.disabled = !isValid;
}

// 顯示載入狀態
function showLoading(message) {
    document.getElementById('resultArea').innerHTML = `
                <div class="result-display bg-light">
                    <i class="fas fa-spinner fa-spin"></i> ${message}
                </div>
            `;
}

// 顯示結果
function showResult(message, type) {
    const alertClass = {
        'success': 'alert-success',
        'error': 'alert-danger',
        'warning': 'alert-warning',
        'info': 'alert-info'
    }[type] || 'alert-info';

    const icon = {
        'success': 'check-circle',
        'error': 'exclamation-circle',
        'warning': 'exclamation-triangle',
        'info': 'info-circle'
    }[type] || 'info-circle';

    document.getElementById('resultArea').innerHTML = `
                <div class="alert ${alertClass}">
                    <i class="fas fa-${icon}"></i> ${message}
                </div>
            `;
}

// 顯示獲勝訊息
function showWinMessage(message) {
    document.getElementById('resultArea').innerHTML = `
                <div class="result-display win-message">
                    <i class="fas fa-trophy"></i> ${message}
                    <br><small>點擊「開始新遊戲」來挑戰下一局！</small>
                </div>
            `;
}

// 禁用遊戲輸入
function disableGame() {
    document.getElementById('guessInput').disabled = true;
    document.getElementById('submitGuess').disabled = true;
}

// 更新歷史記錄顯示
function updateHistoryDisplay() {
    if (gameHistory.length === 0) return;

    document.getElementById('historyCard').style.display = 'block';

    const historyHtml = gameHistory.map((item, index) => `
                <div class="row history-item mb-2 p-2 ${index % 2 === 0 ? 'bg-light' : ''}">
                    <div class="col-2 text-center">${index + 1}</div>
                    <div class="col-4 text-center">${item.guess}</div>
                    <div class="col-4 text-center">
                        <span class="badge bg-primary">${item.a}A${item.b}B</span>
                    </div>
                    <div class="col-2 text-center text-muted">
                        ${item.time.toLocaleTimeString()}
                    </div>
                </div>
            `).join('');

    document.getElementById('historyList').innerHTML = `
                <div class="row mb-2 fw-bold">
                    <div class="col-2 text-center">次數</div>
                    <div class="col-4 text-center">猜測</div>
                    <div class="col-4 text-center">結果</div>
                    <div class="col-2 text-center">時間</div>
                </div>
                <hr>
                ${historyHtml}
            `;
}
