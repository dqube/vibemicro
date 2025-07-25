using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;

namespace BuildingBlocks.Application.Services;

/// <summary>
/// Interface for domain event service
/// </summary>
public interface IDomainEventService
{
    /// <summary>
    /// Publishes domain events from entities
    /// </summary>
    /// <param name="entities">The entities containing domain events</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task PublishDomainEventsAsync<TId, TIdValue>(IEnumerable<Entity<TId, TIdValue>> entities, CancellationToken cancellationToken = default)
        where TId : IStronglyTypedId<TIdValue>
        where TIdValue : notnull;

    /// <summary>
    /// Publishes a single domain event
    /// </summary>
    /// <param name="domainEvent">The domain event to publish</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task PublishDomainEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes multiple domain events
    /// </summary>
    /// <param name="domainEvents">The domain events to publish</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
} 