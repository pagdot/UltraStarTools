using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScanSongLibrary;
using ScanSongLibrary.DbModel;

if (args.Length != 2)
{
    Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} <pathToSongFolder> <path to db>");
    return 1;
}


var songPath = args[0];
if (!Directory.Exists(songPath))
{
    Console.WriteLine($"Path '{songPath}' doesn't exist!");
    return 1;
}
var dbPath = args[1];
if (!File.Exists(dbPath))
{
    Console.WriteLine($"Path '{dbPath}' doesn't exist!");
    return 1;
}

var dbOptions = new DbContextOptionsBuilder<UltrastarContext>();
dbOptions.UseSqlite($"Data Source={dbPath}");
using var db = new UltrastarContext(dbOptions.Options);

var songs = new UltraStarCrawler(songPath, db).Crawl();

Console.WriteLine(JsonSerializer.Serialize(songs));

return 0;
