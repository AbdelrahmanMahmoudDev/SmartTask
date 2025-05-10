using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTask.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddingSenderandReceiverToNotificationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_Sender",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_Sender",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Sender",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReceiverId",
                table: "Notifications",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_ReceiverId",
                table: "Notifications",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderId",
                table: "Notifications",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_ReceiverId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ReceiverId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "Sender",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Sender",
                table: "Notifications",
                column: "Sender");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_Sender",
                table: "Notifications",
                column: "Sender",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
