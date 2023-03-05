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
            await _usDbService.Login();
            await _usDbService.ScanSongs();
        }, _cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}