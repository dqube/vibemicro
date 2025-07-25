namespace BuildingBlocks.Domain.DomainEvents;

/// <summary>
/// Default implementation of the domain event dispatcher
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the DomainEventDispatcher class
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving handlers</param>
    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Dispatches a single domain event
    /// </summary>
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent == null)
            throw new ArgumentNullException(nameof(domainEvent));

        var domainEventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEventType);
        
        var handlers = GetHandlers(handlerType);
        var tasks = handlers.Select(handler => DispatchToHandler(handler, domainEvent, cancellationToken));
        
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Dispatches multiple domain events
    /// </summary>
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        if (domainEvents == null)
            throw new ArgumentNullException(nameof(domainEvents));

        var tasks = domainEvents.Select(domainEvent => DispatchAsync(domainEvent, cancellationToken));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Dispatches a single domain event of a specific type
    /// </summary>
    public async Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent
    {
        if (domainEvent == null)
            throw new ArgumentNullException(nameof(domainEvent));

        var handlers = GetHandlers<TDomainEvent>();
        var tasks = handlers.Select(handler => handler.HandleAsync(domainEvent, cancellationToken));
        
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Gets handlers for a specific domain event type
    /// </summary>
    private IEnumerable<IDomainEventHandler<TDomainEvent>> GetHandlers<TDomainEvent>()
        where TDomainEvent : IDomainEvent
    {
        var handlerType = typeof(IDomainEventHandler<TDomainEvent>);
        return GetHandlers(handlerType).Cast<IDomainEventHandler<TDomainEvent>>();
    }

    /// <summary>
    /// Gets handlers for a specific handler type
    /// </summary>
    private IEnumerable<object> GetHandlers(Type handlerType)
    {
        try
        {
            var handlers = _serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(handlerType));
            return handlers as IEnumerable<object> ?? Enumerable.Empty<object>();
        }
        catch
        {
            return Enumerable.Empty<object>();
        }
    }

    /// <summary>
    /// Dispatches a domain event to a specific handler
    /// </summary>
    private static async Task DispatchToHandler(object handler, IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var handlerMethod = handler.GetType().GetMethod("HandleAsync");
        if (handlerMethod != null)
        {
            var result = handlerMethod.Invoke(handler, new object[] { domainEvent, cancellationToken });
            if (result is Task task)
            {
                await task;
            }
        }
    }
} 