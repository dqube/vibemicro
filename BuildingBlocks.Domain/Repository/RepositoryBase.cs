using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Specifications;
using BuildingBlocks.Domain.StronglyTypedIds;
using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Repository;

/// <summary>
/// Base class for repository implementations with strongly-typed identifiers
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public abstract class RepositoryBase<TEntity, TId, TIdValue> : IRepository<TEntity, TId, TIdValue>
    where TEntity : Entity<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    /// <summary>
    /// Gets an entity by its identifier
    /// </summary>
    public abstract Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple entities by their identifiers
    /// </summary>
    public abstract Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities
    /// </summary>
    public abstract Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities that match the specified predicate
    /// </summary>
    public abstract Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities that match the specified specification
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await GetAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Gets the first entity that matches the specified predicate
    /// </summary>
    public abstract Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity that matches the specified specification
    /// </summary>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Gets the single entity that matches the specified predicate
    /// </summary>
    public abstract Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the single entity that matches the specified specification
    /// </summary>
    public virtual async Task<TEntity> SingleAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await SingleAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Gets the single entity that matches the specified predicate, or null if none found
    /// </summary>
    public abstract Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the single entity that matches the specified specification, or null if none found
    /// </summary>
    public virtual async Task<TEntity?> SingleOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await SingleOrDefaultAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Checks if any entity matches the specified predicate
    /// </summary>
    public abstract Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity matches the specified specification
    /// </summary>
    public virtual async Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await AnyAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Checks if any entities exist
    /// </summary>
    public abstract Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities that match the specified predicate
    /// </summary>
    public abstract Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities that match the specified specification
    /// </summary>
    public virtual async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await CountAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Counts all entities
    /// </summary>
    public abstract Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged result of entities
    /// </summary>
    public abstract Task<IPagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged result of entities that match the specified predicate
    /// </summary>
    public abstract Task<IPagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged result of entities that match the specified specification
    /// </summary>
    public virtual async Task<IPagedResult<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(specification.ToExpression(), pageNumber, pageSize, cancellationToken);
    }

    /// <summary>
    /// Adds a new entity to the repository
    /// </summary>
    public abstract Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities to the repository
    /// </summary>
    public abstract Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the repository
    /// </summary>
    public abstract Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates multiple entities in the repository
    /// </summary>
    public abstract Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity from the repository
    /// </summary>
    public abstract Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity by its identifier
    /// </summary>
    public virtual async Task RemoveAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            await RemoveAsync(entity, cancellationToken);
        }
    }

    /// <summary>
    /// Removes multiple entities from the repository
    /// </summary>
    public abstract Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes entities that match the specified predicate
    /// </summary>
    public virtual async Task<int> RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await GetAsync(predicate, cancellationToken);
        var entityList = entities.ToList();
        await RemoveRangeAsync(entityList, cancellationToken);
        return entityList.Count;
    }

    /// <summary>
    /// Removes entities that match the specified specification
    /// </summary>
    public virtual async Task<int> RemoveAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await RemoveAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Validates an entity before performing operations
    /// </summary>
    /// <param name="entity">The entity to validate</param>
    /// <exception cref="ArgumentNullException">Thrown when entity is null</exception>
    protected virtual void ValidateEntity(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
    }

    /// <summary>
    /// Validates entities before performing operations
    /// </summary>
    /// <param name="entities">The entities to validate</param>
    /// <exception cref="ArgumentNullException">Thrown when entities is null</exception>
    /// <exception cref="ArgumentException">Thrown when entities contains null values</exception>
    protected virtual void ValidateEntities(IEnumerable<TEntity> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        var entityList = entities.ToList();
        if (entityList.Any(e => e == null))
            throw new ArgumentException("Entities collection cannot contain null values", nameof(entities));
    }
} 