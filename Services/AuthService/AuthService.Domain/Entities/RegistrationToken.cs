using AuthService.Domain.DomainEvents;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Extensions;

namespace AuthService.Domain.Entities;

/// <summary>
/// Represents a registration token for email verification or password reset
/// </summary>
public sealed class RegistrationToken : GuidEntity<TokenId>, IAuditableEntity
{
    /// <summary>
    /// Gets the user identifier this token belongs to
    /// </summary>
    public UserId UserId { get; private set; } = null!;

    /// <summary>
    /// Gets the type of token
    /// </summary>
    public TokenType TokenType { get; private set; } = null!;

    /// <summary>
    /// Gets when the token expires
    /// </summary>
    public DateTime Expiration { get; private set; }

    /// <summary>
    /// Gets whether the token has been used
    /// </summary>
    public bool IsUsed { get; private set; }

    /// <summary>
    /// Gets when the token was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets who created the token
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets when the token was last modified
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// Gets who last modified the token
    /// </summary>
    public string? LastModifiedBy { get; set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Private constructor for ORM
    /// </summary>
    private RegistrationToken() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the RegistrationToken class
    /// </summary>
    /// <param name="id">The token identifier</param>
    /// <param name="userId">The user identifier</param>
    /// <param name="tokenType">The token type</param>
    /// <param name="expiration">When the token expires</param>
    /// <param name="createdBy">Who created the token</param>
    public RegistrationToken(TokenId id, UserId userId, TokenType tokenType, DateTime expiration, string createdBy)
        : base()
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        TokenType = tokenType ?? throw new ArgumentNullException(nameof(tokenType));
        Expiration = expiration;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;
        IsUsed = false;

        if (expiration <= DateTime.UtcNow)
            throw new ArgumentException("Token expiration must be in the future", nameof(expiration));

        AddDomainEvent(new RegistrationTokenCreatedDomainEvent(Id, UserId, TokenType));
    }

    /// <summary>
    /// Factory method to create a new registration token
    /// </summary>
    /// <param name="user">The user the token belongs to</param>
    /// <param name="tokenType">The token type</param>
    /// <param name="createdBy">Who created the token</param>
    /// <param name="customExpiration">Custom expiration time (optional)</param>
    /// <returns>A new registration token instance</returns>
    public static RegistrationToken Create(User user, TokenType tokenType, string createdBy, DateTime? customExpiration = null)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var tokenId = TokenId.New();
        var expiration = customExpiration ?? DateTime.UtcNow.Add(tokenType.GetDefaultExpiration());
        
        var token = new RegistrationToken(tokenId, user.Id, tokenType, expiration, createdBy);
        user.AddRegistrationToken(token);
        
        return token;
    }

    /// <summary>
    /// Marks the token as used
    /// </summary>
    /// <param name="modifiedBy">Who used the token</param>
    public void MarkAsUsed(string modifiedBy)
    {
        if (IsUsed)
            return;

        if (IsExpired())
            throw new InvalidOperationException("Cannot use an expired token");

        IsUsed = true;
        LastModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
        LastModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new RegistrationTokenUsedDomainEvent(Id, UserId, TokenType));
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
    /// Checks if the token is valid (not expired and not used)
    /// </summary>
    /// <returns>True if the token is valid</returns>
    public bool IsValid()
    {
        return !IsExpired() && !IsUsed;
    }

    /// <summary>
    /// Gets the remaining time before the token expires
    /// </summary>
    /// <returns>The remaining time, or null if already expired</returns>
    public TimeSpan? GetRemainingTime()
    {
        if (IsExpired())
            return null;

        return Expiration - DateTime.UtcNow;
    }

    /// <summary>
    /// Extends the token expiration
    /// </summary>
    /// <param name="newExpiration">The new expiration time</param>
    /// <param name="modifiedBy">Who extended the token</param>
    public void ExtendExpiration(DateTime newExpiration, string modifiedBy)
    {
        if (IsUsed)
            throw new InvalidOperationException("Cannot extend expiration of a used token");

        if (newExpiration <= DateTime.UtcNow)
            throw new ArgumentException("New expiration must be in the future", nameof(newExpiration));

        if (newExpiration <= Expiration)
            throw new ArgumentException("New expiration must be later than current expiration", nameof(newExpiration));

        Expiration = newExpiration;
        LastModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
        LastModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new RegistrationTokenExtendedDomainEvent(Id, UserId, newExpiration));
    }

    /// <summary>
    /// Validates the token for use
    /// </summary>
    /// <param name="userIdToValidate">The user ID to validate against</param>
    /// <param name="tokenTypeToValidate">The token type to validate against</param>
    /// <returns>True if the token is valid for the specified parameters</returns>
    public bool ValidateForUse(UserId userIdToValidate, TokenType tokenTypeToValidate)
    {
        if (userIdToValidate == null || tokenTypeToValidate == null)
            return false;

        return UserId.Equals(userIdToValidate) && 
               TokenType.Equals(tokenTypeToValidate) && 
               IsValid();
    }

    /// <summary>
    /// Returns the string representation of the registration token
    /// </summary>
    public override string ToString()
    {
        var status = IsUsed ? "Used" : IsExpired() ? "Expired" : "Active";
        return $"Token: {Id} ({TokenType}) - {status}";
    }
} 