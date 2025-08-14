using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab02_EmployeeAPI.Data;
using Lab02_EmployeeAPI.Models;

namespace Lab02_EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeesController(EmployeeDbContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        // 取得所有員工列表
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employees = await _context.Employees
                .Where(e => e.IsActive) // 只取得在職員工
                .OrderBy(e => e.Id)
                .ToListAsync();

            return Ok(employees);
        }

        // GET: api/Employees/5
        // 根據 ID 取得特定員工
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound(new { message = $"找不到 ID 為 {id} 的員工" });
            }

            return Ok(employee);
        }

        // POST: api/Employees
        // 新增員工
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            // 檢查模型驗證
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 檢查 Email 是否重複
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == employee.Email);

            if (existingEmployee != null)
            {
                return BadRequest(new { message = "此電子郵件已被使用" });
            }

            // 設定建立時間
            employee.CreatedAt = DateTime.Now;
            employee.IsActive = true;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        // PUT: api/Employees/5
        // 更新員工資料
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest(new { message = "ID 不符合" });
            }

            // 檢查模型驗證
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 檢查員工是否存在
            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound(new { message = $"找不到 ID 為 {id} 的員工" });
            }

            // 檢查 Email 是否與其他員工重複
            var duplicateEmail = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == employee.Email && e.Id != id);

            if (duplicateEmail != null)
            {
                return BadRequest(new { message = "此電子郵件已被其他員工使用" });
            }

            // 更新員工資料
            existingEmployee.Name = employee.Name;
            existingEmployee.Email = employee.Email;
            existingEmployee.Department = employee.Department;
            existingEmployee.Position = employee.Position;
            existingEmployee.Salary = employee.Salary;
            existingEmployee.HireDate = employee.HireDate;
            existingEmployee.IsActive = employee.IsActive;
            existingEmployee.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(existingEmployee);
        }

        // DELETE: api/Employees/5
        // 刪除員工（軟刪除：標記為非在職）
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = $"找不到 ID 為 {id} 的員工" });
            }

            // 軟刪除：標記為非在職，而不是真正刪除
            employee.IsActive = false;
            employee.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"員工 {employee.Name} 已標記為離職" });
        }

        // 輔助方法：檢查員工是否存在
        private async Task<bool> EmployeeExists(int id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }

        // GET: api/Employees/search?keyword=張
        // 搜尋員工
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Employee>>> SearchEmployees(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "請提供搜尋關鍵字" });
            }

            var employees = await _context.Employees
                .Where(e => e.IsActive &&
                           (e.Name.Contains(keyword) ||
                            e.Email.Contains(keyword) ||
                            e.Department.Contains(keyword) ||
                            (e.Position != null && e.Position.Contains(keyword))))
                .OrderBy(e => e.Name)
                .ToListAsync();

            return Ok(employees);
        }

        // GET: api/Employees/departments
        // 取得所有部門列表
        [HttpGet("departments")]
        public async Task<ActionResult<IEnumerable<string>>> GetDepartments()
        {
            var departments = await _context.Employees
                .Where(e => e.IsActive)
                .Select(e => e.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return Ok(departments);
        }
    }
}