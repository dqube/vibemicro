using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Specifications;
using BuildingBlocks.Domain.StronglyTypedIds;
using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Repository;

/// <summary>
/// Interface for generic repository pattern with strongly-typed identifiers
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public interface IRepository<TEntity, TId, TIdValue> : IReadOnlyRepository<TEntity, TId, TIdValue>
    where TEntity : Entity<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    /// <summary>
    /// Adds a new entity to the repository
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities to the repository
    /// </summary>
    /// <param name="entities">The entities to add</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the repository
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates multiple entities in the repository
    /// </summary>
    /// <param name="entities">The entities to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity from the repository
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity by its identifier
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RemoveAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes multiple entities from the repository
    /// </summary>
    /// <param name="entities">The entities to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes entities that match the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of entities removed</returns>
    Task<int> RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes entities that match the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of entities removed</returns>
    Task<int> RemoveAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for repositories with integer-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed integer ID type</typeparam>
public interface IIntRepository<TEntity, TId> : IRepository<TEntity, TId, int>
    where TEntity : IntEntity<TId>
    where TId : IntId<TId>
{
}

/// <summary>
/// Interface for repositories with long-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed long ID type</typeparam>
public interface ILongRepository<TEntity, TId> : IRepository<TEntity, TId, long>
    where TEntity : LongEntity<TId>
    where TId : LongId<TId>
{
}

/// <summary>
/// Interface for repositories with GUID-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed GUID ID type</typeparam>
public interface IGuidRepository<TEntity, TId> : IRepository<TEntity, TId, Guid>
    where TEntity : GuidEntity<TId>
    where TId : GuidId<TId>
{
}

/// <summary>
/// Interface for repositories with string-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed string ID type</typeparam>
public interface IStringRepository<TEntity, TId> : IRepository<TEntity, TId, string>
    where TEntity : StringEntity<TId>
    where TId : StringId<TId>
{
} 