namespace UsDbDownloader.Data;

public record UsDbSong(string Artist, string Title, int Id, bool IsAlreadyDownloaded = false, bool IsAlreadyAvailable = false, UsDbSongDetails? Details = null)
{
    public UsDbSongDetails? Details { get; set; } = Details;
    public bool IsAlreadyDownloaded { get; set; } = IsAlreadyDownloaded;
}
