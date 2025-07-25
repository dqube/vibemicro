using System.Text.Json.Serialization;

namespace BuildingBlocks.Domain.StronglyTypedIds;

/// <summary>
/// Base readonly struct for GUID-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The derived identifier type</typeparam>
// [StronglyTypedId(typeof(Guid))]
// [JsonConverter(typeof(StronglyTypedIdJsonConverterFactory))]
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
    /// Parses a string representation to create a new instance
    /// </summary>
    /// <param name="value">The string representation of the GUID</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static TId Parse(string value)
    {
        return From(Guid.Parse(value));
    }

    /// <summary>
    /// Tries to parse a string representation to create a new instance
    /// </summary>
    /// <param name="value">The string representation of the GUID</param>
    /// <param name="result">The parsed identifier, if successful</param>
    /// <returns>True if parsing was successful</returns>
    public static bool TryParse(string? value, out TId result)
    {
        if (Guid.TryParse(value, out var guid))
        {
            result = From(guid);
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
    public bool Equals(GuidId<TId> other)
    {
        return Value.Equals(other.Value);
    }

    /// <summary>
    /// Determines whether this identifier equals another object
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>True if the objects are equal</returns>
    public override bool Equals(object? obj)
    {
        return obj is GuidId<TId> other && Equals(other);
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
    public bool Equals(IStronglyTypedId<Guid>? other)
    {
        return other is not null && Value.Equals(other.Value);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="left">The left operand</param>
    /// <param name="right">The right operand</param>
    /// <returns>True if the operands are equal</returns>
    public static bool operator ==(GuidId<TId> left, GuidId<TId> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="left">The left operand</param>
    /// <param name="right">The right operand</param>
    /// <returns>True if the operands are not equal</returns>
    public static bool operator !=(GuidId<TId> left, GuidId<TId> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Implicit conversion from GUID to strongly-typed identifier
    /// </summary>
    /// <param name="value">The GUID value</param>
    /// <returns>A new strongly-typed identifier instance</returns>
    public static implicit operator GuidId<TId>(Guid value)
    {
        return new GuidId<TId>(value);
    }

    /// <summary>
    /// Implicit conversion from strongly-typed identifier to GUID
    /// </summary>
    /// <param name="id">The strongly-typed identifier</param>
    /// <returns>The underlying GUID value</returns>
    public static implicit operator Guid(GuidId<TId> id)
    {
        return id.Value;
    }
} 