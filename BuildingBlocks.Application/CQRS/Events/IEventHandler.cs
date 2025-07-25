namespace BuildingBlocks.Application.CQRS.Events;

/// <summary>
/// Interface for handling application events
/// </summary>
/// <typeparam name="TEvent">The type of event to handle</typeparam>
public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Handles the specified event
    /// </summary>
    /// <param name="eventItem">The event to handle</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task HandleAsync(TEvent eventItem, CancellationToken cancellationToken = default);
} 