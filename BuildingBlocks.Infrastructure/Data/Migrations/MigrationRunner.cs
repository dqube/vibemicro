using BuildingBlocks.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Data.Migrations;

/// <summary>
/// Entity Framework Core implementation of migration runner
/// </summary>
public class MigrationRunner : IMigrationRunner
{
    private readonly BuildingBlocks.Infrastructure.Data.Context.IDbContextFactory<IDbContext> _dbContextFactory;
    private readonly ILogger<MigrationRunner> _logger;

    /// <summary>
    /// Initializes a new instance of the MigrationRunner class
    /// </summary>
    public MigrationRunner(BuildingBlocks.Infrastructure.Data.Context.IDbContextFactory<IDbContext> dbContextFactory, ILogger<MigrationRunner> logger)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Applies all pending migrations
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        _logger.LogInformation("Starting database migration");

        try
        {
            await context.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database migration failed: {Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Applies migrations up to a specific target
    /// </summary>
    /// <param name="targetMigration">The target migration name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task MigrateToAsync(string targetMigration, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(targetMigration))
            throw new ArgumentException("Target migration cannot be null or empty", nameof(targetMigration));

        using var context = _dbContextFactory.CreateDbContext();
        
        _logger.LogInformation("Migrating database to target: {TargetMigration}", targetMigration);

        try
        {
            var migrator = context.Database.GetService<Microsoft.EntityFrameworkCore.Migrations.IMigrator>();
            await migrator.MigrateAsync(targetMigration, cancellationToken);
            _logger.LogInformation("Database migration to {TargetMigration} completed successfully", targetMigration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database migration to {TargetMigration} failed: {Error}", targetMigration, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets all pending migrations
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of pending migration names</returns>
    public async Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
            _logger.LogDebug("Found {Count} pending migrations", pendingMigrations.Count());
            return pendingMigrations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get pending migrations: {Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets all applied migrations
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of applied migration names</returns>
    public async Task<IEnumerable<string>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        try
        {
            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync(cancellationToken);
            _logger.LogDebug("Found {Count} applied migrations", appliedMigrations.Count());
            return appliedMigrations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get applied migrations: {Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Checks if the database exists
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the database exists</returns>
    public async Task<bool> DatabaseExistsAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        try
        {
            var exists = await context.Database.CanConnectAsync(cancellationToken);
            _logger.LogDebug("Database exists: {Exists}", exists);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check database existence: {Error}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Creates the database if it doesn't exist
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task EnsureDatabaseCreatedAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        _logger.LogInformation("Ensuring database is created");

        try
        {
            var created = await context.Database.EnsureCreatedAsync(cancellationToken);
            if (created)
            {
                _logger.LogInformation("Database created successfully");
            }
            else
            {
                _logger.LogDebug("Database already exists");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure database creation: {Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Deletes the database
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task DeleteDatabaseAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        _logger.LogWarning("Deleting database");

        try
        {
            var deleted = await context.Database.EnsureDeletedAsync(cancellationToken);
            if (deleted)
            {
                _logger.LogInformation("Database deleted successfully");
            }
            else
            {
                _logger.LogDebug("Database did not exist");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete database: {Error}", ex.Message);
            throw;
        }
    }
} 