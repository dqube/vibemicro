using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Mediator;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Dispatchers;

/// <summary>
/// Default implementation of event dispatcher
/// </summary>
public class EventDispatcher : IEventDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventDispatcher> _logger;

    /// <summary>
    /// Initializes a new instance of the EventDispatcher class
    /// </summary>
    public EventDispatcher(IMediator mediator, ILogger<EventDispatcher> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Dispatches an event to all registered handlers
    /// </summary>
    /// <typeparam name="TEvent">The event type</typeparam>
    /// <param name="eventItem">The event to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task DispatchAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (eventItem == null)
            throw new ArgumentNullException(nameof(eventItem));

        _logger.LogDebug("Dispatching event {EventType}: {@Event}", typeof(TEvent).Name, eventItem);

        try
        {
            await _mediator.PublishAsync(eventItem, cancellationToken);
            _logger.LogDebug("Successfully dispatched event {EventType}", typeof(TEvent).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch event {EventType}: {Error}", typeof(TEvent).Name, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Dispatches multiple events in sequence
    /// </summary>
    /// <param name="events">The events to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task DispatchAllAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        if (events == null)
            throw new ArgumentNullException(nameof(events));

        var eventList = events.ToList();
        _logger.LogDebug("Dispatching {EventCount} events in sequence", eventList.Count);

        foreach (var eventItem in eventList)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await _mediator.PublishAsync(eventItem, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispatch event {EventType}: {Error}", eventItem.GetType().Name, ex.Message);
                throw;
            }
        }

        _logger.LogDebug("Successfully dispatched {EventCount} events in sequence", eventList.Count);
    }

    /// <summary>
    /// Dispatches multiple events in parallel
    /// </summary>
    /// <param name="events">The events to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task DispatchAllParallelAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        if (events == null)
            throw new ArgumentNullException(nameof(events));

        var eventList = events.ToList();
        _logger.LogDebug("Dispatching {EventCount} events in parallel", eventList.Count);

        var tasks = eventList.Select(async eventItem =>
        {
            try
            {
                await _mediator.PublishAsync(eventItem, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispatch event {EventType}: {Error}", eventItem.GetType().Name, ex.Message);
                throw;
            }
        });

        await Task.WhenAll(tasks);
        _logger.LogDebug("Successfully dispatched {EventCount} events in parallel", eventList.Count);
    }
} 