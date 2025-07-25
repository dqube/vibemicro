using BuildingBlocks.Domain.Repository;
using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Repositories;

/// <summary>
/// Repository interface for RegistrationToken entities
/// </summary>
public interface IRegistrationTokenRepository : IGuidRepository<RegistrationToken, TokenId>
{
    /// <summary>
    /// Gets all tokens for a specific user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of tokens for the user</returns>
    Task<IEnumerable<RegistrationToken>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tokens by type
    /// </summary>
    /// <param name="tokenType">The token type</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of tokens of the specified type</returns>
    Task<IEnumerable<RegistrationToken>> GetByTypeAsync(TokenType tokenType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all valid tokens for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of valid tokens for the user</returns>
    Task<IEnumerable<RegistrationToken>> GetValidTokensByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a valid email verification token for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Valid email verification token if found, null otherwise</returns>
    Task<RegistrationToken?> GetValidEmailVerificationTokenAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a valid password reset token for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Valid password reset token if found, null otherwise</returns>
    Task<RegistrationToken?> GetValidPasswordResetTokenAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all expired tokens
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of expired tokens</returns>
    Task<IEnumerable<RegistrationToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all used tokens
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of used tokens</returns>
    Task<IEnumerable<RegistrationToken>> GetUsedTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tokens that are candidates for cleanup (old, used, or expired)
    /// </summary>
    /// <param name="retentionPeriod">How long to retain tokens after they become inactive</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of tokens that can be cleaned up</returns>
    Task<IEnumerable<RegistrationToken>> GetTokensForCleanupAsync(TimeSpan? retentionPeriod = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes expired tokens older than the specified retention period
    /// </summary>
    /// <param name="retentionPeriod">How long to retain expired tokens</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of tokens deleted</returns>
    Task<int> CleanupExpiredTokensAsync(TimeSpan? retentionPeriod = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all tokens for a user (marks them as used)
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="tokenType">Optional token type to filter by</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of tokens invalidated</returns>
    Task<int> InvalidateTokensForUserAsync(UserId userId, TokenType? tokenType = null, CancellationToken cancellationToken = default);
} 