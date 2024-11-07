namespace UsDbDownloader.Data;

public record SettingsModel(string Destination, bool YtUseOAuth)
{
    public SettingsModel() : this("./songs", false)
    {
    }
}
