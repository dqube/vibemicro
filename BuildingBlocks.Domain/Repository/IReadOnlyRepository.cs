using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Specifications;
using BuildingBlocks.Domain.StronglyTypedIds;
using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Repository;

/// <summary>
/// Interface for read-only repository operations with strongly-typed identifiers
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public interface IReadOnlyRepository<TEntity, TId, TIdValue>
    where TEntity : Entity<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    /// <summary>
    /// Gets an entity by its identifier
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple entities by their identifiers
    /// </summary>
    /// <param name="ids">The entity identifiers</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entities that were found</returns>
    Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>All entities</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities that match the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Entities that match the predicate</returns>
    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities that match the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Entities that match the specification</returns>
    Task<IEnumerable<TEntity>> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity that matches the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The first entity that matches, null if none found</returns>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity that matches the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The first entity that matches, null if none found</returns>
    Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the single entity that matches the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The single entity that matches</returns>
    /// <exception cref="InvalidOperationException">Thrown when zero or more than one entity matches</exception>
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the single entity that matches the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The single entity that matches</returns>
    /// <exception cref="InvalidOperationException">Thrown when zero or more than one entity matches</exception>
    Task<TEntity> SingleAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the single entity that matches the specified predicate, or null if none found
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The single entity that matches, null if none found</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one entity matches</exception>
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the single entity that matches the specified specification, or null if none found
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The single entity that matches, null if none found</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one entity matches</exception>
    Task<TEntity?> SingleOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity matches the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if any entity matches, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity matches the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if any entity matches, false otherwise</returns>
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entities exist
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if any entities exist, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities that match the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of entities that match</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities that match the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of entities that match</returns>
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts all entities
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The total number of entities</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged result of entities
    /// </summary>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A paged result of entities</returns>
    Task<IPagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged result of entities that match the specified predicate
    /// </summary>
    /// <param name="predicate">The predicate to match</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A paged result of entities</returns>
    Task<IPagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged result of entities that match the specified specification
    /// </summary>
    /// <param name="specification">The specification to match</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A paged result of entities</returns>
    Task<IPagedResult<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for paged results
/// </summary>
/// <typeparam name="T">The item type</typeparam>
public interface IPagedResult<T>
{
    /// <summary>
    /// Gets the items in the current page
    /// </summary>
    IEnumerable<T> Items { get; }

    /// <summary>
    /// Gets the current page number (1-based)
    /// </summary>
    int PageNumber { get; }

    /// <summary>
    /// Gets the page size
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Gets the total number of items
    /// </summary>
    int TotalCount { get; }

    /// <summary>
    /// Gets the total number of pages
    /// </summary>
    int TotalPages { get; }

    /// <summary>
    /// Gets whether there is a previous page
    /// </summary>
    bool HasPreviousPage { get; }

    /// <summary>
    /// Gets whether there is a next page
    /// </summary>
    bool HasNextPage { get; }
} 