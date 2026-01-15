namespace KazoHome.SDK.Core.Abstractions;

/// <summary>
/// Interface for cross-project messaging in KazoHome.SDK
/// </summary>
public interface IKazoMessageBus
{
    /// <summary>
    /// Publishes a message to all subscribers
    /// </summary>
    void Publish<TMessage>(TMessage message) where TMessage : class;

    /// <summary>
    /// Publishes a message to a specific channel
    /// </summary>
    void Publish<TMessage>(string channel, TMessage message) where TMessage : class;

    /// <summary>
    /// Subscribes to messages of a specific type
    /// </summary>
    IObservable<TMessage> Subscribe<TMessage>() where TMessage : class;

    /// <summary>
    /// Subscribes to messages on a specific channel
    /// </summary>
    IObservable<TMessage> Subscribe<TMessage>(string channel) where TMessage : class;

    /// <summary>
    /// Sends a request and waits for a response
    /// </summary>
    Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class;
}

/// <summary>
/// Configuration for the message bus
/// </summary>
public record MessageBusOptions
{
    /// <summary>
    /// Whether to persist messages for late subscribers
    /// </summary>
    public bool ReplayLastMessage { get; init; } = false;

    /// <summary>
    /// Number of messages to buffer for slow subscribers
    /// </summary>
    public int BufferSize { get; init; } = 100;
}
