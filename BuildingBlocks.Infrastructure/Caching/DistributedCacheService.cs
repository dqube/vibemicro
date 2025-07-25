using BuildingBlocks.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Caching;

/// <summary>
/// Distributed cache service implementation using IDistributedCache
/// </summary>
public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly CacheConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the DistributedCacheService class
    /// </summary>
    public DistributedCacheService(
        IDistributedCache distributedCache,
        ILogger<DistributedCacheService> logger,
        IOptions<CacheConfiguration> configuration)
    {
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Gets a value from the cache
    /// </summary>
    /// <typeparam name="T">The type of the cached value</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The cached value or default if not found</returns>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        try
        {
            var fullKey = GenerateKey(key);
            var cachedValue = await _distributedCache.GetStringAsync(fullKey, cancellationToken);

            if (cachedValue == null)
            {
                _logger.LogDebug("Cache miss for key: {Key}", fullKey);
                return default;
            }

            _logger.LogDebug("Cache hit for key: {Key}", fullKey);
            return JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value from cache for key: {Key}", key);
            return default;
        }
    }

    /// <summary>
    /// Sets a value in the cache
    /// </summary>
    /// <typeparam name="T">The type of the value to cache</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to cache</param>
    /// <param name="expiry">Optional expiry time</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        if (value == null)
            return;

        try
        {
            var fullKey = GenerateKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            
            var options = new DistributedCacheEntryOptions();
            var expiryTime = expiry ?? TimeSpan.FromMinutes(_configuration.DefaultExpiryMinutes);
            
            if (expiryTime != TimeSpan.Zero)
            {
                options.SetAbsoluteExpiration(expiryTime);
            }

            await _distributedCache.SetStringAsync(fullKey, serializedValue, options, cancellationToken);
            _logger.LogDebug("Cached value for key: {Key} with expiry: {Expiry}", fullKey, expiryTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in cache for key: {Key}", key);
        }
    }

    /// <summary>
    /// Removes a value from the cache
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        try
        {
            var fullKey = GenerateKey(key);
            await _distributedCache.RemoveAsync(fullKey, cancellationToken);
            _logger.LogDebug("Removed cached value for key: {Key}", fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing value from cache for key: {Key}", key);
        }
    }

    /// <summary>
    /// Gets a value from cache or creates it using the factory function
    /// </summary>
    /// <typeparam name="T">The type of the cached value</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="factory">The factory function to create the value if not cached</param>
    /// <param name="expiry">Optional expiry time</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The cached or newly created value</returns>
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue != null && !cachedValue.Equals(default(T)))
        {
            return cachedValue;
        }

        _logger.LogDebug("Creating new value for cache key: {Key}", key);
        var newValue = await factory();
        
        if (newValue != null)
        {
            await SetAsync(key, newValue, expiry, cancellationToken);
        }

        return newValue;
    }

    /// <summary>
    /// Removes multiple keys with a common prefix
    /// </summary>
    /// <param name="prefix">The key prefix</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix cannot be null or empty", nameof(prefix));

        // Note: This is a limitation of IDistributedCache interface
        // For pattern-based removal, consider using Redis-specific implementation
        _logger.LogWarning("RemoveByPrefixAsync is not efficiently supported by IDistributedCache. Consider using RedisCacheService for pattern-based operations.");
    }

    /// <summary>
    /// Checks if a key exists in the cache
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the key exists</returns>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        try
        {
            var fullKey = GenerateKey(key);
            var value = await _distributedCache.GetStringAsync(fullKey, cancellationToken);
            return value != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence in cache for key: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// Generates the full cache key with prefix
    /// </summary>
    /// <param name="key">The original key</param>
    /// <returns>The full cache key</returns>
    private string GenerateKey(string key)
    {
        return string.IsNullOrEmpty(_configuration.KeyPrefix) 
            ? key 
            : $"{_configuration.KeyPrefix}:{key}";
    }
} 