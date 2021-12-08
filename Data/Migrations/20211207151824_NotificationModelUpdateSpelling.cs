using Microsoft.EntityFrameworkCore.Migrations;

namespace ShadowTracker.Data.Migrations
{
    public partial class NotificationModelUpdateSpelling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReciepientId",
                table: "Notifications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReciepientId",
                table: "Notifications",
                type: "text",
                nullable: true);
        }
    }
}
