namespace Shared;

public static class UltraStarCrawler
{
    private static readonly string _artistPrefix = "#ARTIST:";
    private static readonly string _titlePrefix = "#TITLE:";

    private static CrawledSong? Parse(string path)
    {
        var lines = File.ReadLines(path).ToArray();

        if (lines.FirstOrDefault(l => l.StartsWith(_artistPrefix))?.Substring(_artistPrefix.Length) is var artist && artist is null)
            return null;
        
        if (lines.FirstOrDefault(l => l.StartsWith(_titlePrefix))?.Substring(_titlePrefix.Length) is var title && title is null)
            return null;

        return new CrawledSong(artist, title);
    }
    
    private static List<CrawledSong> CrawlDirectory(string path)
    {
        return Directory.GetFiles(path).Where(file => file.EndsWith(".txt")).Select(Parse).Where(song => song != null).ToList()!;
    }

    private static List<CrawledSong> Crawl(string path, int depth)
    {
        if (depth > 5)
            throw new Exception("Max recurse depth reached!");
        
        var subDirectories = Directory.GetDirectories(path).Where(x => !Path.GetFileName(x).StartsWith("."));
        var crawledMainDirectory = CrawlDirectory(path);
        var crawledSubdirectories = subDirectories.SelectMany(p => Crawl(p, depth+1));
        return crawledSubdirectories.Concat(crawledMainDirectory).ToList();
    }

    public static List<CrawledSong> Crawl(string path) => Crawl(path, 0);
}
