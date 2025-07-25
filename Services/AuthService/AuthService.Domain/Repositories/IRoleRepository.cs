using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using BuildingBlocks.Domain.Repository;

namespace AuthService.Domain.Repositories;

/// <summary>
/// Repository interface for Role aggregate
/// </summary>
internal interface IRoleRepository : IRepository<Role, RoleId, int>
{
    /// <summary>
    /// Finds a role by name
    /// </summary>
    /// <param name="name">The role name to search for</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The role if found, null otherwise</returns>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role name is already taken
    /// </summary>
    /// <param name="name">The role name to check</param>
    /// <param name="excludeRoleId">Optional role ID to exclude from the check</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the name is available, false if taken</returns>
    Task<bool> IsNameAvailableAsync(string name, RoleId? excludeRoleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles that have users assigned
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of roles with users</returns>
    Task<IEnumerable<Role>> GetRolesWithUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles that have no users assigned
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of empty roles</returns>
    Task<IEnumerable<Role>> GetEmptyRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles by user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of roles assigned to the user</returns>
    Task<IEnumerable<Role>> GetRolesByUserAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role statistics
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Role statistics</returns>
    Task<RoleStatistics> GetRoleStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Record representing role statistics
/// </summary>
internal sealed record RoleStatistics(
    int TotalRoles,
    int RolesWithUsers,
    int EmptyRoles,
    string MostPopularRole,
    int MaxUsersInRole
); 