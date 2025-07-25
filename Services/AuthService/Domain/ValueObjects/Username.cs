using BuildingBlocks.Domain.ValueObjects;

namespace AuthService.Domain.ValueObjects;

/// <summary>
/// Username value object with validation
/// </summary>
public sealed record Username : SingleValueObject<string>
{
    public const int MinLength = 3;
    public const int MaxLength = 50;

    /// <summary>
    /// Initializes a new instance of the Username class
    /// </summary>
    /// <param name="value">The username value</param>
    public Username(string value) : base(ValidateAndFormat(value))
    {
    }

    /// <summary>
    /// Creates a Username from a string value
    /// </summary>
    /// <param name="value">The username value</param>
    /// <returns>A new Username instance</returns>
    public static Username From(string value) => new(value);

    /// <summary>
    /// Tries to create a Username from a string value
    /// </summary>
    /// <param name="value">The username value</param>
    /// <param name="username">The created username if successful</param>
    /// <returns>True if the username was created successfully</returns>
    public static bool TryFrom(string? value, out Username? username)
    {
        username = null;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            username = new Username(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates and formats the username value
    /// </summary>
    /// <param name="value">The username value to validate</param>
    /// <returns>The validated and formatted username</returns>
    /// <exception cref="ArgumentException">Thrown when the username is invalid</exception>
    private static string ValidateAndFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be null, empty, or whitespace.", nameof(value));

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength)
            throw new ArgumentException($"Username must be at least {MinLength} characters long.", nameof(value));

        if (trimmed.Length > MaxLength)
            throw new ArgumentException($"Username cannot exceed {MaxLength} characters.", nameof(value));

        // Username validation rules
        if (!IsValidUsername(trimmed))
            throw new ArgumentException("Username contains invalid characters. Only letters, numbers, underscores, and hyphens are allowed.", nameof(value));

        return trimmed.ToLowerInvariant(); // Normalize to lowercase
    }

    /// <summary>
    /// Validates username format
    /// </summary>
    /// <param name="username">The username to validate</param>
    /// <returns>True if the username format is valid</returns>
    private static bool IsValidUsername(string username)
    {
        // Username can contain letters, numbers, underscores, and hyphens
        // Must start with a letter or number
        if (!char.IsLetterOrDigit(username[0]))
            return false;

        return username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-');
    }

    /// <summary>
    /// Implicit conversion from string to Username
    /// </summary>
    /// <param name="value">The string value</param>
    public static implicit operator Username(string value) => new(value);

    /// <summary>
    /// Implicit conversion from Username to string
    /// </summary>
    /// <param name="username">The Username instance</param>
    public static implicit operator string(Username username) => username.Value;
} 