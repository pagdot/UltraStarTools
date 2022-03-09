namespace Shared
{
    public partial class UsToolsPlaylist
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public List<UsToolsSong> Songs { get; set; } = null!;
    }
}
