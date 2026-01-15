namespace KazoHome.SDK.Automation.Attributes;

/// <summary>
/// Marks a class as a KazoHome automation
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AutomationAttribute : Attribute
{
    /// <summary>
    /// The display name of the automation
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Optional description of what the automation does
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the automation is enabled by default
    /// </summary>
    public bool EnabledByDefault { get; set; } = true;

    public AutomationAttribute(string name)
    {
        Name = name;
    }
}

/// <summary>
/// Defines a trigger for the automation based on entity state changes
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class TriggerAttribute : Attribute
{
    /// <summary>
    /// The entity ID to watch (e.g., "binary_sensor.motion")
    /// </summary>
    public string? Entity { get; set; }

    /// <summary>
    /// The state value that triggers the automation
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// The previous state value (for from -> to transitions)
    /// </summary>
    public string? FromState { get; set; }

    /// <summary>
    /// Time-based trigger in HH:mm format
    /// </summary>
    public string? Time { get; set; }

    /// <summary>
    /// Event type trigger
    /// </summary>
    public string? Event { get; set; }

    /// <summary>
    /// Debounce time in seconds
    /// </summary>
    public int DebounceSeconds { get; set; } = 0;
}

/// <summary>
/// Defines a condition that must be met for the automation to run
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class ConditionAttribute : Attribute
{
    /// <summary>
    /// The entity ID to check
    /// </summary>
    public string? Entity { get; set; }

    /// <summary>
    /// The state value that must match
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Numeric comparison operator (gt, lt, eq, gte, lte)
    /// </summary>
    public string? NumericOp { get; set; }

    /// <summary>
    /// Numeric value for comparison
    /// </summary>
    public double NumericValue { get; set; }

    /// <summary>
    /// Time range start (HH:mm)
    /// </summary>
    public string? After { get; set; }

    /// <summary>
    /// Time range end (HH:mm)
    /// </summary>
    public string? Before { get; set; }
}

/// <summary>
/// Marks a method as the action to execute when triggered
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ActionAttribute : Attribute
{
    /// <summary>
    /// Optional delay before executing the action
    /// </summary>
    public int DelaySeconds { get; set; } = 0;
}
