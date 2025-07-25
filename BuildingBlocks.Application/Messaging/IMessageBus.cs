using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Messages;

namespace BuildingBlocks.Application.Messaging;

/// <summary>
/// Interface for message bus
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// Publishes a message
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="message">The message to publish</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publishes an integration event
    /// </summary>
    /// <typeparam name="T">The integration event type</typeparam>
    /// <param name="integrationEvent">The integration event to publish</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task PublishIntegrationEventAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) 
        where T : class, IIntegrationEvent;

    /// <summary>
    /// Sends a message to a specific queue
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="queueName">The queue name</param>
    /// <param name="message">The message to send</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Subscribes to messages of a specific type
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="handler">The message handler</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) 
        where T : class;

    /// <summary>
    /// Unsubscribes from messages of a specific type
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UnsubscribeAsync<T>(CancellationToken cancellationToken = default) where T : class;
} 