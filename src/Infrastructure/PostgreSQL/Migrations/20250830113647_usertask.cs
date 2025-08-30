using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class usertask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleUser",
                schema: "OTUS");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                schema: "OTUS",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                schema: "OTUS",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserTasks",
                schema: "OTUS",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TaskId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTasks", x => new { x.UserId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_UserTasks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "OTUS",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTasks_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "OTUS",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                schema: "OTUS",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_TaskId",
                schema: "OTUS",
                table: "UserTasks",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_UserId",
                schema: "OTUS",
                table: "UserTasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                schema: "OTUS",
                table: "Users",
                column: "RoleId",
                principalSchema: "OTUS",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                schema: "OTUS",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserTasks",
                schema: "OTUS");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                schema: "OTUS",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "OTUS",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Deadline",
                schema: "OTUS",
                table: "Tasks");

            migrationBuilder.CreateTable(
                name: "RoleUser",
                schema: "OTUS",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RoleId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "OTUS",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalSchema: "OTUS",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersId",
                schema: "OTUS",
                table: "RoleUser",
                column: "UsersId");
        }
    }
}
