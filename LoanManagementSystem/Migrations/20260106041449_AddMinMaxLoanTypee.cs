using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMinMaxLoanTypee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tenure",
                table: "LoanTypes");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxAmount",
                table: "LoanTypes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinAmount",
                table: "LoanTypes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxAmount",
                table: "LoanTypes");

            migrationBuilder.DropColumn(
                name: "MinAmount",
                table: "LoanTypes");

            migrationBuilder.AddColumn<int>(
                name: "Tenure",
                table: "LoanTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
