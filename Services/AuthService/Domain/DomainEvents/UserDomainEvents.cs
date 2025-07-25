using BuildingBlocks.Domain.DomainEvents;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace AuthService.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a user is created
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="Username">The username</param>
/// <param name="Email">The email address</param>
public sealed record UserCreatedDomainEvent(
    UserId UserId,
    Username Username,
    Email Email) : DomainEventBase;

/// <summary>
/// Domain event raised when a user's email is changed
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="OldEmail">The old email address</param>
/// <param name="NewEmail">The new email address</param>
public sealed record UserEmailChangedDomainEvent(
    UserId UserId,
    Email OldEmail,
    Email NewEmail) : DomainEventBase;

/// <summary>
/// Domain event raised when a user's password is changed
/// </summary>
/// <param name="UserId">The user identifier</param>
public sealed record UserPasswordChangedDomainEvent(
    UserId UserId) : DomainEventBase;

/// <summary>
/// Domain event raised when a user's password is reset by an admin
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="ResetBy">The user who reset the password</param>
public sealed record UserPasswordResetDomainEvent(
    UserId UserId,
    UserId ResetBy) : DomainEventBase;

/// <summary>
/// Domain event raised when a user account is activated
/// </summary>
/// <param name="UserId">The user identifier</param>
public sealed record UserActivatedDomainEvent(
    UserId UserId) : DomainEventBase;

/// <summary>
/// Domain event raised when a user account is deactivated
/// </summary>
/// <param name="UserId">The user identifier</param>
public sealed record UserDeactivatedDomainEvent(
    UserId UserId) : DomainEventBase;

/// <summary>
/// Domain event raised when a role is added to a user
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="RoleId">The role identifier</param>
public sealed record UserRoleAddedDomainEvent(
    UserId UserId,
    RoleId RoleId) : DomainEventBase;

/// <summary>
/// Domain event raised when a role is removed from a user
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="RoleId">The role identifier</param>
public sealed record UserRoleRemovedDomainEvent(
    UserId UserId,
    RoleId RoleId) : DomainEventBase;

/// <summary>
/// Domain event raised when a user account is locked out
/// </summary>
/// <param name="UserId">The user identifier</param>
/// <param name="FailedAttempts">The number of failed attempts</param>
/// <param name="LockoutEnd">When the lockout ends</param>
public sealed record UserLockedOutDomainEvent(
    UserId UserId,
    int FailedAttempts,
    DateTime LockoutEnd) : DomainEventBase;

/// <summary>
/// Domain event raised when a user account is unlocked
/// </summary>
/// <param name="UserId">The user identifier</param>
public sealed record UserUnlockedDomainEvent(
    UserId UserId) : DomainEventBase; 