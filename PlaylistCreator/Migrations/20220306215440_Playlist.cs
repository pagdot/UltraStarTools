using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistCreator.Migrations
{
    public partial class Playlist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ustool_playlists",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ustool_playlists", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ustool_playlistssongs",
                columns: table => new
                {
                    PlaylistId = table.Column<long>(type: "INTEGER", nullable: false),
                    SongId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ustool_playlistssongs", x => new { x.PlaylistId, x.SongId });
                    table.ForeignKey(
                        name: "FK_ustool_playlistssongs_us_songs_SongId",
                        column: x => x.SongId,
                        principalTable: "us_songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ustool_playlistssongs_ustool_playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "ustool_playlists",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ustool_playlistssongs_SongId",
                table: "ustool_playlistssongs",
                column: "SongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ustool_playlistssongs");

            migrationBuilder.DropTable(
                name: "ustool_playlists");
        }
    }
}
