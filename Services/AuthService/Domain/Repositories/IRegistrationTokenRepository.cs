using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Repository;

namespace AuthService.Domain.Repositories;

/// <summary>
/// Repository interface for RegistrationToken entity
/// </summary>
internal interface IRegistrationTokenRepository : IRepository<RegistrationToken, TokenId, Guid>
{
    /// <summary>
    /// Gets tokens by user ID
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of tokens for the user</returns>
    Task<IEnumerable<RegistrationToken>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active tokens for a user and token type
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="tokenType">The token type</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of active tokens</returns>
    Task<IEnumerable<RegistrationToken>> GetActiveTokensAsync(UserId userId, TokenType tokenType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent active token for a user and type
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="tokenType">The token type</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The most recent active token if found</returns>
    Task<RegistrationToken?> GetLatestActiveTokenAsync(UserId userId, TokenType tokenType, CancellationToken cancellationToken = default);

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
    /// Gets tokens expiring within the specified timeframe
    /// </summary>
    /// <param name="timeSpan">The timespan to check</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of tokens expiring soon</returns>
    Task<IEnumerable<RegistrationToken>> GetTokensExpiringWithinAsync(TimeSpan timeSpan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tokens by type
    /// </summary>
    /// <param name="tokenType">The token type</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of tokens of the specified type</returns>
    Task<IEnumerable<RegistrationToken>> GetByTypeAsync(TokenType tokenType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tokens created within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of tokens created within the date range</returns>
    Task<IEnumerable<RegistrationToken>> GetTokensCreatedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates and gets a token for use
    /// </summary>
    /// <param name="tokenId">The token identifier</param>
    /// <param name="userId">The user identifier to validate against</param>
    /// <param name="tokenType">The token type to validate against</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The token if valid for use, null otherwise</returns>
    Task<RegistrationToken?> GetValidTokenForUseAsync(TokenId tokenId, UserId userId, TokenType tokenType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleanup expired and used tokens older than the specified date
    /// </summary>
    /// <param name="olderThan">The cutoff date</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of tokens cleaned up</returns>
    Task<int> CleanupOldTokensAsync(DateTime olderThan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets token statistics
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Token statistics</returns>
    Task<TokenStatistics> GetTokenStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Record representing token statistics
/// </summary>
internal sealed record TokenStatistics(
    int TotalTokens,
    int ActiveTokens,
    int ExpiredTokens,
    int UsedTokens,
    int EmailVerificationTokens,
    int PasswordResetTokens,
    DateTime? OldestActiveToken,
    DateTime? NewestToken
); 