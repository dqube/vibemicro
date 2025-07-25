using AuthService.Domain.Entities;
using AuthService.Domain.Exceptions;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace AuthService.Domain.Services;

/// <summary>
/// Domain service for authentication operations
/// </summary>
internal interface IAuthDomainService
{
    /// <summary>
    /// Authenticates a user with username and password
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    /// <param name="passwordHashProvider">The password hash provider</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Authentication result</returns>
    Task<AuthenticationResult> AuthenticateAsync(Username username, string password, IPasswordHashProvider passwordHashProvider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="email">The email address</param>
    /// <param name="password">The password</param>
    /// <param name="passwordHashProvider">The password hash provider</param>
    /// <param name="createdBy">Who created the user</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created user</returns>
    Task<User> RegisterUserAsync(Username username, Email email, string password, IPasswordHashProvider passwordHashProvider, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an email verification token for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="createdBy">Who created the token</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created token</returns>
    Task<RegistrationToken> CreateEmailVerificationTokenAsync(UserId userId, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a password reset token for a user
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="createdBy">Who created the token</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created token</returns>
    Task<RegistrationToken> CreatePasswordResetTokenAsync(Email email, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies an email using a verification token
    /// </summary>
    /// <param name="tokenId">The token identifier</param>
    /// <param name="userId">The user identifier</param>
    /// <param name="modifiedBy">Who performed the verification</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Token validation result</returns>
    Task<TokenValidationResult> VerifyEmailAsync(TokenId tokenId, UserId userId, string modifiedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets a user's password using a reset token
    /// </summary>
    /// <param name="tokenId">The token identifier</param>
    /// <param name="userId">The user identifier</param>
    /// <param name="newPassword">The new password</param>
    /// <param name="passwordHashProvider">The password hash provider</param>
    /// <param name="modifiedBy">Who performed the reset</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Token validation result</returns>
    Task<TokenValidationResult> ResetPasswordAsync(TokenId tokenId, UserId userId, string newPassword, IPasswordHashProvider passwordHashProvider, string modifiedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="roleId">The role identifier</param>
    /// <param name="assignedBy">Who assigned the role</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated user</returns>
    Task<User> AssignRoleToUserAsync(UserId userId, RoleId roleId, string assignedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="roleId">The role identifier</param>
    /// <param name="removedBy">Who removed the role</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated user</returns>
    Task<User> RemoveRoleFromUserAsync(UserId userId, RoleId roleId, string removedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up expired tokens
    /// </summary>
    /// <param name="olderThanDays">Remove tokens older than this many days</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of tokens cleaned up</returns>
    Task<int> CleanupExpiredTokensAsync(int olderThanDays = 30, CancellationToken cancellationToken = default);
}

/// <summary>
/// Record representing user registration request
/// </summary>
internal sealed record UserRegistrationRequest(
    Username Username,
    Email Email,
    string Password,
    string CreatedBy
);

/// <summary>
/// Record representing user registration result
/// </summary>
internal sealed record UserRegistrationResult(
    bool IsSuccess,
    UserId? UserId,
    TokenId? EmailVerificationTokenId,
    string? FailureReason
)
{
    /// <summary>
    /// Creates a successful registration result
    /// </summary>
    public static UserRegistrationResult Success(UserId userId, TokenId emailVerificationTokenId) =>
        new(true, userId, emailVerificationTokenId, null);

    /// <summary>
    /// Creates a failed registration result
    /// </summary>
    public static UserRegistrationResult Failure(string reason) =>
        new(false, null, null, reason);
} 