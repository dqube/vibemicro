using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BuildingBlocks.API.Middleware.ErrorHandling;

/// <summary>
/// Factory for creating RFC 7807 Problem Details responses
/// </summary>
public static class ProblemDetailsFactory
{
    /// <summary>
    /// Creates a ProblemDetails object from an ErrorResponse
    /// </summary>
    /// <param name="errorResponse">The error response</param>
    /// <param name="httpContext">The HTTP context</param>
    /// <returns>A ProblemDetails object</returns>
    public static ProblemDetails CreateProblemDetails(ErrorResponse errorResponse, HttpContext? httpContext = null)
    {
        var problemDetails = new ProblemDetails
        {
            Type = errorResponse.Type,
            Title = errorResponse.Title,
            Status = errorResponse.Status,
            Detail = errorResponse.Detail,
            Instance = errorResponse.Instance ?? httpContext?.Request.Path
        };

        // Add correlation ID
        if (!string.IsNullOrEmpty(errorResponse.CorrelationId))
        {
            problemDetails.Extensions["correlationId"] = errorResponse.CorrelationId;
        }

        // Add timestamp
        problemDetails.Extensions["timestamp"] = errorResponse.Timestamp;

        // Add validation errors if present
        if (errorResponse.Errors != null && errorResponse.Errors.Count > 0)
        {
            problemDetails.Extensions["errors"] = errorResponse.Errors;
        }

        // Add any additional extensions
        if (errorResponse.Extensions != null)
        {
            foreach (var extension in errorResponse.Extensions)
            {
                problemDetails.Extensions[extension.Key] = extension.Value;
            }
        }

        // Add trace ID if available
        var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
        if (!string.IsNullOrEmpty(traceId))
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        return problemDetails;
    }

    /// <summary>
    /// Creates a validation problem details object
    /// </summary>
    /// <param name="validationErrors">The validation errors</param>
    /// <param name="httpContext">The HTTP context</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <returns>A ValidationProblemDetails object</returns>
    public static ValidationProblemDetails CreateValidationProblemDetails(
        Dictionary<string, string[]> validationErrors,
        HttpContext? httpContext = null,
        string? correlationId = null)
    {
        var problemDetails = new ValidationProblemDetails(validationErrors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400,
            Detail = "Please refer to the errors property for additional details.",
            Instance = httpContext?.Request.Path
        };

        // Add correlation ID
        if (!string.IsNullOrEmpty(correlationId))
        {
            problemDetails.Extensions["correlationId"] = correlationId;
        }

        // Add timestamp
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        // Add trace ID if available
        var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
        if (!string.IsNullOrEmpty(traceId))
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        return problemDetails;
    }

    /// <summary>
    /// Creates a problem details object for exceptions
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <param name="httpContext">The HTTP context</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <param name="includeExceptionDetails">Whether to include exception details</param>
    /// <returns>A ProblemDetails object</returns>
    public static ProblemDetails CreateExceptionProblemDetails(
        Exception exception,
        HttpContext? httpContext = null,
        string? correlationId = null,
        bool includeExceptionDetails = false)
    {
        var (status, title, type) = GetExceptionInfo(exception);

        var problemDetails = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = status,
            Detail = includeExceptionDetails ? exception.Message : "An error occurred while processing your request.",
            Instance = httpContext?.Request.Path
        };

        // Add correlation ID
        if (!string.IsNullOrEmpty(correlationId))
        {
            problemDetails.Extensions["correlationId"] = correlationId;
        }

        // Add timestamp
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        // Add trace ID if available
        var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
        if (!string.IsNullOrEmpty(traceId))
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        // Add exception details in development
        if (includeExceptionDetails)
        {
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;

            if (exception.InnerException != null)
            {
                problemDetails.Extensions["innerException"] = new
                {
                    type = exception.InnerException.GetType().Name,
                    message = exception.InnerException.Message
                };
            }
        }

