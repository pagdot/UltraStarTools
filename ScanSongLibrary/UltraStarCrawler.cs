using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared;

namespace ScanSongLibrary;

public static class UltraStarCrawler
{
    private static readonly string _artistPrefix = "#ARTIST:";
    private static readonly string _titlePrefix = "#TITLE:";
    private static readonly string _coverPrefix = "#COVER:";
    private static readonly string _backgroundPrefix = "#BACKGROUND:";
    private static readonly string _videoPrefix = "#VIDEO:";
    
    private static UsToolsSong? Parse(string path)
    {
        var lines = File.ReadLines(path).ToArray();
        var directory = Directory.GetParent(path)!.FullName;

        if (lines.FirstOrDefault(l => l.StartsWith(_artistPrefix))?.Substring(_artistPrefix.Length) is var artist && artist is null)
            return null;
        
        if (lines.FirstOrDefault(l => l.StartsWith(_titlePrefix))?.Substring(_titlePrefix.Length) is var title && title is null)
            return null;
        
        var cover = lines.FirstOrDefault(l => l.StartsWith(_coverPrefix))?.Substring(_coverPrefix.Length);
        var background = lines.FirstOrDefault(l => l.StartsWith(_backgroundPrefix))?.Substring(_backgroundPrefix.Length);
        var video = lines.FirstOrDefault(l => l.StartsWith(_videoPrefix))?.Substring(_videoPrefix.Length);

        return new UsToolsSong
        {
            Artist = artist,
            Title = title,
            CoverPath = cover?.Insert(0, directory) ?? string.Empty,
            BackgroundPath = background?.Insert(0, directory) ?? string.Empty,
            VideoPath = video?.Insert(0, directory) ?? string.Empty,
            TxtPath = path,
        };
    }
    
    private static List<UsToolsSong> CrawlDirectory(string path)
    {
        return Directory.GetFiles(path).Where(file => file.EndsWith(".txt")).Select(Parse).Where(song => song != null).ToList()!;
    }
    
    public static List<UsToolsSong> Crawl(string path)
    {
        return Directory.GetDirectories(path).SelectMany(p => Crawl(p)).Concat(CrawlDirectory(path)).ToList();
    }
}
