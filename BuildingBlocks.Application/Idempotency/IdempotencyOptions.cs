namespace BuildingBlocks.Application.Idempotency;

/// <summary>
/// Configuration options for idempotency service
/// </summary>
public class IdempotencyOptions
{
    /// <summary>
    /// Gets or sets the default expiry time for idempotency records
    /// </summary>
    public TimeSpan DefaultExpiry { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// Gets or sets the maximum number of concurrent operations for the same key
    /// </summary>
    public int MaxConcurrentOperations { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether to enable automatic cleanup of expired records
    /// </summary>
    public bool EnableAutoCleanup { get; set; } = true;

    /// <summary>
    /// Gets or sets the interval for automatic cleanup of expired records
    /// </summary>
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromHours(6);

    /// <summary>
    /// Gets or sets the prefix for idempotency keys
    /// </summary>
    public string KeyPrefix { get; set; } = "idempotency:";

    /// <summary>
    /// Gets or sets whether to include metadata in idempotency records
    /// </summary>
    public bool IncludeMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to compress stored results
    /// </summary>
    public bool CompressResults { get; set; } = false;

    /// <summary>
    /// Gets or sets the minimum size (in bytes) for compression
    /// </summary>
    public int CompressionThreshold { get; set; } = 1024;

    /// <summary>
    /// Gets or sets the timeout for acquiring locks on idempotency keys
    /// </summary>
    public TimeSpan LockTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets whether to retry operations on lock timeout
    /// </summary>
    public bool RetryOnLockTimeout { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the delay between retry attempts
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(100);
} 