namespace BuildingBlocks.Application.CQRS.Events;

/// <summary>
/// Interface for integration events that cross service boundaries
/// </summary>
public interface IIntegrationEvent : IEvent
{
    /// <summary>
    /// Gets the correlation identifier for tracking related operations across services
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Gets the service that published this event
    /// </summary>
    string? PublishedBy { get; }

    /// <summary>
    /// Gets the target services that should handle this event
    /// </summary>
    IEnumerable<string>? TargetServices { get; }
} 