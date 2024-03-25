using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordleArena.Migrations
{
    /// <inheritdoc />
    public partial class addgameresult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tempo_game_player_results",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    game_id = table.Column<string>(type: "text", nullable: false),
                    ResultInfo = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tempo_game_player_results", x => new { x.user_id, x.game_id });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tempo_game_player_results");
        }
    }
}
