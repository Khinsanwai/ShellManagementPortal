using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShellMgmt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserChildApplicationAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_App",
                schema: "appshell",
                table: "App");

            migrationBuilder.RenameTable(
                name: "App",
                schema: "appshell",
                newName: "Applications",
                newSchema: "appshell");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "appshell",
                table: "Applications",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "appshell",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "appshell",
                table: "Applications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                schema: "appshell",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                schema: "appshell",
                table: "Applications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "URL",
                schema: "appshell",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                schema: "appshell",
                table: "Applications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                schema: "appshell",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Applications",
                schema: "appshell",
                table: "Applications",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserChildApplicationAssignment",
                schema: "appshell",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Wso2UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wso2UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChildApplicationAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChildApplicationAssignment_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "appshell",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Userlogs",
                schema: "appshell",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Application = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Module = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Userlogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserChildApplicationAssignment_ApplicationId",
                schema: "appshell",
                table: "UserChildApplicationAssignment",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserChildApplicationAssignment",
                schema: "appshell");

            migrationBuilder.DropTable(
                name: "Userlogs",
                schema: "appshell");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Applications",
                schema: "appshell",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "appshell",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "appshell",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Icon",
                schema: "appshell",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "appshell",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "URL",
                schema: "appshell",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                schema: "appshell",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "appshell",
                table: "Applications");

            migrationBuilder.RenameTable(
                name: "Applications",
                schema: "appshell",
                newName: "App",
                newSchema: "appshell");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "appshell",
                table: "App",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_App",
                schema: "appshell",
                table: "App",
                column: "Id");
        }
    }
}
