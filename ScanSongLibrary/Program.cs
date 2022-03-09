using Microsoft.EntityFrameworkCore;
using ScanSongLibrary;
using Shared;

if (args.Length != 1)
{
    Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} <pathToSongFolder>");
    return 1;
}

var dbOptions = new DbContextOptionsBuilder<UltrastarContext>();
dbOptions.UseSqlite($"Data Source={Environment.GetEnvironmentVariable("ULTRASTAR_DB")}");
using var db = new UltrastarContext(dbOptions.Options);

var path = args[0];
if (!Directory.Exists(path))
{
    Console.WriteLine($"Path '{path}' doesn't exist!");
    return 1;
}

var oldSongs = db.UsToolsSongs;
var songs = UltraStarCrawler.Crawl(path);

var toRemove = oldSongs.ToList().Where(song => songs.All(s => song.TxtPath != s.TxtPath)).ToList();
oldSongs.RemoveRange(toRemove);

List<UsToolsSong> toAdd = new();
foreach (var song in songs)
{
    if (oldSongs.FirstOrDefault(s => song.TxtPath == s.TxtPath) is var match && match is not null)
    {
        match.Artist = song.Artist;
        match.Title = song.Title;
        match.BackgroundPath = song.BackgroundPath;
        match.CoverPath = song.CoverPath;
        match.VideoPath = song.VideoPath;
    }
    else
        toAdd.Add(song);
}

oldSongs.AddRange(toAdd);
db.SaveChanges();

return 0;
