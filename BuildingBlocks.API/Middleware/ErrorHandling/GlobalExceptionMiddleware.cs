using BuildingBlocks.API.Responses.Base;
using System.Net;
using System.Text.Json;

namespace BuildingBlocks.API.Middleware.ErrorHandling;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the GlobalExceptionMiddleware class
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger</param>
    /// <param name="environment">The host environment</param>
    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles exceptions and creates appropriate responses
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="exception">The exception</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var apiResponse = CreateErrorResponse(exception);
        var statusCode = GetStatusCode(exception);

        response.StatusCode = (int)statusCode;

        // Add trace ID for debugging
        apiResponse.TraceId = context.TraceIdentifier;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var jsonResponse = JsonSerializer.Serialize(apiResponse, jsonOptions);
        await response.WriteAsync(jsonResponse);
    }

    /// <summary>
    /// Creates an error response based on the exception type
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns>The error response</returns>
    private ApiResponse CreateErrorResponse(Exception exception)
    {
        return exception switch
        {
            ArgumentException argEx => ApiResponse.Error(argEx.Message),
            ArgumentNullException argNullEx => ApiResponse.Error($"Required parameter is missing: {argNullEx.ParamName}"),
            UnauthorizedAccessException => ApiResponse.Error("Access denied"),
            KeyNotFoundException => ApiResponse.Error("The requested resource was not found"),
            InvalidOperationException invalidEx => ApiResponse.Error(invalidEx.Message),
            NotSupportedException notSupportedEx => ApiResponse.Error(notSupportedEx.Message),
            TimeoutException => ApiResponse.Error("The operation timed out"),
            TaskCanceledException => ApiResponse.Error("The operation was cancelled"),
            FluentValidation.ValidationException validationEx => CreateValidationErrorResponse(validationEx),
            _ => CreateGenericErrorResponse(exception)
        };
    }

    /// <summary>
    /// Creates a validation error response
    /// </summary>
    /// <param name="validationException">The validation exception</param>
    /// <returns>The validation error response</returns>
    private static ApiResponse CreateValidationErrorResponse(FluentValidation.ValidationException validationException)
    {
        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(e => e.ErrorMessage).ToArray());

        return ApiResponse.ValidationError(errors);
    }

    /// <summary>
    /// Creates a generic error response
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns>The generic error response</returns>
    private ApiResponse CreateGenericErrorResponse(Exception exception)
    {
        var message = _environment.IsDevelopment() 
            ? exception.Message 
            : "An error occurred while processing your request";

        return ApiResponse.Error(message);
    }

    /// <summary>
    /// Gets the HTTP status code for an exception
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns>The HTTP status code</returns>
    private static HttpStatusCode GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentException => HttpStatusCode.BadRequest,
            ArgumentNullException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            KeyNotFoundException => HttpStatusCode.NotFound,
            InvalidOperationException => HttpStatusCode.BadRequest,
            NotSupportedException => HttpStatusCode.BadRequest,
            TimeoutException => HttpStatusCode.RequestTimeout,
            TaskCanceledException => HttpStatusCode.RequestTimeout,
            FluentValidation.ValidationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };
    }
} 