using BuildingBlocks.Application.Caching;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Caching;

/// <summary>
/// Redis cache service implementation with advanced Redis features
/// </summary>
public class RedisCacheService : ICacheService, IDisposable
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly CacheConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the RedisCacheService class
    /// </summary>
    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisCacheService> logger,
        IOptions<CacheConfiguration> configuration)
    {
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _database = _connectionMultiplexer.GetDatabase();
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
            var cachedValue = await _database.StringGetAsync(fullKey);

            if (!cachedValue.HasValue)
            {
                _logger.LogDebug("Cache miss for key: {Key}", fullKey);
                return default;
            }

            _logger.LogDebug("Cache hit for key: {Key}", fullKey);
            return JsonSerializer.Deserialize<T>(cachedValue!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value from Redis cache for key: {Key}", key);
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
            var expiryTime = expiry ?? TimeSpan.FromMinutes(_configuration.DefaultExpiryMinutes);
            
            var success = await _database.StringSetAsync(fullKey, serializedValue, expiryTime);
            
            if (success)
            {
                _logger.LogDebug("Cached value for key: {Key} with expiry: {Expiry}", fullKey, expiryTime);
            }
            else
            {
                _logger.LogWarning("Failed to cache value for key: {Key}", fullKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in Redis cache for key: {Key}", key);
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
            var removed = await _database.KeyDeleteAsync(fullKey);
            
            if (removed)
            {
                _logger.LogDebug("Removed cached value for key: {Key}", fullKey);
            }
            else
            {
                _logger.LogDebug("Key not found for removal: {Key}", fullKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing value from Redis cache for key: {Key}", key);
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
    /// Removes multiple keys with a common prefix using Redis pattern matching
    /// </summary>
    /// <param name="prefix">The key prefix</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix cannot be null or empty", nameof(prefix));

        try
        {
            var fullPrefix = GenerateKey(prefix);
            var pattern = $"{fullPrefix}*";
            
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);
            
            var keyArray = keys.ToArray();
            if (keyArray.Length > 0)
            {
                await _database.KeyDeleteAsync(keyArray);
                _logger.LogDebug("Removed {Count} keys with prefix: {Prefix}", keyArray.Length, fullPrefix);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing keys by prefix from Redis cache: {Prefix}", prefix);
        }
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
            return await _database.KeyExistsAsync(fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence in Redis cache for key: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// Sets the expiry time for an existing key
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="expiry">The expiry time</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the expiry was set successfully</returns>
    public async Task<bool> SetExpiryAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        try
        {
            var fullKey = GenerateKey(key);
            return await _database.KeyExpireAsync(fullKey, expiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting expiry for Redis cache key: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// Gets the time-to-live for a key
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The time-to-live or null if key doesn't exist or has no expiry</returns>
    public async Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        try
        {
            var fullKey = GenerateKey(key);
            return await _database.KeyTimeToLiveAsync(fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting TTL for Redis cache key: {Key}", key);
            return null;
        }
    }

    /// <summary>
    /// Increments a numeric value in the cache
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to increment by</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The new value after increment</returns>
    public async Task<long> IncrementAsync(string key, long value = 1, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        try
        {
            var fullKey = GenerateKey(key);
            return await _database.StringIncrementAsync(fullKey, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing Redis cache key: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// Decrements a numeric value in the cache
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to decrement by</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The new value after decrement</returns>
    public async Task<long> DecrementAsync(string key, long value = 1, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        try
        {
            var fullKey = GenerateKey(key);
            return await _database.StringDecrementAsync(fullKey, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrementing Redis cache key: {Key}", key);
            throw;
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

    /// <summary>
    /// Disposes the service
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the service
    /// </summary>
    /// <param name="disposing">Whether disposing</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // Connection multiplexer is managed by DI container
            _disposed = true;
        }
    }
} 