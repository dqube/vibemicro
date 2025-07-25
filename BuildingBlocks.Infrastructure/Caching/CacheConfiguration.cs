namespace BuildingBlocks.Infrastructure.Caching;

/// <summary>
/// Configuration for cache services
/// </summary>
public class CacheConfiguration
{
    /// <summary>
    /// Gets or sets the default cache expiration time
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets or sets the cache key prefix
    /// </summary>
    public string KeyPrefix { get; set; } = "BuildingBlocks:";

    /// <summary>
    /// Gets or sets whether to use sliding expiration
    /// </summary>
    public bool UseSlidingExpiration { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum cache size (for memory cache)
    /// </summary>
    public long? MaxSize { get; set; }

    /// <summary>
    /// Gets or sets the compression threshold (for distributed cache)
    /// </summary>
    public int CompressionThreshold { get; set; } = 1024;

    /// <summary>
    /// Gets or sets whether to compress cached values
    /// </summary>
    public bool UseCompression { get; set; } = false;

    /// <summary>
    /// Gets or sets the Redis configuration (if using Redis)
    /// </summary>
    public RedisCacheConfiguration? Redis { get; set; }

    /// <summary>
    /// Gets or sets the distributed cache configuration
    /// </summary>
    public DistributedCacheConfiguration? Distributed { get; set; }
}

/// <summary>
/// Redis cache configuration
/// </summary>
public class RedisCacheConfiguration
{
    /// <summary>
    /// Gets or sets the Redis connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Redis database number
    /// </summary>
    public int Database { get; set; } = 0;

    /// <summary>
    /// Gets or sets the key prefix for Redis
    /// </summary>
    public string KeyPrefix { get; set; } = "BuildingBlocks:";

    /// <summary>
    /// Gets or sets the default expiration for Redis cache
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets whether to use Redis clustering
    /// </summary>
    public bool UseClustering { get; set; } = false;

    /// <summary>
    /// Gets or sets the Redis cluster endpoints
    /// </summary>
    public string[] ClusterEndpoints { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the connection timeout
    /// </summary>
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets the command timeout
    /// </summary>
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets the retry count
    /// </summary>
    public int RetryCount { get; set; } = 3;
}

/// <summary>
/// Distributed cache configuration
/// </summary>
public class DistributedCacheConfiguration
{
    /// <summary>
    /// Gets or sets the cache provider type
    /// </summary>
    public DistributedCacheProvider Provider { get; set; } = DistributedCacheProvider.Memory;

    /// <summary>
    /// Gets or sets the connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default expiration
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets or sets the sliding expiration
    /// </summary>
    public TimeSpan? SlidingExpiration { get; set; }

    /// <summary>
    /// Gets or sets whether to use compression
    /// </summary>
    public bool UseCompression { get; set; } = false;

    /// <summary>
    /// Gets or sets the compression threshold
    /// </summary>
    public int CompressionThreshold { get; set; } = 1024;
}

/// <summary>
/// Distributed cache provider types
/// </summary>
public enum DistributedCacheProvider
{
    /// <summary>
    /// Memory cache provider
    /// </summary>
    Memory,

    /// <summary>
    /// Redis cache provider
    /// </summary>
    Redis,

    /// <summary>
    /// SQL Server cache provider
    /// </summary>
    SqlServer
} 