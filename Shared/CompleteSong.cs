namespace Shared;

public record CompleteSong(string Artist, string Title, long? TimesPlayed, long? Rating = null, long? HighScore = null,
    string? HighScoreHolder = null, DateTime? HighScoreDate = null);
