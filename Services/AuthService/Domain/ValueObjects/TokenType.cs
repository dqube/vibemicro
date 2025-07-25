using BuildingBlocks.Domain.ValueObjects;

namespace AuthService.Domain.ValueObjects;

/// <summary>
/// Enumeration representing different types of registration tokens
/// </summary>
internal sealed class TokenType : Enumeration
{
    /// <summary>
    /// Email verification token
    /// </summary>
    public static readonly TokenType EmailVerification = new(1, nameof(EmailVerification));

    /// <summary>
    /// Password reset token
    /// </summary>
    public static readonly TokenType PasswordReset = new(2, nameof(PasswordReset));

    /// <summary>
    /// Initializes a new instance of the TokenType class
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="name">The name</param>
    private TokenType(int id, string name) : base(id, name)
    {
    }

    /// <summary>
    /// Gets all available token types
    /// </summary>
    /// <returns>All token types</returns>
    public static IEnumerable<TokenType> GetAll()
    {
        return GetAll<TokenType>();
    }

    /// <summary>
    /// Gets a token type by name
    /// </summary>
    /// <param name="name">The token type name</param>
    /// <returns>The token type if found</returns>
    public static TokenType? FromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return GetAll().FirstOrDefault(t => 
            string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets a token type by ID
    /// </summary>
    /// <param name="id">The token type ID</param>
    /// <returns>The token type if found</returns>
    public static TokenType? FromId(int id)
    {
        return GetAll().FirstOrDefault(t => t.Id == id);
    }

    /// <summary>
    /// Checks if this is an email verification token
    /// </summary>
    public bool IsEmailVerification => this == EmailVerification;

    /// <summary>
    /// Checks if this is a password reset token
    /// </summary>
    public bool IsPasswordReset => this == PasswordReset;

    /// <summary>
    /// Gets the default expiration time for this token type
    /// </summary>
    /// <returns>The default expiration timespan</returns>
    public TimeSpan GetDefaultExpiration()
    {
        return this switch
        {
            _ when this == EmailVerification => TimeSpan.FromDays(1),
            _ when this == PasswordReset => TimeSpan.FromHours(1),
            _ => TimeSpan.FromHours(24)
        };
    }

    /// <summary>
    /// Implicit conversion from string to TokenType
    /// </summary>
    /// <param name="name">The token type name</param>
    /// <returns>The token type if found, null otherwise</returns>
    public static implicit operator TokenType?(string name) => FromName(name);

    /// <summary>
    /// Implicit conversion from TokenType to string
    /// </summary>
    /// <param name="tokenType">The token type</param>
    /// <returns>The token type name</returns>
    public static implicit operator string(TokenType tokenType) => tokenType?.Name ?? string.Empty;
} 