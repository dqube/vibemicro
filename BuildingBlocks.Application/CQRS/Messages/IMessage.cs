namespace BuildingBlocks.Application.CQRS.Messages;

/// <summary>
/// Interface for application messages
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Gets the unique identifier for this message
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets when the message was created
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the correlation identifier for tracking related operations
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Gets the message type
    /// </summary>
    string MessageType { get; }
} 