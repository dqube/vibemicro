namespace BuildingBlocks.Application.Outbox;

/// <summary>
/// Interface for outbox service handling reliable message publishing
/// </summary>
public interface IOutboxService
{
    /// <summary>
    /// Adds a message to the outbox
    /// </summary>
    /// <param name="message">The outbox message to add</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task AddMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pending messages from the outbox
    /// </summary>
    /// <param name="batchSize">Maximum number of messages to retrieve</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of pending outbox messages</returns>
    Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as processed
    /// </summary>
    /// <param name="messageId">The message identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as failed
    /// </summary>
    /// <param name="messageId">The message identifier</param>
    /// <param name="error">The error that occurred</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retries failed messages that are eligible for retry
    /// </summary>
    /// <param name="maxRetryCount">Maximum retry attempts</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of messages marked for retry</returns>
    Task<int> RetryFailedMessagesAsync(int maxRetryCount = 3, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up old processed messages
    /// </summary>
    /// <param name="olderThan">Delete messages older than this date</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of messages deleted</returns>
    Task<int> CleanupOldMessagesAsync(DateTime olderThan, CancellationToken cancellationToken = default);
} 