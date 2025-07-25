using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.StronglyTypedIds;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Data.UnitOfWork;

/// <summary>
/// Unit of Work implementation using Entity Framework Core
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the UnitOfWork class
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="logger">The logger</param>
    public UnitOfWork(
        IDbContext context, 
        IServiceProvider serviceProvider, 
        ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a repository for the specified entity type with strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed identifier type</typeparam>
    /// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
    /// <returns>The repository instance</returns>
    public IRepository<TEntity, TId, TIdValue> Repository<TEntity, TId, TIdValue>()
        where TEntity : Entity<TId, TIdValue>
        where TId : IStronglyTypedId<TIdValue>
        where TIdValue : notnull
    {
        var key = typeof(IRepository<TEntity, TId, TIdValue>);
        
        if (_repositories.TryGetValue(key, out var existingRepository))
        {
            return (IRepository<TEntity, TId, TIdValue>)existingRepository;
        }

        var repository = _serviceProvider.GetService<IRepository<TEntity, TId, TIdValue>>() 
            ?? new Repository<TEntity, TId, TIdValue>(_context, _serviceProvider.GetRequiredService<ILogger<Repository<TEntity, TId, TIdValue>>>());
        
        _repositories[key] = repository;
        return repository;
    }

    /// <summary>
    /// Gets a read-only repository for the specified entity type with strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed identifier type</typeparam>
    /// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
    /// <returns>The read-only repository instance</returns>
    public IReadOnlyRepository<TEntity, TId, TIdValue> ReadOnlyRepository<TEntity, TId, TIdValue>()
        where TEntity : Entity<TId, TIdValue>
        where TId : IStronglyTypedId<TIdValue>
        where TIdValue : notnull
    {
        var key = typeof(IReadOnlyRepository<TEntity, TId, TIdValue>);
        
        if (_repositories.TryGetValue(key, out var existingRepository))
        {
            return (IReadOnlyRepository<TEntity, TId, TIdValue>)existingRepository;
        }

        var repository = _serviceProvider.GetService<IReadOnlyRepository<TEntity, TId, TIdValue>>() 
            ?? new ReadOnlyRepository<TEntity, TId, TIdValue>(_context, _serviceProvider.GetRequiredService<ILogger<ReadOnlyRepository<TEntity, TId, TIdValue>>>());
        
        _repositories[key] = repository;
        return repository;
    }

    /// <summary>
    /// Gets a repository for entities with integer-based strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed integer ID type</typeparam>
    /// <returns>The repository instance</returns>
    public IIntRepository<TEntity, TId> IntRepository<TEntity, TId>()
        where TEntity : IntEntity<TId>
        where TId : struct, IStronglyTypedId<int>
    {
        return (IIntRepository<TEntity, TId>)Repository<TEntity, TId, int>();
    }

    /// <summary>
    /// Gets a repository for entities with long-based strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed long ID type</typeparam>
    /// <returns>The repository instance</returns>
    public ILongRepository<TEntity, TId> LongRepository<TEntity, TId>()
        where TEntity : LongEntity<TId>
        where TId : struct, IStronglyTypedId<long>
    {
        return (ILongRepository<TEntity, TId>)Repository<TEntity, TId, long>();
    }

    /// <summary>
    /// Gets a repository for entities with GUID-based strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed GUID ID type</typeparam>
    /// <returns>The repository instance</returns>
    public IGuidRepository<TEntity, TId> GuidRepository<TEntity, TId>()
        where TEntity : GuidEntity<TId>
        where TId : struct, IStronglyTypedId<Guid>
    {
        return (IGuidRepository<TEntity, TId>)Repository<TEntity, TId, Guid>();
    }

    /// <summary>
    /// Gets a repository for entities with string-based strongly-typed identifiers
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The strongly-typed string ID type</typeparam>
    /// <returns>The repository instance</returns>
    public IStringRepository<TEntity, TId> StringRepository<TEntity, TId>()
        where TEntity : StringEntity<TId>
        where TId : struct, IStronglyTypedId<string>
    {
        return (IStringRepository<TEntity, TId>)Repository<TEntity, TId, string>();
    }

    /// <summary>
    /// Saves all changes made in this unit of work
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of entities saved</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Saving changes to database");
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Saved {Count} changes to database", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The transaction instance</returns>
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Beginning database transaction");
        var dbTransaction = await _context.BeginTransactionAsync(cancellationToken);
        return new Transaction(dbTransaction, _logger);
    }

    /// <summary>
    /// Executes a function within a transaction
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="func">The function to execute</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result of the function</returns>
    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));

        using var transaction = await BeginTransactionAsync(cancellationToken);
        
        try
        {
            var result = await func();
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Executes an action within a transaction
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        using var transaction = await BeginTransactionAsync(cancellationToken);
        
        try
        {
            await action();
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Disposes the unit of work
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the unit of work asynchronously
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the unit of work
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context?.Dispose();
            _repositories.Clear();
            _disposed = true;
        }
    }

    /// <summary>
    /// Disposes the unit of work asynchronously
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (!_disposed)
        {
            if (_context != null)
            {
                await _context.DisposeAsync();
            }
            
            _repositories.Clear();
            _disposed = true;
        }
    }
}

/// <summary>
/// Transaction wrapper for Entity Framework Core transactions
/// </summary>
public class Transaction : ITransaction
{
    private readonly IDbContextTransaction _transaction;
    private readonly ILogger _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the Transaction class
    /// </summary>
    /// <param name="transaction">The Entity Framework Core transaction</param>
    /// <param name="logger">The logger</param>
    public Transaction(IDbContextTransaction transaction, ILogger logger)
    {
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Commits the transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Committing transaction");
            await _transaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing transaction");
            throw;
        }
    }

    /// <summary>
    /// Rolls back the transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Rolling back transaction");
            await _transaction.RollbackAsync(cancellationToken);
            _logger.LogDebug("Transaction rolled back successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back transaction");
            throw;
        }
    }

    /// <summary>
    /// Disposes the transaction
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the transaction asynchronously
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the transaction
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// Disposes the transaction asynchronously
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (!_disposed)
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            _disposed = true;
        }
    }
} 