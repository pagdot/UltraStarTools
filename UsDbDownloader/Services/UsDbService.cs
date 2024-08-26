using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Shared;
using UsDbDownloader.Data;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace UsDbDownloader.Services;

public partial class UsDbService
{
    private readonly IOptions<UsDbLoginModel> _login;
    private readonly ILogger<UsDbService> _logger;
    private readonly List<CompleteSong> _availableSongs;
    private const string BaseUrl = "https://usdb.animux.de";
    private HttpClient _httpClient = new(new HttpClientHandler()
    {
        UseCookies = true
    });
    private static readonly Option<string>[] _customOptions = new Option<string>[] { new("--username") { Value = "oauth2" }, new("--password") { Value = "" } };
    
    private readonly string _destination;

    public IEnumerable<UsDbSong> Songs { get; private set; } = new List<UsDbSong>();

    public UsDbService(IOptions<UsDbLoginModel> login, IOptions<SettingsModel> settings, ILogger<UsDbService> logger, List<CompleteSong> songs)
    {
        _login = login;
        _logger = logger;
        _destination = settings.Value.Destination;
        _availableSongs = songs;
    }

    private static OptionSet GetYoutubeDlVideoOptions(string path) => new OptionSet()
    {
        FormatSort = "res:480,+size",
        Format = "mp4",
        CustomOptions = _customOptions
    };

    private static OptionSet GetYoutubeDlAudioOptions(string path) => new OptionSet()
    {
        FormatSort = "+size",
        Format = "mp3",
        CustomOptions = _customOptions,
        Output = path
    };

    private async Task<bool> IsLoggedIn(HttpResponseMessage response) => response.IsSuccessStatusCode && (await response.Content.ReadAsStringAsync()).Contains($"<b>{_login.Value.User}</b>");

