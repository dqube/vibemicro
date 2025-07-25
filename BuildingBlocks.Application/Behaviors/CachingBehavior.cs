using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Application.Behaviors;

/// <summary>
/// Pipeline behavior for caching responses
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the CachingBehavior class
    /// </summary>
    /// <param name="cache">The memory cache</param>
    /// <param name="logger">The logger</param>
    public CachingBehavior(IMemoryCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the request with caching
    /// </summary>
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        // Only cache if request implements ICacheable
        if (request is not ICacheable cacheableRequest)
        {
            return await next();
        }

        var cacheKey = cacheableRequest.CacheKey;
        
        if (string.IsNullOrEmpty(cacheKey))
        {
            _logger.LogWarning("Cache key is empty for {RequestType}", typeof(TRequest).Name);
            return await next();
        }

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out TResponse? cachedResponse))
        {
            _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
            return cachedResponse!;
        }

        _logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);

        // Execute the request
        var response = await next();

        // Cache the response
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheableRequest.CacheDuration,
            Priority = CacheItemPriority.Normal
        };

        _cache.Set(cacheKey, response, cacheOptions);
        
        _logger.LogDebug("Cached response for key: {CacheKey}, Duration: {Duration}", 
            cacheKey, cacheableRequest.CacheDuration);

        return response;
    }
}

/// <summary>
/// Interface for cacheable requests
/// </summary>
public interface ICacheable
{
    /// <summary>
    /// Gets the cache key for this request
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// Gets the cache duration for this request
    /// </summary>
    TimeSpan CacheDuration { get; }
} 