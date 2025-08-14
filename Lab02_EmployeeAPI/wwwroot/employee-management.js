// 員工管理系統 JavaScript

// API 基礎 URL
const API_BASE_URL = '/api/employees';

// 全域變數
let currentEmployees = [];
let currentEditingId = null;

// 頁面載入完成後執行
document.addEventListener('DOMContentLoaded', function () {
    loadEmployees();
    setupEventListeners();
});

// 設定事件監聽器
function setupEventListeners() {
    // 搜尋輸入框的 Enter 鍵事件
    document.getElementById('searchInput').addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            searchEmployees();
        }
    });

    // 表單驗證
    document.getElementById('employeeForm').addEventListener('submit', function (e) {
        e.preventDefault();
        saveEmployee();
    });
}

// 載入所有員工
async function loadEmployees() {
    try {
        showLoading(true);

        const response = await fetch(API_BASE_URL);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const employees = await response.json();
        currentEmployees = employees;

        displayEmployees(employees);
        updateEmployeeCount(employees.length);
        showMessage('員工資料載入成功', 'success');

    } catch (error) {
        console.error('載入員工資料失敗:', error);
        showMessage('載入員工資料失敗: ' + error.message, 'error');
    } finally {
        showLoading(false);
    }
}

// 搜尋員工
async function searchEmployees() {
    const keyword = document.getElementById('searchInput').value.trim();

    if (!keyword) {
        loadEmployees();
        return;
    }

    try {
        showLoading(true);

        const response = await fetch(`${API_BASE_URL}/search?keyword=${encodeURIComponent(keyword)}`);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const employees = await response.json();
        currentEmployees = employees;

        displayEmployees(employees);
        updateEmployeeCount(employees.length);

        if (employees.length === 0) {
            showMessage(`找不到包含 "${keyword}" 的員工`, 'info');
        } else {
            showMessage(`找到 ${employees.length} 位員工`, 'success');
        }

    } catch (error) {
        console.error('搜尋員工失敗:', error);
        showMessage('搜尋失敗: ' + error.message, 'error');
    } finally {
        showLoading(false);
    }
}

// 顯示員工列表
function displayEmployees(employees) {
    const tableBody = document.getElementById('employeeTableBody');
    const tableContainer = document.getElementById('employeeTableContainer');
    const noDataArea = document.getElementById('noDataArea');

    if (employees.length === 0) {
        tableContainer.style.display = 'none';
        noDataArea.style.display = 'block';
        return;
    }

    tableContainer.style.display = 'block';
    noDataArea.style.display = 'none';

    tableBody.innerHTML = employees.map(employee => `
        <tr>
            <td>${employee.id}</td>
            <td>${escapeHtml(employee.name)}</td>
            <td>${escapeHtml(employee.email)}</td>
            <td>${escapeHtml(employee.department)}</td>
            <td>${employee.position ? escapeHtml(employee.position) : '-'}</td>
            <td>${employee.salary ? '$' + Number(employee.salary).toLocaleString() : '-'}</td>
            <td>${formatDate(employee.hireDate)}</td>
            <td>
                <span class="badge ${employee.isActive ? 'bg-success' : 'bg-secondary'}">
                    ${employee.isActive ? '在職' : '離職'}
                </span>
            </td>
            <td>
                <button class="btn btn-sm btn-outline-primary btn-action" 
                        onclick="editEmployee(${employee.id})" title="編輯">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger btn-action" 
                        onclick="confirmDeleteEmployee(${employee.id}, '${escapeHtml(employee.name)}')" title="刪除">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

// 顯示新增員工 Modal
function showAddEmployeeModal() {
    currentEditingId = null;
    document.getElementById('modalTitle').textContent = '新增員工';
    document.getElementById('statusGroup').style.display = 'none';
    clearForm();

    const modal = new bootstrap.Modal(document.getElementById('employeeModal'));
    modal.show();
}

// 編輯員工
async function editEmployee(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/${id}`);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const employee = await response.json();

        currentEditingId = id;
        document.getElementById('modalTitle').textContent = '編輯員工';
        document.getElementById('statusGroup').style.display = 'block';

        // 填入表單資料
        document.getElementById('employeeId').value = employee.id;
        document.getElementById('employeeName').value = employee.name;
        document.getElementById('employeeEmail').value = employee.email;
        document.getElementById('employeeDepartment').value = employee.department;
        document.getElementById('employeePosition').value = employee.position || '';
        document.getElementById('employeeSalary').value = employee.salary || '';
        document.getElementById('employeeHireDate').value = employee.hireDate.split('T')[0];
        document.getElementById('employeeStatus').value = employee.isActive.toString();

        const modal = new bootstrap.Modal(document.getElementById('employeeModal'));
        modal.show();

    } catch (error) {
        console.error('載入員工資料失敗:', error);
        showMessage('載入員工資料失敗: ' + error.message, 'error');
    }
}

