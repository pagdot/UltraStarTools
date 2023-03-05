namespace UsDbDownloader.Data;

public record UsDbLoginModel
{
    public string User { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
