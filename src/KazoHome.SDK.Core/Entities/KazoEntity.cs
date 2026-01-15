namespace KazoHome.SDK.Core.Entities;

/// <summary>
/// Represents an entity in Home Assistant
/// </summary>
public class KazoEntity
{
    /// <summary>
    /// The entity ID
    /// </summary>
    public required string EntityId { get; init; }

    /// <summary>
    /// The context for interacting with Home Assistant
    /// </summary>
    protected IKazoHomeContext Context { get; }

    /// <summary>
    /// Creates a new entity
    /// </summary>
    public KazoEntity(IKazoHomeContext context, string entityId)
    {
        Context = context;
        EntityId = entityId;
    }

    /// <summary>
    /// Gets the current state of the entity
    /// </summary>
    public virtual KazoEntityState? State => Context.GetState(EntityId);

    /// <summary>
    /// Observes all state changes for this entity
    /// </summary>
    public IObservable<KazoStateChange> StateAllChanges() =>
        Context.StateAllChanges().Where(s => s.EntityId == EntityId);

    /// <summary>
    /// Observes state changes where the state value changed
    /// </summary>
    public IObservable<KazoStateChange> StateChanges() =>
        StateAllChanges().Where(s => s.Old?.State != s.New?.State);

    /// <summary>
    /// Calls a service on this entity
    /// </summary>
    public void CallService(string domain, string service, object? data = null)
    {
        Context.CallService(domain, service, new KazoServiceTarget { EntityIds = [EntityId] }, data);
    }
}

/// <summary>
/// Represents the state of an entity
/// </summary>
public record KazoEntityState
{
    /// <summary>
    /// The entity ID
    /// </summary>
    public required string EntityId { get; init; }

    /// <summary>
    /// The state value
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    /// The attributes as a JSON element
    /// </summary>
    public JsonElement? AttributesElement { get; init; }

    /// <summary>
    /// The last changed time
    /// </summary>
    public DateTimeOffset? LastChanged { get; init; }

    /// <summary>
    /// The last updated time
    /// </summary>
    public DateTimeOffset? LastUpdated { get; init; }

    /// <summary>
    /// Gets an attribute value
    /// </summary>
    public T? GetAttribute<T>(string name)
    {
        if (AttributesElement?.TryGetProperty(name, out var value) == true)
        {
            return value.Deserialize<T>();
        }
        return default;
    }
}
