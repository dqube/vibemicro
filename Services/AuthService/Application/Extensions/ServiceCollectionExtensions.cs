using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Extensions;
using FluentValidation;
using System.Reflection;

namespace AuthService.Application.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to configure AuthService Application services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the AuthService application layer services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddAuthServiceApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Add BuildingBlocks application services
        services.AddApplication(assembly);

        // Add FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
} 