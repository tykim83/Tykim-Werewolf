using Microsoft.EntityFrameworkCore.Migrations;

namespace Werewolf.DataAccess.Migrations
{
    public partial class AddTurnTypeAndTurnNumberToGameTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Turn",
                table: "Game");

            migrationBuilder.AddColumn<int>(
                name: "TurnNumber",
                table: "Game",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TurnType",
                table: "Game",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TurnNumber",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "TurnType",
                table: "Game");

            migrationBuilder.AddColumn<string>(
                name: "Turn",
                table: "Game",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
