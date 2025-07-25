using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.CQRS.Mediator;

/// <summary>
/// Default implementation of the mediator pattern
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the Mediator class
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Sends a command that doesn't return a result
    /// </summary>
    public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
        if (handler == null)
            throw new InvalidOperationException($"No handler found for command type {typeof(TCommand).Name}");

        await handler.HandleAsync(command, cancellationToken);
    }

    /// <summary>
    /// Sends a command that returns a result
    /// </summary>
    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResult>>();
        if (handler == null)
            throw new InvalidOperationException($"No handler found for command type {typeof(TCommand).Name}");

        return await handler.HandleAsync(command, cancellationToken);
    }

    /// <summary>
    /// Sends a query and returns the result
    /// </summary>
    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResult>>();
        if (handler == null)
            throw new InvalidOperationException($"No handler found for query type {typeof(TQuery).Name}");

        return await handler.HandleAsync(query, cancellationToken);
    }

    /// <summary>
    /// Publishes an event to all handlers
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (eventItem == null)
            throw new ArgumentNullException(nameof(eventItem));

        var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();
        
        var tasks = handlers.Select(handler => handler.HandleAsync(eventItem, cancellationToken));
        await Task.WhenAll(tasks);
    }
} 