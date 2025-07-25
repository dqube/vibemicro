using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Services;

/// <summary>
/// Service for handling domain events
/// </summary>
public class DomainEventService : ApplicationServiceBase, IDomainEventService
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the DomainEventService class
    /// </summary>
    /// <param name="mediator">The mediator</param>
    /// <param name="logger">The logger</param>
    public DomainEventService(IMediator mediator, ILogger<DomainEventService> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Publishes domain events from entities
    /// </summary>
    public async Task PublishDomainEventsAsync<TId, TIdValue>(IEnumerable<Entity<TId, TIdValue>> entities, CancellationToken cancellationToken = default)
        where TId : IStronglyTypedId<TIdValue>
        where TIdValue : notnull
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        var entitiesList = entities.ToList();
        var domainEvents = entitiesList
            .SelectMany(e => e.DomainEvents)
            .ToList();

        if (!domainEvents.Any())
        {
            Logger.LogDebug("No domain events to publish");
            return;
        }

        Logger.LogInformation("Publishing {EventCount} domain events from {EntityCount} entities", 
            domainEvents.Count, entitiesList.Count);

        try
        {
            // Publish all domain events
            await PublishDomainEventsAsync(domainEvents, cancellationToken);

            // Clear domain events from entities
            foreach (var entity in entitiesList)
            {
                entity.ClearDomainEvents();
            }

            Logger.LogInformation("Successfully published {EventCount} domain events", domainEvents.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to publish domain events");
            throw;
        }
    }

    /// <summary>
    /// Publishes a single domain event
    /// </summary>
    public async Task PublishDomainEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent == null)
            throw new ArgumentNullException(nameof(domainEvent));

        Logger.LogDebug("Publishing domain event {EventName} with ID {EventId}", 
            domainEvent.EventName, domainEvent.Id);

        try
        {
            // Wrap domain event in notification for application layer handling
            var notification = CreateDomainEventNotification(domainEvent);
            await _mediator.PublishAsync(notification, cancellationToken);

            Logger.LogDebug("Successfully published domain event {EventName}", domainEvent.EventName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to publish domain event {EventName}", domainEvent.EventName);
            throw;
        }
    }

    /// <summary>
    /// Publishes multiple domain events
    /// </summary>
    public async Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        if (domainEvents == null)
            throw new ArgumentNullException(nameof(domainEvents));

        var eventsList = domainEvents.ToList();
        
        if (!eventsList.Any())
        {
            Logger.LogDebug("No domain events to publish");
            return;
        }

        Logger.LogDebug("Publishing {EventCount} domain events", eventsList.Count);

        try
        {
            var publishTasks = eventsList.Select(domainEvent => PublishDomainEventAsync(domainEvent, cancellationToken));
            await Task.WhenAll(publishTasks);

            Logger.LogDebug("Successfully published {EventCount} domain events", eventsList.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to publish domain events");
            throw;
        }
    }

    /// <summary>
    /// Creates a domain event notification for the given domain event
    /// </summary>
    /// <param name="domainEvent">The domain event</param>
    /// <returns>The domain event notification</returns>
    private static IEvent CreateDomainEventNotification(IDomainEvent domainEvent)
    {
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (IEvent)Activator.CreateInstance(notificationType, domainEvent)!;
    }
} 