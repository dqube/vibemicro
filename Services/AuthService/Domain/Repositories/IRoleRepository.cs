using BuildingBlocks.Domain.Repository;
using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;

namespace AuthService.Domain.Repositories;

/// <summary>
/// Repository interface for Role entities
/// </summary>
public interface IRoleRepository : IReadOnlyRepository<Role, RoleId>
{
    /// <summary>
    /// Finds a role by name
    /// </summary>
    /// <param name="name">The role name to search for</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The role if found, null otherwise</returns>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of all roles</returns>
    Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple roles by their IDs
    /// </summary>
    /// <param name="roleIds">The role identifiers</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Collection of found roles</returns>
    Task<IEnumerable<Role>> GetRolesByIdsAsync(IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role exists
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the role exists</returns>
    Task<bool> ExistsAsync(RoleId roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if multiple roles exist
    /// </summary>
    /// <param name="roleIds">The role identifiers</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Dictionary mapping role IDs to their existence status</returns>
    Task<Dictionary<RoleId, bool>> ExistAsync(IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default);
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