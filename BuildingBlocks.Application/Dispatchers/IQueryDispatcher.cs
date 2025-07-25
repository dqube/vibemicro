using BuildingBlocks.Application.CQRS.Queries;

namespace BuildingBlocks.Application.Dispatchers;

/// <summary>
/// Interface for dispatching queries
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches a query and returns the result
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="query">The query to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The query result</returns>
    Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;
} 