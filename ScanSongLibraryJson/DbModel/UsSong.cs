namespace ScanSongLibrary.DbModel
{
    public class UsSong
    {
        public long Id { get; set; }
        public string Artist { get; set; } = null!;
        public string Title { get; set; } = null!;
        public long? TimesPlayed { get; set; }
        public long? Rating { get; set; }
    }
}
