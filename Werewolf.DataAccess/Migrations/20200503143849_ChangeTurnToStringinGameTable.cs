﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Werewolf.DataAccess.Migrations
{
    public partial class ChangeTurnToStringinGameTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Turn",
                table: "Game",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Turn",
                table: "Game",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
