namespace UsDbDownloader.Data;

public class SongModel
{
    public long Id { get; init; }
    public string Artist { get; init; }
    public string Title { get; init; } = null!;
    public IEnumerable<PlaylistModel> Playlists { get; init; } = Enumerable.Empty<PlaylistModel>();
}
