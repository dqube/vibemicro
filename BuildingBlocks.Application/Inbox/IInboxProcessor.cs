namespace BuildingBlocks.Application.Inbox;

/// <summary>
/// Interface for processing inbox messages
/// </summary>
public interface IInboxProcessor
{
    /// <summary>
    /// Processes pending inbox messages
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of messages processed</returns>
    Task<int> ProcessPendingMessagesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes a specific inbox message
    /// </summary>
    /// <param name="message">The message to process</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if processing was successful</returns>
    Task<bool> ProcessMessageAsync(InboxMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes messages for a specific message group in order
    /// </summary>
    /// <param name="messageGroup">The message group to process</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of messages processed</returns>
    Task<int> ProcessOrderedMessagesAsync(string messageGroup, CancellationToken cancellationToken = default);
} 