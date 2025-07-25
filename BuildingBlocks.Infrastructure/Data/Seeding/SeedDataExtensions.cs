using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Data.Seeding;

/// <summary>
/// Extension methods for data seeding
/// </summary>
public static class SeedDataExtensions
{
    /// <summary>
    /// Runs all registered data seeders
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public static async Task SeedDataAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IDataSeeder>>();
        
        logger.LogInformation("Starting data seeding process");

        try
        {
            var seeders = scope.ServiceProvider.GetServices<IDataSeeder>()
                .OrderBy(s => s.Order)
                .ToList();

            logger.LogInformation("Found {SeederCount} data seeders", seeders.Count);

            foreach (var seeder in seeders)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await seeder.SeedAsync(cancellationToken);
            }

            logger.LogInformation("Data seeding process completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Data seeding process failed: {Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Runs data seeding for a hosting application
    /// </summary>
    /// <param name="host">The host</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public static async Task SeedDataAsync(this IHost host, CancellationToken cancellationToken = default)
    {
        await host.Services.SeedDataAsync(cancellationToken);
    }

    /// <summary>
    /// Registers a data seeder
    /// </summary>
    /// <typeparam name="TSeeder">The seeder type</typeparam>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDataSeeder<TSeeder>(this IServiceCollection services)
        where TSeeder : class, IDataSeeder
    {
        return services.AddScoped<IDataSeeder, TSeeder>();
    }

    /// <summary>
    /// Registers multiple data seeders
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="seederTypes">The seeder types to register</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDataSeeders(this IServiceCollection services, params Type[] seederTypes)
    {
        foreach (var seederType in seederTypes)
        {
            if (!typeof(IDataSeeder).IsAssignableFrom(seederType))
            {
                throw new ArgumentException($"Type {seederType.Name} does not implement IDataSeeder", nameof(seederTypes));
            }

            services.AddScoped(typeof(IDataSeeder), seederType);
        }

        return services;
    }

    /// <summary>
    /// Runs data seeding conditionally based on environment
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="environments">The environments where seeding should run</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public static async Task SeedDataIfEnvironmentAsync(
        this IServiceProvider serviceProvider, 
        CancellationToken cancellationToken = default,
        params string[] environments)
    {
        using var scope = serviceProvider.CreateScope();
        var hostEnvironment = scope.ServiceProvider.GetService<IHostEnvironment>();
        
        if (hostEnvironment == null || environments.Length == 0)
        {
            await serviceProvider.SeedDataAsync(cancellationToken);
            return;
        }

        if (environments.Any(env => string.Equals(hostEnvironment.EnvironmentName, env, StringComparison.OrdinalIgnoreCase)))
        {
            await serviceProvider.SeedDataAsync(cancellationToken);
        }
    }
} 