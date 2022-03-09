namespace Shared
{
    public partial class UsToolsPlaylistsSongs
    {
        public long PlaylistId { get; set; }
        public UsToolsPlaylist Playlist { get; set; } = null!;
        public long SongId { get; set; }
        public UsToolsSong Song { get; set; } = null!;
    }
}
