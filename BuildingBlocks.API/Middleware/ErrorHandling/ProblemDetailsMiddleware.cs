using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Security;
using BuildingBlocks.Domain.Extensions;

namespace BuildingBlocks.API.Middleware.ErrorHandling;

/// <summary>
/// Middleware for handling exceptions and converting them to RFC 7807 Problem Details responses
/// </summary>
public class ProblemDetailsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ProblemDetailsMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    private readonly ProblemDetailsOptions _options;

    /// <summary>
    /// Initializes a new instance of the ProblemDetailsMiddleware class
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger</param>
    /// <param name="environment">The host environment</param>
    /// <param name="options">The problem details options</param>
    public ProblemDetailsMiddleware(
        RequestDelegate next,
        ILogger<ProblemDetailsMiddleware> logger,
        IHostEnvironment environment,
        ProblemDetailsOptions options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            // Handle non-exception scenarios (like 404, 401, etc.)
            if (!context.Response.HasStarted && context.Response.StatusCode >= 400)
            {
                await HandleStatusCodeAsync(context);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles exceptions and converts them to Problem Details responses
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="exception">The exception</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Cannot create problem details response. Response has already started.");
            return;
        }

        var correlationId = GetCorrelationId(context);
        var includeDetails = _environment.IsDevelopment() || _options.IncludeExceptionDetails;

        var problemDetails = ProblemDetailsFactory.CreateExceptionProblemDetails(
            exception, 
            context, 
            correlationId, 
            includeDetails);

        // Apply custom exception mappings
        if (_options.ExceptionMappings.TryGetValue(exception.GetType(), out var mapping))
        {
            problemDetails.Status = mapping.StatusCode;
            problemDetails.Title = mapping.Title;
            problemDetails.Type = mapping.Type;
            
            if (!string.IsNullOrEmpty(mapping.Detail))
            {
                problemDetails.Detail = mapping.Detail;
            }
        }

        await WriteProblemDetailsAsync(context, problemDetails);
    }

    /// <summary>
    /// Handles HTTP status codes and converts them to Problem Details responses
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task HandleStatusCodeAsync(HttpContext context)
    {
        if (!_options.ShouldHandleStatusCode(context.Response.StatusCode))
        {
            return;
        }

        var correlationId = GetCorrelationId(context);
        var problemDetails = ProblemDetailsFactory.CreateStatusCodeProblemDetails(
            context.Response.StatusCode,
            httpContext: context,
            correlationId: correlationId);

        await WriteProblemDetailsAsync(context, problemDetails);
    }

    /// <summary>
    /// Writes the Problem Details response
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="problemDetails">The problem details</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task WriteProblemDetailsAsync(HttpContext context, ProblemDetails problemDetails)
    {
        context.Response.StatusCode = problemDetails.Status ?? 500;
        context.Response.ContentType = "application/problem+json";

        var jsonOptions = JsonExtensions.CreateOptionsForStronglyTypedIds(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.WriteIndented = _environment.IsDevelopment();
        });

        var json = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await context.Response.WriteAsync(json);
    }

    /// <summary>
    /// Gets the correlation ID from the current context
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>The correlation ID</returns>
    private static string? GetCorrelationId(HttpContext context)
    {
        // Try to get correlation ID from various sources
        return context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ??
               context.Request.Headers["CorrelationId"].FirstOrDefault() ??
               context.TraceIdentifier;
    }
}

/// <summary>
/// Configuration options for Problem Details middleware
/// </summary>
public class ProblemDetailsOptions
{
    /// <summary>
    /// Gets or sets whether to include exception details in responses
    /// </summary>
    public bool IncludeExceptionDetails { get; set; } = false;

    /// <summary>
    /// Gets or sets the predicate to determine which status codes should be handled
    /// </summary>
    public Func<int, bool> ShouldHandleStatusCode { get; set; } = statusCode => statusCode >= 400;

    /// <summary>
    /// Gets or sets custom exception mappings
    /// </summary>
    public Dictionary<Type, ExceptionMapping> ExceptionMappings { get; set; } = new();

    /// <summary>
    /// Adds a custom exception mapping
    /// </summary>
    /// <typeparam name="TException">The exception type</typeparam>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="title">The problem title</param>
    /// <param name="type">The problem type URI</param>
    /// <param name="detail">The problem detail</param>
    /// <returns>The options instance for chaining</returns>
    public ProblemDetailsOptions Map<TException>(
        int statusCode,
        string title,
        string? type = null,
        string? detail = null)
        where TException : Exception
    {
        ExceptionMappings[typeof(TException)] = new ExceptionMapping
        {
            StatusCode = statusCode,
            Title = title,
            Type = type ?? "about:blank",
            Detail = detail
        };

        return this;
    }

    /// <summary>
    /// Configures standard exception mappings
    /// </summary>
    /// <returns>The options instance for chaining</returns>
    public ProblemDetailsOptions MapStandardExceptions()
    {
        return this
            .Map<ArgumentException>(400, "Bad Request")
            .Map<ArgumentNullException>(400, "Bad Request")
            .Map<InvalidOperationException>(400, "Bad Request")
            .Map<UnauthorizedAccessException>(401, "Unauthorized")
            .Map<SecurityException>(403, "Forbidden")
            .Map<KeyNotFoundException>(404, "Not Found")
            .Map<NotSupportedException>(405, "Method Not Allowed")
            .Map<TimeoutException>(408, "Request Timeout")
            .Map<TaskCanceledException>(499, "Client Closed Request")
            .Map<OperationCanceledException>(499, "Client Closed Request")
            .Map<NotImplementedException>(501, "Not Implemented");
    }
}

/// <summary>
/// Represents an exception mapping configuration
/// </summary>
public class ExceptionMapping
{
    /// <summary>
    /// Gets or sets the HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the problem title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the problem type URI
    /// </summary>
    public string Type { get; set; } = "about:blank";

    /// <summary>
    /// Gets or sets the problem detail
    /// </summary>
    public string? Detail { get; set; }
}

/// <summary>
/// Extension methods for Problem Details middleware
/// </summary>
public static class ProblemDetailsMiddlewareExtensions
{
    /// <summary>
    /// Adds Problem Details middleware to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">The configuration action</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddProblemDetailsMiddleware(
        this IServiceCollection services,
        Action<ProblemDetailsOptions>? configure = null)
    {
        var options = new ProblemDetailsOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        
        return services;
    }

    /// <summary>
    /// Uses the Problem Details middleware
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseProblemDetailsMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ProblemDetailsMiddleware>();
    }
} 