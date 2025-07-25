using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Services;

/// <summary>
/// Domain service interface for authentication operations
/// </summary>
public interface IAuthDomainService
{
    /// <summary>
    /// Authenticates a user with username and password
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Authentication result</returns>
    Task<AuthenticationResult> AuthenticateAsync(Username username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="password">The password</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Authentication result</returns>
    Task<AuthenticationResult> AuthenticateByEmailAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="email">The email address</param>
    /// <param name="password">The password</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Registration result</returns>
    Task<RegistrationResult> RegisterUserAsync(Username username, string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates password reset process for a user
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Password reset result</returns>
    Task<PasswordResetResult> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes password reset using a token
    /// </summary>
    /// <param name="tokenId">The reset token identifier</param>
    /// <param name="newPassword">The new password</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Password reset completion result</returns>
    Task<PasswordResetResult> CompletePasswordResetAsync(TokenId tokenId, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates email verification process for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Email verification result</returns>
    Task<EmailVerificationResult> InitiateEmailVerificationAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a user's email using a token
    /// </summary>
    /// <param name="tokenId">The verification token identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Email verification result</returns>
    Task<EmailVerificationResult> VerifyEmailAsync(TokenId tokenId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes a user's password
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="currentPassword">The current password</param>
    /// <param name="newPassword">The new password</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Password change result</returns>
    Task<PasswordChangeResult> ChangePasswordAsync(UserId userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a user has the required role
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="requiredRole">The required role</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the user has the required role</returns>
    Task<bool> HasRoleAsync(UserId userId, RoleId requiredRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a user has any of the required roles
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="requiredRoles">The required roles</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the user has any of the required roles</returns>
    Task<bool> HasAnyRoleAsync(UserId userId, IEnumerable<RoleId> requiredRoles, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of authentication operation
/// </summary>
public sealed record AuthenticationResult
{
    public bool IsSuccess { get; init; }
    public User? User { get; init; }
    public string? ErrorMessage { get; init; }
    public AuthenticationFailureReason? FailureReason { get; init; }

    public static AuthenticationResult Success(User user) => new()
    {
        IsSuccess = true,
        User = user
    };

    public static AuthenticationResult Failure(string errorMessage, AuthenticationFailureReason reason) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage,
        FailureReason = reason
    };
}

/// <summary>
/// Reasons for authentication failure
/// </summary>
public enum AuthenticationFailureReason
{
    UserNotFound,
    InvalidPassword,
    UserInactive,
    UserLockedOut,
    AccountNotVerified
}

/// <summary>
/// Result of registration operation
/// </summary>
public sealed record RegistrationResult
{
    public bool IsSuccess { get; init; }
    public User? User { get; init; }
    public RegistrationToken? VerificationToken { get; init; }
    public string? ErrorMessage { get; init; }

    public static RegistrationResult Success(User user, RegistrationToken verificationToken) => new()
    {
        IsSuccess = true,
        User = user,
        VerificationToken = verificationToken
    };

    public static RegistrationResult Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}

/// <summary>
/// Result of password reset operation
/// </summary>
public sealed record PasswordResetResult
{
    public bool IsSuccess { get; init; }
    public RegistrationToken? ResetToken { get; init; }
    public string? ErrorMessage { get; init; }

    public static PasswordResetResult Success(RegistrationToken? resetToken = null) => new()
    {
        IsSuccess = true,
        ResetToken = resetToken
    };

    public static PasswordResetResult Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}

/// <summary>
/// Result of email verification operation
/// </summary>
public sealed record EmailVerificationResult
{
    public bool IsSuccess { get; init; }
    public RegistrationToken? VerificationToken { get; init; }
    public string? ErrorMessage { get; init; }

    public static EmailVerificationResult Success(RegistrationToken? verificationToken = null) => new()
    {
        IsSuccess = true,
        VerificationToken = verificationToken
    };

    public static EmailVerificationResult Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}

/// <summary>
/// Result of password change operation
/// </summary>
public sealed record PasswordChangeResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }

    public static PasswordChangeResult Success() => new()
    {
        IsSuccess = true
    };

    public static PasswordChangeResult Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
} 