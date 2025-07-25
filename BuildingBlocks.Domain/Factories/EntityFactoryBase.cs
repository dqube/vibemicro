using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Domain.Factories;

/// <summary>
/// Base class for entity factories providing common functionality
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public abstract class EntityFactoryBase<TEntity, TId, TIdValue> : IEntityFactory<TEntity, TId, TIdValue>
    where TEntity : Entity<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    /// <summary>
    /// Gets the logger instance
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the EntityFactoryBase class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    protected EntityFactoryBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new entity with default values
    /// </summary>
    /// <returns>The created entity</returns>
    public abstract TEntity Create();

    /// <summary>
    /// Creates a new entity asynchronously with default values
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created entity</returns>
    public virtual async Task<TEntity> CreateAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Create());
    }

    /// <summary>
    /// Logs the creation of an entity
    /// </summary>
    /// <param name="entity">The created entity</param>
    protected void LogEntityCreation(TEntity entity)
    {
        Logger.LogInformation("Created entity {EntityType} with ID: {EntityId}",
            typeof(TEntity).Name, entity.Id);
    }

    /// <summary>
    /// Logs an error during entity creation
    /// </summary>
    /// <param name="exception">The exception that occurred</param>
    protected void LogEntityCreationError(Exception exception)
    {
        Logger.LogError(exception, "Error creating entity {EntityType}", typeof(TEntity).Name);
    }
}

/// <summary>
/// Base class for aggregate root factories providing common functionality
/// </summary>
/// <typeparam name="TAggregateRoot">The aggregate root type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public abstract class AggregateFactoryBase<TAggregateRoot, TId, TIdValue> : EntityFactoryBase<TAggregateRoot, TId, TIdValue>, IAggregateFactory<TAggregateRoot, TId, TIdValue>
    where TAggregateRoot : AggregateRoot<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    /// <summary>
    /// Initializes a new instance of the AggregateFactoryBase class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    protected AggregateFactoryBase(ILogger logger) : base(logger)
    {
    }

    /// <summary>
    /// Creates a new aggregate root with the specified identifier
    /// </summary>
    /// <param name="id">The aggregate identifier</param>
    /// <returns>The created aggregate root</returns>
    public abstract TAggregateRoot CreateWithId(TId id);

    /// <summary>
    /// Creates a new aggregate root with the specified identifier asynchronously
    /// </summary>
    /// <param name="id">The aggregate identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created aggregate root</returns>
    public virtual async Task<TAggregateRoot> CreateWithIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(CreateWithId(id));
    }

    /// <summary>
    /// Logs the creation of an aggregate root
    /// </summary>
    /// <param name="aggregate">The created aggregate root</param>
    protected void LogAggregateCreation(TAggregateRoot aggregate)
    {
        Logger.LogInformation("Created aggregate root {AggregateType} with ID: {AggregateId}, Version: {Version}",
            typeof(TAggregateRoot).Name, aggregate.Id, aggregate.Version);
    }
} 