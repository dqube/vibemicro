using System.Diagnostics;
using System.Text;

namespace BuildingBlocks.API.Middleware.Logging;

/// <summary>
/// Middleware for logging HTTP requests and responses
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly RequestLoggingOptions _options;

    /// <summary>
    /// Initializes a new instance of the RequestLoggingMiddleware class
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger</param>
    /// <param name="options">The logging options</param>
    public RequestLoggingMiddleware(
        RequestDelegate next, 
        ILogger<RequestLoggingMiddleware> logger,
        RequestLoggingOptions options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldSkipLogging(context))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestBody = string.Empty;
        var responseBody = string.Empty;

        // Log request
        if (_options.LogRequestBody && ShouldLogBody(context.Request))
        {
            requestBody = await ReadRequestBodyAsync(context.Request);
        }

        LogRequest(context, requestBody);

        // Capture response
        var originalResponseBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);

            // Log response
            if (_options.LogResponseBody && ShouldLogBody(context.Response))
            {
                responseBody = await ReadResponseBodyAsync(responseBodyStream);
            }

            stopwatch.Stop();
            LogResponse(context, responseBody, stopwatch.ElapsedMilliseconds);

            // Copy the response body back to the original stream
            responseBodyStream.Position = 0;
            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        }
        finally
        {
            context.Response.Body = originalResponseBodyStream;
        }
    }

    /// <summary>
    /// Determines whether to skip logging for the current request
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>True if logging should be skipped</returns>
    private bool ShouldSkipLogging(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        
        return _options.SkipPaths.Any(skipPath => 
            path?.StartsWith(skipPath.ToLowerInvariant()) == true);
    }

    /// <summary>
    /// Determines whether to log the request/response body
    /// </summary>
    /// <param name="request">The HTTP request</param>
    /// <returns>True if the body should be logged</returns>
    private static bool ShouldLogBody(HttpRequest request)
    {
        var contentType = request.ContentType?.ToLowerInvariant();
        return contentType?.Contains("application/json") == true ||
               contentType?.Contains("application/xml") == true ||
               contentType?.Contains("text/") == true;
    }

    /// <summary>
    /// Determines whether to log the response body
    /// </summary>
    /// <param name="response">The HTTP response</param>
    /// <returns>True if the body should be logged</returns>
    private static bool ShouldLogBody(HttpResponse response)
    {
        var contentType = response.ContentType?.ToLowerInvariant();
        return contentType?.Contains("application/json") == true ||
               contentType?.Contains("application/xml") == true ||
               contentType?.Contains("text/") == true;
    }

    /// <summary>
    /// Reads the request body
    /// </summary>
    /// <param name="request">The HTTP request</param>
    /// <returns>The request body as a string</returns>
    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        
        return body;
    }

    /// <summary>
    /// Reads the response body
    /// </summary>
    /// <param name="responseBodyStream">The response body stream</param>
    /// <returns>The response body as a string</returns>
    private static async Task<string> ReadResponseBodyAsync(MemoryStream responseBodyStream)
    {
        responseBodyStream.Position = 0;
        
        using var reader = new StreamReader(responseBodyStream, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        responseBodyStream.Position = 0;
        
        return body;
    }

    /// <summary>
    /// Logs the HTTP request
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="requestBody">The request body</param>
    private void LogRequest(HttpContext context, string requestBody)
    {
        var request = context.Request;
        
        _logger.LogInformation(
            "HTTP {Method} {Path} started. TraceId: {TraceId}, User: {User}, UserAgent: {UserAgent}",
            request.Method,
            request.Path + request.QueryString,
            context.TraceIdentifier,
            context.User?.Identity?.Name ?? "Anonymous",
            request.Headers.UserAgent.ToString());

        if (_options.LogRequestBody && !string.IsNullOrWhiteSpace(requestBody))
        {
            _logger.LogDebug(
                "Request body for {Method} {Path}: {RequestBody}",
                request.Method,
                request.Path,
                requestBody);
        }

        if (_options.LogHeaders)
        {
            var headers = request.Headers
                .Where(h => !_options.SensitiveHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => string.Join(", ", h.Value));

            _logger.LogDebug(
                "Request headers for {Method} {Path}: {@Headers}",
                request.Method,
                request.Path,
                headers);
        }
    }

    /// <summary>
    /// Logs the HTTP response
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="responseBody">The response body</param>
    /// <param name="elapsedMilliseconds">The elapsed time in milliseconds</param>
    private void LogResponse(HttpContext context, string responseBody, long elapsedMilliseconds)
    {
        var request = context.Request;
        var response = context.Response;
        
        var logLevel = response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;

        _logger.Log(logLevel,
            "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms. TraceId: {TraceId}",
            request.Method,
            request.Path + request.QueryString,
            response.StatusCode,
            elapsedMilliseconds,
            context.TraceIdentifier);

        if (_options.LogResponseBody && !string.IsNullOrWhiteSpace(responseBody))
        {
            _logger.LogDebug(
                "Response body for {Method} {Path} ({StatusCode}): {ResponseBody}",
                request.Method,
                request.Path,
                response.StatusCode,
                responseBody);
        }

        if (_options.LogHeaders)
        {
            var headers = response.Headers
                .Where(h => !_options.SensitiveHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => string.Join(", ", h.Value));

            _logger.LogDebug(
                "Response headers for {Method} {Path} ({StatusCode}): {@Headers}",
                request.Method,
                request.Path,
                response.StatusCode,
                headers);
        }
    }
}

/// <summary>
/// Options for request logging middleware
/// </summary>
public class RequestLoggingOptions
{
    /// <summary>
    /// Gets or sets whether to log request bodies
    /// </summary>
    public bool LogRequestBody { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to log response bodies
    /// </summary>
    public bool LogResponseBody { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to log headers
    /// </summary>
    public bool LogHeaders { get; set; } = false;

    /// <summary>
    /// Gets or sets the paths to skip logging
    /// </summary>
    public List<string> SkipPaths { get; set; } = new()
    {
        "/health",
        "/metrics",
        "/swagger",
        "/favicon.ico"
    };

    /// <summary>
    /// Gets or sets the sensitive headers to exclude from logging
    /// </summary>
    public HashSet<string> SensitiveHeaders { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization",
        "Cookie",
        "Set-Cookie",
        "X-API-Key",
        "X-Auth-Token"
    };
} 