namespace Shared
{
    public partial class UsToolsSong
    {
        public long Id { get; set; }
        public string Artist { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string CoverPath { get; set; } = String.Empty;
        public string BackgroundPath { get; set; } = String.Empty;
        public string VideoPath { get; set; } = String.Empty;
        public string TxtPath { get; set; } = String.Empty;
        public IEnumerable<UsToolsPlaylist> Playlists { get; } = null!;
    }
}
