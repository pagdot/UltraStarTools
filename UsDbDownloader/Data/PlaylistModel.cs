namespace UsDbDownloader.Data;

public record PlaylistModel
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public List<SongModel> Songs { get; init; } = new List<SongModel>();
}
