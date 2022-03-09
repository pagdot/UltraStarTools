namespace Shared
{
    public partial class UsSong
    {
        public long Id { get; set; }
        public string Artist { get; set; } = null!;
        public string Title { get; set; } = null!;
        public long TimesPlayed { get; set; }
        public long? Rating { get; set; }
        public long? VideoRatioAspect { get; set; }
        public long? VideoWidth { get; set; }
        public long? VideoHeight { get; set; }
        public long? LyricPosition { get; set; }
        public long? LyricAlpha { get; set; }
        public string? LyricSingFillColor { get; set; }
        public string? LyricActualFillColor { get; set; }
        public string? LyricNextFillColor { get; set; }
        public string? LyricSingOutlineColor { get; set; }
        public string? LyricActualOutlineColor { get; set; }
        public string? LyricNextOutlineColor { get; set; }
    }
}
