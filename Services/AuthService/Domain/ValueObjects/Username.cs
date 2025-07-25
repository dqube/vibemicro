using BuildingBlocks.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace AuthService.Domain.ValueObjects;

/// <summary>
/// Value object representing a username
/// </summary>
internal sealed record Username(string Value) : SingleValueObject<string>(Value)
{
    /// <summary>
    /// Minimum length for a username
    /// </summary>
    public const int MinLength = 3;

    /// <summary>
    /// Maximum length for a username
    /// </summary>
    public const int MaxLength = 50;

    private static readonly Regex UsernameRegex = new(
        @"^[a-zA-Z0-9_.-]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Validates and normalizes the username value
    /// </summary>
    /// <param name="value">The raw username value</param>
    /// <returns>The normalized username</returns>
    /// <exception cref="ArgumentException">Thrown when the username is invalid</exception>
    protected static override string ValidateValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be null or empty", nameof(value));

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength)
            throw new ArgumentException($"Username must be at least {MinLength} characters long", nameof(value));

        if (trimmed.Length > MaxLength)
            throw new ArgumentException($"Username cannot exceed {MaxLength} characters", nameof(value));

        if (!UsernameRegex.IsMatch(trimmed))
            throw new ArgumentException("Username can only contain letters, numbers, underscores, dots, and hyphens", nameof(value));

        return trimmed.ToLowerInvariant();
    }

    /// <summary>
    /// Checks if the username starts with the specified prefix
    /// </summary>
    /// <param name="prefix">The prefix to check</param>
    /// <returns>True if the username starts with the prefix</returns>
    public bool StartsWith(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return false;

        return Value.StartsWith(prefix.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the username contains the specified substring
    /// </summary>
    /// <param name="substring">The substring to check</param>
    /// <returns>True if the username contains the substring</returns>
    public bool Contains(string substring)
    {
        if (string.IsNullOrWhiteSpace(substring))
            return false;

        return Value.Contains(substring.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Implicit conversion from string to Username
    /// </summary>
    /// <param name="value">The string value</param>
    /// <returns>A new Username instance</returns>
    public static implicit operator Username(string value) => new(value);

    /// <summary>
    /// Implicit conversion from Username to string
    /// </summary>
    /// <param name="username">The Username instance</param>
    /// <returns>The underlying string value</returns>
    public static implicit operator string(Username username) => username.Value;
} 