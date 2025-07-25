using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.DomainEvents;

namespace AuthService.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a user is created
/// </summary>
public sealed record UserCreatedDomainEvent(
    UserId UserId,
    Username Username,
    Email Email
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        UserId = UserId.Value,
        Username = Username.Value,
        Email = Email.Value,
        Timestamp = OccurredOn
    };
}

/// <summary>
/// Domain event raised when a user's email is changed
/// </summary>
public sealed record UserEmailChangedDomainEvent(
    UserId UserId,
    Email OldEmail,
    Email NewEmail
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        UserId = UserId.Value,
        OldEmail = OldEmail.Value,
        NewEmail = NewEmail.Value,
        Timestamp = OccurredOn
    };
}

/// <summary>
/// Domain event raised when a user's password is changed
/// </summary>
public sealed record UserPasswordChangedDomainEvent(
    UserId UserId
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        UserId = UserId.Value,
        Timestamp = OccurredOn
    };
}

/// <summary>
/// Domain event raised when a user account is activated
/// </summary>
public sealed record UserActivatedDomainEvent(
    UserId UserId
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        UserId = UserId.Value,
        Timestamp = OccurredOn
    };
}

/// <summary>
/// Domain event raised when a user account is deactivated
/// </summary>
public sealed record UserDeactivatedDomainEvent(
    UserId UserId
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        UserId = UserId.Value,
        Timestamp = OccurredOn
    };
}

/// <summary>
/// Domain event raised when a user account is locked out
/// </summary>
public sealed record UserLockedOutDomainEvent(
    UserId UserId,
    DateTime LockoutEnd
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        UserId = UserId.Value,
        LockoutEnd,
        Timestamp = OccurredOn
    };
}

/// <summary>
/// Domain event raised when a user account is unlocked
/// </summary>
public sealed record UserUnlockedDomainEvent(
    UserId UserId
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        UserId = UserId.Value,
        Timestamp = OccurredOn
    };
}

/// <summary>
/// Domain event raised when a role is assigned to a user
/// </summary>
public sealed record UserRoleAssignedDomainEvent(
    UserId UserId,
    RoleId RoleId
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        UserId = UserId.Value,
        RoleId = RoleId.Value,
        Timestamp = OccurredOn
    };
}

/// <summary>
/// Domain event raised when a role is removed from a user
/// </summary>
public sealed record UserRoleRemovedDomainEvent(
    UserId UserId,
    RoleId RoleId
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        UserId = UserId.Value,
        RoleId = RoleId.Value,
        Timestamp = OccurredOn
    };
} 