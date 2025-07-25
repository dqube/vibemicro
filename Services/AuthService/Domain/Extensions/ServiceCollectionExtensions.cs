using Microsoft.Extensions.DependencyInjection;
using AuthService.Domain.Services;
using BuildingBlocks.Domain.Extensions;
using System.Reflection;

namespace AuthService.Domain.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to configure AuthService Domain services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the AuthService domain layer services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddAuthServiceDomain(this IServiceCollection services)
    {
        // Add BuildingBlocks domain services
        services.AddDomain(Assembly.GetExecutingAssembly());

        // Register AuthService domain services
        services.AddScoped<IAuthDomainService, AuthDomainService>();

        return services;
    }
} 