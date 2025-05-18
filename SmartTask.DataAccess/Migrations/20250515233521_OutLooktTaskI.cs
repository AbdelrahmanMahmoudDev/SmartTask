using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTask.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class OutLooktTaskI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OutLooktTaskId",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OutLooktTaskId",
                table: "Events",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
