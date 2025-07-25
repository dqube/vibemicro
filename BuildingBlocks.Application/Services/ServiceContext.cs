namespace BuildingBlocks.Application.Services;

/// <summary>
/// Default implementation of service execution context
/// </summary>
public class ServiceContext : IServiceContext
{
    private readonly Dictionary<string, object> _properties = new();

    /// <summary>
    /// Gets the user identifier
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Gets the user name
    /// </summary>
    public string? UserName { get; init; }

    /// <summary>
    /// Gets the tenant identifier
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    /// Gets the correlation identifier
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Gets the trace identifier
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// Gets the timestamp when the context was created
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets additional context properties
    /// </summary>
    public IReadOnlyDictionary<string, object> Properties => _properties.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the ServiceContext class
    /// </summary>
    public ServiceContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ServiceContext class with properties
    /// </summary>
    /// <param name="properties">Initial properties</param>
    public ServiceContext(IDictionary<string, object>? properties)
    {
        if (properties != null)
        {
            foreach (var kvp in properties)
            {
                _properties[kvp.Key] = kvp.Value;
            }
        }
    }

    /// <summary>
    /// Gets a property value by key
    /// </summary>
    /// <typeparam name="T">The type of the property value</typeparam>
    /// <param name="key">The property key</param>
    /// <returns>The property value if found, default otherwise</returns>
    public T? GetProperty<T>(string key)
    {
        if (string.IsNullOrEmpty(key) || !_properties.TryGetValue(key, out var value))
            return default;

        if (value is T typedValue)
            return typedValue;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Checks if a property exists
    /// </summary>
    /// <param name="key">The property key</param>
    /// <returns>True if the property exists</returns>
    public bool HasProperty(string key)
    {
        return !string.IsNullOrEmpty(key) && _properties.ContainsKey(key);
    }

    /// <summary>
    /// Sets a property value
    /// </summary>
    /// <param name="key">The property key</param>
    /// <param name="value">The property value</param>
    public void SetProperty(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Property key cannot be null or empty", nameof(key));

        _properties[key] = value;
    }

    /// <summary>
    /// Removes a property
    /// </summary>
    /// <param name="key">The property key</param>
    /// <returns>True if the property was removed</returns>
    public bool RemoveProperty(string key)
    {
        return !string.IsNullOrEmpty(key) && _properties.Remove(key);
    }

    /// <summary>
    /// Creates a new service context with the specified user information
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="userName">The user name</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="correlationId">The correlation identifier</param>
    /// <param name="traceId">The trace identifier</param>
    /// <returns>A new service context</returns>
    public static ServiceContext Create(
        string? userId = null, 
        string? userName = null, 
        string? tenantId = null, 
        string? correlationId = null, 
        string? traceId = null)
    {
        return new ServiceContext
        {
            UserId = userId,
            UserName = userName,
            TenantId = tenantId,
            CorrelationId = correlationId,
            TraceId = traceId
        };
    }
} 