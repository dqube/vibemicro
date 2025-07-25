using BuildingBlocks.Domain.Services;
using BuildingBlocks.Domain.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace BuildingBlocks.Domain.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to configure Domain services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the domain layer services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">The assemblies to scan for domain services</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDomain(this IServiceCollection services, params Assembly[] assemblies)
    {
        // Add domain services
        services.AddDomainServices(assemblies);
        
        // Add validators
        services.AddDomainValidators(assemblies);

        // Configure JSON for strongly typed IDs
        // services.ConfigureJsonOptionsForStronglyTypedIds(); // Temporarily disabled

        return services;
    }

    /// <summary>
    /// Adds domain services from the specified assemblies
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDomainServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            assemblies = new[] { Assembly.GetCallingAssembly() };

        foreach (var assembly in assemblies)
        {
            var serviceTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => typeof(IDomainService).IsAssignableFrom(t))
                .ToList();

            foreach (var serviceType in serviceTypes)
            {
                var interfaces = serviceType.GetInterfaces()
                    .Where(i => typeof(IDomainService).IsAssignableFrom(i) && i != typeof(IDomainService));

                foreach (var @interface in interfaces)
                {
                    services.TryAddScoped(@interface, serviceType);
                }

                // Also register as self
                services.TryAddScoped(serviceType);
            }
        }

        return services;
    }

    /// <summary>
    /// Adds domain validators from the specified assemblies
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDomainValidators(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            assemblies = new[] { Assembly.GetCallingAssembly() };

        foreach (var assembly in assemblies)
        {
            var validatorTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainValidator<>)))
                .ToList();

            foreach (var validatorType in validatorTypes)
            {
                var interfaces = validatorType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainValidator<>));

                foreach (var @interface in interfaces)
                {
                    services.TryAddScoped(@interface, validatorType);
                }
            }
        }

        return services;
    }
} 