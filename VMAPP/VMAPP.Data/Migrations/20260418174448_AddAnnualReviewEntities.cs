using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMAPP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnualReviewEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnnualReviewCompanyId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnnualReviewCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualReviewCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnualReviewCompanies_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnnualReviewCompanies_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnnualReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    AnnualReviewCompanyId = table.Column<int>(type: "int", nullable: false),
                    ReportNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Passed = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnualReports_AnnualReviewCompanies_AnnualReviewCompanyId",
                        column: x => x.AnnualReviewCompanyId,
                        principalTable: "AnnualReviewCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnnualReports_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnnualReports_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnnualReports_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AnnualReviewCompanyId",
                table: "AspNetUsers",
                column: "AnnualReviewCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualReports_AnnualReviewCompanyId",
                table: "AnnualReports",
                column: "AnnualReviewCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualReports_CreatedById",
                table: "AnnualReports",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualReports_ModifiedById",
                table: "AnnualReports",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualReports_VehicleId",
                table: "AnnualReports",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualReviewCompanies_CreatedById",
                table: "AnnualReviewCompanies",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualReviewCompanies_ModifiedById",
                table: "AnnualReviewCompanies",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AnnualReviewCompanies_AnnualReviewCompanyId",
                table: "AspNetUsers",
                column: "AnnualReviewCompanyId",
                principalTable: "AnnualReviewCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AnnualReviewCompanies_AnnualReviewCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AnnualReports");

            migrationBuilder.DropTable(
                name: "AnnualReviewCompanies");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AnnualReviewCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AnnualReviewCompanyId",
                table: "AspNetUsers");
        }
    }
}
