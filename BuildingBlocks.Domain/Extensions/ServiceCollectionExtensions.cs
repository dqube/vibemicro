using BuildingBlocks.Domain.Services;
using BuildingBlocks.Domain.Factories;
using BuildingBlocks.Domain.Policies;
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
        
        // Add factories
        services.AddDomainFactories(assemblies);
        
        // Add policies
        services.AddDomainPolicies(assemblies);
        
        // Add validators
        services.AddDomainValidators(assemblies);

        // Configure JSON for strongly typed IDs
        services.ConfigureJsonOptionsForStronglyTypedIds();

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
    /// Adds domain factories from the specified assemblies
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDomainFactories(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            assemblies = new[] { Assembly.GetCallingAssembly() };

        foreach (var assembly in assemblies)
        {
            // Register entity factories
            var entityFactoryTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && 
                    (i.GetGenericTypeDefinition() == typeof(IEntityFactory<,,>) ||
                     i.GetGenericTypeDefinition() == typeof(IAggregateFactory<,,>))))
                .ToList();

            foreach (var factoryType in entityFactoryTypes)
            {
                var interfaces = factoryType.GetInterfaces()
                    .Where(i => i.IsGenericType && 
                        (i.GetGenericTypeDefinition() == typeof(IEntityFactory<,,>) ||
                         i.GetGenericTypeDefinition() == typeof(IAggregateFactory<,,>)));

                foreach (var @interface in interfaces)
                {
                    services.TryAddScoped(@interface, factoryType);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Adds domain policies from the specified assemblies
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDomainPolicies(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            assemblies = new[] { Assembly.GetCallingAssembly() };

        foreach (var assembly in assemblies)
        {
            // Register synchronous policies
            var policyTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPolicy<,>)))
                .ToList();

            foreach (var policyType in policyTypes)
            {
                var interfaces = policyType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPolicy<,>));

                foreach (var @interface in interfaces)
                {
                    services.TryAddScoped(@interface, policyType);
                }
            }

            // Register asynchronous policies
            var asyncPolicyTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncPolicy<,>)))
                .ToList();

            foreach (var policyType in asyncPolicyTypes)
            {
                var interfaces = policyType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncPolicy<,>));

                foreach (var @interface in interfaces)
                {
                    services.TryAddScoped(@interface, policyType);
                }
            }

            // Register validation policies
            var validationPolicyTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => typeof(IValidationPolicy<>).IsAssignableFromGeneric(t))
                .ToList();

            foreach (var policyType in validationPolicyTypes)
            {
                var interfaces = policyType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidationPolicy<>));

                foreach (var @interface in interfaces)
                {
                    services.TryAddScoped(@interface, policyType);
                }
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

    /// <summary>
    /// Helper method to check if a type is assignable from a generic type
    /// </summary>
    /// <param name="genericType">The generic type</param>
    /// <param name="type">The type to check</param>
    /// <returns>True if assignable</returns>
    private static bool IsAssignableFromGeneric(this Type genericType, Type type)
    {
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType);
    }
} 