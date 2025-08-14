using System.ComponentModel.DataAnnotations;

namespace Lab02_EmployeeAPI.Models
{
    public class Employee
    {
        // 主鍵，EF Core 會自動識別名為 Id 的屬性為主鍵
        public int Id { get; set; }

        // 員工姓名（必填，最大長度 100）
        [Required(ErrorMessage = "員工姓名為必填")]
        [MaxLength(100, ErrorMessage = "員工姓名不能超過100個字元")]
        public string Name { get; set; } = string.Empty;

        // 電子郵件（必填，要符合信箱格式）
        [Required(ErrorMessage = "電子郵件為必填")]
        [EmailAddress(ErrorMessage = "請輸入正確的電子郵件格式")]
        public string Email { get; set; } = string.Empty;

        // 部門（必填，最大長度 50）
        [Required(ErrorMessage = "部門為必填")]
        [MaxLength(50, ErrorMessage = "部門名稱不能超過50個字元")]
        public string Department { get; set; } = string.Empty;

        // 職位（選填，最大長度 50）
        [MaxLength(50, ErrorMessage = "職位名稱不能超過50個字元")]
        public string? Position { get; set; }

        // 薪水（選填，要大於0）
        [Range(0, double.MaxValue, ErrorMessage = "薪水必須大於等於0")]
        public decimal? Salary { get; set; }

        // 入職日期（必填）
        [Required(ErrorMessage = "入職日期為必填")]
        public DateTime HireDate { get; set; }

        // 是否在職（預設為 true）
        public bool IsActive { get; set; } = true;

        // 建立時間（系統自動設定）
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 更新時間（可為空）
        public DateTime? UpdatedAt { get; set; }
    }
}