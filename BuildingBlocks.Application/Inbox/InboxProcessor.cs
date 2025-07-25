using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Mediator;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Application.Inbox;

/// <summary>
/// Default implementation of inbox processor
/// </summary>
public class InboxProcessor : IInboxProcessor
{
    private readonly IInboxService _inboxService;
    private readonly IMediator _mediator;
    private readonly ILogger<InboxProcessor> _logger;

    /// <summary>
    /// Initializes a new instance of the InboxProcessor class
    /// </summary>
    public InboxProcessor(
        IInboxService inboxService,
        IMediator mediator,
        ILogger<InboxProcessor> logger)
    {
        _inboxService = inboxService ?? throw new ArgumentNullException(nameof(inboxService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes pending inbox messages
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of messages processed</returns>
    public async Task<int> ProcessPendingMessagesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting to process pending inbox messages");

        var pendingMessages = await _inboxService.GetPendingMessagesAsync(100, cancellationToken);
        var processedCount = 0;

        foreach (var message in pendingMessages)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var success = await ProcessMessageAsync(message, cancellationToken);
                if (success)
                {
                    processedCount++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing inbox message {MessageId}", message.Id);
                await _inboxService.MarkAsFailedAsync(message.Id, ex.Message, cancellationToken);
            }
        }

        _logger.LogInformation("Processed {ProcessedCount} inbox messages", processedCount);
        return processedCount;
    }

    /// <summary>
    /// Processes a specific inbox message
    /// </summary>
    /// <param name="message">The message to process</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if processing was successful</returns>
    public async Task<bool> ProcessMessageAsync(InboxMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        _logger.LogDebug("Processing inbox message {MessageId} of type {MessageType}", 
            message.Id, message.MessageType);

        try
        {
            // Deserialize and process the message based on its type
            var messageObject = DeserializeMessage(message);
            
            if (messageObject is IEvent eventMessage)
            {
                // Send the message through the mediator
                await _mediator.PublishAsync(eventMessage, cancellationToken);
            }

            // Mark as processed
            await _inboxService.MarkAsProcessedAsync(message.Id, cancellationToken);

            _logger.LogDebug("Successfully processed inbox message {MessageId}", message.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process inbox message {MessageId}: {Error}", 
                message.Id, ex.Message);

            await _inboxService.MarkAsFailedAsync(message.Id, ex.Message, cancellationToken);
            return false;
        }
    }

    /// <summary>
    /// Processes messages for a specific message group in order
    /// </summary>
    /// <param name="messageGroup">The message group to process</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of messages processed</returns>
    public async Task<int> ProcessOrderedMessagesAsync(string messageGroup, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(messageGroup))
            throw new ArgumentException("Message group cannot be null or empty", nameof(messageGroup));

        _logger.LogDebug("Processing ordered messages for group {MessageGroup}", messageGroup);

        // Get all pending messages for the group, ordered by sequence number
        var pendingMessages = await _inboxService.GetPendingMessagesAsync(1000, cancellationToken);
        var groupMessages = pendingMessages
            .Where(m => m.MessageGroup == messageGroup && m.IsOrderedMessage())
            .OrderBy(m => m.SequenceNumber)
            .ToList();

        var processedCount = 0;

        foreach (var message in groupMessages)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var success = await ProcessMessageAsync(message, cancellationToken);
                if (success)
                {
                    processedCount++;
                }
                else
                {
                    // Stop processing if any message in the sequence fails
                    _logger.LogWarning("Stopping ordered processing for group {MessageGroup} due to failed message {MessageId}", 
                        messageGroup, message.Id);
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ordered message {MessageId} in group {MessageGroup}", 
                    message.Id, messageGroup);
                await _inboxService.MarkAsFailedAsync(message.Id, ex.Message, cancellationToken);
                break; // Stop processing the sequence
            }
        }

        _logger.LogInformation("Processed {ProcessedCount} ordered messages for group {MessageGroup}", 
            processedCount, messageGroup);
        
        return processedCount;
    }

    /// <summary>
    /// Deserializes a message based on its type
    /// </summary>
    /// <param name="message">The inbox message</param>
    /// <returns>The deserialized message object</returns>
    private object? DeserializeMessage(InboxMessage message)
    {
        try
        {
            // This is a simplified implementation
            // In a real scenario, you would use a proper message registry or type mapping
            var messageType = Type.GetType(message.MessageType);
            if (messageType == null)
            {
                _logger.LogWarning("Unknown message type: {MessageType}", message.MessageType);
                return null;
            }

            return JsonSerializer.Deserialize(message.Content, messageType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize message {MessageId} of type {MessageType}", 
                message.Id, message.MessageType);
            return null;
        }
    }
} 