using BuildingBlocks.Domain.DomainEvents;

namespace BuildingBlocks.Application.CQRS.Events;

/// <summary>
/// Notification wrapper for domain events to be handled in the application layer
/// </summary>
/// <typeparam name="TDomainEvent">The type of domain event</typeparam>
public class DomainEventNotification<TDomainEvent> : IEvent
    where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the wrapped domain event
    /// </summary>
    public TDomainEvent DomainEvent { get; }

    /// <summary>
    /// Gets the unique identifier for this event
    /// </summary>
    public Guid Id => DomainEvent.Id;

    /// <summary>
    /// Gets when the event occurred
    /// </summary>
    public DateTime OccurredOn => DomainEvent.OccurredOn;

    /// <summary>
    /// Gets the name of the event
    /// </summary>
    public string EventName => DomainEvent.EventName;

    /// <summary>
    /// Gets the version of the event
    /// </summary>
    public int Version => DomainEvent.Version;

    /// <summary>
    /// Initializes a new instance of the DomainEventNotification class
    /// </summary>
    /// <param name="domainEvent">The domain event to wrap</param>
    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent ?? throw new ArgumentNullException(nameof(domainEvent));
    }

    /// <summary>
    /// Returns the string representation of the domain event notification
    /// </summary>
    public override string ToString()
    {
        return $"DomainEventNotification<{typeof(TDomainEvent).Name}> [Id: {Id}, OccurredOn: {OccurredOn:yyyy-MM-dd HH:mm:ss}]";
    }
} 