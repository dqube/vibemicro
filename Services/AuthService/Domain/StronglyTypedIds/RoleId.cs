using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.StronglyTypedIds;

/// <summary>
/// Strongly-typed identifier for Role entity
/// </summary>
internal readonly struct RoleId : IStronglyTypedId<int>, IEquatable<RoleId>
{
    /// <summary>
    /// Gets the underlying integer value
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Initializes a new instance of the RoleId struct
    /// </summary>
    /// <param name="value">The integer value</param>
    public RoleId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Role ID must be greater than zero", nameof(value));
        Value = value;
    }

    /// <summary>
    /// Returns the string representation of the integer
    /// </summary>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Determines whether this instance is equal to another
    /// </summary>
    public bool Equals(RoleId other) => Value.Equals(other.Value);

    /// <summary>
    /// Determines whether this instance is equal to another IStronglyTypedId
    /// </summary>
    public bool Equals(IStronglyTypedId<int>? other) => other is not null && Value.Equals(other.Value);

    /// <summary>
    /// Determines whether this instance is equal to the specified object
    /// </summary>
    public override bool Equals(object? obj) => obj is RoleId other && Equals(other);

    /// <summary>
    /// Gets the hash code for this instance
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(RoleId left, RoleId right) => left.Equals(right);

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(RoleId left, RoleId right) => !left.Equals(right);

    /// <summary>
    /// Implicit conversion from int to RoleId
    /// </summary>
    /// <param name="value">The integer value</param>
    /// <returns>A new RoleId instance</returns>
    public static implicit operator RoleId(int value) => new(value);

    /// <summary>
    /// Implicit conversion from RoleId to int
    /// </summary>
    /// <param name="roleId">The RoleId instance</param>
    /// <returns>The underlying integer value</returns>
    public static implicit operator int(RoleId roleId) => roleId.Value;
} 