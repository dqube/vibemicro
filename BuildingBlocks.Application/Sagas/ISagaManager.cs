using BuildingBlocks.Application.CQRS.Events;

namespace BuildingBlocks.Application.Sagas;

/// <summary>
/// Interface for managing saga lifecycle
/// </summary>
public interface ISagaManager
{
    /// <summary>
    /// Starts a new saga
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <param name="correlationId">The correlation identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created saga</returns>
    Task<TSaga> StartSagaAsync<TSaga>(string correlationId, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga;

    /// <summary>
    /// Gets a saga by its identifier
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <param name="sagaId">The saga identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The saga if found, null otherwise</returns>
    Task<TSaga?> GetSagaAsync<TSaga>(Guid sagaId, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga;

    /// <summary>
    /// Gets a saga by its correlation identifier
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <param name="correlationId">The correlation identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The saga if found, null otherwise</returns>
    Task<TSaga?> GetSagaByCorrelationIdAsync<TSaga>(string correlationId, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga;

    /// <summary>
    /// Saves saga state
    /// </summary>
    /// <param name="saga">The saga to save</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task SaveSagaAsync(ISaga saga, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles an event by finding appropriate sagas and routing the event to them
    /// </summary>
    /// <param name="eventItem">The event to handle</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of sagas that handled the event</returns>
    Task<int> HandleEventAsync(IEvent eventItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active sagas
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of active sagas</returns>
    Task<IEnumerable<ISaga>> GetActiveSagasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes and removes finished sagas
    /// </summary>
    /// <param name="olderThan">Complete sagas older than this date</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of sagas cleaned up</returns>
    Task<int> CleanupCompletedSagasAsync(DateTime olderThan, CancellationToken cancellationToken = default);
} 