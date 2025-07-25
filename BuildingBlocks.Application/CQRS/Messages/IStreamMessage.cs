namespace BuildingBlocks.Application.CQRS.Messages;

/// <summary>
/// Interface for streaming messages
/// </summary>
public interface IStreamMessage : IMessage
{
    /// <summary>
    /// Gets the stream identifier
    /// </summary>
    string StreamId { get; }

    /// <summary>
    /// Gets the position in the stream
    /// </summary>
    long Position { get; }

    /// <summary>
    /// Gets the partition key for the message
    /// </summary>
    string? PartitionKey { get; }
} 