namespace KazoHome.SDK.Core.Events;

/// <summary>
/// Represents an event from Home Assistant
/// </summary>
public record KazoEvent
{
    /// <summary>
    /// The type of event
    /// </summary>
    public required string EventType { get; init; }

    /// <summary>
    /// The time origin of the event
    /// </summary>
    public DateTimeOffset TimeOrigin { get; init; }

    /// <summary>
    /// The raw data element
    /// </summary>
    public JsonElement? DataElement { get; init; }

    /// <summary>
    /// Deserializes the data to a specific type
    /// </summary>
    public T? GetData<T>() where T : class
    {
        return DataElement?.Deserialize<T>();
    }
}

/// <summary>
/// Represents a state change event
/// </summary>
public record KazoStateChange
{
    /// <summary>
    /// The entity ID that changed
    /// </summary>
    public required string EntityId { get; init; }

    /// <summary>
    /// The old state
    /// </summary>
    public KazoEntityState? Old { get; init; }

    /// <summary>
    /// The new state
    /// </summary>
    public KazoEntityState? New { get; init; }
}
