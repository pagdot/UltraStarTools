namespace UsDbDownloader.Data;

public record UsDbSongDetails(string YoutubeId, string CoverUrl, double Bpm, double Gap, bool GoldenNotes, bool Songcheck,
    DateTime Date, int Rating, int Ratings, string Comments, string Language)
{
    public string YoutubeId { get; set; } = YoutubeId;
}