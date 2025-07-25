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
/// Generic repository implementation using Entity Framework Core
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public class Repository<TEntity, TId, TIdValue> : RepositoryBase<TEntity, TId, TIdValue>
    where TEntity : Entity<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    private readonly IDbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly ILogger<Repository<TEntity, TId, TIdValue>> _logger;

    /// <summary>
    /// Initializes a new instance of the Repository class
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger</param>
    public Repository(IDbContext context, ILogger<Repository<TEntity, TId, TIdValue>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbSet = _context.Set<TEntity>();
    }

    /// <summary>
    /// Gets an entity by its identifier
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    public override async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        if (id == null || id.Equals(default(TId)))
            return null;

        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// Gets multiple entities by their identifiers
    /// </summary>
    /// <param name="ids">The entity identifiers</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entities that were found</returns>
    public override async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        if (ids == null || !ids.Any())
            return Enumerable.Empty<TEntity>();

        var idList = ids.ToList();
        return await _dbSet.Where(e => idList.Contains(e.Id)).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>All entities</returns>
    public override async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets entities that match the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Entities that match the predicate</returns>
    public override async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the first entity that matches the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The first entity that matches, null if none found</returns>
    public override async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets the single entity that matches the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The single entity that matches</returns>
    public override async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await _dbSet.SingleAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets the single entity that matches the specified predicate, or null if none found
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The single entity that matches, null if none found</returns>
    public override async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Checks if any entity matches the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if any entity matches, false otherwise</returns>
    public override async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Checks if any entities exist
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if any entities exist, false otherwise</returns>
    public override async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Counts entities that match the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of entities that match</returns>
    public override async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Counts all entities
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The total number of entities</returns>
    public override async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a paged result of entities
    /// </summary>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A paged result of entities</returns>
    public override async Task<IPagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);
        var skip = (pageNumber - 1) * pageSize;
        var items = await _dbSet.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

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
    public override async Task<IPagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        var query = _dbSet.Where(predicate);
        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (pageNumber - 1) * pageSize;
        var items = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<TEntity>(items, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Adds a new entity to the repository
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The added entity</returns>
    public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ValidateEntity(entity);

        var entry = await _context.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    /// <summary>
    /// Adds multiple entities to the repository
    /// </summary>
    /// <param name="entities">The entities to add</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public override Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ValidateEntities(entities);

        _dbSet.AddRange(entities);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing entity in the repository
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated entity</returns>
    public override Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ValidateEntity(entity);

        var entry = _context.Update(entity);
        return Task.FromResult(entry.Entity);
    }

    /// <summary>
    /// Updates multiple entities in the repository
    /// </summary>
    /// <param name="entities">The entities to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public override Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ValidateEntities(entities);

        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes an entity from the repository
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public override Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ValidateEntity(entity);

        _context.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes multiple entities from the repository
    /// </summary>
    /// <param name="entities">The entities to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public override Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ValidateEntities(entities);

        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Implementation of IPagedResult for Entity Framework Core
/// </summary>
/// <typeparam name="T">The item type</typeparam>
public class PagedResult<T> : IPagedResult<T>
{
    /// <summary>
    /// Gets the items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; }

    /// <summary>
    /// Gets the current page number (1-based)
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the page size
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of items
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets the total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Initializes a new instance of the PagedResult class
    /// </summary>
    /// <param name="items">The items</param>
    /// <param name="totalCount">The total count</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The page size</param>
    public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
} 