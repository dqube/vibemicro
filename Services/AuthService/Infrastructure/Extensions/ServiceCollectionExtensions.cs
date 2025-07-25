using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Health;
using AuthService.Infrastructure.Authentication;
using AuthService.Infrastructure.Services;
using AuthService.Domain.Repositories;
using AuthService.Domain.Services;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Application.Outbox;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Services;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.UnitOfWork;
using BuildingBlocks.Infrastructure.Data.Interceptors;

namespace AuthService.Infrastructure.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to configure AuthService Infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the AuthService infrastructure layer services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddAuthServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add BuildingBlocks infrastructure services (caching, messaging, etc.)
        services.AddInfrastructure(configuration);

        // Add database context with interceptors
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            
            // Add interceptors
            options.AddInterceptors(
                new AuditInterceptor(),
                new DomainEventInterceptor(),
                new SoftDeleteInterceptor()
            );
        });

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork<AuthDbContext>>();

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRegistrationTokenRepository, RegistrationTokenRepository>();

        // Register domain services
        services.AddScoped<IAuthDomainService, AuthDomainService>();

        // Register authentication services
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Register AuthService-specific outbox/inbox implementations
        services.AddScoped<IOutboxService, AuthOutboxService>();
        services.AddScoped<IInboxService, AuthInboxService>();

        // Add BuildingBlocks outbox/inbox processing services
        services.AddOutboxServices();
        services.AddInboxServices();

        // Add background services for outbox/inbox processing
        services.AddHostedService<OutboxBackgroundService>();
        services.AddHostedService<InboxBackgroundService>();

        // Configure background service options
        services.Configure<OutboxBackgroundServiceOptions>(options =>
        {
            options.ProcessingInterval = TimeSpan.FromSeconds(30);
            options.BatchSize = 50;
            options.MaxRetryCount = 3;
        });

        services.Configure<InboxBackgroundServiceOptions>(options =>
        {
            options.ProcessingInterval = TimeSpan.FromSeconds(30);
            options.BatchSize = 50;
            options.MaxRetryCount = 3;
        });

        // Add health checks
        services.AddAuthServiceHealthChecks();

        return services;
    }

    /// <summary>
    /// Adds AuthService health checks
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddAuthServiceHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<AuthDatabaseHealthCheck>("authservice_database", tags: new[] { "database", "authservice" })
            .AddCheck<AuthServiceHealthCheck>("authservice_domain", tags: new[] { "domain", "authservice" });

        return services;
    }
} 