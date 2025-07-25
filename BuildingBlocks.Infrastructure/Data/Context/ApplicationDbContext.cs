using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Data.Context;

/// <summary>
/// Application database context
/// </summary>
public class ApplicationDbContext : DbContextBase
{
    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext class
    /// </summary>
    /// <param name="options">The database context options</param>
    /// <param name="logger">The logger</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger)
        : base(options, logger)
    {
    }

    /// <summary>
    /// Configures the model
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure schema
        modelBuilder.HasDefaultSchema("dbo");

        // Additional application-specific configurations can be added here
    }
} 