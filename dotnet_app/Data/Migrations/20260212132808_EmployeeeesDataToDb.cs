using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_app.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeeesDataToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_verified",
                table: "Employees",
                newName: "IsVerified");

            migrationBuilder.RenameColumn(
                name: "account_creation_token",
                table: "Employees",
                newName: "AccountCreationToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "Employees",
                newName: "is_verified");

            migrationBuilder.RenameColumn(
                name: "AccountCreationToken",
                table: "Employees",
                newName: "account_creation_token");
        }
    }
}
