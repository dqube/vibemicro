namespace AuthService.Domain.Services;

/// <summary>
/// Reasons for authentication failure
/// </summary>
public enum AuthenticationFailureReason
{
    /// <summary>
    /// The user was not found
    /// </summary>
    UserNotFound,

    /// <summary>
    /// The password provided was invalid
    /// </summary>
    InvalidPassword,

    /// <summary>
    /// The user account is inactive
    /// </summary>
    UserInactive,

    /// <summary>
    /// The user account is locked out
    /// </summary>
    UserLockedOut,

    /// <summary>
    /// The user account has not been verified
    /// </summary>
    AccountNotVerified
} 