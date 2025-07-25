using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.StronglyTypedIds;

/// <summary>
/// Strongly-typed identifier for RegistrationToken entity
/// </summary>
internal readonly struct TokenId : IStronglyTypedId<Guid>, IEquatable<TokenId>
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
    /// Creates a new instance with a generated GUID
    /// </summary>
    /// <returns>A new TokenId with a generated GUID</returns>
    public static TokenId New() => new(Guid.NewGuid());

    /// <summary>
    /// Gets an empty TokenId
    /// </summary>
    public static TokenId Empty => new(Guid.Empty);

    /// <summary>
    /// Checks if the identifier is empty
    /// </summary>
    public bool IsEmpty => Value == Guid.Empty;

    /// <summary>
    /// Returns the string representation of the GUID
    /// </summary>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Determines whether this instance is equal to another
    /// </summary>
    public bool Equals(TokenId other) => Value.Equals(other.Value);

    /// <summary>
    /// Determines whether this instance is equal to another IStronglyTypedId
    /// </summary>
    public bool Equals(IStronglyTypedId<Guid>? other) => other is not null && Value.Equals(other.Value);

    /// <summary>
    /// Determines whether this instance is equal to the specified object
    /// </summary>
    public override bool Equals(object? obj) => obj is TokenId other && Equals(other);

    /// <summary>
    /// Gets the hash code for this instance
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(TokenId left, TokenId right) => left.Equals(right);

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(TokenId left, TokenId right) => !left.Equals(right);

    /// <summary>
    /// Implicit conversion from GUID to TokenId
    /// </summary>
    /// <param name="value">The GUID value</param>
    /// <returns>A new TokenId instance</returns>
    public static implicit operator TokenId(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from TokenId to GUID
    /// </summary>
    /// <param name="tokenId">The TokenId instance</param>
    /// <returns>The underlying GUID value</returns>
    public static implicit operator Guid(TokenId tokenId) => tokenId.Value;
} 