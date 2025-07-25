using BuildingBlocks.Application.CQRS.Messages;

namespace BuildingBlocks.Application.Dispatchers;

/// <summary>
/// Interface for dispatching generic messages
/// </summary>
public interface IMessageDispatcher
{
    /// <summary>
    /// Dispatches a message to the appropriate handler
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <param name="message">The message to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task DispatchAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage;

    /// <summary>
    /// Dispatches a message with a return value
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="message">The message to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result from the message handler</returns>
    Task<TResult> DispatchAsync<TMessage, TResult>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage;

    /// <summary>
    /// Dispatches multiple messages in sequence
    /// </summary>
    /// <param name="messages">The messages to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task DispatchAllAsync(IEnumerable<IMessage> messages, CancellationToken cancellationToken = default);
} 