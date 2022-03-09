using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaylistCreator.Migrations
{
    public partial class Playlist2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ustool_playlistssongs_us_songs_SongId",
                table: "ustool_playlistssongs");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "ustool_playlists",
                newName: "Id");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ustool_playlists",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "UsToolsSongs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Artist = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    CoverPath = table.Column<string>(type: "TEXT", nullable: false),
                    BackgroundPath = table.Column<string>(type: "TEXT", nullable: false),
                    VideoPath = table.Column<string>(type: "TEXT", nullable: false),
                    TxtPath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsToolsSongs", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ustool_playlistssongs_UsToolsSongs_SongId",
                table: "ustool_playlistssongs",
                column: "SongId",
                principalTable: "UsToolsSongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ustool_playlistssongs_UsToolsSongs_SongId",
                table: "ustool_playlistssongs");

            migrationBuilder.DropTable(
                name: "UsToolsSongs");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ustool_playlists",
                newName: "ID");

            migrationBuilder.AlterColumn<long>(
                name: "ID",
                table: "ustool_playlists",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddForeignKey(
                name: "FK_ustool_playlistssongs_us_songs_SongId",
                table: "ustool_playlistssongs",
                column: "SongId",
                principalTable: "us_songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
