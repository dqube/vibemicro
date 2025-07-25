using BuildingBlocks.Domain.ValueObjects;

namespace AuthService.Domain.ValueObjects;

/// <summary>
/// Token type value object representing the type of registration token
/// </summary>
public sealed record TokenType : SingleValueObject<string>
{
    /// <summary>
    /// Email verification token type
    /// </summary>
    public static readonly TokenType EmailVerification = new("EmailVerification");

    /// <summary>
    /// Password reset token type
    /// </summary>
    public static readonly TokenType PasswordReset = new("PasswordReset");

    /// <summary>
    /// All valid token types
    /// </summary>
    public static readonly IReadOnlyList<TokenType> ValidTypes = new[]
    {
        EmailVerification,
        PasswordReset
    };

    /// <summary>
    /// Initializes a new instance of the TokenType class
    /// </summary>
    /// <param name="value">The token type value</param>
    public TokenType(string value) : base(ValidateTokenType(value))
    {
    }

    /// <summary>
    /// Creates a TokenType from a string value
    /// </summary>
    /// <param name="value">The token type value</param>
    /// <returns>A new TokenType instance</returns>
    public static TokenType From(string value) => new(value);

    /// <summary>
    /// Tries to create a TokenType from a string value
    /// </summary>
    /// <param name="value">The token type value</param>
    /// <param name="tokenType">The created token type if successful</param>
    /// <returns>True if the token type was created successfully</returns>
    public static bool TryFrom(string? value, out TokenType? tokenType)
    {
        tokenType = null;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            tokenType = new TokenType(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if the token type is for email verification
    /// </summary>
    public bool IsEmailVerification => Value == EmailVerification.Value;

    /// <summary>
    /// Checks if the token type is for password reset
    /// </summary>
    public bool IsPasswordReset => Value == PasswordReset.Value;

    /// <summary>
    /// Validates the token type value
    /// </summary>
    /// <param name="value">The token type value to validate</param>
    /// <returns>The validated token type value</returns>
    /// <exception cref="ArgumentException">Thrown when the token type is invalid</exception>
    private static string ValidateTokenType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Token type cannot be null, empty, or whitespace.", nameof(value));

        var trimmed = value.Trim();

        if (!ValidTypes.Any(vt => vt.Value == trimmed))
            throw new ArgumentException($"Invalid token type '{trimmed}'. Valid types are: {string.Join(", ", ValidTypes.Select(vt => vt.Value))}.", nameof(value));

        return trimmed;
    }

    /// <summary>
    /// Implicit conversion from string to TokenType
    /// </summary>
    /// <param name="value">The string value</param>
    public static implicit operator TokenType(string value) => new(value);

    /// <summary>
    /// Implicit conversion from TokenType to string
    /// </summary>
    /// <param name="tokenType">The TokenType instance</param>
    public static implicit operator string(TokenType tokenType) => tokenType.Value;
} 