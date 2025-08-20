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

    // 🎯 修改：限制只能輸入數字，且即時檢查重複
    document.getElementById('guessInput').addEventListener('input', function (e) {
        // 只保留數字
        e.target.value = e.target.value.replace(/[^0-9]/g, '');

        // 🎯 新增：檢查並移除重複數字
        e.target.value = removeDuplicateDigits(e.target.value);

        updateSubmitButton();
        validateInput(); // 🎯 新增：即時驗證
    });
}

// 🎯 新增：移除重複數字的函數
function removeDuplicateDigits(input) {
    let result = '';
    let seenDigits = new Set();

    for (let char of input) {
        if (!seenDigits.has(char)) {
            seenDigits.add(char);
            result += char;
        }
    }

    return result;
}

// 🎯 新增：即時輸入驗證
function validateInput() {
    const input = document.getElementById('guessInput');
    const guess = input.value;

    // 清除之前的提示
    clearValidationMessage();

    if (guess.length > 0) {
        if (hasDuplicateDigits(guess)) {
            showValidationMessage('數字不可重複！', 'warning');
            input.classList.add('is-invalid');
        } else {
            input.classList.remove('is-invalid');
            if (guess.length === 4) {
                showValidationMessage('輸入正確！', 'success');
            }
        }
    }
}

// 🎯 新增：檢查是否有重複數字
function hasDuplicateDigits(str) {
    const digits = str.split('');
    const uniqueDigits = [...new Set(digits)];
    return digits.length !== uniqueDigits.length;
}

// 🎯 新增：顯示輸入驗證訊息
function showValidationMessage(message, type) {
    const inputGroup = document.getElementById('guessInput').parentElement;
    const existingMsg = inputGroup.querySelector('.validation-message');

    if (existingMsg) {
        existingMsg.remove();
    }

    const alertClass = type === 'success' ? 'text-success' : 'text-warning';
    const icon = type === 'success' ? 'check-circle' : 'exclamation-triangle';

    const msgDiv = document.createElement('div');
    msgDiv.className = `validation-message mt-1 ${alertClass}`;
    msgDiv.innerHTML = `<i class="fas fa-${icon}"></i> <small>${message}</small>`;

    inputGroup.appendChild(msgDiv);
}

// 🎯 新增：清除驗證訊息
function clearValidationMessage() {
    const existingMsg = document.querySelector('.validation-message');
    if (existingMsg) {
        existingMsg.remove();
    }
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
            document.getElementById('guessInput').classList.remove('is-invalid');
            document.getElementById('guessInput').focus();
            document.getElementById('historyCard').style.display = 'none';

            clearValidationMessage(); // 🎯 新增：清除驗證訊息

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

    // 🎯 修改：加強輸入驗證
    if (guess.length !== 4) {
        showResult('請輸入完整的 4 位數字！', 'warning');
        return;
    }

    // 🎯 新增：檢查重複數字
    if (hasDuplicateDigits(guess)) {
        showResult('數字不可重複，請重新輸入！', 'warning');
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
            document.getElementById('guessInput').classList.remove('is-invalid');
            clearValidationMessage(); // 🎯 新增：清除驗證訊息

            if (!result.isWin) {
                document.getElementById('guessInput').focus();
            }

        } else {
            showResult(result.errorMessage, 'error');
        }

    } catch (error) {
        console.error('提交猜測失敗:', error);
        showResult('提交猜測失敗: ' + error.message, 'error');
    }
}

// 🎯 修改：更新提交按鈕狀態（加入重複檢查）
function updateSubmitButton() {
    const guess = document.getElementById('guessInput').value;
    const submitBtn = document.getElementById('submitGuess');

    // 按鈕啟用條件：長度為4、有遊戲ID、無重複數字
    const isValid = guess.length === 4 &&
        currentGameId !== null &&
        !hasDuplicateDigits(guess);

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
    clearValidationMessage(); // 🎯 新增：清除驗證訊息
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