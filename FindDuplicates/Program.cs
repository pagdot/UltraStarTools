using System.Text;
using Microsoft.EntityFrameworkCore;
using Shared;

if (args.Length < 1)
{
    Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} <pathToDb>");
    return 1;
}

var dbOptions = new DbContextOptionsBuilder<UltrastarContext>();
dbOptions.UseSqlite($"Data Source={args[0]}");
using var db = new UltrastarContext(dbOptions.Options);
foreach (var duplicates in db.UsToolsSongs.ToList().GroupBy(s => new {s.Artist, s.Title}).Where(x => x.Count() > 1))
{
    Console.WriteLine($"Found duplicates for {duplicates.Key.Artist} - {duplicates.Key.Title}:");

    foreach (var song in duplicates)
    {
        Console.WriteLine($"- {(string.IsNullOrWhiteSpace(song.CoverPath) ? "" : "[CO]")}{(string.IsNullOrWhiteSpace(song.VideoPath) ? "" : "[VID]")}{(string.IsNullOrWhiteSpace(song.BackgroundPath) ? "" : "[BG]")} {song.TxtPath}");
    }
}

return 0;
