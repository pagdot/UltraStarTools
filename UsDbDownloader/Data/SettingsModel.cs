namespace UsDbDownloader.Data;

public record SettingsModel
{
    public SettingsModel(string destination)
    {
        Destination = destination;
    }
    public SettingsModel()
    {
        Destination = "./songs";
    }

    public string Destination { get; init; }
}
