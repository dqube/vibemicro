using BuildingBlocks.Domain.StronglyTypedIds;
using System.Text.Json.Serialization;

namespace AuthService.Domain.StronglyTypedIds;

/// <summary>
/// Strongly-typed identifier for RegistrationToken entities
/// </summary>
// [StronglyTypedId(typeof(Guid))]
// [JsonConverter(typeof(StronglyTypedIdJsonConverterFactory))]
public readonly struct TokenId : IStronglyTypedId<Guid>, IEquatable<TokenId>
{
    /// <summary>
    /// Gets the underlying GUID value
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the TokenId struct
    /// </summary>
    /// <param name="value">The GUID value</param>
    public TokenId(Guid value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new instance with a new GUID
    /// </summary>
    /// <returns>A new token identifier</returns>
    public static TokenId New() => new(Guid.NewGuid());

    /// <summary>
    /// Gets an empty identifier
    /// </summary>
    public static TokenId Empty => new(Guid.Empty);

    /// <summary>
    /// Checks if the identifier is empty
    /// </summary>
    public bool IsEmpty => Value == Guid.Empty;

    /// <summary>
    /// Creates a new instance from the underlying value
    /// </summary>
    /// <param name="value">The GUID value</param>
    /// <returns>A new token identifier</returns>
    public static TokenId From(Guid value) => new(value);

    /// <summary>
    /// Parses a string representation to create a new instance
    /// </summary>
    /// <param name="value">The string representation of the GUID</param>
    /// <returns>A new token identifier</returns>
    public static TokenId Parse(string value) => new(Guid.Parse(value));

    /// <summary>
    /// Tries to parse a string representation to create a new instance
    /// </summary>
    /// <param name="value">The string representation of the GUID</param>
    /// <param name="result">The parsed identifier, if successful</param>
    /// <returns>True if parsing was successful</returns>
    public static bool TryParse(string? value, out TokenId result)
    {
        if (Guid.TryParse(value, out var guid))
        {
            result = new TokenId(guid);
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
    public bool Equals(TokenId other) => Value.Equals(other.Value);

    /// <summary>
    /// Determines whether this identifier equals another object
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>True if the objects are equal</returns>
    public override bool Equals(object? obj) => obj is TokenId other && Equals(other);

    /// <summary>
    /// Gets the hash code for this identifier
    /// </summary>
    /// <returns>The hash code</returns>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Returns the string representation of this identifier
    /// </summary>
    /// <returns>The string representation</returns>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Determines whether this identifier equals another strongly-typed identifier
    /// </summary>
    /// <param name="other">The other identifier to compare</param>
    /// <returns>True if the identifiers are equal</returns>
    public bool Equals(IStronglyTypedId<Guid>? other) => other is not null && Value.Equals(other.Value);

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(TokenId left, TokenId right) => left.Equals(right);

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(TokenId left, TokenId right) => !(left == right);

    /// <summary>
    /// Implicit conversion from GUID to TokenId
    /// </summary>
    public static implicit operator TokenId(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from TokenId to GUID
    /// </summary>
    public static implicit operator Guid(TokenId id) => id.Value;
} 