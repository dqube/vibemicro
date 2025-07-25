using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Caching;

/// <summary>
/// Memory cache service implementation
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly CacheConfiguration _configuration;
    private long _hits;
    private long _misses;

    /// <summary>
    /// Initializes a new instance of the MemoryCacheService class
    /// </summary>
    /// <param name="memoryCache">The memory cache</param>
    /// <param name="configuration">The cache configuration</param>
    /// <param name="logger">The logger</param>
    public MemoryCacheService(
        IMemoryCache memoryCache, 
        CacheConfiguration configuration, 
        ILogger<MemoryCacheService> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a value from the cache
    /// </summary>
    /// <typeparam name="T">The type of value</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The cached value if found, null otherwise</returns>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        var normalizedKey = NormalizeKey(key);

        if (_memoryCache.TryGetValue(normalizedKey, out var cachedValue))
        {
            Interlocked.Increment(ref _hits);
            _logger.LogDebug("Cache hit for key: {Key}", normalizedKey);

            if (cachedValue is T typedValue)
            {
                return Task.FromResult<T?>(typedValue);
            }

            if (cachedValue is string jsonValue && typeof(T) != typeof(string))
            {
                try
                {
                    var deserializedValue = JsonSerializer.Deserialize<T>(jsonValue);
                    return Task.FromResult(deserializedValue);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize cached value for key: {Key}", normalizedKey);
                }
            }
        }

        Interlocked.Increment(ref _misses);
        _logger.LogDebug("Cache miss for key: {Key}", normalizedKey);
        return Task.FromResult<T?>(default);
    }

    /// <summary>
    /// Sets a value in the cache
    /// </summary>
    /// <typeparam name="T">The type of value</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to cache</param>
    /// <param name="expiration">The expiration time</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        var normalizedKey = NormalizeKey(key);
        var cacheExpiration = expiration ?? _configuration.DefaultExpiration;

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheExpiration,
            Priority = CacheItemPriority.Normal,
            Size = 1
        };

        // Configure sliding expiration if enabled
        if (_configuration.UseSlidingExpiration)
        {
            cacheEntryOptions.SlidingExpiration = TimeSpan.FromMinutes(30);
        }

        object cacheValue = value!;

        // Serialize complex objects to JSON for consistency
        if (typeof(T) != typeof(string) && !typeof(T).IsValueType)
        {
            try
            {
                cacheValue = JsonSerializer.Serialize(value);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to serialize value for key: {Key}", normalizedKey);
                return Task.CompletedTask;
            }
        }

        _memoryCache.Set(normalizedKey, cacheValue, cacheEntryOptions);
        _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", normalizedKey, cacheExpiration);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes a value from the cache
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        var normalizedKey = NormalizeKey(key);
        _memoryCache.Remove(normalizedKey);
        _logger.LogDebug("Removed cached value for key: {Key}", normalizedKey);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes multiple values from the cache
    /// </summary>
    /// <param name="keys">The cache keys</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        foreach (var key in keys)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var normalizedKey = NormalizeKey(key);
                _memoryCache.Remove(normalizedKey);
            }
        }

        _logger.LogDebug("Removed multiple cached values");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if a key exists in the cache
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the key exists, false otherwise</returns>
    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            return Task.FromResult(false);

        var normalizedKey = NormalizeKey(key);
        var exists = _memoryCache.TryGetValue(normalizedKey, out _);
        return Task.FromResult(exists);
    }

    /// <summary>
    /// Gets or sets a value in the cache
    /// </summary>
    /// <typeparam name="T">The type of value</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="factory">The factory function to create the value if not cached</param>
    /// <param name="expiration">The expiration time</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The cached or newly created value</returns>
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var value = await factory();
        if (value != null)
        {
            await SetAsync(key, value, expiration, cancellationToken);
        }

        return value;
    }

    /// <summary>
    /// Clears all values from the cache
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        // MemoryCache doesn't have a built-in clear method
        // We would need to track keys or use a different approach
        _logger.LogWarning("Memory cache clear operation is not fully supported. Consider using a different cache provider for this functionality.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets cache statistics
    /// </summary>
    /// <returns>The cache statistics</returns>
    public Task<CacheStatistics> GetStatisticsAsync()
    {
        var statistics = new CacheStatistics
        {
            Hits = _hits,
            Misses = _misses,
            Count = 0 // MemoryCache doesn't expose count directly
        };

        return Task.FromResult(statistics);
    }

    /// <summary>
    /// Normalizes a cache key
    /// </summary>
    /// <param name="key">The original key</param>
    /// <returns>The normalized key</returns>
    private string NormalizeKey(string key)
    {
        return _configuration.KeyPrefix + key;
    }
} 