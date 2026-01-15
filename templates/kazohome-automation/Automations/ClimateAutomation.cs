using System.Reactive.Linq;

namespace KazoHomeAutomation.Automations;

/// <summary>
/// Example automation that adjusts climate based on time and presence
/// </summary>
// TODO: Use [Automation] attribute from Source Generator when available
// [Automation("Climate Automation")]
// [Trigger(Time = "06:00")]
// [Trigger(Entity = "binary_sensor.presence", State = "home")]
public class ClimateAutomation : BackgroundService
{
    private readonly ILogger<ClimateAutomation> _logger;
    // private readonly IKazoHomeContext _context;

    public ClimateAutomation(ILogger<ClimateAutomation> logger)
    {
        _logger = logger;
        // _context = context;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Climate automation started");

        // Example: Schedule-based automation
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;

            // Morning warm-up (6:00 AM)
            if (now.Hour == 6 && now.Minute == 0)
            {
                await SetClimateAsync("heat", 21);
            }
            // Daytime eco mode (9:00 AM)
            else if (now.Hour == 9 && now.Minute == 0)
            {
                await SetClimateAsync("auto", 19);
            }
            // Evening comfort (5:00 PM)
            else if (now.Hour == 17 && now.Minute == 0)
            {
                await SetClimateAsync("heat", 22);
            }
            // Night setback (10:00 PM)
            else if (now.Hour == 22 && now.Minute == 0)
            {
                await SetClimateAsync("heat", 18);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task SetClimateAsync(string hvacMode, int temperature)
    {
        // TODO: Call service when SDK is available
        // _context.CallService("climate", "set_hvac_mode",
        //     new ServiceTarget { EntityIds = ["climate.thermostat"] },
        //     new { hvac_mode = hvacMode });
        // 
        // _context.CallService("climate", "set_temperature",
        //     new ServiceTarget { EntityIds = ["climate.thermostat"] },
        //     new { temperature = temperature });

        _logger.LogInformation("Climate set to {Mode} at {Temp}°C", hvacMode, temperature);
        await Task.CompletedTask;
    }
}
