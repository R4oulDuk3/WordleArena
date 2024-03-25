using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordleArena.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bots",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    in_use = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bots", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "document_provider_states",
                columns: table => new
                {
                    provider = table.Column<string>(type: "text", nullable: false),
                    serialized_state = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_document_provider_states", x => x.provider);
                });

            migrationBuilder.CreateTable(
                name: "hashes",
                columns: table => new
                {
                    value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hashes", x => x.value);
                });

            migrationBuilder.CreateTable(
                name: "player_matchmaking_infos",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    enter_timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_matchmaking_infos", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "wordle_words",
                columns: table => new
                {
                    target_word = table.Column<string>(type: "text", nullable: false),
                    word_lenght = table.Column<int>(type: "integer", nullable: false),
                    frequency = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wordle_words", x => x.target_word);
                });

            migrationBuilder.CreateTable(
                name: "word_definitions",
                columns: table => new
                {
                    word = table.Column<string>(type: "text", nullable: false),
                    is_in_dictionary = table.Column<bool>(type: "boolean", nullable: false),
                    inflected = table.Column<bool>(type: "boolean", nullable: false),
                    PossibleMeanings = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_word_definitions", x => x.word);
                    table.ForeignKey(
                        name: "fk_word_definitions_wordle_words_wordle_word_target_word",
                        column: x => x.word,
                        principalTable: "wordle_words",
                        principalColumn: "target_word",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bots");

            migrationBuilder.DropTable(
                name: "document_provider_states");

            migrationBuilder.DropTable(
                name: "hashes");

            migrationBuilder.DropTable(
                name: "player_matchmaking_infos");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "word_definitions");

            migrationBuilder.DropTable(
                name: "wordle_words");
        }
    }
}
