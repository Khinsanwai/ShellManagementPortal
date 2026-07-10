using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShellMgmt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserMenuAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserMenuAssignment",
                schema: "appshell",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Wso2UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wso2UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMenuAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMenuAssignment_MenuItem_MenuItemId",
                        column: x => x.MenuItemId,
                        principalSchema: "appshell",
                        principalTable: "MenuItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMenuAssignment_MenuItemId",
                schema: "appshell",
                table: "UserMenuAssignment",
                column: "MenuItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMenuAssignment",
                schema: "appshell");
        }
    }
}
