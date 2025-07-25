namespace BuildingBlocks.Infrastructure.Data.Context;

/// <summary>
/// Interface for database context factory
/// </summary>
/// <typeparam name="TContext">The database context type</typeparam>
public interface IDbContextFactory<out TContext>
    where TContext : IDbContext
{
    /// <summary>
    /// Creates a new database context instance
    /// </summary>
    /// <returns>The database context</returns>
    TContext CreateDbContext();
}

/// <summary>
/// Interface for async database context factory
/// </summary>
/// <typeparam name="TContext">The database context type</typeparam>
public interface IAsyncDbContextFactory<TContext>
    where TContext : IDbContext
{
    /// <summary>
    /// Creates a new database context instance asynchronously
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The database context</returns>
    Task<TContext> CreateDbContextAsync(CancellationToken cancellationToken = default);
} 