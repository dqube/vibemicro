namespace BuildingBlocks.Domain.Exceptions;

/// <summary>
/// Exception thrown when an aggregate cannot be found
/// </summary>
public class AggregateNotFoundException : DomainException
{
    /// <summary>
    /// Gets the type of the aggregate that was not found
    /// </summary>
    public Type AggregateType { get; }

    /// <summary>
    /// Gets the identifier that was used to search for the aggregate
    /// </summary>
    public object AggregateId { get; }

    /// <summary>
    /// Initializes a new instance of the AggregateNotFoundException class
    /// </summary>
    /// <param name="aggregateType">The type of the aggregate</param>
    /// <param name="aggregateId">The identifier of the aggregate</param>
    public AggregateNotFoundException(Type aggregateType, object aggregateId)
        : base($"Aggregate of type '{aggregateType.Name}' with id '{aggregateId}' was not found", 
               "AGGREGATE_NOT_FOUND", 
               new { AggregateType = aggregateType.Name, AggregateId = aggregateId })
    {
        AggregateType = aggregateType;
        AggregateId = aggregateId;
    }

    /// <summary>
    /// Initializes a new instance of the AggregateNotFoundException class
    /// </summary>
    /// <param name="aggregateType">The type of the aggregate</param>
    /// <param name="aggregateId">The identifier of the aggregate</param>
    /// <param name="innerException">The inner exception</param>
    public AggregateNotFoundException(Type aggregateType, object aggregateId, Exception innerException)
        : base($"Aggregate of type '{aggregateType.Name}' with id '{aggregateId}' was not found", 
               "AGGREGATE_NOT_FOUND", 
               new { AggregateType = aggregateType.Name, AggregateId = aggregateId }, 
               innerException)
    {
        AggregateType = aggregateType;
        AggregateId = aggregateId;
    }

    /// <summary>
    /// Creates an AggregateNotFoundException for a specific aggregate type
    /// </summary>
    /// <typeparam name="TAggregate">The aggregate type</typeparam>
    /// <param name="aggregateId">The identifier of the aggregate</param>
    /// <returns>A new AggregateNotFoundException</returns>
    public static AggregateNotFoundException For<TAggregate>(object aggregateId)
    {
        return new AggregateNotFoundException(typeof(TAggregate), aggregateId);
    }

    /// <summary>
    /// Creates an AggregateNotFoundException for a specific aggregate type with an inner exception
    /// </summary>
    /// <typeparam name="TAggregate">The aggregate type</typeparam>
    /// <param name="aggregateId">The identifier of the aggregate</param>
    /// <param name="innerException">The inner exception</param>
    /// <returns>A new AggregateNotFoundException</returns>
    public static AggregateNotFoundException For<TAggregate>(object aggregateId, Exception innerException)
    {
        return new AggregateNotFoundException(typeof(TAggregate), aggregateId, innerException);
    }
} 