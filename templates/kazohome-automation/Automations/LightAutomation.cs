using System.Reactive.Linq;

namespace KazoHomeAutomation.Automations;

/// <summary>
/// Example automation that responds to motion sensor to control lights
/// </summary>
// TODO: Use [Automation] attribute from Source Generator when available
// [Automation("Light Automation")]
// [Trigger(Entity = "binary_sensor.motion", State = "on")]
public class LightAutomation : BackgroundService
{
    private readonly ILogger<LightAutomation> _logger;
    // private readonly IKazoHomeContext _context;
    private IDisposable? _subscription;

    public LightAutomation(ILogger<LightAutomation> logger)
    {
        _logger = logger;
        // _context = context;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Light automation started");

        // TODO: Subscribe to entity state changes when SDK is available
        // _subscription = _context.Entity("binary_sensor.motion")
        //     .StateChanges()
        //     .Where(s => s.New?.State == "on")
        //     .Throttle(TimeSpan.FromSeconds(30))
        //     .Subscribe(async s =>
        //     {
        //         _logger.LogInformation("Motion detected, turning on lights");
        //         await TurnOnLightsAsync();
        //     });

        return Task.CompletedTask;
    }

    private async Task TurnOnLightsAsync()
    {
        // TODO: Call service when SDK is available
        // _context.CallService("light", "turn_on", 
        //     new ServiceTarget { EntityIds = ["light.living_room"] },
        //     new { brightness = 255 });
        
        _logger.LogInformation("Lights turned on");
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription?.Dispose();
        await base.StopAsync(cancellationToken);
    }
}
