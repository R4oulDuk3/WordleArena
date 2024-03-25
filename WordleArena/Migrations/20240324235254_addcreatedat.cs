using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordleArena.Migrations
{
    /// <inheritdoc />
    public partial class addcreatedat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "finished_at",
                table: "tempo_game_player_results",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "finished_at",
                table: "tempo_game_player_results");
        }
    }
}
