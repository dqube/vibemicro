using BuildingBlocks.Application.CQRS.Queries;

namespace AuthService.Application.Queries.User;

/// <summary>
/// Query to get a user by their ID
/// </summary>
/// <param name="UserId">The user's ID</param>
public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse?>;

/// <summary>
/// User response DTO
/// </summary>
/// <param name="UserId">The user's ID</param>
/// <param name="Username">The username</param>
/// <param name="Email">The email address</param>
/// <param name="IsActive">Whether the user is active</param>
/// <param name="Roles">The user's roles</param>
/// <param name="CreatedAt">When the user was created</param>
/// <param name="IsLockedOut">Whether the user is currently locked out</param>
/// <param name="LockoutEnd">When the lockout ends (if applicable)</param>
public sealed record UserResponse(
    Guid UserId,
    string Username,
    string Email,
    bool IsActive,
    IReadOnlyList<RoleResponse> Roles,
    DateTime CreatedAt,
    bool IsLockedOut,
    DateTime? LockoutEnd);

/// <summary>
/// Role response DTO
/// </summary>
/// <param name="RoleId">The role ID</param>
/// <param name="Name">The role name</param>
/// <param name="Description">The role description</param>
public sealed record RoleResponse(
    int RoleId,
    string Name,
    string? Description); 