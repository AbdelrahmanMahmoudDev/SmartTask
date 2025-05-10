using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTask.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedtheTaskModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "AssignTasks");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "AssignTasks");

            migrationBuilder.RenameColumn(
                name: "AssignedToId",
                table: "Tasks",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_AssignedToId",
                table: "Tasks",
                newName: "IX_Tasks_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_ApplicationUserId",
                table: "Tasks",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_ApplicationUserId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Tasks",
                newName: "AssignedToId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_ApplicationUserId",
                table: "Tasks",
                newName: "IX_Tasks_AssignedToId");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "AssignTasks",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "AssignTasks",
                type: "Date",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_AssignedToId",
                table: "Tasks",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