    public async Task<bool> Login()
    {
        var response = await _httpClient.PostAsync(BaseUrl, new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("user", _login.Value.User),
            new KeyValuePair<string, string>("pass", _login.Value.Password),
            new KeyValuePair<string, string>("login", "Login"),
        }));
        return await IsLoggedIn(response);
    }

    private async Task<HtmlNode> LoadAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        if (!await IsLoggedIn(response))
        {
            await Login();
            response = await _httpClient.GetAsync(url);
        }

        var content = new HtmlDocument();
        content.LoadHtml(await response.Content.ReadAsStringAsync());
        return content.DocumentNode;
    }

    private async Task<HtmlNode> PostAsync(string url, HttpContent content)
    {
        var response = await _httpClient.PostAsync(url, content);
        if (!await IsLoggedIn(response))
        {
            await Login();
            response = await _httpClient.PostAsync(url, content);
        }
        
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

            var imageSrc = baseContainer.SelectSingleNode("tr[2]/td[2]/center/img").GetAttributeValue("src", string.Empty);
            var language = baseContainer.SelectSingleNode("tr[3]/td[2]").InnerText;

            var commentNode = songPage.SelectSingleNode("/html/body/table/tr/td[3]/body/table[2]");
            var youtubeMatch =
                youtubeRegex.Match(commentNode.InnerHtml);

            var bpmString = baseContainer.SelectSingleNode("tr[7]/td[2]").InnerText.Replace(',', '.');
            var gapString = baseContainer.SelectSingleNode("tr[8]/td[2]").InnerText.Replace(',', '.');
            var hasGoldenNotes = baseContainer.SelectSingleNode("tr[9]/td[2]").InnerText.Contains("yes", StringComparison.Ordinal);
            var hasSongCheck = baseContainer.SelectSingleNode("tr[10]/td[2]").InnerText.Contains("yes", StringComparison.Ordinal);
            var date = DateTime.ParseExact(baseContainer.SelectSingleNode("tr[11]/td[2]").InnerText, "dd.MM.yy - HH:mm", CultureInfo.InvariantCulture);
            var rating = baseContainer.SelectNodes("tr[14]/td[2]/img")?
                    .Count(x => x.GetAttributeValue("src", string.Empty).Contains("star.png")) ?? 0;
            var ratingCountMatch = songRatingCountRegex.Match(baseContainer.SelectSingleNode("tr[14]/td[2]").InnerText);
        
            song.Details = new UsDbSongDetails(
                youtubeMatch.Success ? youtubeMatch.Groups[1].Value : string.Empty,
                imageSrc,
                string.IsNullOrWhiteSpace(bpmString) ? 0.0 : double.Parse(bpmString, CultureInfo.InvariantCulture),
                string.IsNullOrWhiteSpace(gapString) ? 0.0 : double.Parse(gapString, CultureInfo.InvariantCulture),
                hasGoldenNotes,
                hasSongCheck,
                date,
                rating,
                ratingCountMatch.Success ? int.Parse(ratingCountMatch.Groups[1].Value) : 0,
                commentNode.InnerHtml,
                language
            );

        }
        catch (Exception e)
        {
            throw new Exception($"Failed to get details for \"{song.Artist} - {song.Title}\"", e);
        }

        _logger.LogInformation($"Downloaded details for {song.Artist} - {song.Title}");

        return song;
    }

    public async Task<bool> DownloadSong(UsDbSong song)
    {
        try
        {
            if (song.Details is null)
                song = await GetDetails(song);

            if (song is null)
                throw new Exception("Failed to fetch song details!");
            
            var youtubeRegex = YoutubeVideoRegex();
            var videoRegex = VideoRegex();
            var audioRegex = AudioRegex();
            var coverRegex = CoverRegex();

            var name = $"{song.Artist} - {song.Title}";
            var directory = $"{_destination}/{name}";
            _logger.LogInformation($"Downloading {song.Artist} - {song.Title} to '{directory}'");
            Directory.CreateDirectory(directory);
            await Login();
            var txtPage = await PostAsync($"{BaseUrl}/index.php?link=gettxt&id={song.Id}", new FormUrlEncodedContent(new []{new KeyValuePair<string, string>("wd", "1")}));
            var baseContainer = txtPage.SelectSingleNode("/html/body/table/tr/td[3]/body/table[1]/center/tr/td/form/textarea") ?? txtPage.SelectSingleNode("/html/body/table/tr/td[3]/body/table[1]/center/tr/td/textarea");
            var txt = baseContainer?.InnerText ?? string.Empty;
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
                return false;
            }

            txt = audioRegex.Match(txt).Success
                ? audioRegex.Replace(txt, $"#MP3:{name}.mp3")
                : txt.Insert(0, $"#MP3:{name}.mp3\n");
            txt = videoRegex.Match(txt).Success
                ? videoRegex.Replace(txt, $"#VIDEO:{name}.mp4")
                : txt.Insert(0, $"#VIDEO:{name}.mp4\n");

            var client = new YoutubeDL();

            _logger.LogInformation("Starting Download of Video");
            var result = await client.RunVideoDownload($"https://www.youtube.com/watch?v={song.Details.YoutubeId}", overrideOptions: GetYoutubeDlVideoOptions($"{directory}/{name}.mp4"));
            if (!result.Success)
                throw new Exception($"Failed to download video:\n{result.ErrorOutput}");

            _logger.LogInformation("Video download finished");
            _logger.LogInformation("Starting Download of MP3");
            result = await client.RunVideoDownload($"https://www.youtube.com/watch?v={song.Details.YoutubeId}", overrideOptions: GetYoutubeDlAudioOptions($"{directory}/{name}.mp4"));
            if (!result.Success)
                throw new Exception($"Failed to download audio:\n{result.ErrorOutput}");
            _logger.LogInformation("MP3 download finished");

            await File.WriteAllTextAsync($"{directory}/{name}.txt", txt);
            await File.WriteAllTextAsync($"{directory}/comments.html", song.Details.Comments);
            song.IsAlreadyDownloaded = true;
            _logger.LogInformation($"Finished downloading {song.Artist} - {song.Title}");
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to download \"{song.Artist} - {song.Title}\": {e}");
            return false;
        }
        return true;
    }

    public async Task ScanSongs()
    {
        var downloadedSongs = UltraStarCrawler.Crawl(_destination);

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
            {
                var artist = artistMap.First(z => z.Item1 == y.Item1).Item2;
                var isAlreadyAvailable = _availableSongs.Any(x => x.Artist == artist && x.Title == y.Item2);
                var isAlreadyDownloaded = downloadedSongs.Any(x => x.Artist == artist && x.Title == y.Item2);
                return new UsDbSong(artist, y.Item2,
                    int.Parse(y.Item3.Replace("?link=detail&id=", string.Empty)), isAlreadyDownloaded, isAlreadyAvailable);
            }));
        }));
        
        Songs = (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();
        
        _logger.LogInformation($"Scraped basic info about {Songs.Count()} songs");
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
