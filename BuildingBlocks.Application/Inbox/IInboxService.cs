namespace BuildingBlocks.Application.Inbox;

/// <summary>
/// Interface for inbox service handling reliable message consumption
/// </summary>
public interface IInboxService
{
    /// <summary>
    /// Adds a message to the inbox
    /// </summary>
    /// <param name="message">The inbox message to add</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task AddMessageAsync(InboxMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pending messages from the inbox
    /// </summary>
    /// <param name="batchSize">Maximum number of messages to retrieve</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of pending inbox messages</returns>
    Task<IEnumerable<InboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a message with the given identifier already exists
    /// </summary>
    /// <param name="messageId">The message identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the message exists</returns>
    Task<bool> MessageExistsAsync(Guid messageId, CancellationToken cancellationToken = default);

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