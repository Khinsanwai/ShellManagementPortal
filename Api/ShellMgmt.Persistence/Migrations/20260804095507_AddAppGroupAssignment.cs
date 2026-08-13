using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShellMgmt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppGroupAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppGroupAssignment",
                schema: "appshell",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    Wso2GroupId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wso2GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppGroupAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppGroupAssignment_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "appshell",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppGroupAssignment_ApplicationId",
                schema: "appshell",
                table: "AppGroupAssignment",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppGroupAssignment",
                schema: "appshell");
        }
    }
}
