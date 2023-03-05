using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using UsDbDownloader.Data;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace UsDbDownloader.Services;

public partial class UsDbService
{
    private readonly ILogger<UsDbService> _logger;
    private const string BaseUrl = "https://usdb.animux.de";
    private const string SessionCookieName = "PHPSESSID";
    private HttpClient _httpClient = new(new HttpClientHandler()
    {
        UseCookies = true
    });

    public IEnumerable<UsDbSong> Songs { get; private set; } = new List<UsDbSong>();

    public UsDbService(IOptions<UsDbLoginModel> login, ILogger<UsDbService> logger)
    {
        _logger = logger;
        Task.Run(async () =>
        {
            await Login(login.Value.User, login.Value.Password);
            await ScanSongs();
        });
    }

    private async Task<bool> Login(string user, string password)
    {
        var response = await _httpClient.PostAsync(BaseUrl, new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("user", user),
            new KeyValuePair<string, string>("pass", password),
            new KeyValuePair<string, string>("login", "Login"),
        }));
        return response.IsSuccessStatusCode && (await response.Content.ReadAsStringAsync()).Contains($"<b>{user}</b>");
    }

    private async Task<HtmlNode> LoadAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        var content = new HtmlDocument();
        content.LoadHtml(await response.Content.ReadAsStringAsync());
        return content.DocumentNode;
    }

    private async Task<HtmlNode> PostAsync(string url, HttpContent content)
    {
        var response = await _httpClient.PostAsync(url, content);
        var htmlContent = new HtmlDocument();
        htmlContent.LoadHtml(await response.Content.ReadAsStringAsync());
        return htmlContent.DocumentNode;
    }

    public async Task<UsDbSong> GetDetails(UsDbSong song)
    {
        try
        {
            var songPage = await LoadAsync($"{BaseUrl}/index.php?link=detail&id={song.Id}");
            var baseContainer = songPage.SelectSingleNode("/html/body/table/tr/td[3]/body/table[1]/center/tr/td/table");
            var youtubeRegex = YoutubeUrlRegex();
            var songRatingCountRegex = SongRatingCountRegex();

            var commentNode = songPage.SelectSingleNode("/html/body/table/tr/td[3]/body/table[2]");
            var youtubeMatch =
                youtubeRegex.Match(commentNode.InnerHtml);

            var ratingCountMatch = songRatingCountRegex.Match(baseContainer.SelectSingleNode("tr[10]/td[2]").InnerText);
        
            song.Details = new UsDbSongDetails(
                youtubeMatch.Success ? youtubeMatch.Groups[1].Value : string.Empty,
                baseContainer.SelectSingleNode("tr[2]/td[2]/center/img").GetAttributeValue("src", string.Empty),
                double.Parse(baseContainer.SelectSingleNode("tr[3]/td[2]").InnerText.Replace(',', '.'), CultureInfo.InvariantCulture),
                double.Parse(baseContainer.SelectSingleNode("tr[4]/td[2]").InnerText.Replace(',', '.')),
                baseContainer.SelectSingleNode("tr[5]/td[2]").InnerText.Contains("yes", StringComparison.Ordinal),
                baseContainer.SelectSingleNode("tr[6]/td[2]").InnerText.Contains("yes", StringComparison.Ordinal),
                DateTime.ParseExact(baseContainer.SelectSingleNode("tr[7]/td[2]").InnerText, "dd.MM.yy - HH:mm", CultureInfo.InvariantCulture),
                baseContainer.SelectNodes("tr[10]/td[2]/img")?
                    .Count(x => x.GetAttributeValue("src", string.Empty).Contains("star.png")) ?? 0,
                ratingCountMatch.Success ? int.Parse(ratingCountMatch.Groups[1].Value) : 0,
                commentNode.InnerHtml
            );

        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to get details for \"{song.Artist} - {song.Title}\": {e}");
        }
        return song;
    }

    public async Task DownloadSong(UsDbSong song)
    {
        try
        {
            if (song.Details is null)
                song = await GetDetails(song);

            if (song.Details is null)
                throw new Exception("Failed to fetch song details!");
            
            var youtubeRegex = YoutubeVideoRegex();
            var videoRegex = VideoRegex();
            var audioRegex = AudioRegex();
            var coverRegex = CoverRegex();

            var name = $"{song.Artist} - {song.Title}";
            var directory = $"songs/{name}";
            Directory.CreateDirectory(directory);
            var txtPage = await PostAsync($"{BaseUrl}/index.php?link=gettxt&id={song.Id}", new FormUrlEncodedContent(new []{new KeyValuePair<string, string>("wd", "1")}));
            var baseContainer = txtPage.SelectSingleNode("/html/body/table/tr/td[3]/body/table[1]/center/tr/td/form/textarea");
            var txt = baseContainer.InnerText ?? string.Empty;
            if (youtubeRegex.Match(txt) is { } match && match.Success)
                song.Details.YoutubeId = match.Groups[1].Value;

            if ((await _httpClient.GetAsync($"https://usdb.animux.de/data/cover/{song.Id}.jpg")) is var coverResponse &&
                coverResponse.IsSuccessStatusCode)
            {
                await File.WriteAllBytesAsync($"{directory}/cover.jpg", await coverResponse.Content.ReadAsByteArrayAsync());
                txt = coverRegex.Match(txt).Success ? coverRegex.Replace(txt, $"#COVER:cover.jpg") : txt.Insert(0, $"#COVER:cover.jpg\n");
            }

            if (string.IsNullOrEmpty(song.Details.YoutubeId))
            {
                _logger.LogInformation("Skipping download for \"{song.Artitst} - {song.Title}\" because no audio+video was found");
                return;
            }

            txt = audioRegex.Match(txt).Success
                ? audioRegex.Replace(txt, $"#MP3:{name}.mp3")
                : txt.Insert(0, $"#MP3:{name}.mp3\n");
            txt = videoRegex.Match(txt).Success
                ? videoRegex.Replace(txt, $"#VIDEO:{name}.mp4")
                : txt.Insert(0, $"#VIDEO:{name}.mp4\n");

            var client = new YoutubeExplode.YoutubeClient();
            var manifest = await client.Videos.Streams.GetManifestAsync(song.Details.YoutubeId);
            var mp4StreamInfo = manifest
                .GetVideoOnlyStreams()
                .Where(s => s.Container == Container.Mp4 && s.VideoResolution.Height <= 480)
                .GetWithHighestVideoQuality();
            var mp3StreamInfo = manifest.GetAudioOnlyStreams().Where(x => x.Container == Container.Mp4)
                .GetWithHighestBitrate();

            await client.Videos.Streams.DownloadAsync(mp4StreamInfo, $"{directory}/{name}.mp4");
            await client.Videos.DownloadAsync(new[] {mp3StreamInfo},
                new ConversionRequestBuilder($"{directory}/{name}.mp3").SetContainer(Container.Mp3).Build());

            await File.WriteAllTextAsync($"{directory}/{name}.txt", txt);
            await File.WriteAllTextAsync($"{directory}/comments.html", song.Details.Comments);
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to download \"{song.Artist} - {song.Title}\": {e}");
        }
    }

    private async Task ScanSongs()
    {
        var artistEntryHrefRegex = ArtistEntryHrefRegex();
        var songEntryRegex = SongEntryRegex();

        var pages = (await LoadAsync($"{BaseUrl}/index.php?link=byartist"))
            .SelectNodes("/html/body/table/tr/td[3]/body/table/center/tr/td/h1/a")
            .Select(x => x.GetAttributeValue("href", null));

        var tasks = pages.Select(page => Task.Run(async () =>
        {
            var baseContainer =
                (await LoadAsync($"{BaseUrl}/{page}")).SelectSingleNode(
                    "/html/body/table/tr/td[3]/body/table/center/tr/td");
            var artistMap = baseContainer
                .SelectNodes("a")
                .Where(x => x.GetAttributeValue("href", null)?.StartsWith("javascript:") ?? false)
                .Select(x =>
                    new Tuple<string, string>(
                        artistEntryHrefRegex.Match(x.GetAttributeValue("href", string.Empty)).Groups[1].Value,
                        x.InnerText));
            var songMap = baseContainer
                .SelectNodes("script")
                .Select(x => songEntryRegex.Matches(x.InnerText).Select(y =>
                    new Tuple<string, string, string>(y.Groups[1].Value, y.Groups[3].Value, y.Groups[2].Value)));
            return songMap.SelectMany(x => x.Select(y =>
                new UsDbSong(artistMap.First(z => z.Item1 == y.Item1).Item2, y.Item2,
                    int.Parse(y.Item3.Replace("?link=detail&id=", string.Empty)))));
        }));
        
        Songs = (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();
    }

    [GeneratedRegex("^javascript:show\\(\"(.+?)\"\\)$")]
    private static partial Regex ArtistEntryHrefRegex();

    [GeneratedRegex("\\$\\(\"(.+)\"\\)\\.innerHTML \\+= \"<a target='_blank' href='(.+?)'>(.+?)</a>")]
    private static partial Regex SongEntryRegex();

    [GeneratedRegex("src=\"https://www.youtube.com/embed/(.*?)\"")]
    private static partial Regex YoutubeUrlRegex();

    [GeneratedRegex(@"\((\d+?)\)")]
    private static partial Regex SongRatingCountRegex();
    

    [GeneratedRegex(@"^#VIDEO:\w=([\w\-]+)(?:,.*)?$", RegexOptions.Multiline)]
    private static partial Regex YoutubeVideoRegex();
    
    [GeneratedRegex(@"^#VIDEO:.+$", RegexOptions.Multiline)]
    private static partial Regex VideoRegex();
    
    [GeneratedRegex(@"^#MP3:.+$", RegexOptions.Multiline)]
    private static partial Regex AudioRegex();
    
    [GeneratedRegex(@"^#COVER:.+$", RegexOptions.Multiline)]
    private static partial Regex CoverRegex();
}
