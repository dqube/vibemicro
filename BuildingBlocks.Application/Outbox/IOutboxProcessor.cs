namespace BuildingBlocks.Application.Outbox;

/// <summary>
/// Interface for processing outbox messages
/// </summary>
public interface IOutboxProcessor
{
    /// <summary>
    /// Processes pending outbox messages
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of messages processed</returns>
    Task<int> ProcessPendingMessagesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes a specific outbox message
    /// </summary>
    /// <param name="message">The message to process</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if processing was successful</returns>
    Task<bool> ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default);
} 