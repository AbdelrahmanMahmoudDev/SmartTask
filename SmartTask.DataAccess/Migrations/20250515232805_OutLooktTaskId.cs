using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTask.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class OutLooktTaskId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutLooktTaskId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutLooktTaskId",
                table: "Events");
        }
    }
}
