using System.Text.Json.Serialization;

namespace BuildingBlocks.Domain.StronglyTypedIds;

/// <summary>
/// Base readonly struct for integer-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The derived identifier type</typeparam>
[StronglyTypedId(typeof(int))]
[JsonConverter(typeof(StronglyTypedIdJsonConverterFactory))]
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
        Value = value;
    }

    /// <summary>
    /// Gets a zero identifier
    /// </summary>
    public static TId Zero => (TId)Activator.CreateInstance(typeof(TId), 0)!;

    /// <summary>
    /// Checks if the identifier is zero
    /// </summary>
    public bool IsZero => Value == 0;

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
    /// Parses a string representation to create a new instance
    /// </summary>
    /// <param name="value">The string representation of the integer</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static TId Parse(string value)
    {
        return From(int.Parse(value));
    }

    /// <summary>
    /// Tries to parse a string representation to create a new instance
    /// </summary>
    /// <param name="value">The string representation of the integer</param>
    /// <param name="result">The parsed identifier, if successful</param>
    /// <returns>True if parsing was successful</returns>
    public static bool TryParse(string? value, out TId result)
    {
        if (int.TryParse(value, out var intValue))
        {
            result = From(intValue);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether two identifiers are equal
    /// </summary>
    /// <param name="other">The other identifier to compare</param>
    /// <returns>True if the identifiers are equal</returns>
    public bool Equals(IntId<TId> other)
    {
        return Value == other.Value;
    }

    /// <summary>
    /// Determines whether this identifier equals another object
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>True if the objects are equal</returns>
    public override bool Equals(object? obj)
    {
        return obj is IntId<TId> other && Equals(other);
    }

    /// <summary>
    /// Gets the hash code for this identifier
    /// </summary>
    /// <returns>The hash code</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// Returns the string representation of this identifier
    /// </summary>
    /// <returns>The string representation</returns>
    public override string ToString()
    {
        return Value.ToString();
    }

    /// <summary>
    /// Returns the string representation of this identifier in the specified format
    /// </summary>
    /// <param name="format">The format string</param>
    /// <returns>The formatted string representation</returns>
    public string ToString(string? format)
    {
        return Value.ToString(format);
    }

    /// <summary>
    /// Determines whether this identifier equals another strongly-typed identifier
    /// </summary>
    /// <param name="other">The other identifier to compare</param>
    /// <returns>True if the identifiers are equal</returns>
    public bool Equals(IStronglyTypedId<int>? other)
    {
        return other is not null && Value == other.Value;
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="left">The left operand</param>
    /// <param name="right">The right operand</param>
    /// <returns>True if the operands are equal</returns>
    public static bool operator ==(IntId<TId> left, IntId<TId> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="left">The left operand</param>
    /// <param name="right">The right operand</param>
    /// <returns>True if the operands are not equal</returns>
    public static bool operator !=(IntId<TId> left, IntId<TId> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Implicit conversion from integer to strongly-typed identifier
    /// </summary>
    /// <param name="value">The integer value</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static implicit operator IntId<TId>(int value)
    {
        return new IntId<TId>(value);
    }

    /// <summary>
    /// Implicit conversion from strongly-typed identifier to integer
    /// </summary>
    /// <param name="id">The strongly-typed identifier</param>
    /// <returns>The underlying integer value</returns>
    public static implicit operator int(IntId<TId> id)
    {
        return id.Value;
    }
} 