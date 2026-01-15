namespace KazoHome.SDK.Core.Abstractions;

/// <summary>
/// Represents a context for interacting with Home Assistant through KazoHome.SDK
/// </summary>
public interface IKazoHomeContext
{
    /// <summary>
    /// All Events from Home Assistant
    /// </summary>
    IObservable<KazoEvent> Events { get; }

    /// <summary>
    /// The observable state stream, all changes including attributes
    /// </summary>
    IObservable<KazoStateChange> StateAllChanges();

    /// <summary>
    /// Get state for a single entity
    /// </summary>
    KazoEntityState? GetState(string entityId);

    /// <summary>
    /// Gets all the entities in Home Assistant
    /// </summary>
    IReadOnlyList<KazoEntity> GetAllEntities();

    /// <summary>
    /// Calls a service in Home Assistant
    /// </summary>
    void CallService(string domain, string service, KazoServiceTarget? target = null, object? data = null);

    /// <summary>
    /// Calls a service that returns a response
    /// </summary>
    Task<JsonElement?> CallServiceWithResponseAsync(string domain, string service, KazoServiceTarget? target = null, object? data = null);

    /// <summary>
    /// Sends an event to Home Assistant
    /// </summary>
    void SendEvent(string eventType, object? data = null);

    /// <summary>
    /// Gets the entity by ID
    /// </summary>
    KazoEntity Entity(string entityId);
}

/// <summary>
/// Represents a service target for Home Assistant service calls
/// </summary>
public record KazoServiceTarget
{
    /// <summary>
    /// Entity IDs to target
    /// </summary>
    public IReadOnlyList<string>? EntityIds { get; init; }

    /// <summary>
    /// Device IDs to target
    /// </summary>
    public IReadOnlyList<string>? DeviceIds { get; init; }

    /// <summary>
    /// Area IDs to target
    /// </summary>
    public IReadOnlyList<string>? AreaIds { get; init; }
}
