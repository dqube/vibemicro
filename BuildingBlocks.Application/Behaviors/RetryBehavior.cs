using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Behaviors;

/// <summary>
/// Pipeline behavior for retry logic
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<RetryBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the RetryBehavior class
    /// </summary>
    /// <param name="logger">The logger</param>
    public RetryBehavior(ILogger<RetryBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the request with retry logic
    /// </summary>
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        // Only retry if request implements IRetryable
        if (request is not IRetryable retryableRequest)
        {
            return await next();
        }

        var requestName = typeof(TRequest).Name;
        var maxAttempts = retryableRequest.MaxRetryAttempts;
        var retryDelay = retryableRequest.RetryDelay;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                _logger.LogDebug("Executing {RequestName}, attempt {Attempt}/{MaxAttempts}", 
                    requestName, attempt, maxAttempts);

                var response = await next();
                
                if (attempt > 1)
                {
                    _logger.LogInformation("Request {RequestName} succeeded on attempt {Attempt}", 
                        requestName, attempt);
                }

                return response;
            }
            catch (Exception ex) when (attempt < maxAttempts && retryableRequest.ShouldRetry(ex))
            {
                _logger.LogWarning(ex, "Request {RequestName} failed on attempt {Attempt}/{MaxAttempts}, retrying in {RetryDelay}ms", 
                    requestName, attempt, maxAttempts, retryDelay.TotalMilliseconds);

                await Task.Delay(retryDelay, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request {RequestName} failed on final attempt {Attempt}/{MaxAttempts}", 
                    requestName, attempt, maxAttempts);
                throw;
            }
        }

        // This should never be reached, but included for completeness
        throw new InvalidOperationException($"Retry logic failed for {requestName}");
    }
}

/// <summary>
/// Interface for retryable requests
/// </summary>
public interface IRetryable
{
    /// <summary>
    /// Gets the maximum number of retry attempts
    /// </summary>
    int MaxRetryAttempts { get; }

    /// <summary>
    /// Gets the delay between retry attempts
    /// </summary>
    TimeSpan RetryDelay { get; }

    /// <summary>
    /// Determines whether the request should be retried for the given exception
    /// </summary>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>True if the request should be retried</returns>
    bool ShouldRetry(Exception exception);
} 