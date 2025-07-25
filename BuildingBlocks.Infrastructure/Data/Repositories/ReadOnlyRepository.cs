using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using BuildingBlocks.Domain.StronglyTypedIds;
using BuildingBlocks.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BuildingBlocks.Infrastructure.Data.Repositories;

/// <summary>
/// Read-only repository implementation using Entity Framework Core
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public class ReadOnlyRepository<TEntity, TId, TIdValue> : IReadOnlyRepository<TEntity, TId, TIdValue>
    where TEntity : Entity<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    protected readonly IDbContext Context;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly ILogger<ReadOnlyRepository<TEntity, TId, TIdValue>> Logger;

    /// <summary>
    /// Initializes a new instance of the ReadOnlyRepository class
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger</param>
    public ReadOnlyRepository(IDbContext context, ILogger<ReadOnlyRepository<TEntity, TId, TIdValue>> logger)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        DbSet = Context.Set<TEntity>();
    }

    /// <summary>
    /// Gets an entity by its identifier
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        if (id == null || id.Equals(default(TId)))
            return null;

        return await DbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// Gets multiple entities by their identifiers
    /// </summary>
    /// <param name="ids">The entity identifiers</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entities that were found</returns>
    public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        if (ids == null || !ids.Any())
            return Enumerable.Empty<TEntity>();

        var idList = ids.ToList();
        return await DbSet.AsNoTracking()
            .Where(e => idList.Contains(e.Id))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>All entities</returns>
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets entities that match the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Entities that match the predicate</returns>
    public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await DbSet.AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets entities that match the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Entities that match the specification</returns>
    public virtual async Task<IEnumerable<TEntity>> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        return await DbSet.AsNoTracking()
            .Where(specification.ToExpression())
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the first entity that matches the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The first entity that matches, null if none found</returns>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets the first entity that matches the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The first entity that matches, null if none found</returns>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Gets the single entity that matches the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The single entity that matches</returns>
    public virtual async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await DbSet.AsNoTracking()
            .SingleAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets the single entity that matches the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The single entity that matches</returns>
    public virtual async Task<TEntity> SingleAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        return await DbSet.AsNoTracking()
            .SingleAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Gets the single entity that matches the specified predicate, or null if none found
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The single entity that matches, null if none found</returns>
    public virtual async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await DbSet.AsNoTracking()
            .SingleOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets the single entity that matches the specified specification, or null if none found
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The single entity that matches, null if none found</returns>
    public virtual async Task<TEntity?> SingleOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        return await DbSet.AsNoTracking()
            .SingleOrDefaultAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Checks if any entity matches the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if any entity matches, false otherwise</returns>
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await DbSet.AsNoTracking()
            .AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Checks if any entity matches the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if any entity matches, false otherwise</returns>
    public virtual async Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        return await DbSet.AsNoTracking()
            .AnyAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Checks if any entities exist
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if any entities exist, false otherwise</returns>
    public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Counts entities that match the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of entities that match</returns>
    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await DbSet.AsNoTracking()
            .CountAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Counts entities that match the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of entities that match</returns>
    public virtual async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        return await DbSet.AsNoTracking()
            .CountAsync(specification.ToExpression(), cancellationToken);
    }

    /// <summary>
    /// Counts all entities
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The total number of entities</returns>
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().CountAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a paged result of entities
    /// </summary>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A paged result of entities</returns>
    public virtual async Task<IPagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await DbSet.AsNoTracking().CountAsync(cancellationToken);
        var skip = (pageNumber - 1) * pageSize;
        var items = await DbSet.AsNoTracking()
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TEntity>(items, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Gets a paged result of entities that match the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A paged result of entities</returns>
    public virtual async Task<IPagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        var query = DbSet.AsNoTracking().Where(predicate);
        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (pageNumber - 1) * pageSize;
        var items = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<TEntity>(items, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Gets a paged result of entities that match the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A paged result of entities</returns>
    public virtual async Task<IPagedResult<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        var query = DbSet.AsNoTracking().Where(specification.ToExpression());
        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (pageNumber - 1) * pageSize;
        var items = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<TEntity>(items, totalCount, pageNumber, pageSize);
    }
} 