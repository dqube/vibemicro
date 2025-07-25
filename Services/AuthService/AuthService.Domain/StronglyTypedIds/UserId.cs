using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.StronglyTypedIds;

/// <summary>
/// Strongly-typed identifier for User entity
/// </summary>
internal readonly struct UserId : IStronglyTypedId<Guid>, IEquatable<UserId>
{
    /// <summary>
    /// Gets the underlying GUID value
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the UserId struct
    /// </summary>
    /// <param name="value">The GUID value</param>
    public UserId(Guid value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new instance with a generated GUID
    /// </summary>
    /// <returns>A new UserId with a generated GUID</returns>
    public static UserId New() => new(Guid.NewGuid());

    /// <summary>
    /// Gets an empty UserId
    /// </summary>
    public static UserId Empty => new(Guid.Empty);

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
    public bool Equals(UserId other) => Value.Equals(other.Value);

    /// <summary>
    /// Determines whether this instance is equal to another IStronglyTypedId
    /// </summary>
    public bool Equals(IStronglyTypedId<Guid>? other) => other is not null && Value.Equals(other.Value);

    /// <summary>
    /// Determines whether this instance is equal to the specified object
    /// </summary>
    public override bool Equals(object? obj) => obj is UserId other && Equals(other);

    /// <summary>
    /// Gets the hash code for this instance
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(UserId left, UserId right) => left.Equals(right);

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(UserId left, UserId right) => !left.Equals(right);

    /// <summary>
    /// Implicit conversion from GUID to UserId
    /// </summary>
    /// <param name="value">The GUID value</param>
    /// <returns>A new UserId instance</returns>
    public static implicit operator UserId(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from UserId to GUID
    /// </summary>
    /// <param name="userId">The UserId instance</param>
    /// <returns>The underlying GUID value</returns>
    public static implicit operator Guid(UserId userId) => userId.Value;
} 