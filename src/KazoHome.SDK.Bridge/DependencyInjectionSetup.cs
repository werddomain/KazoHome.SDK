using Microsoft.Extensions.DependencyInjection;
using KazoHome.SDK.Bridge.Internal;

namespace KazoHome.SDK.Bridge;

/// <summary>
/// Extension methods for setting up the Python Bridge in DI
/// </summary>
public static class DependencyInjectionSetup
{
    /// <summary>
    /// Adds the Python Bridge services to the service collection
    /// </summary>
    public static IServiceCollection AddKazoHomeBridge(
        this IServiceCollection services,
        Action<PythonBridgeOptions>? configure = null)
    {
        services.Configure<PythonBridgeOptions>(options =>
        {
            configure?.Invoke(options);
        });

        services.AddSingleton<IPythonBridge, UnixDomainSocketBridge>();

        return services;
    }
}
