namespace BuildingBlocks.Application.CQRS.Events;

/// <summary>
/// Base class for integration events
/// </summary>
public abstract class IntegrationEventBase : IIntegrationEvent
{
    /// <summary>
    /// Gets the unique identifier for this event
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; private set; }

    /// <summary>
    /// Gets the name of the event
    /// </summary>
    public string EventName { get; private set; }

    /// <summary>
    /// Gets the version of the event
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Gets the correlation identifier for tracking related operations across services
    /// </summary>
    public string? CorrelationId { get; private set; }

    /// <summary>
    /// Gets the service that published this event
    /// </summary>
    public string? PublishedBy { get; private set; }

    /// <summary>
    /// Gets the target services that should handle this event
    /// </summary>
    public IEnumerable<string>? TargetServices { get; private set; }

    /// <summary>
    /// Initializes a new instance of the IntegrationEventBase class
    /// </summary>
    protected IntegrationEventBase()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventName = GetType().Name;
        Version = 1;
    }

    /// <summary>
    /// Initializes a new instance of the IntegrationEventBase class with specified values
    /// </summary>
    /// <param name="correlationId">The correlation identifier</param>
    /// <param name="publishedBy">The service that published this event</param>
    /// <param name="targetServices">The target services</param>
    protected IntegrationEventBase(string? correlationId, string? publishedBy = null, IEnumerable<string>? targetServices = null)
        : this()
    {
        CorrelationId = correlationId;
        PublishedBy = publishedBy;
        TargetServices = targetServices;
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
    /// Sets the publishing service
    /// </summary>
    /// <param name="publishedBy">The service that published this event</param>
    public void SetPublishedBy(string publishedBy)
    {
        PublishedBy = publishedBy;
    }

    /// <summary>
    /// Sets the target services
    /// </summary>
    /// <param name="targetServices">The target services</param>
    public void SetTargetServices(IEnumerable<string> targetServices)
    {
        TargetServices = targetServices;
    }

    /// <summary>
    /// Returns the string representation of the integration event
    /// </summary>
    public override string ToString()
    {
        return $"{EventName} [Id: {Id}, OccurredOn: {OccurredOn:yyyy-MM-dd HH:mm:ss}, CorrelationId: {CorrelationId}]";
    }
} 