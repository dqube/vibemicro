using BuildingBlocks.Application.CQRS.Messages;
using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Dispatchers;

/// <summary>
/// Default implementation of message dispatcher that routes messages to appropriate specialized dispatchers
/// </summary>
public class MessageDispatcher : IMessageDispatcher
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ILogger<MessageDispatcher> _logger;

    /// <summary>
    /// Initializes a new instance of the MessageDispatcher class
    /// </summary>
    public MessageDispatcher(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        IEventDispatcher eventDispatcher,
        ILogger<MessageDispatcher> logger)
    {
        _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        _queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Dispatches a message to the appropriate handler
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <param name="message">The message to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task DispatchAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        _logger.LogDebug("Dispatching message {MessageType}: {@Message}", typeof(TMessage).Name, message);

        try
        {
            switch (message)
            {
                case ICommand command:
                    await _commandDispatcher.DispatchAsync(command, cancellationToken);
                    break;
                case IEvent eventItem:
                    await _eventDispatcher.DispatchAsync(eventItem, cancellationToken);
                    break;
                default:
                    throw new NotSupportedException($"Message type {typeof(TMessage).Name} is not supported for void dispatch");
            }

            _logger.LogDebug("Successfully dispatched message {MessageType}", typeof(TMessage).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch message {MessageType}: {Error}", typeof(TMessage).Name, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Dispatches a message with a return value
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="message">The message to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result from the message handler</returns>
    public async Task<TResult> DispatchAsync<TMessage, TResult>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        _logger.LogDebug("Dispatching message {MessageType} with result type {ResultType}: {@Message}", 
            typeof(TMessage).Name, typeof(TResult).Name, message);

        try
        {
            TResult result = message switch
            {
                ICommand<TResult> command => await _commandDispatcher.DispatchAsync<ICommand<TResult>, TResult>(command, cancellationToken),
                IQuery<TResult> query => await _queryDispatcher.DispatchAsync<IQuery<TResult>, TResult>(query, cancellationToken),
                _ => throw new NotSupportedException($"Message type {typeof(TMessage).Name} is not supported for result dispatch")
            };

            _logger.LogDebug("Successfully dispatched message {MessageType} with result: {@Result}", typeof(TMessage).Name, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch message {MessageType}: {Error}", typeof(TMessage).Name, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Dispatches multiple messages in sequence
    /// </summary>
    /// <param name="messages">The messages to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task DispatchAllAsync(IEnumerable<IMessage> messages, CancellationToken cancellationToken = default)
    {
        if (messages == null)
            throw new ArgumentNullException(nameof(messages));

        var messageList = messages.ToList();
        _logger.LogDebug("Dispatching {MessageCount} messages in sequence", messageList.Count);

        foreach (var message in messageList)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await DispatchAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispatch message {MessageType}: {Error}", message.GetType().Name, ex.Message);
                throw;
            }
        }

        _logger.LogDebug("Successfully dispatched {MessageCount} messages in sequence", messageList.Count);
    }
} 