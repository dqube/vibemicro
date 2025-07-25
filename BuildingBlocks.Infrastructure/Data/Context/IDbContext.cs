using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BuildingBlocks.Infrastructure.Data.Context;

/// <summary>
/// Interface for database context
/// </summary>
public interface IDbContext : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the database
    /// </summary>
    DatabaseFacade Database { get; }

    /// <summary>
    /// Gets the change tracker
    /// </summary>
    ChangeTracker ChangeTracker { get; }

    /// <summary>
    /// Gets a DbSet for the specified entity type
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <returns>The DbSet</returns>
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    /// <summary>
    /// Creates a DbSet for the specified entity type
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <returns>The DbSet</returns>
    DbSet<TEntity> Set<TEntity>(string name) where TEntity : class;

    /// <summary>
    /// Finds an entity by its key values
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="keyValues">The key values</param>
    /// <returns>The entity if found, null otherwise</returns>
    TEntity? Find<TEntity>(params object[] keyValues) where TEntity : class;

    /// <summary>
    /// Finds an entity by its key values asynchronously
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="keyValues">The key values</param>
    /// <returns>The entity if found, null otherwise</returns>
    ValueTask<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;

    /// <summary>
    /// Finds an entity by its key values asynchronously
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="keyValues">The key values</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    ValueTask<TEntity?> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken) where TEntity : class;

    /// <summary>
    /// Adds an entity to the context
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="entity">The entity to add</param>
    /// <returns>The entity entry</returns>
    EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Adds an entity to the context asynchronously
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entity entry</returns>
    ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;

    /// <summary>
    /// Updates an entity in the context
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="entity">The entity to update</param>
    /// <returns>The entity entry</returns>
    EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Removes an entity from the context
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="entity">The entity to remove</param>
    /// <returns>The entity entry</returns>
    EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Saves all changes made in this context
    /// </summary>
    /// <returns>The number of state entries written to the database</returns>
    int SaveChanges();

    /// <summary>
    /// Saves all changes made in this context asynchronously
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    /// <returns>The database transaction</returns>
    Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a SQL command
    /// </summary>
    /// <param name="sql">The SQL command</param>
    /// <param name="parameters">The parameters</param>
    /// <returns>The number of rows affected</returns>
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);

    /// <summary>
    /// Executes a SQL command
    /// </summary>
    /// <param name="sql">The SQL command</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of rows affected</returns>
    Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken);
} 