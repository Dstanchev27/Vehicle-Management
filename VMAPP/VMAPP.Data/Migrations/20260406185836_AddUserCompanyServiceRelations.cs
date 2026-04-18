using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMAPP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCompanyServiceRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "InsuranceCompanyId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleServiceId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_InsuranceCompanyId",
                table: "AspNetUsers",
                column: "InsuranceCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_VehicleServiceId",
                table: "AspNetUsers",
                column: "VehicleServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_InsuranceCompanies_InsuranceCompanyId",
                table: "AspNetUsers",
                column: "InsuranceCompanyId",
                principalTable: "InsuranceCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_VehicleServices_VehicleServiceId",
                table: "AspNetUsers",
                column: "VehicleServiceId",
                principalTable: "VehicleServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_InsuranceCompanies_InsuranceCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_VehicleServices_VehicleServiceId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_InsuranceCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_VehicleServiceId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "InsuranceCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VehicleServiceId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
