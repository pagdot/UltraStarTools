using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistCreator.Migrations
{
    public partial class Playlist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsToolsPlaylists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsToolsPlaylists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsToolsPlaylistUsToolsSong",
                columns: table => new
                {
                    PlaylistsId = table.Column<long>(type: "INTEGER", nullable: false),
                    SongsId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsToolsPlaylistUsToolsSong", x => new { x.PlaylistsId, x.SongsId });
                    table.ForeignKey(
                        name: "FK_UsToolsPlaylistUsToolsSong_UsToolsPlaylists_PlaylistsId",
                        column: x => x.PlaylistsId,
                        principalTable: "UsToolsPlaylists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsToolsPlaylistUsToolsSong_UsToolsSongs_SongsId",
                        column: x => x.SongsId,
                        principalTable: "UsToolsSongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsToolsPlaylistUsToolsSong_SongsId",
                table: "UsToolsPlaylistUsToolsSong",
                column: "SongsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsToolsPlaylistUsToolsSong");

            migrationBuilder.DropTable(
                name: "UsToolsPlaylists");
        }
    }
}
