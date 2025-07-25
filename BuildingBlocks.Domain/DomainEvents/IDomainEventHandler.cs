namespace BuildingBlocks.Domain.DomainEvents;

/// <summary>
/// Interface for domain event handlers
/// </summary>
/// <typeparam name="TDomainEvent">The type of domain event to handle</typeparam>
public interface IDomainEventHandler<in TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Handles the specified domain event
    /// </summary>
    /// <param name="domainEvent">The domain event to handle</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for domain event handlers that can handle multiple event types
/// </summary>
public interface IDomainEventHandler
{
    /// <summary>
    /// Checks if this handler can handle the specified domain event type
    /// </summary>
    /// <param name="domainEventType">The domain event type</param>
    /// <returns>True if the handler can handle the event type</returns>
    bool CanHandle(Type domainEventType);

    /// <summary>
    /// Handles the specified domain event
    /// </summary>
    /// <param name="domainEvent">The domain event to handle</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
} 