using BuildingBlocks.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.DomainEvents;

namespace AuthService.Domain.Entities;

/// <summary>
/// Registration token entity for email verification and password reset
/// </summary>
public class RegistrationToken : Entity<TokenId>, IAuditableEntity
{
    /// <summary>
    /// Gets the user identifier this token belongs to
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the token type
    /// </summary>
    public TokenType TokenType { get; private set; }

    /// <summary>
    /// Gets the token expiration time
    /// </summary>
    public DateTime Expiration { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the token has been used
    /// </summary>
    public bool IsUsed { get; private set; }

    /// <summary>
    /// Gets the creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the user who created this entity
    /// </summary>
    public UserId? CreatedBy { get; private set; }

    /// <summary>
    /// Private constructor for Entity Framework
    /// </summary>
    private RegistrationToken() : base(TokenId.Empty)
    {
        UserId = UserId.Empty;
        TokenType = null!;
    }

    /// <summary>
    /// Initializes a new instance of the RegistrationToken class
    /// </summary>
    /// <param name="id">The token identifier</param>
    /// <param name="userId">The user identifier</param>
    /// <param name="tokenType">The token type</param>
    /// <param name="expiration">The token expiration time</param>
    /// <param name="createdBy">The user who created this entity</param>
    public RegistrationToken(TokenId id, UserId userId, TokenType tokenType, DateTime expiration, UserId? createdBy = null)
        : base(id)
    {
        UserId = userId;
        TokenType = tokenType ?? throw new ArgumentNullException(nameof(tokenType));
        Expiration = expiration;
        IsUsed = false;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Creates a new email verification token
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="validityPeriod">The validity period (default 24 hours)</param>
    /// <param name="createdBy">The user who created this entity</param>
    /// <returns>A new RegistrationToken instance</returns>
    public static RegistrationToken CreateEmailVerification(UserId userId, TimeSpan? validityPeriod = null, UserId? createdBy = null)
    {
        var expiration = DateTime.UtcNow.Add(validityPeriod ?? TimeSpan.FromHours(24));
        var token = new RegistrationToken(TokenId.New(), userId, TokenType.EmailVerification, expiration, createdBy);
        
        token.AddDomainEvent(new EmailVerificationTokenCreatedDomainEvent(token.Id, userId, expiration));
        return token;
    }

    /// <summary>
    /// Creates a new password reset token
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="validityPeriod">The validity period (default 1 hour)</param>
    /// <param name="createdBy">The user who created this entity</param>
    /// <returns>A new RegistrationToken instance</returns>
    public static RegistrationToken CreatePasswordReset(UserId userId, TimeSpan? validityPeriod = null, UserId? createdBy = null)
    {
        var expiration = DateTime.UtcNow.Add(validityPeriod ?? TimeSpan.FromHours(1));
        var token = new RegistrationToken(TokenId.New(), userId, TokenType.PasswordReset, expiration, createdBy);
        
        token.AddDomainEvent(new PasswordResetTokenCreatedDomainEvent(token.Id, userId, expiration));
        return token;
    }

    /// <summary>
    /// Uses the token, marking it as consumed
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the token is already used or expired</exception>
    public void Use()
    {
        if (IsUsed)
            throw new InvalidOperationException("Token has already been used.");

        if (IsExpired())
            throw new InvalidOperationException("Token has expired.");

        IsUsed = true;

        if (TokenType.IsEmailVerification)
            AddDomainEvent(new EmailVerificationTokenUsedDomainEvent(Id, UserId));
        else if (TokenType.IsPasswordReset)
            AddDomainEvent(new PasswordResetTokenUsedDomainEvent(Id, UserId));
    }

    /// <summary>
    /// Checks if the token is expired
    /// </summary>
    /// <returns>True if the token is expired</returns>
    public bool IsExpired()
    {
        return DateTime.UtcNow > Expiration;
    }

    /// <summary>
    /// Checks if the token is valid (not used and not expired)
    /// </summary>
    /// <returns>True if the token is valid</returns>
    public bool IsValid()
    {
        return !IsUsed && !IsExpired();
    }

    /// <summary>
    /// Validates the token for use
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the token is invalid</exception>
    public void ValidateForUse()
    {
        if (IsUsed)
            throw new InvalidOperationException("Token has already been used.");

        if (IsExpired())
            throw new InvalidOperationException("Token has expired.");
    }

    /// <summary>
    /// Gets the time remaining until expiration
    /// </summary>
    /// <returns>Time remaining, or null if expired</returns>
    public TimeSpan? GetTimeRemaining()
    {
        var remaining = Expiration - DateTime.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : null;
    }
} 