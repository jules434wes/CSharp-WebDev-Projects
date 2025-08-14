using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lab02_EmployeeAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "Department", "Email", "HireDate", "IsActive", "Name", "Position", "Salary", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), "資訊部", "ming.zhang@company.com", new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "張小明", "軟體工程師", 60000m, null },
                    { 2, new DateTime(2023, 3, 20, 9, 0, 0, 0, DateTimeKind.Unspecified), "人事部", "hua.li@company.com", new DateTime(2023, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "李小華", "人事專員", 45000m, null },
                    { 3, new DateTime(2022, 8, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), "銷售部", "datong.wang@company.com", new DateTime(2022, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "王大同", "業務經理", 80000m, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
