using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Extensions;
using FluentValidation;
using System.Reflection;
using BuildingBlocks.Application.Behaviors;
using AuthService.Application.Behaviors;
using AuthService.Application.Services;

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

        // Add BuildingBlocks application services with mediator and behaviors
        services.AddApplication(assembly);

        // Add FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        // Add auth-specific application services
        services.AddScoped<IAuthApplicationService, AuthApplicationService>();

        // Register auth-specific pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthValidationBehavior<,>));

        // The base Application layer already adds these, but we can override if needed:
        // - ValidationBehavior<,> (now using AuthValidationBehavior)
        // - LoggingBehavior<,>
        // - PerformanceBehavior<,>
        // - CachingBehavior<,>
        // - TransactionBehavior<,>
        // - RetryBehavior<,>

        return services;
    }
} 