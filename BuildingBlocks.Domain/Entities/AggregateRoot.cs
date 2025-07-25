using BuildingBlocks.Domain.StronglyTypedIds;

namespace BuildingBlocks.Domain.Entities;

/// <summary>
/// Base class for aggregate roots in the domain with strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public abstract class AggregateRoot<TId, TIdValue> : Entity<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    /// <summary>
    /// Gets or sets the version of this aggregate for optimistic concurrency control
    /// </summary>
    public long Version { get; protected set; }

    /// <summary>
    /// Increments the version of this aggregate
    /// </summary>
    protected void IncrementVersion()
    {
        Version++;
    }

    /// <summary>
    /// Marks an entity as modified and increments the version
    /// </summary>
    protected void MarkAsModified()
    {
        IncrementVersion();
    }

    /// <summary>
    /// Sets the version of this aggregate (used by infrastructure)
    /// </summary>
    /// <param name="version">The version to set</param>
    public void SetVersion(long version)
    {
        Version = version;
    }

    /// <summary>
    /// Checks if this aggregate is new (has no version)
    /// </summary>
    public bool IsNew => Version == 0;

    /// <summary>
    /// Returns the string representation of the aggregate root
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} [Id: {Id}, Version: {Version}]";
    }
}

/// <summary>
/// Base class for aggregate roots with integer-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The strongly-typed integer ID type</typeparam>
public abstract class IntAggregateRoot<TId> : AggregateRoot<TId, int>
    where TId : struct, IStronglyTypedId<int>
{
}

/// <summary>
/// Base class for aggregate roots with long-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The strongly-typed long ID type</typeparam>
public abstract class LongAggregateRoot<TId> : AggregateRoot<TId, long>
    where TId : struct, IStronglyTypedId<long>
{
}

/// <summary>
/// Base class for aggregate roots with GUID-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The strongly-typed GUID ID type</typeparam>
public abstract class GuidAggregateRoot<TId> : AggregateRoot<TId, Guid>
    where TId : struct, IStronglyTypedId<Guid>
{
    /// <summary>
    /// Initializes a new instance with a new GUID
    /// </summary>
    protected GuidAggregateRoot()
    {
        // For GUID aggregates, we can automatically generate the ID
        if (Id.Equals(default(TId)))
        {
            var guidValue = Guid.NewGuid();
            Id = (TId)Activator.CreateInstance(typeof(TId), guidValue)!;
        }
    }

    /// <summary>
    /// Initializes a new instance with the specified ID
    /// </summary>
    /// <param name="id">The aggregate ID</param>
    protected GuidAggregateRoot(TId id)
    {
        Id = id;
    }
}

/// <summary>
/// Base class for aggregate roots with string-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The strongly-typed string ID type</typeparam>
public abstract class StringAggregateRoot<TId> : AggregateRoot<TId, string>
    where TId : struct, IStronglyTypedId<string>
{
} 