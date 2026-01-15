namespace KazoHome.SDK.Bridge.Protocol;

/// <summary>
/// Message envelope for bridge communication
/// </summary>
public record BridgeMessage
{
    /// <summary>
    /// Unique message ID for request/response correlation
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Message type: request, response, notification, event
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Method name for requests
    /// </summary>
    public string? Method { get; init; }

    /// <summary>
    /// Payload data
    /// </summary>
    public JsonElement? Payload { get; init; }

    /// <summary>
    /// Error information for failed requests
    /// </summary>
    public BridgeError? Error { get; init; }

    /// <summary>
    /// Timestamp of the message
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Error information in bridge messages
/// </summary>
public record BridgeError
{
    /// <summary>
    /// Error code
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Error message
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Additional error data
    /// </summary>
    public JsonElement? Data { get; init; }
}

/// <summary>
/// Message type constants
/// </summary>
public static class BridgeMessageTypes
{
    public const string Request = "request";
    public const string Response = "response";
    public const string Notification = "notification";
    public const string Event = "event";
}
