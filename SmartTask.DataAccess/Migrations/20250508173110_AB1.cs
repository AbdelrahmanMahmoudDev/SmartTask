using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTask.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AB1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers");

            //migrationBuilder.AddColumn<int>(
            //    name: "BranchId",
            //    table: "Projects",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "DepartmentId",
            //    table: "Projects",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "BranchId",
            //    table: "AspNetUsers",
            //    type: "int",
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Projects_BranchId",
            //    table: "Projects",
            //    column: "BranchId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Projects_DepartmentId",
            //    table: "Projects",
            //    column: "DepartmentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AspNetUsers_BranchId",
            //    table: "AspNetUsers",
            //    column: "BranchId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_AspNetUsers_Branches_BranchId",
            //    table: "AspNetUsers",
            //    column: "BranchId",
            //    principalTable: "Branches",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_AspNetUsers_Departments_DepartmentId",
            //    table: "AspNetUsers",
            //    column: "DepartmentId",
            //    principalTable: "Departments",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Projects_Branches_BranchId",
            //    table: "Projects",
            //    column: "BranchId",
            //    principalTable: "Branches",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Projects_Departments_DepartmentId",
            //    table: "Projects",
            //    column: "DepartmentId",
            //    principalTable: "Departments",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Branches_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Branches_BranchId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Departments_DepartmentId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_BranchId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DepartmentId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }
    }
}
