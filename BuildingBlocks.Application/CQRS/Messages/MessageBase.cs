namespace BuildingBlocks.Application.CQRS.Messages;

/// <summary>
/// Base class for application messages
/// </summary>
public abstract class MessageBase : IMessage
{
    /// <summary>
    /// Gets the unique identifier for this message
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets when the message was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the correlation identifier for tracking related operations
    /// </summary>
    public string? CorrelationId { get; private set; }

    /// <summary>
    /// Gets the message type
    /// </summary>
    public string MessageType { get; private set; }

    /// <summary>
    /// Initializes a new instance of the MessageBase class
    /// </summary>
    protected MessageBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        MessageType = GetType().Name;
    }

    /// <summary>
    /// Initializes a new instance of the MessageBase class with correlation ID
    /// </summary>
    /// <param name="correlationId">The correlation identifier</param>
    protected MessageBase(string? correlationId) : this()
    {
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Sets the correlation identifier
    /// </summary>
    /// <param name="correlationId">The correlation identifier</param>
    public void SetCorrelationId(string correlationId)
    {
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Returns the string representation of the message
    /// </summary>
    public override string ToString()
    {
        return $"{MessageType} [Id: {Id}, CreatedAt: {CreatedAt:yyyy-MM-dd HH:mm:ss}, CorrelationId: {CorrelationId}]";
    }
} 