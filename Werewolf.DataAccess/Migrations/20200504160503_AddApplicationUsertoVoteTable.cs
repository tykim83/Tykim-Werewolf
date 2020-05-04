using Microsoft.EntityFrameworkCore.Migrations;

namespace Werewolf.DataAccess.Migrations
{
    public partial class AddApplicationUsertoVoteTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Vote",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vote_ApplicationUserId",
                table: "Vote",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_AspNetUsers_ApplicationUserId",
                table: "Vote",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vote_AspNetUsers_ApplicationUserId",
                table: "Vote");

            migrationBuilder.DropIndex(
                name: "IX_Vote_ApplicationUserId",
                table: "Vote");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Vote");
        }
    }
}
