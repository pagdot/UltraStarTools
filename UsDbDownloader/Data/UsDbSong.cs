namespace UsDbDownloader.Data;

public record UsDbSong(string Artist, string Title, int Id, UsDbSongDetails? Details = null)
{
    public UsDbSongDetails? Details { get; set; } = Details;
}
