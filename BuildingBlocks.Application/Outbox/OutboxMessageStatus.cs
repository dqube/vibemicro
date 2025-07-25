namespace BuildingBlocks.Application.Outbox;

/// <summary>
/// Represents the status of an outbox message
/// </summary>
public enum OutboxMessageStatus
{
    /// <summary>
    /// Message is waiting to be processed
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Message is currently being processed
    /// </summary>
    Processing = 1,

    /// <summary>
    /// Message has been successfully processed
    /// </summary>
    Processed = 2,

    /// <summary>
    /// Message processing failed
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Message has been cancelled
    /// </summary>
    Cancelled = 4
} 