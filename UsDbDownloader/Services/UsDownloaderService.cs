namespace UsDbDownloader.Services;

public class UsDownloaderService : IHostedService
{
    private readonly UsDbService _usDbService;
    private readonly ILogger<UsDownloaderService> _logger;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public UsDownloaderService(UsDbService usDbService, ILogger<UsDownloaderService> logger)
    {
        _usDbService = usDbService;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            var random = new Random();
            await _usDbService.Login();
            await _usDbService.ScanSongs();

            foreach (var song in _usDbService.Songs)
            {
                if (song.Details is { })
                    continue;
                    
                await _usDbService.GetDetails(song);
                Thread.Sleep(random.Next(1000, 10000));
            }
        }, _cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}