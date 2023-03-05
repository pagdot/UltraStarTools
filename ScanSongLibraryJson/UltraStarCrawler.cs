using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ScanSongLibrary.DbModel;
using Shared;

namespace ScanSongLibrary;

public class UltraStarCrawler
{
    private static readonly string _artistPrefix = "#ARTIST:";
    private static readonly string _titlePrefix = "#TITLE:";

    private readonly string _songPath;
    private readonly List<UsScore> _scores;
    private readonly List<UsSong> _songs;

    public UltraStarCrawler(string songPath, UltrastarContext context)
    {
        _songPath = songPath;
        _scores = context.UsScores.Include(x => x.Song).ToList();
        _songs = context.UsSongs.ToList();
    }

    private CompleteSong? Parse(string path)
    {
        var lines = File.ReadLines(path).ToArray();

        if (lines.FirstOrDefault(l => l.StartsWith(_artistPrefix))?.Substring(_artistPrefix.Length) is var artist && artist is null)
            return null;
        
        if (lines.FirstOrDefault(l => l.StartsWith(_titlePrefix))?.Substring(_titlePrefix.Length) is var title && title is null)
            return null;

        var song = _songs.FirstOrDefault(song =>
            song.Artist.Equals(artist, StringComparison.InvariantCultureIgnoreCase) &&
            song.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase));
        
        var highscore = _scores.OrderByDescending(x => x.Score).FirstOrDefault(x => 
            x.Song.Artist.Equals(artist, StringComparison.InvariantCultureIgnoreCase) &&
            x.Song.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase));

        return new CompleteSong(artist, title, song?.TimesPlayed, song?.Rating, highscore?.Score, highscore?.Player, highscore?.Date);
    }
    
    private List<CompleteSong> CrawlDirectory(string path)
    {
        return Directory.GetFiles(path).Where(file => file.EndsWith(".txt")).Select(Parse).Where(song => song != null).ToList()!;
    }

    private List<CompleteSong> Crawl(string path, int depth = 0)
    {
        if (depth > 5)
            throw new Exception("Max recurse depth reached!");
        
        var subDirectories = Directory.GetDirectories(path).Where(x => !Path.GetFileName(x).StartsWith("."));
        var crawledMainDirectory = CrawlDirectory(path);
        var crawledSubdirectories = subDirectories.SelectMany(p => Crawl(p, depth+1));
        return crawledSubdirectories.Concat(crawledMainDirectory).ToList();
    }

    public List<CompleteSong> Crawl() => Crawl(_songPath);
}
