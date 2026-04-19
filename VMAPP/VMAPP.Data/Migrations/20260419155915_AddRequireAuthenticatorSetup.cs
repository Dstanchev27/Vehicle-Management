using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMAPP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequireAuthenticatorSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequireAuthenticatorSetup",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequireAuthenticatorSetup",
                table: "AspNetUsers");
        }
    }
}
