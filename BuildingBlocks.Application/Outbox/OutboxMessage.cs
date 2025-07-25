namespace BuildingBlocks.Application.Outbox;

/// <summary>
/// Represents a message in the outbox pattern
/// </summary>
public class OutboxMessage
{
    /// <summary>
    /// Gets or sets the unique identifier of the message
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the type of the message
    /// </summary>
    public string MessageType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serialized message content
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination endpoint or topic
    /// </summary>
    public string? Destination { get; set; }

    /// <summary>
    /// Gets or sets the message headers
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Gets or sets the current status of the message
    /// </summary>
    public OutboxMessageStatus Status { get; set; } = OutboxMessageStatus.Pending;

    /// <summary>
    /// Gets or sets the date and time when the message was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the message was last processed
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Gets or sets the number of retry attempts
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the error message if processing failed
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier for tracking related messages
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets additional metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Checks if the message is eligible for retry
    /// </summary>
    /// <returns>True if the message can be retried</returns>
    public bool CanRetry()
    {
        return Status == OutboxMessageStatus.Failed && RetryCount < MaxRetryCount;
    }

    /// <summary>
    /// Marks the message as processed
    /// </summary>
    public void MarkAsProcessed()
    {
        Status = OutboxMessageStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        Error = null;
    }

    /// <summary>
    /// Marks the message as failed
    /// </summary>
    /// <param name="error">The error that occurred</param>
    public void MarkAsFailed(string error)
    {
        Status = OutboxMessageStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
        Error = error;
        RetryCount++;
    }

    /// <summary>
    /// Resets the message for retry
    /// </summary>
    public void ResetForRetry()
    {
        if (CanRetry())
        {
            Status = OutboxMessageStatus.Pending;
            ProcessedAt = null;
            Error = null;
        }
    }
} 