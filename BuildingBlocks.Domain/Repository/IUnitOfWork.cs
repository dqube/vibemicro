using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;

namespace BuildingBlocks.Domain.Repository;

/// <summary>
/// Interface for unit of work pattern with strongly-typed identifiers
/// </summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets a repository for the specified entity type with strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed identifier type</typeparam>
    /// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
    /// <returns>The repository instance</returns>
    IRepository<TEntity, TId, TIdValue> Repository<TEntity, TId, TIdValue>()
        where TEntity : Entity<TId, TIdValue>
        where TId : IStronglyTypedId<TIdValue>
        where TIdValue : notnull;

    /// <summary>
    /// Gets a read-only repository for the specified entity type with strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed identifier type</typeparam>
    /// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
    /// <returns>The read-only repository instance</returns>
    IReadOnlyRepository<TEntity, TId, TIdValue> ReadOnlyRepository<TEntity, TId, TIdValue>()
        where TEntity : Entity<TId, TIdValue>
        where TId : IStronglyTypedId<TIdValue>
        where TIdValue : notnull;

    /// <summary>
    /// Gets a repository for entities with integer-based strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed integer ID type</typeparam>
    /// <returns>The repository instance</returns>
    IIntRepository<TEntity, TId> IntRepository<TEntity, TId>()
        where TEntity : IntEntity<TId>
        where TId : struct, IStronglyTypedId<int>;

    /// <summary>
    /// Gets a repository for entities with long-based strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed long ID type</typeparam>
    /// <returns>The repository instance</returns>
    ILongRepository<TEntity, TId> LongRepository<TEntity, TId>()
        where TEntity : LongEntity<TId>
        where TId : struct, IStronglyTypedId<long>;

    /// <summary>
    /// Gets a repository for entities with GUID-based strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed GUID ID type</typeparam>
    /// <returns>The repository instance</returns>
    IGuidRepository<TEntity, TId> GuidRepository<TEntity, TId>()
        where TEntity : GuidEntity<TId>
        where TId : struct, IStronglyTypedId<Guid>;

    /// <summary>
    /// Gets a repository for entities with string-based strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed string ID type</typeparam>
    /// <returns>The repository instance</returns>
    IStringRepository<TEntity, TId> StringRepository<TEntity, TId>()
        where TEntity : StringEntity<TId>
        where TId : struct, IStronglyTypedId<string>;

    /// <summary>
    /// Saves all changes made in this unit of work
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of entities saved</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The transaction instance</returns>
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a function within a transaction
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="func">The function to execute</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result of the function</returns>
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action within a transaction
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for database transactions
/// </summary>
public interface ITransaction : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Commits the transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);
} 