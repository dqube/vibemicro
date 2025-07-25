namespace BuildingBlocks.Infrastructure.Messaging.MessageBus;

/// <summary>
/// Interface for message bus operations
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// Publishes a message to the specified topic or queue
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="message">The message to publish</param>
    /// <param name="topic">The topic or queue name</param>
    /// <param name="headers">Optional message headers</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task PublishAsync<T>(T message, string topic, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a message to the message bus
    /// </summary>
    /// <param name="messageType">The message type name</param>
    /// <param name="messageContent">The serialized message content</param>
    /// <param name="headers">Optional message headers</param>
    /// <param name="destination">Optional destination override</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task PublishAsync(string messageType, string messageContent, IDictionary<string, string>? headers = null, string? destination = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to messages from the specified topic or queue
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="topic">The topic or queue name</param>
    /// <param name="handler">The message handler</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task SubscribeAsync<T>(string topic, Func<T, IDictionary<string, string>, Task> handler, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribes from messages from the specified topic
    /// </summary>
    /// <param name="topic">The topic or queue name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request message and waits for a response
    /// </summary>
    /// <typeparam name="TRequest">The request message type</typeparam>
    /// <typeparam name="TResponse">The response message type</typeparam>
    /// <param name="request">The request message</param>
    /// <param name="topic">The topic or queue name</param>
    /// <param name="timeout">The request timeout</param>
    /// <param name="headers">Optional message headers</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The response message</returns>
    Task<TResponse> RequestAsync<TRequest, TResponse>(
        TRequest request, 
        string topic, 
        TimeSpan timeout, 
        IDictionary<string, string>? headers = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts the message bus
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the message bus
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    Task StopAsync(CancellationToken cancellationToken = default);
} 