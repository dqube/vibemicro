using BuildingBlocks.Domain.Repository;
using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Repositories;

/// <summary>
/// Repository interface for User entities
/// </summary>
public interface IUserRepository : IGuidRepository<User, UserId>
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
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a username is unique
    /// </summary>
    /// <param name="username">The username to check</param>
    /// <param name="excludeUserId">User ID to exclude from the check (for updates)</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the username is unique</returns>
    Task<bool> IsUsernameUniqueAsync(Username username, UserId? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an email is unique
    /// </summary>
    /// <param name="email">The email address to check</param>
    /// <param name="excludeUserId">User ID to exclude from the check (for updates)</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the email is unique</returns>
    Task<bool> IsEmailUniqueAsync(string email, UserId? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active users
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of active users</returns>
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users with a specific role
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of users with the specified role</returns>
    Task<IEnumerable<User>> GetUsersByRoleAsync(RoleId roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all locked out users
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of locked out users</returns>
    Task<IEnumerable<User>> GetLockedOutUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users with excessive failed login attempts
    /// </summary>
    /// <param name="threshold">The failed attempts threshold</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of users with excessive failed attempts</returns>
    Task<IEnumerable<User>> GetUsersWithExcessiveFailedAttemptsAsync(int threshold = 3, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches users by username or email
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of matching users</returns>
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users created within a date range
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of users created within the date range</returns>
    Task<IEnumerable<User>> GetUsersCreatedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
} 