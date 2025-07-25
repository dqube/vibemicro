namespace BuildingBlocks.Application.CQRS.Queries;

/// <summary>
/// Interface for handling queries
/// </summary>
/// <typeparam name="TQuery">The type of query to handle</typeparam>
/// <typeparam name="TResult">The type of result returned by the query</typeparam>
public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles the specified query
    /// </summary>
    /// <param name="query">The query to handle</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation with the result</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
} 