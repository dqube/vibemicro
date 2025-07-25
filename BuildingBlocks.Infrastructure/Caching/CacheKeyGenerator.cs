using System.Security.Cryptography;
using System.Text;

namespace BuildingBlocks.Infrastructure.Caching;

/// <summary>
/// Utility class for generating standardized cache keys
/// </summary>
public static class CacheKeyGenerator
{
    /// <summary>
    /// Generates a cache key from multiple parts
    /// </summary>
    /// <param name="parts">The key parts</param>
    /// <returns>The generated cache key</returns>
    public static string Generate(params string[] parts)
    {
        if (parts == null || parts.Length == 0)
            throw new ArgumentException("At least one key part must be provided", nameof(parts));

        var nonEmptyParts = parts.Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(":", nonEmptyParts);
    }

    /// <summary>
    /// Generates a cache key for an entity by type and ID
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="id">The entity identifier</param>
    /// <returns>The generated cache key</returns>
    public static string ForEntity<T>(object id)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        return Generate(typeof(T).Name.ToLowerInvariant(), id.ToString()!);
    }

    /// <summary>
    /// Generates a cache key for an entity by type name and ID
    /// </summary>
    /// <param name="entityType">The entity type name</param>
    /// <param name="id">The entity identifier</param>
    /// <returns>The generated cache key</returns>
    public static string ForEntity(string entityType, object id)
    {
        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type cannot be null or empty", nameof(entityType));
        
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        return Generate(entityType.ToLowerInvariant(), id.ToString()!);
    }

    /// <summary>
    /// Generates a cache key for a collection with optional filtering
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="filter">Optional filter parameters</param>
    /// <returns>The generated cache key</returns>
    public static string ForCollection<T>(object? filter = null)
    {
        var entityName = typeof(T).Name.ToLowerInvariant();
        
        if (filter == null)
            return Generate(entityName, "collection");

        var filterHash = GenerateParameterHash(filter);
        return Generate(entityName, "collection", filterHash);
    }

    /// <summary>
    /// Generates a cache key for a collection by type name with optional filtering
    /// </summary>
    /// <param name="entityType">The entity type name</param>
    /// <param name="filter">Optional filter parameters</param>
    /// <returns>The generated cache key</returns>
    public static string ForCollection(string entityType, object? filter = null)
    {
        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type cannot be null or empty", nameof(entityType));

        var normalizedType = entityType.ToLowerInvariant();
        
        if (filter == null)
            return Generate(normalizedType, "collection");

        var filterHash = GenerateParameterHash(filter);
        return Generate(normalizedType, "collection", filterHash);
    }

    /// <summary>
    /// Generates a cache key for user-specific data
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="dataType">The type of user data</param>
    /// <param name="additionalParts">Additional key parts</param>
    /// <returns>The generated cache key</returns>
    public static string ForUser(string userId, string dataType, params string[] additionalParts)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        
        if (string.IsNullOrWhiteSpace(dataType))
            throw new ArgumentException("Data type cannot be null or empty", nameof(dataType));

        var parts = new List<string> { "user", userId, dataType.ToLowerInvariant() };
        if (additionalParts?.Length > 0)
        {
            parts.AddRange(additionalParts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        return string.Join(":", parts);
    }

    /// <summary>
    /// Generates a cache key for query results
    /// </summary>
    /// <param name="queryType">The query type</param>
    /// <param name="parameters">The query parameters</param>
    /// <returns>The generated cache key</returns>
    public static string ForQuery(string queryType, object? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(queryType))
            throw new ArgumentException("Query type cannot be null or empty", nameof(queryType));

        var normalizedType = queryType.ToLowerInvariant();
        
        if (parameters == null)
            return Generate("query", normalizedType);

        var parameterHash = GenerateParameterHash(parameters);
        return Generate("query", normalizedType, parameterHash);
    }

    /// <summary>
    /// Generates a cache key for query results by type
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <param name="parameters">The query parameters</param>
    /// <returns>The generated cache key</returns>
    public static string ForQuery<TQuery>(object? parameters = null)
    {
        return ForQuery(typeof(TQuery).Name, parameters);
    }

    /// <summary>
    /// Generates a cache key for session data
    /// </summary>
    /// <param name="sessionId">The session identifier</param>
    /// <param name="dataKey">The data key within the session</param>
    /// <returns>The generated cache key</returns>
    public static string ForSession(string sessionId, string dataKey)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
        
        if (string.IsNullOrWhiteSpace(dataKey))
            throw new ArgumentException("Data key cannot be null or empty", nameof(dataKey));

        return Generate("session", sessionId, dataKey.ToLowerInvariant());
    }

    /// <summary>
    /// Generates a cache key for temporary data with expiration
    /// </summary>
    /// <param name="category">The data category</param>
    /// <param name="identifier">The data identifier</param>
    /// <param name="expiry">The expiration time (used in key generation for clarity)</param>
    /// <returns>The generated cache key</returns>
    public static string ForTemporary(string category, string identifier, TimeSpan expiry)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be null or empty", nameof(category));
        
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Identifier cannot be null or empty", nameof(identifier));

        var expiryMinutes = (int)expiry.TotalMinutes;
        return Generate("temp", category.ToLowerInvariant(), identifier, $"{expiryMinutes}m");
    }

    /// <summary>
    /// Generates a hash from an object's properties for use in cache keys
    /// </summary>
    /// <param name="obj">The object to hash</param>
    /// <returns>A short hash string</returns>
    public static string GenerateParameterHash(object obj)
    {
        if (obj == null)
            return "null";

        var json = System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
        
        // Take first 8 bytes and convert to base64 for a shorter hash
        var shortHash = Convert.ToBase64String(hash[..8])
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
        
        return shortHash;
    }

    /// <summary>
    /// Creates a cache key with timestamp for time-sensitive data
    /// </summary>
    /// <param name="baseKey">The base cache key</param>
    /// <param name="timeWindow">The time window for grouping (e.g., hourly, daily)</param>
    /// <returns>The generated cache key with timestamp</returns>
    public static string WithTimeWindow(string baseKey, TimeSpan timeWindow)
    {
        if (string.IsNullOrWhiteSpace(baseKey))
            throw new ArgumentException("Base key cannot be null or empty", nameof(baseKey));

        var now = DateTime.UtcNow;
        var windowTicks = timeWindow.Ticks;
        var windowedTime = new DateTime((now.Ticks / windowTicks) * windowTicks, DateTimeKind.Utc);
        var timeString = windowedTime.ToString("yyyyMMddHHmm");
        
        return Generate(baseKey, timeString);
    }

    /// <summary>
    /// Validates that a cache key meets length and character requirements
    /// </summary>
    /// <param name="key">The cache key to validate</param>
    /// <param name="maxLength">Maximum allowed length (default: 250)</param>
    /// <returns>True if the key is valid</returns>
    public static bool IsValidKey(string key, int maxLength = 250)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        if (key.Length > maxLength)
            return false;

        // Check for invalid characters (spaces, control characters)
        return key.All(c => !char.IsControl(c) && !char.IsWhiteSpace(c));
    }

    /// <summary>
    /// Sanitizes a cache key by removing or replacing invalid characters
    /// </summary>
    /// <param name="key">The cache key to sanitize</param>
    /// <param name="maxLength">Maximum allowed length (default: 250)</param>
    /// <returns>The sanitized cache key</returns>
    public static string Sanitize(string key, int maxLength = 250)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        // Replace invalid characters with underscores
        var sanitized = new StringBuilder();
        foreach (var c in key)
        {
            if (char.IsControl(c) || char.IsWhiteSpace(c))
                sanitized.Append('_');
            else
                sanitized.Append(c);
        }

        var result = sanitized.ToString();
        
        // Truncate if too long
        if (result.Length > maxLength)
        {
            result = result[..maxLength];
        }

        return result;
    }
} 