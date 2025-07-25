using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Exceptions;

namespace AuthService.Domain.Exceptions;

/// <summary>
/// Exception thrown when a user is not found
/// </summary>
internal sealed class UserNotFoundException : DomainException
{
    public UserNotFoundException(UserId userId)
        : base($"User with ID '{userId}' was not found", "USER_NOT_FOUND", new { UserId = userId.Value })
    {
    }

    public UserNotFoundException(Username username)
        : base($"User with username '{username}' was not found", "USER_NOT_FOUND", new { Username = username.Value })
    {
    }

    public UserNotFoundException(Email email)
        : base($"User with email '{email}' was not found", "USER_NOT_FOUND", new { Email = email.Value })
    {
    }
}

/// <summary>
/// Exception thrown when a role is not found
/// </summary>
public sealed class RoleNotFoundException : DomainException
{
    public RoleNotFoundException(RoleId roleId)
        : base($"Role with ID '{roleId}' was not found", "ROLE_NOT_FOUND", new { RoleId = roleId.Value })
    {
    }

    public RoleNotFoundException(string roleName)
        : base($"Role with name '{roleName}' was not found", "ROLE_NOT_FOUND", new { RoleName = roleName })
    {
    }
}

/// <summary>
/// Exception thrown when a registration token is not found
/// </summary>
public sealed class RegistrationTokenNotFoundException : DomainException
{
    public RegistrationTokenNotFoundException(TokenId tokenId)
        : base($"Registration token with ID '{tokenId}' was not found", "TOKEN_NOT_FOUND", new { TokenId = tokenId.Value })
    {
    }
}

/// <summary>
/// Exception thrown when attempting to use an invalid token
/// </summary>
public sealed class InvalidTokenException : DomainException
{
    public InvalidTokenException(TokenId tokenId, string reason)
        : base($"Token '{tokenId}' is invalid: {reason}", "INVALID_TOKEN", new { TokenId = tokenId.Value, Reason = reason })
    {
    }
}

/// <summary>
/// Exception thrown when a user account is locked out
/// </summary>
public sealed class UserLockedOutException : DomainException
{
    public UserLockedOutException(UserId userId, DateTime lockoutEnd)
        : base($"User account '{userId}' is locked out until {lockoutEnd:yyyy-MM-dd HH:mm:ss}", "USER_LOCKED_OUT", 
               new { UserId = userId.Value, LockoutEnd = lockoutEnd })
    {
    }
}

/// <summary>
/// Exception thrown when a user account is inactive
/// </summary>
public sealed class UserInactiveException : DomainException
{
    public UserInactiveException(UserId userId)
        : base($"User account '{userId}' is inactive", "USER_INACTIVE", new { UserId = userId.Value })
    {
    }
}

/// <summary>
/// Exception thrown when authentication fails
/// </summary>
internal sealed class AuthenticationFailedException : DomainException
{
    public AuthenticationFailedException(string reason)
        : base($"Authentication failed: {reason}", "AUTHENTICATION_FAILED", new { Reason = reason })
    {
    }

    public AuthenticationFailedException(Username username, string reason)
        : base($"Authentication failed for user '{username}': {reason}", "AUTHENTICATION_FAILED", 
               new { Username = username.Value, Reason = reason })
    {
    }
}

/// <summary>
/// Exception thrown when a username is already taken
/// </summary>
internal sealed class UsernameAlreadyExistsException : DomainException
{
    public UsernameAlreadyExistsException(Username username)
        : base($"Username '{username}' is already taken", "USERNAME_ALREADY_EXISTS", new { Username = username.Value })
    {
    }
}

/// <summary>
/// Exception thrown when an email is already registered
/// </summary>
public sealed class EmailAlreadyExistsException : DomainException
{
    public EmailAlreadyExistsException(Email email)
        : base($"Email '{email}' is already registered", "EMAIL_ALREADY_EXISTS", new { Email = email.Value })
    {
    }
}

/// <summary>
/// Exception thrown when a role name is already taken
/// </summary>
public sealed class RoleNameAlreadyExistsException : DomainException
{
    public RoleNameAlreadyExistsException(string roleName)
        : base($"Role name '{roleName}' is already taken", "ROLE_NAME_ALREADY_EXISTS", new { RoleName = roleName })
    {
    }
}

/// <summary>
/// Exception thrown when attempting invalid role operations
/// </summary>
public sealed class InvalidRoleOperationException : DomainException
{
    public InvalidRoleOperationException(string operation, string reason)
        : base($"Invalid role operation '{operation}': {reason}", "INVALID_ROLE_OPERATION", 
               new { Operation = operation, Reason = reason })
    {
    }

    public InvalidRoleOperationException(RoleId roleId, string operation, string reason)
        : base($"Invalid role operation '{operation}' on role '{roleId}': {reason}", "INVALID_ROLE_OPERATION", 
               new { RoleId = roleId.Value, Operation = operation, Reason = reason })
    {
    }
}

/// <summary>
/// Exception thrown when password validation fails
/// </summary>
public sealed class InvalidPasswordException : DomainException
{
    public InvalidPasswordException(string reason)
        : base($"Password is invalid: {reason}", "INVALID_PASSWORD", new { Reason = reason })
    {
    }
}

/// <summary>
/// Exception thrown when token operations fail
/// </summary>
public sealed class TokenOperationException : DomainException
{
    public TokenOperationException(string operation, string reason)
        : base($"Token operation '{operation}' failed: {reason}", "TOKEN_OPERATION_FAILED", 
               new { Operation = operation, Reason = reason })
    {
    }

    public TokenOperationException(TokenId tokenId, string operation, string reason)
        : base($"Token operation '{operation}' failed for token '{tokenId}': {reason}", "TOKEN_OPERATION_FAILED", 
               new { TokenId = tokenId.Value, Operation = operation, Reason = reason })
    {
    }
}

/// <summary>
/// Record representing authentication result with failure details
/// </summary>
internal sealed record AuthenticationResult(
    bool IsSuccess,
    UserId? UserId,
    string? FailureReason,
    int FailedAttempts,
    DateTime? LockoutEnd
)
{
    /// <summary>
    /// Creates a successful authentication result
    /// </summary>
    public static AuthenticationResult Success(UserId userId) =>
        new(true, userId, null, 0, null);

    /// <summary>
    /// Creates a failed authentication result
    /// </summary>
    public static AuthenticationResult Failure(string reason, int failedAttempts = 0, DateTime? lockoutEnd = null) =>
        new(false, null, reason, failedAttempts, lockoutEnd);
}

/// <summary>
/// Record representing token validation result
/// </summary>
internal sealed record TokenValidationResult(
    bool IsValid,
    TokenId? TokenId,
    string? FailureReason
)
{
    /// <summary>
    /// Creates a successful validation result
    /// </summary>
    public static TokenValidationResult Success(TokenId tokenId) =>
        new(true, tokenId, null);

    /// <summary>
    /// Creates a failed validation result
    /// </summary>
    public static TokenValidationResult Failure(string reason) =>
        new(false, null, reason);
} 