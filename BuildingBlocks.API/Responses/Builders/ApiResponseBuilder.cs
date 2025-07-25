using BuildingBlocks.API.Responses.Base;
using System.Net;

namespace BuildingBlocks.API.Responses.Builders;

/// <summary>
/// Builder for creating API responses with a fluent interface
/// </summary>
/// <typeparam name="T">The type of data in the response</typeparam>
public class ApiResponseBuilder<T>
{
    private bool _isSuccess = true;
    private T? _data;
    private string? _message;
    private object? _errors;
    private Dictionary<string, object>? _metadata;
    private HttpStatusCode _statusCode = HttpStatusCode.OK;

    /// <summary>
    /// Sets the response as successful
    /// </summary>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> Success()
    {
        _isSuccess = true;
        _statusCode = HttpStatusCode.OK;
        return this;
    }

    /// <summary>
    /// Sets the response as failed
    /// </summary>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> Failure()
    {
        _isSuccess = false;
        _statusCode = HttpStatusCode.BadRequest;
        return this;
    }

    /// <summary>
    /// Sets the response data
    /// </summary>
    /// <param name="data">The response data</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> WithData(T data)
    {
        _data = data;
        return this;
    }

    /// <summary>
    /// Sets the response message
    /// </summary>
    /// <param name="message">The response message</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> WithMessage(string message)
    {
        _message = message;
        return this;
    }

    /// <summary>
    /// Sets the response errors
    /// </summary>
    /// <param name="errors">The response errors</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> WithErrors(object errors)
    {
        _errors = errors;
        return this;
    }

    /// <summary>
    /// Sets the HTTP status code
    /// </summary>
    /// <param name="statusCode">The HTTP status code</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> WithStatusCode(HttpStatusCode statusCode)
    {
        _statusCode = statusCode;
        return this;
    }

    /// <summary>
    /// Adds metadata to the response
    /// </summary>
    /// <param name="key">The metadata key</param>
    /// <param name="value">The metadata value</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> WithMetadata(string key, object value)
    {
        _metadata ??= new Dictionary<string, object>();
        _metadata[key] = value;
        return this;
    }

    /// <summary>
    /// Adds multiple metadata items to the response
    /// </summary>
    /// <param name="metadata">The metadata dictionary</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> WithMetadata(Dictionary<string, object> metadata)
    {
        _metadata = metadata;
        return this;
    }

    /// <summary>
    /// Builds the API response
    /// </summary>
    /// <returns>The constructed API response</returns>
    public ApiResponse<T> Build()
    {
        var response = new ApiResponse<T>
        {
            IsSuccess = _isSuccess,
            Data = _data,
            Message = _message,
            Errors = _errors,
            StatusCode = (int)_statusCode,
            Timestamp = DateTime.UtcNow
        };

        if (_metadata?.Count > 0)
        {
            response.Metadata = _metadata;
        }

        return response;
    }

    /// <summary>
    /// Configures the builder for a successful response
    /// </summary>
    /// <param name="data">The response data</param>
    /// <param name="message">Optional success message</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> ForSuccess(T data, string? message = null)
    {
        return Success()
            .WithData(data)
            .WithMessage(message ?? "Request completed successfully")
            .WithStatusCode(HttpStatusCode.OK);
    }

    /// <summary>
    /// Configures the builder for a created response
    /// </summary>
    /// <param name="data">The created resource data</param>
    /// <param name="message">Optional creation message</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> ForCreated(T data, string? message = null)
    {
        return Success()
            .WithData(data)
            .WithMessage(message ?? "Resource created successfully")
            .WithStatusCode(HttpStatusCode.Created);
    }

