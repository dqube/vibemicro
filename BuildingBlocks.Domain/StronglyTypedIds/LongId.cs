using System.Text.Json.Serialization;

namespace BuildingBlocks.Domain.StronglyTypedIds;

/// <summary>
/// Base readonly struct for long-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The derived identifier type</typeparam>
// [StronglyTypedId(typeof(long))]
// [JsonConverter(typeof(StronglyTypedIdJsonConverterFactory))]
public readonly struct LongId<TId> : IStronglyTypedId<long>, IEquatable<LongId<TId>>
    where TId : struct, IStronglyTypedId<long>
{
    /// <summary>
    /// Gets the underlying long value
    /// </summary>
    public long Value { get; }

    /// <summary>
    /// Initializes a new instance of the LongId struct
    /// </summary>
    /// <param name="value">The long value</param>
    public LongId(long value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets a zero identifier
    /// </summary>
    public static TId Zero => (TId)Activator.CreateInstance(typeof(TId), 0L)!;

    /// <summary>
    /// Checks if the identifier is zero
    /// </summary>
    public bool IsZero => Value == 0L;

    /// <summary>
    /// Creates a new instance from the underlying value
    /// </summary>
    /// <param name="value">The long value</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static TId From(long value)
    {
        return (TId)Activator.CreateInstance(typeof(TId), value)!;
    }

    /// <summary>
    /// Parses a string representation to create a new instance
    /// </summary>
    /// <param name="value">The string representation of the long</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static TId Parse(string value)
    {
        return From(long.Parse(value));
    }

    /// <summary>
    /// Tries to parse a string representation to create a new instance
    /// </summary>
    /// <param name="value">The string representation of the long</param>
    /// <param name="result">The parsed identifier, if successful</param>
    /// <returns>True if parsing was successful</returns>
    public static bool TryParse(string? value, out TId result)
    {
        if (long.TryParse(value, out var longValue))
        {
            result = From(longValue);
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
    public bool Equals(LongId<TId> other)
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
        return obj is LongId<TId> other && Equals(other);
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
    public bool Equals(IStronglyTypedId<long>? other)
    {
        return other is not null && Value == other.Value;
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="left">The left operand</param>
    /// <param name="right">The right operand</param>
    /// <returns>True if the operands are equal</returns>
    public static bool operator ==(LongId<TId> left, LongId<TId> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="left">The left operand</param>
    /// <param name="right">The right operand</param>
    /// <returns>True if the operands are not equal</returns>
    public static bool operator !=(LongId<TId> left, LongId<TId> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Implicit conversion from long to strongly-typed identifier
    /// </summary>
    /// <param name="value">The long value</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static implicit operator LongId<TId>(long value)
    {
        return new LongId<TId>(value);
    }

    /// <summary>
    /// Implicit conversion from strongly-typed identifier to long
    /// </summary>
    /// <param name="id">The strongly-typed identifier</param>
    /// <returns>The underlying long value</returns>
    public static implicit operator long(LongId<TId> id)
    {
        return id.Value;
    }
} 