namespace BuildingBlocks.Domain.DomainEvents;

/// <summary>
/// Base class for domain events
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for this domain event
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets when the domain event occurred
    /// </summary>
    public DateTime OccurredOn { get; private set; }

    /// <summary>
    /// Gets the name of the domain event
    /// </summary>
    public string EventName { get; private set; }

    /// <summary>
    /// Gets the version of the domain event
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Initializes a new instance of the DomainEventBase class
    /// </summary>
    protected DomainEventBase()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventName = GetType().Name;
        Version = 1;
    }

    /// <summary>
    /// Initializes a new instance of the DomainEventBase class with specified values
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="occurredOn">When the event occurred</param>
    /// <param name="eventName">The name of the event</param>
    /// <param name="version">The version of the event</param>
    internal DomainEventBase(Guid id, DateTime occurredOn, string eventName, int version)
    {
        Id = id;
        OccurredOn = occurredOn;
        EventName = eventName;
        Version = version;
    }

    /// <summary>
    /// Returns the string representation of the domain event
    /// </summary>
    public override string ToString()
    {
        return $"{EventName} (Id: {Id}, OccurredOn: {OccurredOn:yyyy-MM-dd HH:mm:ss}, Version: {Version})";
    }
} 