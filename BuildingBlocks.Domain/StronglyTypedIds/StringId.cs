namespace BuildingBlocks.Domain.StronglyTypedIds;

/// <summary>
/// Base readonly struct for string-based strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The derived identifier type</typeparam>
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
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("String ID cannot be null or whitespace", nameof(value));
        Value = value;
    }

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
    /// Tries to create a new instance from the underlying value
    /// </summary>
    /// <param name="value">The string value</param>
    /// <param name="result">The resulting strongly-typed identifier</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool TryFrom(string value, out TId result)
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
    /// Gets the length of the identifier string
    /// </summary>
    public int Length => Value.Length;

    /// <summary>
    /// Checks if the identifier is empty or whitespace
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// Converts the identifier to uppercase
    /// </summary>
    /// <returns>A new identifier with uppercase value</returns>
    public TId ToUpper()
    {
        return From(Value.ToUpperInvariant());
    }

    /// <summary>
    /// Converts the identifier to lowercase
    /// </summary>
    /// <returns>A new identifier with lowercase value</returns>
    public TId ToLower()
    {
        return From(Value.ToLowerInvariant());
    }

    /// <summary>
    /// Trims whitespace from the identifier
    /// </summary>
    /// <returns>A new identifier with trimmed value</returns>
    public TId Trim()
    {
        return From(Value.Trim());
    }

    /// <summary>
    /// Checks if the identifier starts with the specified value
    /// </summary>
    /// <param name="prefix">The prefix to check</param>
    /// <returns>True if the identifier starts with the prefix</returns>
    public bool StartsWith(string prefix)
    {
        return Value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the identifier ends with the specified value
    /// </summary>
    /// <param name="suffix">The suffix to check</param>
    /// <returns>True if the identifier ends with the suffix</returns>
    public bool EndsWith(string suffix)
    {
        return Value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the identifier contains the specified value
    /// </summary>
    /// <param name="substring">The substring to check</param>
    /// <returns>True if the identifier contains the substring</returns>
    public bool Contains(string substring)
    {
        return Value.Contains(substring, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns the string representation of the identifier
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Determines whether this instance is equal to another
    /// </summary>
    public bool Equals(StringId<TId> other) => Value.Equals(other.Value, StringComparison.Ordinal);

    /// <summary>
    /// Determines whether this instance is equal to another IStronglyTypedId
    /// </summary>
    public bool Equals(IStronglyTypedId<string>? other) => other is not null && Value.Equals(other.Value, StringComparison.Ordinal);

    /// <summary>
    /// Determines whether this instance is equal to the specified object
    /// </summary>
    public override bool Equals(object? obj) => obj is StringId<TId> other && Equals(other);

    /// <summary>
    /// Gets the hash code for this instance
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(StringId<TId> left, StringId<TId> right) => left.Equals(right);

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(StringId<TId> left, StringId<TId> right) => !left.Equals(right);

    /// <summary>
    /// Implicit conversion to string
    /// </summary>
    public static implicit operator string(StringId<TId> id) => id.Value;

    /// <summary>
    /// Explicit conversion from string
    /// </summary>
    public static explicit operator StringId<TId>(string value) => new(value);
} 