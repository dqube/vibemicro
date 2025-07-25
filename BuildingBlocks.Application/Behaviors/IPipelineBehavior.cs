namespace BuildingBlocks.Application.Behaviors;

/// <summary>
/// Interface for pipeline behaviors that can intercept command/query execution
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
{
    /// <summary>
    /// Handles the request and calls the next behavior in the pipeline
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="next">The next behavior in the pipeline</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The response</returns>
    Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default);
}

/// <summary>
/// Delegate for handling requests in the pipeline
/// </summary>
/// <typeparam name="TResponse">The response type</typeparam>
/// <returns>The response</returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(); 