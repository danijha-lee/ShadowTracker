using Microsoft.EntityFrameworkCore.Migrations;

namespace ShadowTracker.Data.Migrations
{
    public partial class ToDoowneruser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerUserId",
                table: "ToDos",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToDos_OwnerUserId",
                table: "ToDos",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDos_AspNetUsers_OwnerUserId",
                table: "ToDos",
                column: "OwnerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDos_AspNetUsers_OwnerUserId",
                table: "ToDos");

            migrationBuilder.DropIndex(
                name: "IX_ToDos_OwnerUserId",
                table: "ToDos");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "ToDos");
        }
    }
}
