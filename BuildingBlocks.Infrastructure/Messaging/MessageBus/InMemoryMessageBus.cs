using BuildingBlocks.Infrastructure.Messaging.Serialization;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace BuildingBlocks.Infrastructure.Messaging.MessageBus;

/// <summary>
/// In-memory implementation of message bus for testing and local development
/// </summary>
public class InMemoryMessageBus : IMessageBus, IDisposable
{
    private readonly IMessageSerializer _serializer;
    private readonly ILogger<InMemoryMessageBus> _logger;
    private readonly ConcurrentDictionary<string, List<Func<object, IDictionary<string, string>, Task>>> _subscriptions;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<object>> _pendingRequests;
    private bool _disposed;
    private bool _started;

    /// <summary>
    /// Initializes a new instance of the InMemoryMessageBus class
    /// </summary>
    public InMemoryMessageBus(IMessageSerializer serializer, ILogger<InMemoryMessageBus> logger)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subscriptions = new ConcurrentDictionary<string, List<Func<object, IDictionary<string, string>, Task>>>();
        _pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<object>>();
    }

    /// <summary>
    /// Publishes a message to the specified topic or queue
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="message">The message to publish</param>
    /// <param name="topic">The topic or queue name</param>
    /// <param name="headers">Optional message headers</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task PublishAsync<T>(T message, string topic, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Topic cannot be null or empty", nameof(topic));

        if (!_started)
            throw new InvalidOperationException("Message bus is not started");

        _logger.LogDebug("Publishing message to topic: {Topic}", topic);

        headers ??= new Dictionary<string, string>();
        headers["MessageType"] = typeof(T).AssemblyQualifiedName!;
        headers["PublishedAt"] = DateTime.UtcNow.ToString("O");

        if (_subscriptions.TryGetValue(topic, out var handlers))
        {
            var tasks = handlers.Select(handler => 
                ExecuteHandler(handler, message!, headers, topic));

            await Task.WhenAll(tasks);
        }

        _logger.LogDebug("Message published to topic: {Topic}", topic);
    }

    /// <summary>
    /// Publishes a message to the message bus
    /// </summary>
    /// <param name="messageType">The message type name</param>
    /// <param name="messageContent">The serialized message content</param>
    /// <param name="headers">Optional message headers</param>
    /// <param name="destination">Optional destination override</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task PublishAsync(string messageType, string messageContent, IDictionary<string, string>? headers = null, string? destination = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(messageType))
            throw new ArgumentException("Message type cannot be null or empty", nameof(messageType));

        if (string.IsNullOrWhiteSpace(messageContent))
            throw new ArgumentException("Message content cannot be null or empty", nameof(messageContent));

        if (!_started)
            throw new InvalidOperationException("Message bus is not started");

        var topic = destination ?? messageType;
        
        _logger.LogDebug("Publishing message of type {MessageType} to topic: {Topic}", messageType, topic);

        headers ??= new Dictionary<string, string>();
        headers["MessageType"] = messageType;
        headers["PublishedAt"] = DateTime.UtcNow.ToString("O");

        if (_subscriptions.TryGetValue(topic, out var handlers))
        {
            // Deserialize the message content to byte array for handlers
            var messageBytes = System.Text.Encoding.UTF8.GetBytes(messageContent);
            var messageObject = _serializer.Deserialize(messageBytes, Type.GetType(messageType)!);

            var tasks = handlers.Select(handler => 
                ExecuteHandler(handler, messageObject, headers, topic));

            await Task.WhenAll(tasks);
        }

        _logger.LogDebug("Message of type {MessageType} published to topic: {Topic}", messageType, topic);
    }

    /// <summary>
    /// Subscribes to messages from the specified topic or queue
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="topic">The topic or queue name</param>
    /// <param name="handler">The message handler</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task SubscribeAsync<T>(string topic, Func<T, IDictionary<string, string>, Task> handler, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Topic cannot be null or empty", nameof(topic));

        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        _logger.LogDebug("Subscribing to topic: {Topic} for message type: {MessageType}", topic, typeof(T).Name);

        var wrappedHandler = new Func<object, IDictionary<string, string>, Task>((msg, headers) =>
        {
            if (msg is T typedMessage)
            {
                return handler(typedMessage, headers);
            }
            return Task.CompletedTask;
        });

        _subscriptions.AddOrUpdate(topic, 
            new List<Func<object, IDictionary<string, string>, Task>> { wrappedHandler },
            (key, existing) =>
            {
                existing.Add(wrappedHandler);
                return existing;
            });

        return Task.CompletedTask;
    }

    /// <summary>
    /// Unsubscribes from messages from the specified topic
    /// </summary>
    /// <param name="topic">The topic or queue name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Topic cannot be null or empty", nameof(topic));

        _logger.LogDebug("Unsubscribing from topic: {Topic}", topic);

        _subscriptions.TryRemove(topic, out _);

        return Task.CompletedTask;
    }

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
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(
        TRequest request, 
        string topic, 
        TimeSpan timeout, 
        IDictionary<string, string>? headers = null, 
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Topic cannot be null or empty", nameof(topic));

        var correlationId = Guid.NewGuid().ToString();
        var replyTo = $"{topic}_reply_{correlationId}";

        headers ??= new Dictionary<string, string>();
        headers["CorrelationId"] = correlationId;
        headers["ReplyTo"] = replyTo;

        var tcs = new TaskCompletionSource<object>();
        _pendingRequests[correlationId] = tcs;

        // Subscribe to reply topic
        await SubscribeAsync<TResponse>(replyTo, async (response, responseHeaders) =>
        {
            if (_pendingRequests.TryRemove(correlationId, out var pendingTcs))
            {
                pendingTcs.SetResult(response!);
            }
        }, cancellationToken);

        try
        {
            // Publish request
            await PublishAsync(request, topic, headers, cancellationToken);

            // Wait for response with timeout
            using var timeoutCts = new CancellationTokenSource(timeout);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeout, combinedCts.Token));
            
            if (completedTask == tcs.Task)
            {
                return (TResponse)await tcs.Task;
            }
            else
            {
                throw new TimeoutException($"Request to {topic} timed out after {timeout}");
            }
        }
        finally
        {
            await UnsubscribeAsync(replyTo, cancellationToken);
            _pendingRequests.TryRemove(correlationId, out _);
        }
    }

    /// <summary>
    /// Starts the message bus
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting in-memory message bus");
        _started = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the message bus
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping in-memory message bus");
        _started = false;
        _subscriptions.Clear();
        
        // Cancel all pending requests
        foreach (var request in _pendingRequests.Values)
        {
            request.TrySetCanceled();
        }
        _pendingRequests.Clear();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes a message handler with error handling
    /// </summary>
    /// <param name="handler">The handler to execute</param>
    /// <param name="message">The message</param>
    /// <param name="headers">The message headers</param>
    /// <param name="topic">The topic name</param>
    /// <returns>A task representing the execution</returns>
    private async Task ExecuteHandler(Func<object, IDictionary<string, string>, Task> handler, object message, IDictionary<string, string> headers, string topic)
    {
        try
        {
            await handler(message, headers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing handler for topic {Topic}: {Error}", topic, ex.Message);
            // In a real implementation, you might want to implement dead letter queues or retry logic
        }
    }

    /// <summary>
    /// Disposes the message bus
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            StopAsync().Wait();
            _disposed = true;
        }
    }
} 