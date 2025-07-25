namespace BuildingBlocks.Application.CQRS.Events;

/// <summary>
/// Interface for application events
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Gets the unique identifier for this event
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets when the event occurred
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the name of the event
    /// </summary>
    string EventName { get; }

    /// <summary>
    /// Gets the version of the event
    /// </summary>
    int Version { get; }
} 