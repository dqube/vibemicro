using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.Application.Services;
using BuildingBlocks.Application.Outbox;
using BuildingBlocks.Application.Inbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace BuildingBlocks.Application.Extensions;

/// <summary>
/// Extension methods for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the application layer services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">The assemblies to scan for handlers</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services, params Assembly[] assemblies)
    {
        // Add mediator
        services.TryAddScoped<IMediator, Mediator>();

        // Add domain event service
        services.TryAddScoped<IDomainEventService, DomainEventService>();

        // Add outbox services
        services.AddOutboxServices();

        // Add inbox services
        services.AddInboxServices();

        // Add pipeline behaviors
        services.AddPipelineBehaviors();

        // Register handlers from assemblies
        if (assemblies.Length > 0)
        {
            services.AddHandlers(assemblies);
        }

        return services;
    }

    /// <summary>
    /// Adds pipeline behaviors
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddPipelineBehaviors(this IServiceCollection services)
    {
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));

        return services;
    }

    /// <summary>
    /// Adds handlers from the specified assemblies
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            // Register command handlers
            RegisterHandlers(services, assembly, typeof(BuildingBlocks.Application.CQRS.Commands.ICommandHandler<>));
            RegisterHandlers(services, assembly, typeof(BuildingBlocks.Application.CQRS.Commands.ICommandHandler<,>));

            // Register query handlers
            RegisterHandlers(services, assembly, typeof(BuildingBlocks.Application.CQRS.Queries.IQueryHandler<,>));

            // Register event handlers
            RegisterHandlers(services, assembly, typeof(BuildingBlocks.Application.CQRS.Events.IEventHandler<>));

            // Register application services
            RegisterApplicationServices(services, assembly);
        }

        return services;
    }

    /// <summary>
    /// Adds application services marker
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">The assemblies to scan for application services</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            RegisterApplicationServices(services, assembly);
        }

        return services;
    }

    /// <summary>
    /// Adds outbox pattern services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddOutboxServices(this IServiceCollection services)
    {
        services.TryAddScoped<IOutboxProcessor, OutboxProcessor>();
        
        return services;
    }

    /// <summary>
    /// Adds inbox pattern services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddInboxServices(this IServiceCollection services)
    {
        services.TryAddScoped<IInboxProcessor, InboxProcessor>();
        
        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly, Type handlerInterface)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetInterfaces().Any(i => 
                i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface);

            foreach (var @interface in interfaces)
            {
                services.TryAddTransient(@interface, handlerType);
            }
        }
    }

    private static void RegisterApplicationServices(IServiceCollection services, Assembly assembly)
    {
        var serviceTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => typeof(IApplicationService).IsAssignableFrom(t))
            .ToList();

        foreach (var serviceType in serviceTypes)
        {
            var interfaces = serviceType.GetInterfaces()
                .Where(i => typeof(IApplicationService).IsAssignableFrom(i) && i != typeof(IApplicationService));

            foreach (var @interface in interfaces)
            {
                services.TryAddScoped(@interface, serviceType);
            }

            // Also register as self
            services.TryAddScoped(serviceType);
        }
    }
} 