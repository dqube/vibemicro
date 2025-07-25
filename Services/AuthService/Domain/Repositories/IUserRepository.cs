using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Repository;

namespace AuthService.Domain.Repositories;

/// <summary>
/// Repository interface for User aggregate
/// </summary>
internal interface IUserRepository : IRepository<User, UserId, Guid>
{
    /// <summary>
    /// Finds a user by username
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a user by email address
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a username is already taken
    /// </summary>
    /// <param name="username">The username to check</param>
    /// <param name="excludeUserId">Optional user ID to exclude from the check</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the username is available, false if taken</returns>
    Task<bool> IsUsernameAvailableAsync(Username username, UserId? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an email is already registered
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <param name="excludeUserId">Optional user ID to exclude from the check</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the email is available, false if taken</returns>
    Task<bool> IsEmailAvailableAsync(Email email, UserId? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active users
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of active users</returns>
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all locked out users
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of locked out users</returns>
    Task<IEnumerable<User>> GetLockedOutUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by role
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of users with the specified role</returns>
    Task<IEnumerable<User>> GetUsersByRoleAsync(RoleId roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users with failed login attempts above the threshold
    /// </summary>
    /// <param name="minFailedAttempts">Minimum number of failed attempts</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of users with failed attempts</returns>
    Task<IEnumerable<User>> GetUsersWithFailedAttemptsAsync(int minFailedAttempts = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users created within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of users created within the date range</returns>
    Task<IEnumerable<User>> GetUsersCreatedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches users by username pattern
    /// </summary>
    /// <param name="pattern">The search pattern</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of users matching the pattern</returns>
    Task<IEnumerable<User>> SearchByUsernameAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by email domain
    /// </summary>
    /// <param name="domain">The email domain</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of users from the specified domain</returns>
    Task<IEnumerable<User>> GetUsersByEmailDomainAsync(string domain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user statistics
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>User statistics</returns>
    Task<UserStatistics> GetUserStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Record representing user statistics
/// </summary>
internal sealed record UserStatistics(
    int TotalUsers,
    int ActiveUsers,
    int InactiveUsers,
    int LockedOutUsers,
    int UsersWithFailedAttempts,
    DateTime LastUserCreated
); 