using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;

namespace BuildingBlocks.Domain.Factories;

/// <summary>
/// Interface for entity factories
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public interface IEntityFactory<TEntity, TId, TIdValue>
    where TEntity : Entity<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    /// <summary>
    /// Creates a new entity with default values
    /// </summary>
    /// <returns>The created entity</returns>
    TEntity Create();

    /// <summary>
    /// Creates a new entity asynchronously with default values
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created entity</returns>
    Task<TEntity> CreateAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for aggregate root factories
/// </summary>
/// <typeparam name="TAggregateRoot">The aggregate root type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public interface IAggregateFactory<TAggregateRoot, TId, TIdValue> : IEntityFactory<TAggregateRoot, TId, TIdValue>
    where TAggregateRoot : AggregateRoot<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    /// <summary>
    /// Creates a new aggregate root with the specified identifier
    /// </summary>
    /// <param name="id">The aggregate identifier</param>
    /// <returns>The created aggregate root</returns>
    TAggregateRoot CreateWithId(TId id);

    /// <summary>
    /// Creates a new aggregate root with the specified identifier asynchronously
    /// </summary>
    /// <param name="id">The aggregate identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created aggregate root</returns>
    Task<TAggregateRoot> CreateWithIdAsync(TId id, CancellationToken cancellationToken = default);
} 