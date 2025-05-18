using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTask.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class taskidallownull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Tasks_TaskId",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "Events",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Tasks_TaskId",
                table: "Events",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Tasks_TaskId",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Tasks_TaskId",
                table: "Events",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
