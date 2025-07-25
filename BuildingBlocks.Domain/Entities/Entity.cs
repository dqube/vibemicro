using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.StronglyTypedIds;

namespace BuildingBlocks.Domain.Entities;

/// <summary>
/// Base class for all entities in the domain with strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public abstract class Entity<TId, TIdValue> : IEquatable<Entity<TId, TIdValue>>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets or sets the unique identifier for this entity
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// Gets the domain events associated with this entity
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to this entity
    /// </summary>
    /// <param name="domainEvent">The domain event to add</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a domain event from this entity
    /// </summary>
    /// <param name="domainEvent">The domain event to remove</param>
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from this entity
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Determines whether the specified entity is equal to the current entity
    /// </summary>
    public bool Equals(Entity<TId, TIdValue>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current entity
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is Entity<TId, TIdValue> entity && Equals(entity);
    }

    /// <summary>
    /// Returns the hash code for this entity
    /// </summary>
    public override int GetHashCode()
    {
        return EqualityComparer<TId>.Default.GetHashCode(Id);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(Entity<TId, TIdValue>? left, Entity<TId, TIdValue>? right)
    {
        return EqualityComparer<Entity<TId, TIdValue>>.Default.Equals(left, right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(Entity<TId, TIdValue>? left, Entity<TId, TIdValue>? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Returns the string representation of the entity
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} [Id: {Id}]";
    }
}

/// <summary>
/// Base class for entities with integer-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The strongly-typed integer ID type</typeparam>
public abstract class IntEntity<TId> : Entity<TId, int>
    where TId : struct, IStronglyTypedId<int>
{
}

/// <summary>
/// Base class for entities with long-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The strongly-typed long ID type</typeparam>
public abstract class LongEntity<TId> : Entity<TId, long>
    where TId : struct, IStronglyTypedId<long>
{
}

/// <summary>
/// Base class for entities with GUID-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The strongly-typed GUID ID type</typeparam>
public abstract class GuidEntity<TId> : Entity<TId, Guid>
    where TId : struct, IStronglyTypedId<Guid>
{
}

/// <summary>
/// Base class for entities with string-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The strongly-typed string ID type</typeparam>
public abstract class StringEntity<TId> : Entity<TId, string>
    where TId : struct, IStronglyTypedId<string>
{
} 