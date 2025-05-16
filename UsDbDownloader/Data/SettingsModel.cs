namespace UsDbDownloader.Data;

public record SettingsModel(string Destination, string CookiePath)
{
    public SettingsModel() : this("./songs", "")
    {
    }
}
