using BuildingBlocks.API.Responses.Base;
using BuildingBlocks.Application.CQRS.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.API.Endpoints.Base;

/// <summary>
/// Base class for API endpoints
/// </summary>
public abstract class EndpointBase
{
    /// <summary>
    /// Gets the mediator
    /// </summary>
    protected IMediator Mediator { get; }

    /// <summary>
    /// Gets the logger
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the EndpointBase class
    /// </summary>
    /// <param name="mediator">The mediator</param>
    /// <param name="logger">The logger</param>
    protected EndpointBase(IMediator mediator, ILogger logger)
    {
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a successful API response
    /// </summary>
    /// <typeparam name="T">The data type</typeparam>
    /// <param name="data">The response data</param>
    /// <param name="message">The response message</param>
    /// <returns>The API response</returns>
    protected static ApiResponse<T> Success<T>(T data, string? message = null)
    {
        return ApiResponse<T>.Success(data, message);
    }

    /// <summary>
    /// Creates a successful API response without data
    /// </summary>
    /// <param name="message">The response message</param>
    /// <returns>The API response</returns>
    protected static ApiResponse Success(string? message = null)
    {
        return ApiResponse.Success(message);
    }

    /// <summary>
    /// Creates an error API response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="errors">The validation errors</param>
    /// <returns>The API response</returns>
    protected static ApiResponse Error(string message, Dictionary<string, string[]>? errors = null)
    {
        return ApiResponse.Error(message, errors);
    }

    /// <summary>
    /// Creates an error API response with data
    /// </summary>
    /// <typeparam name="T">The data type</typeparam>
    /// <param name="message">The error message</param>
    /// <param name="errors">The validation errors</param>
    /// <returns>The API response</returns>
    protected static ApiResponse<T> Error<T>(string message, Dictionary<string, string[]>? errors = null)
    {
        return ApiResponse<T>.Error(message, errors);
    }

    /// <summary>
    /// Creates a not found API response
    /// </summary>
    /// <param name="message">The not found message</param>
    /// <returns>The API response</returns>
    protected static ApiResponse NotFound(string? message = null)
    {
        return ApiResponse.NotFound(message ?? "Resource not found");
    }

    /// <summary>
    /// Creates a not found API response with data type
    /// </summary>
    /// <typeparam name="T">The data type</typeparam>
    /// <param name="message">The not found message</param>
    /// <returns>The API response</returns>
    protected static ApiResponse<T> NotFound<T>(string? message = null)
    {
        return ApiResponse<T>.NotFound(message ?? "Resource not found");
    }

    /// <summary>
    /// Creates a validation error API response
    /// </summary>
    /// <param name="errors">The validation errors</param>
    /// <returns>The API response</returns>
    protected static ApiResponse ValidationError(Dictionary<string, string[]> errors)
    {
        return ApiResponse.ValidationError(errors);
    }

    /// <summary>
    /// Handles exceptions and returns appropriate error response
    /// </summary>
    /// <param name="ex">The exception</param>
    /// <param name="context">The error context</param>
    /// <returns>The error response</returns>
    protected IResult HandleException(Exception ex, string context = "")
    {
        Logger.LogError(ex, "Error occurred in {Context}", context);

        return ex switch
        {
            ArgumentException argEx => Results.BadRequest(Error(argEx.Message)),
            UnauthorizedAccessException => Results.Unauthorized(),
            KeyNotFoundException => Results.NotFound(NotFound()),
            InvalidOperationException invalidEx => Results.BadRequest(Error(invalidEx.Message)),
            _ => Results.Problem(
                title: "An error occurred",
                detail: "An unexpected error occurred while processing your request",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Executes an operation safely with exception handling
    /// </summary>
    /// <typeparam name="T">The result type</typeparam>
    /// <param name="operation">The operation to execute</param>
    /// <param name="context">The operation context</param>
    /// <returns>The operation result</returns>
    protected async Task<IResult> SafeExecuteAsync<T>(Func<Task<T>> operation, string context = "")
    {
        try
        {
            var result = await operation();
            return Results.Ok(Success(result));
        }
        catch (Exception ex)
        {
            return HandleException(ex, context);
        }
    }

    /// <summary>
    /// Executes an operation safely with exception handling (no return value)
    /// </summary>
    /// <param name="operation">The operation to execute</param>
    /// <param name="context">The operation context</param>
    /// <returns>The operation result</returns>
    protected async Task<IResult> SafeExecuteAsync(Func<Task> operation, string context = "")
    {
        try
        {
            await operation();
            return Results.Ok(Success("Operation completed successfully"));
        }
        catch (Exception ex)
        {
            return HandleException(ex, context);
        }
    }
}

/// <summary>
/// Base class for typed API endpoints
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public abstract class EndpointBase<TRequest, TResponse> : EndpointBase
{
    /// <summary>
    /// Initializes a new instance of the EndpointBase class
    /// </summary>
    /// <param name="mediator">The mediator</param>
    /// <param name="logger">The logger</param>
    protected EndpointBase(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    /// <summary>
    /// Handles the endpoint request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The response</returns>
    public abstract Task<IResult> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
} 