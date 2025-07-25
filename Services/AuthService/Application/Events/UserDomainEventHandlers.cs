using BuildingBlocks.Application.CQRS.Events;
using AuthService.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Events;

/// <summary>
/// Handler for user created domain event
/// </summary>
public class UserCreatedDomainEventHandler : IEventHandler<UserCreatedDomainEvent>
{
    private readonly ILogger<UserCreatedDomainEventHandler> _logger;

    public UserCreatedDomainEventHandler(ILogger<UserCreatedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User created: {UserId} with username {Username} and email {Email}",
            domainEvent.UserId, domainEvent.Username.Value, domainEvent.Email.Value);

        // Here you could:
        // - Send welcome email
        // - Create user profile in other services
        // - Log analytics event
        // - etc.

        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler for user email changed domain event
/// </summary>
public class UserEmailChangedDomainEventHandler : IEventHandler<UserEmailChangedDomainEvent>
{
    private readonly ILogger<UserEmailChangedDomainEventHandler> _logger;

    public UserEmailChangedDomainEventHandler(ILogger<UserEmailChangedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserEmailChangedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User {UserId} changed email from {OldEmail} to {NewEmail}",
            domainEvent.UserId, domainEvent.OldEmail.Value, domainEvent.NewEmail.Value);

        // Here you could:
        // - Send email change notification
        // - Update email in other services
        // - Invalidate related caches
        // - etc.

        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler for user locked out domain event
/// </summary>
public class UserLockedOutDomainEventHandler : IEventHandler<UserLockedOutDomainEvent>
{
    private readonly ILogger<UserLockedOutDomainEventHandler> _logger;

    public UserLockedOutDomainEventHandler(ILogger<UserLockedOutDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserLockedOutDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("User {UserId} has been locked out after {FailedAttempts} failed attempts until {LockoutEnd}",
            domainEvent.UserId, domainEvent.FailedAttempts, domainEvent.LockoutEnd);

        // Here you could:
        // - Send security alert email
        // - Log security event
        // - Notify administrators
        // - etc.

        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler for user password changed domain event
/// </summary>
public class UserPasswordChangedDomainEventHandler : IEventHandler<UserPasswordChangedDomainEvent>
{
    private readonly ILogger<UserPasswordChangedDomainEventHandler> _logger;

    public UserPasswordChangedDomainEventHandler(ILogger<UserPasswordChangedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserPasswordChangedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User {UserId} changed their password", domainEvent.UserId);

        // Here you could:
        // - Send password changed notification
        // - Invalidate all user sessions
        // - Log security event
        // - etc.

        await Task.CompletedTask;
    }
} 