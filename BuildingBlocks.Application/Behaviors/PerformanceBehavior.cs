using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Application.Behaviors;

/// <summary>
/// Pipeline behavior for performance monitoring
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly long _performanceThresholdMs;

    /// <summary>
    /// Initializes a new instance of the PerformanceBehavior class
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="performanceThresholdMs">The performance threshold in milliseconds (default: 500ms)</param>
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger, long performanceThresholdMs = 500)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _performanceThresholdMs = performanceThresholdMs;
    }

    /// <summary>
    /// Handles the request with performance monitoring
    /// </summary>
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (elapsedMs > _performanceThresholdMs)
            {
                _logger.LogWarning("Long running request {RequestName} took {ElapsedMilliseconds}ms (threshold: {ThresholdMs}ms)",
                    requestName, elapsedMs, _performanceThresholdMs);

                // If request implements IPerformanceAware, notify it about slow execution
                if (request is IPerformanceAware performanceAware)
                {
                    await performanceAware.OnSlowExecutionAsync(elapsedMs, cancellationToken);
                }
            }
            else
            {
                _logger.LogDebug("Request {RequestName} completed in {ElapsedMilliseconds}ms", 
                    requestName, elapsedMs);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Request {RequestName} failed after {ElapsedMilliseconds}ms", 
                requestName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}

/// <summary>
/// Interface for requests that are aware of their performance characteristics
/// </summary>
public interface IPerformanceAware
{
    /// <summary>
    /// Called when the request execution is slower than expected
    /// </summary>
    /// <param name="elapsedMs">The elapsed time in milliseconds</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task OnSlowExecutionAsync(long elapsedMs, CancellationToken cancellationToken = default);
} 