namespace BuildingBlocks.Domain.DomainEvents;

/// <summary>
/// Interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for this domain event
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets when the domain event occurred
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the name of the domain event
    /// </summary>
    string EventName { get; }

    /// <summary>
    /// Gets the version of the domain event
    /// </summary>
    int Version { get; }
} 