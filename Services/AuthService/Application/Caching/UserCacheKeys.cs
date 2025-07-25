using BuildingBlocks.Application.Caching;

namespace AuthService.Application.Caching;

/// <summary>
/// Cache keys for user-related operations
/// </summary>
public static class UserCacheKeys
{
    private const string UserPrefix = "user";
    private const string RolePrefix = "role";

    /// <summary>
    /// Gets cache key for user by ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>The cache key</returns>
    public static string GetUserById(Guid userId) => $"{UserPrefix}:id:{userId}";

    /// <summary>
    /// Gets cache key for user by username
    /// </summary>
    /// <param name="username">The username</param>
    /// <returns>The cache key</returns>
    public static string GetUserByUsername(string username) => $"{UserPrefix}:username:{username.ToLowerInvariant()}";

    /// <summary>
    /// Gets cache key for user by email
    /// </summary>
    /// <param name="email">The email</param>
    /// <returns>The cache key</returns>
    public static string GetUserByEmail(string email) => $"{UserPrefix}:email:{email.ToLowerInvariant()}";

    /// <summary>
    /// Gets cache key for all roles
    /// </summary>
    /// <returns>The cache key</returns>
    public static string GetAllRoles() => $"{RolePrefix}:all";

    /// <summary>
    /// Gets cache key for role by ID
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <returns>The cache key</returns>
    public static string GetRoleById(int roleId) => $"{RolePrefix}:id:{roleId}";

    /// <summary>
    /// Gets cache key pattern for user-related cache invalidation
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>The cache key pattern</returns>
    public static string GetUserPattern(Guid userId) => $"{UserPrefix}:*:{userId}*";
} 