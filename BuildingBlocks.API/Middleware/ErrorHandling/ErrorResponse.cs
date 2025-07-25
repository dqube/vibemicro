using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Middleware.ErrorHandling;

/// <summary>
/// Represents an error response following RFC 7807 Problem Details standard
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error type (URI reference)
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "about:blank";

    /// <summary>
    /// Gets or sets the error title (short, human-readable summary)
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTTP status code
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the detailed error description
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>
    /// Gets or sets the URI reference that identifies the specific occurrence
    /// </summary>
    [JsonPropertyName("instance")]
    public string? Instance { get; set; }

    /// <summary>
    /// Gets or sets additional error properties
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? Extensions { get; set; }

    /// <summary>
    /// Gets or sets the correlation ID for tracking
    /// </summary>
    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the error occurred
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets validation errors (for 400 Bad Request responses)
    /// </summary>
    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Creates a new ErrorResponse for validation errors
    /// </summary>
    /// <param name="validationErrors">The validation errors</param>
    /// <param name="instance">The request instance</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <returns>An ErrorResponse configured for validation errors</returns>
    public static ErrorResponse ValidationError(
        Dictionary<string, string[]> validationErrors,
        string? instance = null,
        string? correlationId = null)
    {
        return new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400,
            Detail = "Please refer to the errors property for additional details.",
            Instance = instance,
            CorrelationId = correlationId,
            Errors = validationErrors
        };
    }

    /// <summary>
    /// Creates a new ErrorResponse for bad request
    /// </summary>
    /// <param name="detail">The error detail</param>
    /// <param name="instance">The request instance</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <returns>An ErrorResponse configured for bad request</returns>
    public static ErrorResponse BadRequest(
        string detail,
        string? instance = null,
        string? correlationId = null)
    {
        return new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = 400,
            Detail = detail,
            Instance = instance,
            CorrelationId = correlationId
        };
    }

    /// <summary>
    /// Creates a new ErrorResponse for unauthorized access
    /// </summary>
    /// <param name="detail">The error detail</param>
    /// <param name="instance">The request instance</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <returns>An ErrorResponse configured for unauthorized</returns>
    public static ErrorResponse Unauthorized(
        string? detail = null,
        string? instance = null,
        string? correlationId = null)
    {
        return new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Title = "Unauthorized",
            Status = 401,
            Detail = detail ?? "The request has not been applied because it lacks valid authentication credentials.",
            Instance = instance,
            CorrelationId = correlationId
        };
    }

    /// <summary>
    /// Creates a new ErrorResponse for forbidden access
    /// </summary>
    /// <param name="detail">The error detail</param>
    /// <param name="instance">The request instance</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <returns>An ErrorResponse configured for forbidden</returns>
    public static ErrorResponse Forbidden(
        string? detail = null,
        string? instance = null,
        string? correlationId = null)
    {
        return new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            Title = "Forbidden",
            Status = 403,
            Detail = detail ?? "The server understood the request but refuses to authorize it.",
            Instance = instance,
            CorrelationId = correlationId
        };
    }

    /// <summary>
    /// Creates a new ErrorResponse for not found
    /// </summary>
    /// <param name="detail">The error detail</param>
    /// <param name="instance">The request instance</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <returns>An ErrorResponse configured for not found</returns>
    public static ErrorResponse NotFound(
        string? detail = null,
        string? instance = null,
        string? correlationId = null)
    {
        return new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Not Found",
            Status = 404,
            Detail = detail ?? "The requested resource could not be found.",
            Instance = instance,
            CorrelationId = correlationId
        };
    }

    /// <summary>
    /// Creates a new ErrorResponse for conflict
    /// </summary>
    /// <param name="detail">The error detail</param>
    /// <param name="instance">The request instance</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <returns>An ErrorResponse configured for conflict</returns>
    public static ErrorResponse Conflict(
        string detail,
        string? instance = null,
        string? correlationId = null)
    {
        return new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            Title = "Conflict",
            Status = 409,
            Detail = detail,
            Instance = instance,
            CorrelationId = correlationId
        };
    }

    /// <summary>
    /// Creates a new ErrorResponse for internal server error
    /// </summary>
    /// <param name="detail">The error detail</param>
    /// <param name="instance">The request instance</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <returns>An ErrorResponse configured for internal server error</returns>
    public static ErrorResponse InternalServerError(
        string? detail = null,
        string? instance = null,
        string? correlationId = null)
    {
        return new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = 500,
            Detail = detail ?? "An error occurred while processing your request.",
            Instance = instance,
            CorrelationId = correlationId
        };
    }

    /// <summary>
    /// Creates a new ErrorResponse for service unavailable
    /// </summary>
    /// <param name="detail">The error detail</param>
    /// <param name="instance">The request instance</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <returns>An ErrorResponse configured for service unavailable</returns>
    public static ErrorResponse ServiceUnavailable(
        string? detail = null,
        string? instance = null,
        string? correlationId = null)
    {
        return new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4",
            Title = "Service Unavailable",
            Status = 503,
            Detail = detail ?? "The service is temporarily unavailable.",
            Instance = instance,
            CorrelationId = correlationId
        };
    }

    /// <summary>
    /// Creates a new ErrorResponse with custom values
    /// </summary>
    /// <param name="type">The error type</param>
    /// <param name="title">The error title</param>
    /// <param name="status">The HTTP status code</param>
    /// <param name="detail">The error detail</param>
    /// <param name="instance">The request instance</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <returns>A custom ErrorResponse</returns>
    public static ErrorResponse Custom(
        string type,
        string title,
        int status,
        string? detail = null,
        string? instance = null,
        string? correlationId = null)
    {
        return new ErrorResponse
        {
            Type = type,
            Title = title,
            Status = status,
            Detail = detail,
            Instance = instance,
            CorrelationId = correlationId
        };
    }

    /// <summary>
    /// Adds an extension property to the error response
    /// </summary>
    /// <param name="key">The property key</param>
    /// <param name="value">The property value</param>
    /// <returns>The current ErrorResponse instance for chaining</returns>
    public ErrorResponse WithExtension(string key, object value)
    {
        Extensions ??= new Dictionary<string, object>();
        Extensions[key] = value;
        return this;
    }
} 