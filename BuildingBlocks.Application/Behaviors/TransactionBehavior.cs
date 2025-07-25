using BuildingBlocks.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Behaviors;

/// <summary>
/// Pipeline behavior for transaction management
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the TransactionBehavior class
    /// </summary>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="logger">The logger</param>
    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the request with transaction management
    /// </summary>
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        // Only use transactions for requests that implement ITransactional
        if (request is not ITransactional)
        {
            return await next();
        }

        var requestName = typeof(TRequest).Name;
        
        _logger.LogDebug("Starting transaction for {RequestName}", requestName);

        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var response = await next();
                
                _logger.LogDebug("Committing transaction for {RequestName}", requestName);
                
                return response;
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}", requestName);
            throw;
        }
    }
}

/// <summary>
/// Marker interface for requests that should run in a transaction
/// </summary>
public interface ITransactional
{
} 