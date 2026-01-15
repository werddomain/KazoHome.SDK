namespace KazoHome.SDK.Core.Persistence;

/// <summary>
/// Interface for persisting data in Home Assistant config directory
/// </summary>
public interface IKazoPersistence
{
    /// <summary>
    /// Saves data to a JSON file
    /// </summary>
    Task SaveAsync<T>(string key, T data, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Loads data from a JSON file
    /// </summary>
    Task<T?> LoadAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Deletes a persisted file
    /// </summary>
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a key exists
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}

/// <summary>
/// Configuration for persistence
/// </summary>
public record PersistenceOptions
{
    /// <summary>
    /// The base path for persistence (defaults to /config for HA)
    /// </summary>
    public string BasePath { get; init; } = "/config/kazohome";

    /// <summary>
    /// Whether to use SQLite instead of JSON files
    /// </summary>
    public bool UseSqlite { get; init; } = false;

    /// <summary>
    /// The SQLite database file name
    /// </summary>
    public string SqliteFileName { get; init; } = "kazohome.db";
}
