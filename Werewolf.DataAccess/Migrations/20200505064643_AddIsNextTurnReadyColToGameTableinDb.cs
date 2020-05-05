using Microsoft.EntityFrameworkCore.Migrations;

namespace Werewolf.DataAccess.Migrations
{
    public partial class AddIsNextTurnReadyColToGameTableinDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNextTurnReady",
                table: "Game",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNextTurnReady",
                table: "Game");
        }
    }
}
