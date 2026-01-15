namespace KazoHomeAddon;

/// <summary>
/// Background worker for the Home Assistant addon
/// </summary>
public class AddonWorker : BackgroundService
{
    private readonly ILogger<AddonWorker> _logger;

    public AddonWorker(ILogger<AddonWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("KazoHome Addon starting at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            // TODO: Add your addon logic here
            // Example: Subscribe to Home Assistant events, call services, etc.

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
