using BuildingBlocks.Domain.StronglyTypedIds;
using System.Text.Json.Serialization;

namespace AuthService.Domain.StronglyTypedIds;

/// <summary>
/// Strongly-typed identifier for Role entities
/// </summary>
[StronglyTypedId(typeof(int))]
[JsonConverter(typeof(StronglyTypedIdJsonConverterFactory))]
public readonly struct RoleId : IStronglyTypedId<int>, IEquatable<RoleId>
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
        Value = value;
    }

    /// <summary>
    /// Gets a zero identifier
    /// </summary>
    public static RoleId Zero => new(0);

    /// <summary>
    /// Checks if the identifier is zero
    /// </summary>
    public bool IsZero => Value == 0;

    /// <summary>
    /// Creates a new instance from the underlying value
    /// </summary>
    /// <param name="value">The integer value</param>
    /// <returns>A new role identifier</returns>
    public static RoleId From(int value) => new(value);

    /// <summary>
    /// Parses a string representation to create a new instance
    /// </summary>
    /// <param name="value">The string representation of the integer</param>
    /// <returns>A new role identifier</returns>
    public static RoleId Parse(string value) => new(int.Parse(value));

    /// <summary>
    /// Tries to parse a string representation to create a new instance
    /// </summary>
    /// <param name="value">The string representation of the integer</param>
    /// <param name="result">The parsed identifier, if successful</param>
    /// <returns>True if parsing was successful</returns>
    public static bool TryParse(string? value, out RoleId result)
    {
        if (int.TryParse(value, out var intValue))
        {
            result = new RoleId(intValue);
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
    public bool Equals(RoleId other) => Value == other.Value;

    /// <summary>
    /// Determines whether this identifier equals another object
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>True if the objects are equal</returns>
    public override bool Equals(object? obj) => obj is RoleId other && Equals(other);

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
    public bool Equals(IStronglyTypedId<int>? other) => other is not null && Value == other.Value;

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(RoleId left, RoleId right) => left.Equals(right);

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(RoleId left, RoleId right) => !(left == right);

    /// <summary>
    /// Implicit conversion from integer to RoleId
    /// </summary>
    public static implicit operator RoleId(int value) => new(value);

    /// <summary>
    /// Implicit conversion from RoleId to integer
    /// </summary>
    public static implicit operator int(RoleId id) => id.Value;

    // Predefined role IDs based on the database seeding
    public static readonly RoleId Cashier = new(1);
    public static readonly RoleId Supervisor = new(2);
    public static readonly RoleId Manager = new(3);
    public static readonly RoleId Admin = new(4);
    public static readonly RoleId Inventory = new(5);
    public static readonly RoleId Reporting = new(6);
} 