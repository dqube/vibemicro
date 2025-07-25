namespace BuildingBlocks.Domain.StronglyTypedIds;

/// <summary>
/// Base readonly struct for GUID-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The derived identifier type</typeparam>
public readonly struct GuidId<TId> : IStronglyTypedId<Guid>, IEquatable<GuidId<TId>>
    where TId : struct, IStronglyTypedId<Guid>
{
    /// <summary>
    /// Gets the underlying GUID value
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the GuidId struct
    /// </summary>
    /// <param name="value">The GUID value</param>
    public GuidId(Guid value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new instance with a new GUID
    /// </summary>
    /// <returns>A new identifier with a generated GUID</returns>
    public static TId New()
    {
        return (TId)Activator.CreateInstance(typeof(TId), Guid.NewGuid())!;
    }

    /// <summary>
    /// Gets an empty identifier
    /// </summary>
    public static TId Empty => (TId)Activator.CreateInstance(typeof(TId), Guid.Empty)!;

    /// <summary>
    /// Checks if the identifier is empty
    /// </summary>
    public bool IsEmpty => Value == Guid.Empty;

    /// <summary>
    /// Creates a new instance from the underlying value
    /// </summary>
    /// <param name="value">The GUID value</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static TId From(Guid value)
    {
        return (TId)Activator.CreateInstance(typeof(TId), value)!;
    }

    /// <summary>
    /// Tries to create a new instance from the underlying value
    /// </summary>
    /// <param name="value">The GUID value</param>
    /// <param name="result">The resulting strongly-typed identifier</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool TryFrom(Guid value, out TId result)
    {
        try
        {
            result = From(value);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Returns the string representation of the GUID
    /// </summary>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Returns the string representation of the GUID in the specified format
    /// </summary>
    /// <param name="format">The format string</param>
    /// <returns>The formatted GUID string</returns>
    public string ToString(string format) => Value.ToString(format);

    /// <summary>
    /// Determines whether this instance is equal to another
    /// </summary>
    public bool Equals(GuidId<TId> other) => Value.Equals(other.Value);

    /// <summary>
    /// Determines whether this instance is equal to another IStronglyTypedId
    /// </summary>
    public bool Equals(IStronglyTypedId<Guid>? other) => other is not null && Value.Equals(other.Value);

    /// <summary>
    /// Determines whether this instance is equal to the specified object
    /// </summary>
    public override bool Equals(object? obj) => obj is GuidId<TId> other && Equals(other);

    /// <summary>
    /// Gets the hash code for this instance
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(GuidId<TId> left, GuidId<TId> right) => left.Equals(right);

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(GuidId<TId> left, GuidId<TId> right) => !left.Equals(right);

    /// <summary>
    /// Implicit conversion to GUID
    /// </summary>
    public static implicit operator Guid(GuidId<TId> id) => id.Value;

    /// <summary>
    /// Explicit conversion from GUID
    /// </summary>
    public static explicit operator GuidId<TId>(Guid value) => new(value);
} 