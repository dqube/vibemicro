using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Dispatchers;

/// <summary>
/// Default implementation of query dispatcher
/// </summary>
public class QueryDispatcher : IQueryDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILogger<QueryDispatcher> _logger;

    /// <summary>
    /// Initializes a new instance of the QueryDispatcher class
    /// </summary>
    public QueryDispatcher(IMediator mediator, ILogger<QueryDispatcher> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Dispatches a query and returns the result
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="query">The query to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The query result</returns>
    public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        _logger.LogDebug("Dispatching query {QueryType}: {@Query}", typeof(TQuery).Name, query);

        try
        {
            var result = await _mediator.QueryAsync<TQuery, TResult>(query, cancellationToken);
            _logger.LogDebug("Successfully dispatched query {QueryType} with result: {@Result}", typeof(TQuery).Name, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch query {QueryType}: {Error}", typeof(TQuery).Name, ex.Message);
            throw;
        }
    }
} 