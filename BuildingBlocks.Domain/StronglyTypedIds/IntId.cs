namespace BuildingBlocks.Domain.StronglyTypedIds;

/// <summary>
/// Base readonly struct for integer-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The derived identifier type</typeparam>
public readonly struct IntId<TId> : IStronglyTypedId<int>, IEquatable<IntId<TId>>
    where TId : struct, IStronglyTypedId<int>
{
    /// <summary>
    /// Gets the underlying integer value
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Initializes a new instance of the IntId struct
    /// </summary>
    /// <param name="value">The integer value</param>
    public IntId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Integer ID must be greater than zero", nameof(value));
        Value = value;
    }

    /// <summary>
    /// Creates a new instance from the underlying value
    /// </summary>
    /// <param name="value">The integer value</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static TId From(int value)
    {
        return (TId)Activator.CreateInstance(typeof(TId), value)!;
    }

    /// <summary>
    /// Tries to create a new instance from the underlying value
    /// </summary>
    /// <param name="value">The integer value</param>
    /// <param name="result">The resulting strongly-typed identifier</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool TryFrom(int value, out TId result)
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
    /// Creates a new instance with the next available identifier
    /// </summary>
    /// <param name="current">The current identifier value</param>
    /// <returns>A new identifier with incremented value</returns>
    public static TId Next(TId current)
    {
        return From(current.Value + 1);
    }

    /// <summary>
    /// Gets the minimum valid identifier value
    /// </summary>
    public static TId Min => From(1);

    /// <summary>
    /// Gets the maximum valid identifier value
    /// </summary>
    public static TId Max => From(int.MaxValue);

    /// <summary>
    /// Returns the string representation of the integer
    /// </summary>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Determines whether this instance is equal to another
    /// </summary>
    public bool Equals(IntId<TId> other) => Value.Equals(other.Value);

    /// <summary>
    /// Determines whether this instance is equal to another IStronglyTypedId
    /// </summary>
    public bool Equals(IStronglyTypedId<int>? other) => other is not null && Value.Equals(other.Value);

    /// <summary>
    /// Determines whether this instance is equal to the specified object
    /// </summary>
    public override bool Equals(object? obj) => obj is IntId<TId> other && Equals(other);

    /// <summary>
    /// Gets the hash code for this instance
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(IntId<TId> left, IntId<TId> right) => left.Equals(right);

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(IntId<TId> left, IntId<TId> right) => !left.Equals(right);

    /// <summary>
    /// Implicit conversion to int
    /// </summary>
    public static implicit operator int(IntId<TId> id) => id.Value;

    /// <summary>
    /// Explicit conversion from int
    /// </summary>
    public static explicit operator IntId<TId>(int value) => new(value);
} 