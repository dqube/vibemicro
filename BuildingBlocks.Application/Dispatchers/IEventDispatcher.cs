using BuildingBlocks.Application.CQRS.Events;

namespace BuildingBlocks.Application.Dispatchers;

/// <summary>
/// Interface for dispatching events
/// </summary>
public interface IEventDispatcher
{
    /// <summary>
    /// Dispatches an event to all registered handlers
    /// </summary>
    /// <typeparam name="TEvent">The event type</typeparam>
    /// <param name="eventItem">The event to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task DispatchAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    /// <summary>
    /// Dispatches multiple events in sequence
    /// </summary>
    /// <param name="events">The events to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task DispatchAllAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches multiple events in parallel
    /// </summary>
    /// <param name="events">The events to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task DispatchAllParallelAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
} 