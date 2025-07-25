using BuildingBlocks.Application.Caching;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Caching;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Data.Repositories;
using BuildingBlocks.Infrastructure.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Infrastructure.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to configure Infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the infrastructure layer services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add data layer
        services.AddDataLayer(configuration);

        // Add caching
        services.AddCaching(configuration);

        // Add messaging
        services.AddMessaging(configuration);

        // Add logging
        services.AddStructuredLogging(configuration);

        // Add authentication
        services.AddInfrastructureAuthentication(configuration);

        // Add authorization
        services.AddInfrastructureAuthorization(configuration);

        // Add storage
        services.AddStorage(configuration);

        // Add communication
        services.AddCommunication(configuration);

        // Add monitoring
        services.AddMonitoring(configuration);

        // Add background services
        services.AddBackgroundServices(configuration);

        // Add external services
        services.AddExternalServices(configuration);

        // Add security
        services.AddSecurity(configuration);

        // Add mapping
        services.AddMapping(configuration);

        // Add validation
        services.AddInfrastructureValidation(configuration);

        // Add serialization
        services.AddSerialization(configuration);

        // Add idempotency
        services.AddIdempotency(configuration);

        return services;
    }

    /// <summary>
    /// Adds data layer services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // Use in-memory database for development/testing
                options.UseInMemoryDatabase("DefaultDatabase");
            }
        });

        // Register DbContext as IDbContext
        services.TryAddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Add Unit of Work
        services.TryAddScoped<IUnitOfWork, UnitOfWork>();

        // Add generic repositories
        services.TryAddScoped(typeof(IRepository<,,>), typeof(Repository<,,>));
        services.TryAddScoped(typeof(IReadOnlyRepository<,,>), typeof(ReadOnlyRepository<,,>));

        return services;
    }

    /// <summary>
    /// Adds caching services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure cache settings
        var cacheConfig = new CacheConfiguration();
        configuration.GetSection("Cache").Bind(cacheConfig);
        services.TryAddSingleton(cacheConfig);

        // Add memory cache
        services.AddMemoryCache();

        // Register cache services
        services.TryAddScoped<ICacheService, MemoryCacheService>();
        services.TryAddScoped<BuildingBlocks.Application.Caching.ICacheService, MemoryCacheService>();

        // Add distributed cache based on configuration
        var provider = configuration.GetValue<string>("Cache:Provider");
        switch (provider?.ToLowerInvariant())
        {
            case "redis":
                var redisConnectionString = configuration.GetConnectionString("Redis");
                if (!string.IsNullOrEmpty(redisConnectionString))
                {
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = redisConnectionString;
                    });
                }
                break;

            case "sqlserver":
                var sqlConnectionString = configuration.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrEmpty(sqlConnectionString))
                {
                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = sqlConnectionString;
                        options.SchemaName = "dbo";
                        options.TableName = "DistributedCache";
                    });
                }
                break;

            default:
                // Use memory cache as distributed cache
                services.AddDistributedMemoryCache();
                break;
        }

        return services;
    }

    /// <summary>
    /// Adds messaging services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement messaging services (MessageBus, EventBus, etc.)
        return services;
    }

    /// <summary>
    /// Adds structured logging services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddStructuredLogging(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement structured logging services
        return services;
    }

    /// <summary>
    /// Adds authentication services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddInfrastructureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement authentication services
        return services;
    }

    /// <summary>
    /// Adds authorization services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddInfrastructureAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement authorization services
        return services;
    }

    /// <summary>
    /// Adds storage services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement storage services
        return services;
    }

    /// <summary>
    /// Adds communication services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddCommunication(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement communication services
        return services;
    }

    /// <summary>
    /// Adds monitoring services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement monitoring services
        return services;
    }

    /// <summary>
    /// Adds background services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement background services
        return services;
    }

    /// <summary>
    /// Adds external services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement external services
        return services;
    }

    /// <summary>
    /// Adds security services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement security services
        return services;
    }

    /// <summary>
    /// Adds mapping services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddMapping(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement mapping services
        return services;
    }

    /// <summary>
    /// Adds validation services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddInfrastructureValidation(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement validation services
        return services;
    }

    /// <summary>
    /// Adds serialization services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddSerialization(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement serialization services
        return services;
    }

    /// <summary>
    /// Adds idempotency services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddIdempotency(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement idempotency services
        return services;
    }
} 