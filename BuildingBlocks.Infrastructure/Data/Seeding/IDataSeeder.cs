namespace BuildingBlocks.Infrastructure.Data.Seeding;

/// <summary>
/// Interface for seeding data into the database
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Gets the order in which this seeder should be executed
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Gets a value indicating whether this seeder can be run multiple times
    /// </summary>
    bool CanRunMultipleTimes { get; }

    /// <summary>
    /// Seeds data into the database
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    Task SeedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the seeder should run
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the seeder should run</returns>
    Task<bool> ShouldRunAsync(CancellationToken cancellationToken = default);
} 