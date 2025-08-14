using Lab02_EmployeeAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; // 新增這行

using System.Collections.Generic;
using System.Reflection.Emit;

namespace Lab02_EmployeeAPI.Data
{
    public class EmployeeDbContext : DbContext
    {
        // 建構子：接收資料庫設定選項
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
        {
        }

        // 定義資料表：Employees
        public DbSet<Employee> Employees { get; set; }

        // 設定模型建立規則
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 設定 Employee 資料表
            modelBuilder.Entity<Employee>(entity =>
            {
                // 設定資料表名稱
                entity.ToTable("Employees");

                // 設定主鍵
                entity.HasKey(e => e.Id);

                // 設定 Id 為自動遞增
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // 設定 Name 欄位：不可為空，最大長度 100
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                // 設定 Email 欄位：不可為空，建立索引確保唯一性
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                // 建立 Email 的唯一索引
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Employees_Email");

                // 設定 Department 欄位
                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(50);

                // 設定 Position 欄位：可為空
                entity.Property(e => e.Position)
                    .HasMaxLength(50);

                // 設定 Salary 欄位：可為空，精確度 18,2
                entity.Property(e => e.Salary)
                    .HasColumnType("decimal(18,2)");

                // 設定 HireDate 欄位：不可為空
                entity.Property(e => e.HireDate)
                    .IsRequired();

                // 設定 IsActive 欄位：預設值 true
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                // 設定 CreatedAt 欄位：預設為目前時間
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                // 設定 UpdatedAt 欄位：可為空
                entity.Property(e => e.UpdatedAt);
            });

            // 種子資料（使用固定的時間值）
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "張小明",
                    Email = "ming.zhang@company.com",
                    Department = "資訊部",
                    Position = "軟體工程師",
                    Salary = 60000,
                    HireDate = new DateTime(2023, 1, 15),
                    IsActive = true,
                    CreatedAt = new DateTime(2023, 1, 15, 9, 0, 0) // 🔧 使用固定時間
                },
                new Employee
                {
                    Id = 2,
                    Name = "李小華",
                    Email = "hua.li@company.com",
                    Department = "人事部",
                    Position = "人事專員",
                    Salary = 45000,
                    HireDate = new DateTime(2023, 3, 20),
                    IsActive = true,
                    CreatedAt = new DateTime(2023, 3, 20, 9, 0, 0) // 🔧 使用固定時間
                },
                new Employee
                {
                    Id = 3,
                    Name = "王大同",
                    Email = "datong.wang@company.com",
                    Department = "銷售部",
                    Position = "業務經理",
                    Salary = 80000,
                    HireDate = new DateTime(2022, 8, 10),
                    IsActive = true,
                    CreatedAt = new DateTime(2022, 8, 10, 9, 0, 0) // 🔧 使用固定時間
                }
            );
        }
    }
}