using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace BuildingBlocks.API.Middleware.Logging;

/// <summary>
/// Middleware for handling correlation IDs in requests
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private readonly CorrelationIdOptions _options;

    /// <summary>
    /// Initializes a new instance of the CorrelationIdMiddleware class
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger instance</param>
    /// <param name="options">The correlation ID options</param>
    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger,
        IOptions<CorrelationIdOptions> options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        
        // Add correlation ID to the response headers
        if (_options.IncludeInResponse)
        {
            context.Response.Headers[_options.ResponseHeaderName] = correlationId;
        }

        // Add correlation ID to the HTTP context items for later access
        context.Items[_options.HttpContextItemName] = correlationId;

        // Add correlation ID to the current activity for distributed tracing
        Activity.Current?.SetTag(_options.ActivityTagName, correlationId);

        // Add correlation ID to the logger scope
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            [_options.LoggerScopeKey] = correlationId
        });

        _logger.LogDebug("Processing request with correlation ID: {CorrelationId}", correlationId);

        try
        {
            await _next(context);
        }
        finally
        {
            _logger.LogDebug("Completed request with correlation ID: {CorrelationId}", correlationId);
        }
    }

    /// <summary>
    /// Gets an existing correlation ID from the request or creates a new one
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>The correlation ID</returns>
    private string GetOrCreateCorrelationId(HttpContext context)
    {
        // Check if correlation ID is provided in request headers
        if (context.Request.Headers.TryGetValue(_options.RequestHeaderName, out var headerValues))
        {
            var correlationId = headerValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                if (_options.ValidateCorrelationId && !IsValidCorrelationId(correlationId))
                {
                    _logger.LogWarning("Invalid correlation ID received: {CorrelationId}. Generating new one.", correlationId);
                    return GenerateCorrelationId();
                }
                
                return correlationId;
            }
        }

        // Check if correlation ID is provided as a query parameter
        if (_options.AllowQueryParameter && 
            context.Request.Query.TryGetValue(_options.QueryParameterName, out var queryValues))
        {
            var correlationId = queryValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                if (_options.ValidateCorrelationId && !IsValidCorrelationId(correlationId))
                {
                    _logger.LogWarning("Invalid correlation ID received in query: {CorrelationId}. Generating new one.", correlationId);
                    return GenerateCorrelationId();
                }
                
                return correlationId;
            }
        }

        // Generate a new correlation ID
        return GenerateCorrelationId();
    }

    /// <summary>
    /// Generates a new correlation ID
    /// </summary>
    /// <returns>A new correlation ID</returns>
    private string GenerateCorrelationId()
    {
        return _options.CorrelationIdGenerator switch
        {
            CorrelationIdGenerator.Guid => Guid.NewGuid().ToString(),
            CorrelationIdGenerator.ShortGuid => Guid.NewGuid().ToString("N")[..8],
            CorrelationIdGenerator.Timestamp => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            CorrelationIdGenerator.TraceId => Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString(),
            _ => Guid.NewGuid().ToString()
        };
    }

    /// <summary>
    /// Validates if a correlation ID is in the correct format
    /// </summary>
    /// <param name="correlationId">The correlation ID to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    private bool IsValidCorrelationId(string correlationId)
    {
        if (string.IsNullOrWhiteSpace(correlationId))
            return false;

        // Check length constraints
        if (correlationId.Length < _options.MinLength || correlationId.Length > _options.MaxLength)
            return false;

        // Check for allowed characters (alphanumeric, hyphens, underscores)
        return correlationId.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
    }
}

/// <summary>
/// Options for correlation ID middleware
/// </summary>
public class CorrelationIdOptions
{
    /// <summary>
    /// Gets or sets the name of the request header that contains the correlation ID
    /// </summary>
    public string RequestHeaderName { get; set; } = "X-Correlation-ID";

    /// <summary>
    /// Gets or sets the name of the response header that will contain the correlation ID
    /// </summary>
    public string ResponseHeaderName { get; set; } = "X-Correlation-ID";

    /// <summary>
    /// Gets or sets the name of the query parameter that can contain the correlation ID
    /// </summary>
    public string QueryParameterName { get; set; } = "correlationId";

    /// <summary>
    /// Gets or sets the key used to store the correlation ID in HTTP context items
    /// </summary>
    public string HttpContextItemName { get; set; } = "CorrelationId";

    /// <summary>
    /// Gets or sets the key used in logger scope for the correlation ID
    /// </summary>
    public string LoggerScopeKey { get; set; } = "CorrelationId";

    /// <summary>
    /// Gets or sets the tag name used in activities for the correlation ID
    /// </summary>
    public string ActivityTagName { get; set; } = "correlation.id";

    /// <summary>
    /// Gets or sets whether to include the correlation ID in the response headers
    /// </summary>
    public bool IncludeInResponse { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to allow correlation ID to be passed as a query parameter
    /// </summary>
    public bool AllowQueryParameter { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to validate the format of incoming correlation IDs
    /// </summary>
    public bool ValidateCorrelationId { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum length for correlation IDs
    /// </summary>
    public int MinLength { get; set; } = 8;

    /// <summary>
    /// Gets or sets the maximum length for correlation IDs
    /// </summary>
    public int MaxLength { get; set; } = 128;

    /// <summary>
    /// Gets or sets the correlation ID generator to use
    /// </summary>
    public CorrelationIdGenerator CorrelationIdGenerator { get; set; } = CorrelationIdGenerator.Guid;
}

/// <summary>
/// Defines the available correlation ID generators
/// </summary>
public enum CorrelationIdGenerator
{
    /// <summary>
    /// Use a full GUID (32 characters plus hyphens)
    /// </summary>
    Guid,

    /// <summary>
    /// Use a short GUID (first 8 characters)
    /// </summary>
    ShortGuid,

    /// <summary>
    /// Use a timestamp-based ID
    /// </summary>
    Timestamp,

    /// <summary>
    /// Use the current activity trace ID
    /// </summary>
    TraceId
}

/// <summary>
/// Extension methods for accessing correlation ID from HTTP context
/// </summary>
public static class CorrelationIdExtensions
{
    /// <summary>
    /// Gets the correlation ID from the HTTP context
    /// </summary>
    /// <param name="httpContext">The HTTP context</param>
    /// <param name="options">The correlation ID options</param>
    /// <returns>The correlation ID or null if not found</returns>
    public static string? GetCorrelationId(this HttpContext httpContext, CorrelationIdOptions? options = null)
    {
        var itemName = options?.HttpContextItemName ?? "CorrelationId";
        return httpContext.Items[itemName] as string;
    }

    /// <summary>
    /// Sets the correlation ID in the HTTP context
    /// </summary>
    /// <param name="httpContext">The HTTP context</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <param name="options">The correlation ID options</param>
    public static void SetCorrelationId(this HttpContext httpContext, string correlationId, CorrelationIdOptions? options = null)
    {
        var itemName = options?.HttpContextItemName ?? "CorrelationId";
        httpContext.Items[itemName] = correlationId;
    }
} 