        return problemDetails;
    }

    /// <summary>
    /// Gets exception information for problem details
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns>Status code, title, and type</returns>
    private static (int status, string title, string type) GetExceptionInfo(Exception exception)
    {
        return exception switch
        {
            ArgumentException => (400, "Bad Request", "https://tools.ietf.org/html/rfc7231#section-6.5.1"),
            ArgumentNullException => (400, "Bad Request", "https://tools.ietf.org/html/rfc7231#section-6.5.1"),
            InvalidOperationException => (400, "Bad Request", "https://tools.ietf.org/html/rfc7231#section-6.5.1"),
            UnauthorizedAccessException => (401, "Unauthorized", "https://tools.ietf.org/html/rfc7235#section-3.1"),
            NotImplementedException => (501, "Not Implemented", "https://tools.ietf.org/html/rfc7231#section-6.6.2"),
            TimeoutException => (408, "Request Timeout", "https://tools.ietf.org/html/rfc7231#section-6.5.7"),
            OperationCanceledException => (499, "Client Closed Request", "https://httpstatuses.com/499"),
            TaskCanceledException => (499, "Client Closed Request", "https://httpstatuses.com/499"),
            _ => (500, "Internal Server Error", "https://tools.ietf.org/html/rfc7231#section-6.6.1")
        };
    }

    /// <summary>
    /// Creates a problem details object for specific HTTP status codes
    /// </summary>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="detail">The error detail</param>
    /// <param name="httpContext">The HTTP context</param>
    /// <param name="correlationId">The correlation ID</param>
    /// <returns>A ProblemDetails object</returns>
    public static ProblemDetails CreateStatusCodeProblemDetails(
        int statusCode,
        string? detail = null,
        HttpContext? httpContext = null,
        string? correlationId = null)
    {
        var (title, type) = GetStatusCodeInfo(statusCode);

        var problemDetails = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = statusCode,
            Detail = detail ?? GetDefaultDetailForStatusCode(statusCode),
            Instance = httpContext?.Request.Path
        };

        // Add correlation ID
        if (!string.IsNullOrEmpty(correlationId))
        {
            problemDetails.Extensions["correlationId"] = correlationId;
        }

        // Add timestamp
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        // Add trace ID if available
        var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
        if (!string.IsNullOrEmpty(traceId))
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        return problemDetails;
    }

    /// <summary>
    /// Gets title and type for HTTP status codes
    /// </summary>
    /// <param name="statusCode">The HTTP status code</param>
    /// <returns>Title and type</returns>
    private static (string title, string type) GetStatusCodeInfo(int statusCode)
    {
        return statusCode switch
        {
            400 => ("Bad Request", "https://tools.ietf.org/html/rfc7231#section-6.5.1"),
            401 => ("Unauthorized", "https://tools.ietf.org/html/rfc7235#section-3.1"),
            403 => ("Forbidden", "https://tools.ietf.org/html/rfc7231#section-6.5.3"),
            404 => ("Not Found", "https://tools.ietf.org/html/rfc7231#section-6.5.4"),
            405 => ("Method Not Allowed", "https://tools.ietf.org/html/rfc7231#section-6.5.5"),
            406 => ("Not Acceptable", "https://tools.ietf.org/html/rfc7231#section-6.5.6"),
            408 => ("Request Timeout", "https://tools.ietf.org/html/rfc7231#section-6.5.7"),
            409 => ("Conflict", "https://tools.ietf.org/html/rfc7231#section-6.5.8"),
            410 => ("Gone", "https://tools.ietf.org/html/rfc7231#section-6.5.9"),
            422 => ("Unprocessable Entity", "https://tools.ietf.org/html/rfc4918#section-11.2"),
            429 => ("Too Many Requests", "https://tools.ietf.org/html/rfc6585#section-4"),
            500 => ("Internal Server Error", "https://tools.ietf.org/html/rfc7231#section-6.6.1"),
            501 => ("Not Implemented", "https://tools.ietf.org/html/rfc7231#section-6.6.2"),
            502 => ("Bad Gateway", "https://tools.ietf.org/html/rfc7231#section-6.6.3"),
            503 => ("Service Unavailable", "https://tools.ietf.org/html/rfc7231#section-6.6.4"),
            504 => ("Gateway Timeout", "https://tools.ietf.org/html/rfc7231#section-6.6.5"),
            _ => ("Error", "about:blank")
        };
    }

    /// <summary>
    /// Gets default detail message for HTTP status codes
    /// </summary>
    /// <param name="statusCode">The HTTP status code</param>
    /// <returns>Default detail message</returns>
    private static string GetDefaultDetailForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => "The request could not be understood by the server due to malformed syntax.",
            401 => "The request has not been applied because it lacks valid authentication credentials.",
            403 => "The server understood the request but refuses to authorize it.",
            404 => "The requested resource could not be found.",
            405 => "The method specified in the request is not allowed for the resource.",
            406 => "The server cannot produce a response matching the list of acceptable values.",
            408 => "The server timed out waiting for the request.",
            409 => "The request could not be completed due to a conflict with the current state of the resource.",
            410 => "The requested resource is no longer available and will not be available again.",
            422 => "The server understands the content type of the request entity, but was unable to process the contained instructions.",
            429 => "The user has sent too many requests in a given amount of time.",
            500 => "The server encountered an unexpected condition that prevented it from fulfilling the request.",
            501 => "The server does not support the functionality required to fulfill the request.",
            502 => "The server, while acting as a gateway or proxy, received an invalid response from the upstream server.",
            503 => "The server is currently unable to handle the request due to a temporary overloading or maintenance of the server.",
            504 => "The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server.",
            _ => "An error occurred while processing the request."
        };
    }
} 