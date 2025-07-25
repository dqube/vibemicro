using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories;
using AuthService.Domain.Repositories;
using BuildingBlocks.Infrastructure.Extensions;

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
        // Add BuildingBlocks infrastructure services
        services.AddInfrastructure(configuration);

        // Add database context
        services.AddDbContext<AuthDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRegistrationTokenRepository, RegistrationTokenRepository>();

        return services;
    }
} 