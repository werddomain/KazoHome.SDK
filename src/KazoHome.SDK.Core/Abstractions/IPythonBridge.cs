namespace KazoHome.SDK.Core.Abstractions;

/// <summary>
/// Interface for the Python Bridge communication layer
/// </summary>
public interface IPythonBridge : IAsyncDisposable
{
    /// <summary>
    /// Indicates whether the bridge is connected
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Connects to the Python proxy
    /// </summary>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects from the Python proxy
    /// </summary>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request to the Python proxy and waits for a response
    /// </summary>
    Task<TResponse> SendRequestAsync<TRequest, TResponse>(string method, TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification to the Python proxy (fire and forget)
    /// </summary>
    Task SendNotificationAsync<TRequest>(string method, TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to events from the Python proxy
    /// </summary>
    IObservable<TEvent> SubscribeToEvents<TEvent>(string eventType);
}

/// <summary>
/// Configuration options for the Python Bridge
/// </summary>
public record PythonBridgeOptions
{
    /// <summary>
    /// The communication type to use
    /// </summary>
    public BridgeCommunicationType CommunicationType { get; init; } = BridgeCommunicationType.UnixDomainSocket;

    /// <summary>
    /// The path to the Unix Domain Socket (when using UDS)
    /// </summary>
    public string SocketPath { get; init; } = "/tmp/kazohome_bridge.sock";

    /// <summary>
    /// The gRPC server address (when using gRPC)
    /// </summary>
    public string GrpcAddress { get; init; } = "http://localhost:50051";

    /// <summary>
    /// Connection timeout in milliseconds
    /// </summary>
    public int ConnectionTimeoutMs { get; init; } = 30000;

    /// <summary>
    /// Request timeout in milliseconds
    /// </summary>
    public int RequestTimeoutMs { get; init; } = 10000;
}

/// <summary>
/// The type of communication to use for the Python Bridge
/// </summary>
public enum BridgeCommunicationType
{
    /// <summary>
    /// Use Unix Domain Sockets for communication (recommended for local deployment)
    /// </summary>
    UnixDomainSocket,

    /// <summary>
    /// Use gRPC for communication (recommended for remote debugging)
    /// </summary>
    Grpc
}
