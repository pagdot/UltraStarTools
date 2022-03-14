namespace Shared
{
    public partial class UsScore
    {
        public long SongId { get; set; }
        public UsSong Song { get; set; } = null!;
        public long Difficulty { get; set; }
        public string Player { get; set; } = null!;
        public long Score { get; set; }
        public DateTime Date { get; set; }
    }
}