// 儲存員工（新增或更新）
async function saveEmployee() {
    if (!validateForm()) {
        return;
    }

    const employeeData = {
        name: document.getElementById('employeeName').value.trim(),
        email: document.getElementById('employeeEmail').value.trim(),
        department: document.getElementById('employeeDepartment').value.trim(),
        position: document.getElementById('employeePosition').value.trim() || null,
        salary: document.getElementById('employeeSalary').value ? parseFloat(document.getElementById('employeeSalary').value) : null,
        hireDate: document.getElementById('employeeHireDate').value
    };

    // 如果是編輯模式，加入 ID 和狀態
    if (currentEditingId) {
        employeeData.id = currentEditingId;
        employeeData.isActive = document.getElementById('employeeStatus').value === 'true';
    }

    try {
        const url = currentEditingId ? `${API_BASE_URL}/${currentEditingId}` : API_BASE_URL;
        const method = currentEditingId ? 'PUT' : 'POST';

        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(employeeData)
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        // 關閉 Modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('employeeModal'));
        modal.hide();

        // 重新載入員工列表
        await loadEmployees();

        const action = currentEditingId ? '更新' : '新增';
        showMessage(`員工 ${employeeData.name} ${action}成功`, 'success');

    } catch (error) {
        console.error('儲存員工失敗:', error);
        showMessage('儲存失敗: ' + error.message, 'error');
    }
}

// 確認刪除員工
function confirmDeleteEmployee(id, name) {
    document.getElementById('deleteEmployeeName').textContent = name;

    const confirmBtn = document.getElementById('confirmDeleteBtn');
    confirmBtn.onclick = () => deleteEmployee(id);

    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}

// 刪除員工
async function deleteEmployee(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        // 關閉 Modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
        modal.hide();

        // 重新載入員工列表
        await loadEmployees();

        showMessage(result.message || '員工刪除成功', 'success');

    } catch (error) {
        console.error('刪除員工失敗:', error);
        showMessage('刪除失敗: ' + error.message, 'error');
    }
}

// 表單驗證
function validateForm() {
    clearValidationErrors();

    let isValid = true;

    const name = document.getElementById('employeeName').value.trim();
    const email = document.getElementById('employeeEmail').value.trim();
    const department = document.getElementById('employeeDepartment').value.trim();
    const hireDate = document.getElementById('employeeHireDate').value;

    if (!name) {
        showFieldError('employeeName', 'nameError', '請輸入員工姓名');
        isValid = false;
    }

    if (!email) {
        showFieldError('employeeEmail', 'emailError', '請輸入電子郵件');
        isValid = false;
    } else if (!isValidEmail(email)) {
        showFieldError('employeeEmail', 'emailError', '請輸入正確的電子郵件格式');
        isValid = false;
    }

    if (!department) {
        showFieldError('employeeDepartment', 'departmentError', '請輸入部門');
        isValid = false;
    }

    if (!hireDate) {
        showFieldError('employeeHireDate', 'hireDateError', '請選擇入職日期');
        isValid = false;
    }

    return isValid;
}

// 顯示欄位錯誤
function showFieldError(fieldId, errorId, message) {
    const field = document.getElementById(fieldId);
    const errorDiv = document.getElementById(errorId);

    field.classList.add('is-invalid');
    if (errorDiv) {
        errorDiv.textContent = message;
    }
}

// 清除驗證錯誤
function clearValidationErrors() {
    const fields = ['employeeName', 'employeeEmail', 'employeeDepartment', 'employeeHireDate'];
    fields.forEach(fieldId => {
        const field = document.getElementById(fieldId);
        field.classList.remove('is-invalid');
    });
}

// 清除表單
function clearForm() {
    document.getElementById('employeeForm').reset();
    clearValidationErrors();
}

// 顯示/隱藏載入動畫
function showLoading(show) {
    document.getElementById('loadingArea').style.display = show ? 'block' : 'none';
    document.getElementById('employeeTableContainer').style.display = show ? 'none' : 'block';
}

// 更新員工數量
function updateEmployeeCount(count) {
    document.getElementById('employeeCount').textContent = `共 ${count} 位員工`;
}

// 顯示訊息
function showMessage(message, type) {
    const messageArea = document.getElementById('messageArea');
    const alertClass = type === 'error' ? 'error-message' :
        type === 'success' ? 'success-message' : 'info-message';

    messageArea.innerHTML = `
        <div class="${alertClass}">
            <i class="fas fa-${getMessageIcon(type)}"></i> ${message}
        </div>
    `;

    // 3秒後自動隱藏訊息
    setTimeout(() => {
        messageArea.innerHTML = '';
    }, 3000);
}

// 取得訊息圖示
function getMessageIcon(type) {
    switch (type) {
        case 'success': return 'check-circle';
        case 'error': return 'exclamation-circle';
        case 'info': return 'info-circle';
        default: return 'info-circle';
    }
}

// 工具函數
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('zh-TW');
}

function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}