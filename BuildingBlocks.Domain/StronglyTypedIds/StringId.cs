using System.Text.Json.Serialization;

namespace BuildingBlocks.Domain.StronglyTypedIds;

/// <summary>
/// Base readonly struct for string-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The derived identifier type</typeparam>
// [StronglyTypedId(typeof(string))]
// [JsonConverter(typeof(StronglyTypedIdJsonConverterFactory))]
public readonly struct StringId<TId> : IStronglyTypedId<string>, IEquatable<StringId<TId>>
    where TId : struct, IStronglyTypedId<string>
{
    /// <summary>
    /// Gets the underlying string value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the StringId struct
    /// </summary>
    /// <param name="value">The string value</param>
    public StringId(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets an empty identifier
    /// </summary>
    public static TId Empty => (TId)Activator.CreateInstance(typeof(TId), string.Empty)!;

    /// <summary>
    /// Checks if the identifier is empty
    /// </summary>
    public bool IsEmpty => string.IsNullOrEmpty(Value);

    /// <summary>
    /// Checks if the identifier is null or whitespace
    /// </summary>
    public bool IsNullOrWhiteSpace => string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// Creates a new instance from the underlying value
    /// </summary>
    /// <param name="value">The string value</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static TId From(string value)
    {
        return (TId)Activator.CreateInstance(typeof(TId), value)!;
    }

    /// <summary>
    /// Creates a new instance from the underlying value, or empty if null
    /// </summary>
    /// <param name="value">The string value</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static TId FromOrEmpty(string? value)
    {
        return From(value ?? string.Empty);
    }

    /// <summary>
    /// Tries to create a new instance from the underlying value
    /// </summary>
    /// <param name="value">The string value</param>
    /// <param name="result">The created identifier, if successful</param>
    /// <returns>True if creation was successful</returns>
    public static bool TryFrom(string? value, out TId result)
    {
        if (!string.IsNullOrEmpty(value))
        {
            result = From(value);
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
    public bool Equals(StringId<TId> other)
    {
        return string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether this identifier equals another object
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>True if the objects are equal</returns>
    public override bool Equals(object? obj)
    {
        return obj is StringId<TId> other && Equals(other);
    }

    /// <summary>
    /// Gets the hash code for this identifier
    /// </summary>
    /// <returns>The hash code</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode(StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns the string representation of this identifier
    /// </summary>
    /// <returns>The string representation</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Determines whether this identifier equals another strongly-typed identifier
    /// </summary>
    /// <param name="other">The other identifier to compare</param>
    /// <returns>True if the identifiers are equal</returns>
    public bool Equals(IStronglyTypedId<string>? other)
    {
        return other is not null && string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="left">The left operand</param>
    /// <param name="right">The right operand</param>
    /// <returns>True if the operands are equal</returns>
    public static bool operator ==(StringId<TId> left, StringId<TId> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="left">The left operand</param>
    /// <param name="right">The right operand</param>
    /// <returns>True if the operands are not equal</returns>
    public static bool operator !=(StringId<TId> left, StringId<TId> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Implicit conversion from string to strongly-typed identifier
    /// </summary>
    /// <param name="value">The string value</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static implicit operator StringId<TId>(string value)
    {
        return new StringId<TId>(value);
    }

    /// <summary>
    /// Implicit conversion from strongly-typed identifier to string
    /// </summary>
    /// <param name="id">The strongly-typed identifier</param>
    /// <returns>The underlying string value</returns>
    public static implicit operator string(StringId<TId> id)
    {
        return id.Value;
    }
} 