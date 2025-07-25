namespace BuildingBlocks.Domain.Exceptions;

/// <summary>
/// Exception thrown when a concurrency conflict occurs
/// </summary>
public class ConcurrencyException : DomainException
{
    /// <summary>
    /// Gets the type of the entity that had the concurrency conflict
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    /// Gets the identifier of the entity that had the concurrency conflict
    /// </summary>
    public object EntityId { get; }

    /// <summary>
    /// Gets the expected version
    /// </summary>
    public long? ExpectedVersion { get; }

    /// <summary>
    /// Gets the actual version
    /// </summary>
    public long? ActualVersion { get; }

    /// <summary>
    /// Initializes a new instance of the ConcurrencyException class
    /// </summary>
    /// <param name="entityType">The type of the entity</param>
    /// <param name="entityId">The identifier of the entity</param>
    public ConcurrencyException(Type entityType, object entityId)
        : base($"Concurrency conflict occurred for entity '{entityType.Name}' with id '{entityId}'", 
               "CONCURRENCY_CONFLICT", 
               new { EntityType = entityType.Name, EntityId = entityId })
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Initializes a new instance of the ConcurrencyException class
    /// </summary>
    /// <param name="entityType">The type of the entity</param>
    /// <param name="entityId">The identifier of the entity</param>
    /// <param name="expectedVersion">The expected version</param>
    /// <param name="actualVersion">The actual version</param>
    public ConcurrencyException(Type entityType, object entityId, long expectedVersion, long actualVersion)
        : base($"Concurrency conflict occurred for entity '{entityType.Name}' with id '{entityId}'. Expected version: {expectedVersion}, Actual version: {actualVersion}", 
               "CONCURRENCY_CONFLICT", 
               new { EntityType = entityType.Name, EntityId = entityId, ExpectedVersion = expectedVersion, ActualVersion = actualVersion })
    {
        EntityType = entityType;
        EntityId = entityId;
        ExpectedVersion = expectedVersion;
        ActualVersion = actualVersion;
    }

    /// <summary>
    /// Initializes a new instance of the ConcurrencyException class
    /// </summary>
    /// <param name="entityType">The type of the entity</param>
    /// <param name="entityId">The identifier of the entity</param>
    /// <param name="innerException">The inner exception</param>
    public ConcurrencyException(Type entityType, object entityId, Exception innerException)
        : base($"Concurrency conflict occurred for entity '{entityType.Name}' with id '{entityId}'", 
               "CONCURRENCY_CONFLICT", 
               new { EntityType = entityType.Name, EntityId = entityId }, 
               innerException)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Creates a ConcurrencyException for a specific entity type
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="entityId">The identifier of the entity</param>
    /// <returns>A new ConcurrencyException</returns>
    public static ConcurrencyException For<TEntity>(object entityId)
    {
        return new ConcurrencyException(typeof(TEntity), entityId);
    }

    /// <summary>
    /// Creates a ConcurrencyException for a specific entity type with version information
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="entityId">The identifier of the entity</param>
    /// <param name="expectedVersion">The expected version</param>
    /// <param name="actualVersion">The actual version</param>
    /// <returns>A new ConcurrencyException</returns>
    public static ConcurrencyException For<TEntity>(object entityId, long expectedVersion, long actualVersion)
    {
        return new ConcurrencyException(typeof(TEntity), entityId, expectedVersion, actualVersion);
    }
} 