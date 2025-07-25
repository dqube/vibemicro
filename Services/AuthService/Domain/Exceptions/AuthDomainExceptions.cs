using BuildingBlocks.Domain.Exceptions;
using AuthService.Domain.StronglyTypedIds;

namespace AuthService.Domain.Exceptions;

/// <summary>
/// Exception thrown when a user is not found
/// </summary>
public sealed class UserNotFoundException : DomainException
{
    public UserId UserId { get; }

    public UserNotFoundException(UserId userId) : base($"User with ID '{userId}' was not found.")
    {
        UserId = userId;
    }

    public UserNotFoundException(string username) : base($"User with username '{username}' was not found.")
    {
        UserId = UserId.Empty;
    }
}

/// <summary>
/// Exception thrown when a role is not found
/// </summary>
public sealed class RoleNotFoundException : DomainException
{
    public RoleId RoleId { get; }

    public RoleNotFoundException(RoleId roleId) : base($"Role with ID '{roleId}' was not found.")
    {
        RoleId = roleId;
    }

    public RoleNotFoundException(string roleName) : base($"Role with name '{roleName}' was not found.")
    {
        RoleId = RoleId.Zero;
    }
}

/// <summary>
/// Exception thrown when a registration token is not found
/// </summary>
public sealed class RegistrationTokenNotFoundException : DomainException
{
    public TokenId TokenId { get; }

    public RegistrationTokenNotFoundException(TokenId tokenId) : base($"Registration token with ID '{tokenId}' was not found.")
    {
        TokenId = tokenId;
    }
}

/// <summary>
/// Exception thrown when authentication fails
/// </summary>
public sealed class AuthenticationFailedException : DomainException
{
    public string Identifier { get; }
    public AuthenticationFailureReason Reason { get; }

    public AuthenticationFailedException(string identifier, AuthenticationFailureReason reason) 
        : base(GetMessageForReason(identifier, reason))
    {
        Identifier = identifier;
        Reason = reason;
    }

    private static string GetMessageForReason(string identifier, AuthenticationFailureReason reason)
    {
        return reason switch
        {
            AuthenticationFailureReason.UserNotFound => $"User '{identifier}' was not found.",
            AuthenticationFailureReason.InvalidPassword => "Invalid password provided.",
            AuthenticationFailureReason.UserInactive => $"User '{identifier}' is inactive.",
            AuthenticationFailureReason.UserLockedOut => $"User '{identifier}' is locked out.",
            AuthenticationFailureReason.AccountNotVerified => $"User '{identifier}' account is not verified.",
            _ => $"Authentication failed for user '{identifier}'."
        };
    }
}

/// <summary>
/// Exception thrown when a user account is locked out
/// </summary>
public sealed class UserLockedOutException : DomainException
{
    public UserId UserId { get; }
    public DateTime LockoutEnd { get; }

    public UserLockedOutException(UserId userId, DateTime lockoutEnd) 
        : base($"User '{userId}' is locked out until {lockoutEnd:yyyy-MM-dd HH:mm:ss} UTC.")
    {
        UserId = userId;
        LockoutEnd = lockoutEnd;
    }
}

/// <summary>
/// Exception thrown when a user account is inactive
/// </summary>
public sealed class UserInactiveException : DomainException
{
    public UserId UserId { get; }

    public UserInactiveException(UserId userId) : base($"User '{userId}' is inactive.")
    {
        UserId = userId;
    }
}

/// <summary>
/// Exception thrown when a registration token is invalid
/// </summary>
public sealed class InvalidRegistrationTokenException : DomainException
{
    public TokenId TokenId { get; }
    public TokenValidationFailureReason Reason { get; }

    public InvalidRegistrationTokenException(TokenId tokenId, TokenValidationFailureReason reason) 
        : base(GetMessageForReason(tokenId, reason))
    {
        TokenId = tokenId;
        Reason = reason;
    }

    private static string GetMessageForReason(TokenId tokenId, TokenValidationFailureReason reason)
    {
        return reason switch
        {
            TokenValidationFailureReason.Expired => $"Registration token '{tokenId}' has expired.",
            TokenValidationFailureReason.AlreadyUsed => $"Registration token '{tokenId}' has already been used.",
            TokenValidationFailureReason.WrongType => $"Registration token '{tokenId}' is not the correct type for this operation.",
            _ => $"Registration token '{tokenId}' is invalid."
        };
    }
}

/// <summary>
/// Reasons for token validation failure
/// </summary>
public enum TokenValidationFailureReason
{
    Expired,
    AlreadyUsed,
    WrongType,
    NotFound
}

/// <summary>
/// Exception thrown when attempting to duplicate a username
/// </summary>
public sealed class DuplicateUsernameException : DomainException
{
    public string Username { get; }

    public DuplicateUsernameException(string username) : base($"Username '{username}' is already taken.")
    {
        Username = username;
    }
}

/// <summary>
/// Exception thrown when attempting to duplicate an email address
/// </summary>
public sealed class DuplicateEmailException : DomainException
{
    public string Email { get; }

    public DuplicateEmailException(string email) : base($"Email address '{email}' is already in use.")
    {
        Email = email;
    }
}

/// <summary>
/// Exception thrown when a password doesn't meet strength requirements
/// </summary>
public sealed class WeakPasswordException : DomainException
{
    public WeakPasswordException() : base("Password does not meet strength requirements.")
    {
    }

    public WeakPasswordException(string details) : base($"Password does not meet strength requirements: {details}")
    {
    }
}

/// <summary>
/// Exception thrown when attempting to assign an invalid role
/// </summary>
public sealed class InvalidRoleException : DomainException
{
    public RoleId RoleId { get; }

    public InvalidRoleException(RoleId roleId) : base($"Role '{roleId}' is not valid or does not exist.")
    {
        RoleId = roleId;
    }
}

/// <summary>
/// Exception thrown when attempting unauthorized role operations
/// </summary>
public sealed class UnauthorizedRoleOperationException : DomainException
{
    public UserId UserId { get; }
    public RoleId RoleId { get; }

    public UnauthorizedRoleOperationException(UserId userId, RoleId roleId) 
        : base($"User '{userId}' is not authorized to perform operations with role '{roleId}'.")
    {
        UserId = userId;
        RoleId = roleId;
    }
}

/// <summary>
/// Exception thrown when email verification is required but not completed
/// </summary>
public sealed class EmailVerificationRequiredException : DomainException
{
    public UserId UserId { get; }

    public EmailVerificationRequiredException(UserId userId) 
        : base($"Email verification is required for user '{userId}' before this operation can be performed.")
    {
        UserId = userId;
    }
}

/// <summary>
/// Exception thrown when attempting to perform operations on a non-existent token
/// </summary>
public sealed class TokenOperationException : DomainException
{
    public TokenId TokenId { get; }

    public TokenOperationException(TokenId tokenId, string operation) 
        : base($"Cannot perform '{operation}' operation on token '{tokenId}'.")
    {
        TokenId = tokenId;
    }
} 