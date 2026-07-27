using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShellMgmt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppDisplayOrderAndIsVisible : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Columns already exist in database
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                schema: "appshell",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                schema: "appshell",
                table: "Applications");
        }
    }
}
