using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistCreator.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "us_songs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Artist = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    TimesPlayed = table.Column<long>(type: "INTEGER", nullable: false),
                    Rating = table.Column<long>(type: "INTEGER", nullable: true),
                    VideoRatioAspect = table.Column<long>(type: "INTEGER", nullable: true),
                    VideoWidth = table.Column<long>(type: "INTEGER", nullable: true),
                    VideoHeight = table.Column<long>(type: "INTEGER", nullable: true),
                    LyricPosition = table.Column<long>(type: "INTEGER", nullable: true),
                    LyricAlpha = table.Column<long>(type: "INTEGER", nullable: true),
                    LyricSingFillColor = table.Column<string>(type: "TEXT", nullable: true),
                    LyricActualFillColor = table.Column<string>(type: "TEXT", nullable: true),
                    LyricNextFillColor = table.Column<string>(type: "TEXT", nullable: true),
                    LyricSingOutlineColor = table.Column<string>(type: "TEXT", nullable: true),
                    LyricActualOutlineColor = table.Column<string>(type: "TEXT", nullable: true),
                    LyricNextOutlineColor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_us_songs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "us_statistics_info",
                columns: table => new
                {
                    ResetTime = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "us_users_info",
                columns: table => new
                {
                    WebID = table.Column<long>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    SendSavePlayer = table.Column<long>(type: "INTEGER", nullable: false),
                    AutoMode = table.Column<long>(type: "INTEGER", nullable: false),
                    AutoPlayer = table.Column<long>(type: "INTEGER", nullable: false),
                    AutoScoreEasy = table.Column<long>(type: "INTEGER", nullable: false),
                    AutoScoreMedium = table.Column<long>(type: "INTEGER", nullable: false),
                    AutoScoreHard = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "us_webs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_us_webs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "us_webs_stats",
                columns: table => new
                {
                    WebID = table.Column<long>(type: "INTEGER", nullable: false),
                    SongID = table.Column<long>(type: "INTEGER", nullable: false),
                    Max_Score_0 = table.Column<long>(type: "INTEGER", nullable: true),
                    Media_Score_0 = table.Column<long>(type: "INTEGER", nullable: true),
                    User_Score_0 = table.Column<string>(type: "TEXT", nullable: true),
                    Max_Score_1 = table.Column<long>(type: "INTEGER", nullable: true),
                    Media_Score_1 = table.Column<long>(type: "INTEGER", nullable: true),
                    User_Score_1 = table.Column<string>(type: "TEXT", nullable: true),
                    Max_Score_2 = table.Column<long>(type: "INTEGER", nullable: true),
                    Media_Score_2 = table.Column<long>(type: "INTEGER", nullable: true),
                    User_Score_2 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "us_scores",
                columns: table => new
                {
                    SongId = table.Column<long>(type: "INTEGER", nullable: false),
                    Difficulty = table.Column<long>(type: "INTEGER", nullable: false),
                    Player = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<long>(type: "INTEGER", nullable: false),
                    Date = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_us_scores_us_songs_SongId",
                        column: x => x.SongId,
                        principalTable: "us_songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_us_scores_SongId",
                table: "us_scores",
                column: "SongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "us_scores");

            migrationBuilder.DropTable(
                name: "us_statistics_info");

            migrationBuilder.DropTable(
                name: "us_users_info");

            migrationBuilder.DropTable(
                name: "us_webs");

            migrationBuilder.DropTable(
                name: "us_webs_stats");

            migrationBuilder.DropTable(
                name: "us_songs");
        }
    }
}
