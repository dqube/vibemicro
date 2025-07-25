using BuildingBlocks.Application.CQRS.Events;

namespace AuthService.Application.Events;

/// <summary>
/// Integration event raised when a user is registered
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="Username">The username</param>
/// <param name="Email">The email address</param>
/// <param name="FullName">The user's full name</param>
/// <param name="RegisteredAt">When the user was registered</param>
public sealed record UserRegisteredIntegrationEvent(
    Guid UserId,
    string Username,
    string Email,
    string? FullName,
    DateTime RegisteredAt) : IntegrationEventBase;

/// <summary>
/// Integration event raised when a user's email is verified
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="Email">The verified email address</param>
/// <param name="VerifiedAt">When the email was verified</param>
public sealed record UserEmailVerifiedIntegrationEvent(
    Guid UserId,
    string Email,
    DateTime VerifiedAt) : IntegrationEventBase;

/// <summary>
/// Integration event raised when a user's profile is updated
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="Username">The username</param>
/// <param name="Email">The email address</param>
/// <param name="IsActive">Whether the user is active</param>
/// <param name="UpdatedAt">When the profile was updated</param>
public sealed record UserProfileUpdatedIntegrationEvent(
    Guid UserId,
    string Username,
    string Email,
    bool IsActive,
    DateTime UpdatedAt) : IntegrationEventBase;

/// <summary>
/// Integration event raised when a user is deactivated
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="Username">The username</param>
/// <param name="DeactivatedAt">When the user was deactivated</param>
/// <param name="Reason">The reason for deactivation</param>
public sealed record UserDeactivatedIntegrationEvent(
    Guid UserId,
    string Username,
    DateTime DeactivatedAt,
    string? Reason) : IntegrationEventBase;

/// <summary>
/// Integration event raised when a user's role changes
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="Username">The username</param>
/// <param name="RoleId">The role identifier</param>
/// <param name="RoleName">The role name</param>
/// <param name="Action">The action (Added or Removed)</param>
/// <param name="ChangedAt">When the role was changed</param>
public sealed record UserRoleChangedIntegrationEvent(
    Guid UserId,
    string Username,
    int RoleId,
    string RoleName,
    string Action,
    DateTime ChangedAt) : IntegrationEventBase;

/// <summary>
/// Integration event raised when a user account is locked
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="Username">The username</param>
/// <param name="LockoutEnd">When the lockout ends</param>
/// <param name="Reason">The reason for lockout</param>
/// <param name="LockedAt">When the account was locked</param>
public sealed record UserAccountLockedIntegrationEvent(
    Guid UserId,
    string Username,
    DateTime LockoutEnd,
    string Reason,
    DateTime LockedAt) : IntegrationEventBase;