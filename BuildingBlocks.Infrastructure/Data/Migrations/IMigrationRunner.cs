namespace BuildingBlocks.Infrastructure.Data.Migrations;

/// <summary>
/// Interface for running database migrations
/// </summary>
public interface IMigrationRunner
{
    /// <summary>
    /// Applies all pending migrations
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    Task MigrateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies migrations up to a specific target
    /// </summary>
    /// <param name="targetMigration">The target migration name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task MigrateToAsync(string targetMigration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all pending migrations
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of pending migration names</returns>
    Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all applied migrations
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of applied migration names</returns>
    Task<IEnumerable<string>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the database exists
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the database exists</returns>
    Task<bool> DatabaseExistsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates the database if it doesn't exist
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    Task EnsureDatabaseCreatedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the database
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    Task DeleteDatabaseAsync(CancellationToken cancellationToken = default);
} 