    /// <summary>
    /// Configures the builder for a no content response
    /// </summary>
    /// <param name="message">Optional message</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> ForNoContent(string? message = null)
    {
        return Success()
            .WithMessage(message ?? "Request completed successfully")
            .WithStatusCode(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Configures the builder for a bad request response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="errors">Optional error details</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> ForBadRequest(string message, object? errors = null)
    {
        return Failure()
            .WithMessage(message)
            .WithErrors(errors)
            .WithStatusCode(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Configures the builder for a not found response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> ForNotFound(string? message = null)
    {
        return Failure()
            .WithMessage(message ?? "Resource not found")
            .WithStatusCode(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Configures the builder for an unauthorized response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> ForUnauthorized(string? message = null)
    {
        return Failure()
            .WithMessage(message ?? "Unauthorized access")
            .WithStatusCode(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Configures the builder for a forbidden response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> ForForbidden(string? message = null)
    {
        return Failure()
            .WithMessage(message ?? "Access forbidden")
            .WithStatusCode(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Configures the builder for a conflict response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="errors">Optional error details</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> ForConflict(string message, object? errors = null)
    {
        return Failure()
            .WithMessage(message)
            .WithErrors(errors)
            .WithStatusCode(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// Configures the builder for an internal server error response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <returns>The builder instance</returns>
    public ApiResponseBuilder<T> ForInternalServerError(string? message = null)
    {
        return Failure()
            .WithMessage(message ?? "An internal server error occurred")
            .WithStatusCode(HttpStatusCode.InternalServerError);
    }
}

/// <summary>
/// Static factory for creating API response builders
/// </summary>
public static class ApiResponseBuilder
{
    /// <summary>
    /// Creates a new API response builder
    /// </summary>
    /// <typeparam name="T">The type of data in the response</typeparam>
    /// <returns>A new API response builder</returns>
    public static ApiResponseBuilder<T> Create<T>()
    {
        return new ApiResponseBuilder<T>();
    }

    /// <summary>
    /// Creates a successful response builder with data
    /// </summary>
    /// <typeparam name="T">The type of data in the response</typeparam>
    /// <param name="data">The response data</param>
    /// <param name="message">Optional success message</param>
    /// <returns>A configured API response builder</returns>
    public static ApiResponseBuilder<T> Success<T>(T data, string? message = null)
    {
        return new ApiResponseBuilder<T>().ForSuccess(data, message);
    }

    /// <summary>
    /// Creates a created response builder with data
    /// </summary>
    /// <typeparam name="T">The type of data in the response</typeparam>
    /// <param name="data">The created resource data</param>
    /// <param name="message">Optional creation message</param>
    /// <returns>A configured API response builder</returns>
    public static ApiResponseBuilder<T> Created<T>(T data, string? message = null)
    {
        return new ApiResponseBuilder<T>().ForCreated(data, message);
    }

    /// <summary>
    /// Creates a no content response builder
    /// </summary>
    /// <param name="message">Optional message</param>
    /// <returns>A configured API response builder</returns>
    public static ApiResponseBuilder<object> NoContent(string? message = null)
    {
        return new ApiResponseBuilder<object>().ForNoContent(message);
    }

    /// <summary>
    /// Creates a bad request response builder
    /// </summary>
    /// <typeparam name="T">The type of data in the response</typeparam>
    /// <param name="message">The error message</param>
    /// <param name="errors">Optional error details</param>
    /// <returns>A configured API response builder</returns>
    public static ApiResponseBuilder<T> BadRequest<T>(string message, object? errors = null)
    {
        return new ApiResponseBuilder<T>().ForBadRequest(message, errors);
    }

    /// <summary>
    /// Creates a not found response builder
    /// </summary>
    /// <typeparam name="T">The type of data in the response</typeparam>
    /// <param name="message">The error message</param>
    /// <returns>A configured API response builder</returns>
    public static ApiResponseBuilder<T> NotFound<T>(string? message = null)
    {
        return new ApiResponseBuilder<T>().ForNotFound(message);
    }

    /// <summary>
    /// Creates an unauthorized response builder
    /// </summary>
    /// <typeparam name="T">The type of data in the response</typeparam>
    /// <param name="message">The error message</param>
    /// <returns>A configured API response builder</returns>
    public static ApiResponseBuilder<T> Unauthorized<T>(string? message = null)
    {
        return new ApiResponseBuilder<T>().ForUnauthorized(message);
    }

    /// <summary>
    /// Creates a forbidden response builder
    /// </summary>
    /// <typeparam name="T">The type of data in the response</typeparam>
    /// <param name="message">The error message</param>
    /// <returns>A configured API response builder</returns>
    public static ApiResponseBuilder<T> Forbidden<T>(string? message = null)
    {
        return new ApiResponseBuilder<T>().ForForbidden(message);
    }

    /// <summary>
    /// Creates a conflict response builder
    /// </summary>
    /// <typeparam name="T">The type of data in the response</typeparam>
    /// <param name="message">The error message</param>
    /// <param name="errors">Optional error details</param>
    /// <returns>A configured API response builder</returns>
    public static ApiResponseBuilder<T> Conflict<T>(string message, object? errors = null)
    {
        return new ApiResponseBuilder<T>().ForConflict(message, errors);
    }

    /// <summary>
    /// Creates an internal server error response builder
    /// </summary>
    /// <typeparam name="T">The type of data in the response</typeparam>
    /// <param name="message">The error message</param>
    /// <returns>A configured API response builder</returns>
    public static ApiResponseBuilder<T> InternalServerError<T>(string? message = null)
    {
        return new ApiResponseBuilder<T>().ForInternalServerError(message);
    }
} 