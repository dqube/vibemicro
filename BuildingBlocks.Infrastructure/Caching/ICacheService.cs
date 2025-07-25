namespace BuildingBlocks.Infrastructure.Caching;

/// <summary>
/// Interface for cache service
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a value from the cache
    /// </summary>
    /// <typeparam name="T">The type of value</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The cached value if found, null otherwise</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a value in the cache
    /// </summary>
    /// <typeparam name="T">The type of value</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to cache</param>
    /// <param name="expiration">The expiration time</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a value from the cache
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes multiple values from the cache
    /// </summary>
    /// <param name="keys">The cache keys</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a key exists in the cache
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the key exists, false otherwise</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or sets a value in the cache
    /// </summary>
    /// <typeparam name="T">The type of value</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="factory">The factory function to create the value if not cached</param>
    /// <param name="expiration">The expiration time</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The cached or newly created value</returns>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all values from the cache
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets cache statistics
    /// </summary>
    /// <returns>The cache statistics</returns>
    Task<CacheStatistics> GetStatisticsAsync();
}

/// <summary>
/// Cache statistics
/// </summary>
public class CacheStatistics
{
    /// <summary>
    /// Gets or sets the number of cache hits
    /// </summary>
    public long Hits { get; set; }

    /// <summary>
    /// Gets or sets the number of cache misses
    /// </summary>
    public long Misses { get; set; }

    /// <summary>
    /// Gets or sets the total number of items in the cache
    /// </summary>
    public long Count { get; set; }

    /// <summary>
    /// Gets the hit ratio
    /// </summary>
    public double HitRatio => Hits + Misses > 0 ? (double)Hits / (Hits + Misses) : 0;
} 