namespace BuildingBlocks.Application.Services;

/// <summary>
/// Interface for service execution context
/// </summary>
public interface IServiceContext
{
    /// <summary>
    /// Gets the user identifier
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the user name
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Gets the tenant identifier
    /// </summary>
    string? TenantId { get; }

    /// <summary>
    /// Gets the correlation identifier
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Gets the trace identifier
    /// </summary>
    string? TraceId { get; }

    /// <summary>
    /// Gets the timestamp when the context was created
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets additional context properties
    /// </summary>
    IReadOnlyDictionary<string, object> Properties { get; }

    /// <summary>
    /// Gets a property value by key
    /// </summary>
    /// <typeparam name="T">The type of the property value</typeparam>
    /// <param name="key">The property key</param>
    /// <returns>The property value if found, default otherwise</returns>
    T? GetProperty<T>(string key);

    /// <summary>
    /// Checks if a property exists
    /// </summary>
    /// <param name="key">The property key</param>
    /// <returns>True if the property exists</returns>
    bool HasProperty(string key);
} 