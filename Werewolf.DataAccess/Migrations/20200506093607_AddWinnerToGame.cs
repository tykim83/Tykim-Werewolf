using Microsoft.EntityFrameworkCore.Migrations;

namespace Werewolf.DataAccess.Migrations
{
    public partial class AddWinnerToGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Winner",
                table: "Game",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Winner",
                table: "Game");
        }
    }
}
