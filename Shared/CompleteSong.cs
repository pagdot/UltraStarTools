namespace Shared;

public record CompleteSong(string Artist, string Title, IEnumerable<SimpleScore> Scores, long? Rating = null);

