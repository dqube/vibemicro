using BuildingBlocks.Infrastructure.Data.Context;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Data.Seeding;

/// <summary>
/// Base class for data seeders providing common functionality
/// </summary>
public abstract class DataSeederBase : IDataSeeder
{
    /// <summary>
    /// Gets the database context factory
    /// </summary>
    protected IDbContextFactory DbContextFactory { get; }

    /// <summary>
    /// Gets the logger instance
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the order in which this seeder should be executed
    /// </summary>
    public abstract int Order { get; }

    /// <summary>
    /// Gets a value indicating whether this seeder can be run multiple times
    /// </summary>
    public virtual bool CanRunMultipleTimes => false;

    /// <summary>
    /// Initializes a new instance of the DataSeederBase class
    /// </summary>
    /// <param name="dbContextFactory">The database context factory</param>
    /// <param name="logger">The logger instance</param>
    protected DataSeederBase(IDbContextFactory dbContextFactory, ILogger logger)
    {
        DbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Seeds data into the database
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Starting data seeding with {SeederType}", GetType().Name);

        try
        {
            if (!await ShouldRunAsync(cancellationToken))
            {
                Logger.LogDebug("Skipping seeder {SeederType} - conditions not met", GetType().Name);
                return;
            }

            using var context = DbContextFactory.CreateDbContext();
            await SeedDataAsync(context, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("Completed data seeding with {SeederType}", GetType().Name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during data seeding with {SeederType}: {Error}", GetType().Name, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Checks if the seeder should run
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the seeder should run</returns>
    public virtual async Task<bool> ShouldRunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var context = DbContextFactory.CreateDbContext();
            return await ShouldRunCoreAsync(context, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking if seeder {SeederType} should run: {Error}", GetType().Name, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Core seeding implementation to be provided by derived classes
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    protected abstract Task SeedDataAsync(IDbContext context, CancellationToken cancellationToken);

    /// <summary>
    /// Core logic for determining if seeder should run
    /// Override in derived classes for custom logic
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the seeder should run</returns>
    protected virtual Task<bool> ShouldRunCoreAsync(IDbContext context, CancellationToken cancellationToken)
    {
        // By default, run if CanRunMultipleTimes is true or if no data exists
        return Task.FromResult(CanRunMultipleTimes);
    }

    /// <summary>
    /// Logs the start of seeding for a specific entity type
    /// </summary>
    /// <param name="entityType">The entity type being seeded</param>
    /// <param name="count">The number of entities being seeded</param>
    protected void LogSeedingStart(string entityType, int count)
    {
        Logger.LogDebug("Seeding {Count} {EntityType} entities", count, entityType);
    }

    /// <summary>
    /// Logs the completion of seeding for a specific entity type
    /// </summary>
    /// <param name="entityType">The entity type that was seeded</param>
    /// <param name="count">The number of entities that were seeded</param>
    protected void LogSeedingComplete(string entityType, int count)
    {
        Logger.LogDebug("Successfully seeded {Count} {EntityType} entities", count, entityType);
    }
} 