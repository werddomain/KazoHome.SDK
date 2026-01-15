namespace KazoHomeIntegration;

/// <summary>
/// Main service that handles communication with the Python proxy
/// </summary>
public class IntegrationService : BackgroundService
{
    private readonly ILogger<IntegrationService> _logger;
    // private readonly IPythonBridge _bridge;

    public IntegrationService(ILogger<IntegrationService> logger)
    {
        _logger = logger;
        // _bridge = bridge;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("KazoHome Integration starting at: {time}", DateTimeOffset.Now);

        try
        {
            // TODO: Connect to Python proxy when SDK is available
            // await _bridge.ConnectAsync(stoppingToken);
            
            _logger.LogInformation("Connected to Home Assistant Python proxy");

            // TODO: Subscribe to events from HA
            // _bridge.SubscribeToEvents<StateChangedEvent>("ha.state_changed")
            //     .Subscribe(HandleStateChanged);

            while (!stoppingToken.IsCancellationRequested)
            {
                // Main processing loop
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Integration shutting down");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in integration service");
            throw;
        }
        finally
        {
            // TODO: Disconnect from bridge
            // await _bridge.DisconnectAsync();
        }
    }

    // Example event handler
    private void HandleStateChanged(object stateChange)
    {
        _logger.LogDebug("State changed: {state}", stateChange);
        // Process state change...
    }
}

// Example strongly-typed event
public record StateChangedEvent(
    string EntityId,
    string? OldState,
    string? NewState,
    Dictionary<string, object>? Attributes
);
