namespace BuildingBlocks.Application.Idempotency;

/// <summary>
/// Represents an idempotency record for tracking executed operations
/// </summary>
public class IdempotencyRecord
{
    /// <summary>
    /// Gets or sets the idempotency key
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serialized result of the operation
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// Gets or sets the type of the result
    /// </summary>
    public string? ResultType { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the operation was executed
    /// </summary>
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the expiry date and time for this record
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets additional metadata about the operation
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets whether the result is compressed
    /// </summary>
    public bool IsCompressed { get; set; } = false;

    /// <summary>
    /// Gets or sets the hash of the operation parameters (for additional validation)
    /// </summary>
    public string? ParametersHash { get; set; }

    /// <summary>
    /// Gets or sets the user or service that executed the operation
    /// </summary>
    public string? ExecutedBy { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier for tracking related operations
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Checks if this record has expired
    /// </summary>
    /// <returns>True if the record has expired</returns>
    public bool IsExpired()
    {
        return ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the age of this record
    /// </summary>
    /// <returns>The age of the record</returns>
    public TimeSpan GetAge()
    {
        return DateTime.UtcNow - ExecutedAt;
    }

    /// <summary>
    /// Gets the time until expiry
    /// </summary>
    /// <returns>The time until expiry, or null if no expiry is set</returns>
    public TimeSpan? GetTimeUntilExpiry()
    {
        if (!ExpiresAt.HasValue)
            return null;

        var timeUntilExpiry = ExpiresAt.Value - DateTime.UtcNow;
        return timeUntilExpiry > TimeSpan.Zero ? timeUntilExpiry : TimeSpan.Zero;
    }

    /// <summary>
    /// Sets the expiry time for this record
    /// </summary>
    /// <param name="expiry">The expiry duration from now</param>
    public void SetExpiry(TimeSpan expiry)
    {
        ExpiresAt = DateTime.UtcNow.Add(expiry);
    }

    /// <summary>
    /// Adds metadata to this record
    /// </summary>
    /// <param name="key">The metadata key</param>
    /// <param name="value">The metadata value</param>
    public void AddMetadata(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        Metadata[key] = value ?? string.Empty;
    }

    /// <summary>
    /// Gets metadata from this record
    /// </summary>
    /// <param name="key">The metadata key</param>
    /// <returns>The metadata value or null if not found</returns>
    public string? GetMetadata(string key)
    {
        return Metadata.TryGetValue(key, out var value) ? value : null;
    }
} 