using BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Outbox;

/// <summary>
/// Default implementation of outbox processor
/// </summary>
public class OutboxProcessor : IOutboxProcessor
{
    private readonly IOutboxService _outboxService;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<OutboxProcessor> _logger;

    /// <summary>
    /// Initializes a new instance of the OutboxProcessor class
    /// </summary>
    public OutboxProcessor(
        IOutboxService outboxService,
        IMessageBus messageBus,
        ILogger<OutboxProcessor> logger)
    {
        _outboxService = outboxService ?? throw new ArgumentNullException(nameof(outboxService));
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes pending outbox messages
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of messages processed</returns>
    public async Task<int> ProcessPendingMessagesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting to process pending outbox messages");

        var pendingMessages = await _outboxService.GetPendingMessagesAsync(100, cancellationToken);
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
                _logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
                await _outboxService.MarkAsFailedAsync(message.Id, ex.Message, cancellationToken);
            }
        }

        _logger.LogInformation("Processed {ProcessedCount} outbox messages", processedCount);
        return processedCount;
    }

    /// <summary>
    /// Processes a specific outbox message
    /// </summary>
    /// <param name="message">The message to process</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if processing was successful</returns>
    public async Task<bool> ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        _logger.LogDebug("Processing outbox message {MessageId} of type {MessageType}", 
            message.Id, message.MessageType);

        try
        {
            // Publish the message to the message bus
            await _messageBus.PublishAsync(
                message.MessageType,
                message.Content,
                message.Headers,
                message.Destination,
                cancellationToken);

            // Mark as processed
            await _outboxService.MarkAsProcessedAsync(message.Id, cancellationToken);

            _logger.LogDebug("Successfully processed outbox message {MessageId}", message.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process outbox message {MessageId}: {Error}", 
                message.Id, ex.Message);

            await _outboxService.MarkAsFailedAsync(message.Id, ex.Message, cancellationToken);
            return false;
        }
    }
} 