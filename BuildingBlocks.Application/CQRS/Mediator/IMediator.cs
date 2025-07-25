using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Queries;

namespace BuildingBlocks.Application.CQRS.Mediator;

/// <summary>
/// Interface for mediating commands, queries, and events
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a command that doesn't return a result
    /// </summary>
    /// <typeparam name="TCommand">The type of command</typeparam>
    /// <param name="command">The command to send</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>
    /// Sends a command that returns a result
    /// </summary>
    /// <typeparam name="TCommand">The type of command</typeparam>
    /// <typeparam name="TResult">The type of result</typeparam>
    /// <param name="command">The command to send</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation with the result</returns>
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;

    /// <summary>
    /// Sends a query and returns the result
    /// </summary>
    /// <typeparam name="TQuery">The type of query</typeparam>
    /// <typeparam name="TResult">The type of result</typeparam>
    /// <param name="query">The query to send</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation with the result</returns>
    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;

    /// <summary>
    /// Publishes an event to all handlers
    /// </summary>
    /// <typeparam name="TEvent">The type of event</typeparam>
    /// <param name="eventItem">The event to publish</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default)
        where TEvent : IEvent;
} 