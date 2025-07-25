using BuildingBlocks.Application.CQRS.Events;
using AuthService.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Events;

/// <summary>
/// Handler for email verification token created domain event
/// </summary>
public class EmailVerificationTokenCreatedDomainEventHandler : IEventHandler<EmailVerificationTokenCreatedDomainEvent>
{
    private readonly ILogger<EmailVerificationTokenCreatedDomainEventHandler> _logger;

    public EmailVerificationTokenCreatedDomainEventHandler(ILogger<EmailVerificationTokenCreatedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(EmailVerificationTokenCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Email verification token created for user {UserId} with token {TokenId}, expires at {Expiration}",
            domainEvent.UserId, domainEvent.TokenId, domainEvent.Expiration);

        // Here you could:
        // - Send verification email to user
        // - Log analytics event
        // - Set up reminder notifications
        // - etc.

        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler for password reset token created domain event
/// </summary>
public class PasswordResetTokenCreatedDomainEventHandler : IEventHandler<PasswordResetTokenCreatedDomainEvent>
{
    private readonly ILogger<PasswordResetTokenCreatedDomainEventHandler> _logger;

    public PasswordResetTokenCreatedDomainEventHandler(ILogger<PasswordResetTokenCreatedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(PasswordResetTokenCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Password reset token created for user {UserId} with token {TokenId}, expires at {Expiration}",
            domainEvent.UserId, domainEvent.TokenId, domainEvent.Expiration);

        // Here you could:
        // - Send password reset email to user
        // - Log security event
        // - Set up token expiration cleanup
        // - etc.

        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler for email verification token used domain event
/// </summary>
public class EmailVerificationTokenUsedDomainEventHandler : IEventHandler<EmailVerificationTokenUsedDomainEvent>
{
    private readonly ILogger<EmailVerificationTokenUsedDomainEventHandler> _logger;

    public EmailVerificationTokenUsedDomainEventHandler(ILogger<EmailVerificationTokenUsedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(EmailVerificationTokenUsedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Email verification token {TokenId} used successfully for user {UserId}",
            domainEvent.TokenId, domainEvent.UserId);

        // Here you could:
        // - Send welcome email to user
        // - Update user profile status in other services
        // - Grant initial permissions or bonuses
        // - etc.

        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler for password reset token used domain event
/// </summary>
public class PasswordResetTokenUsedDomainEventHandler : IEventHandler<PasswordResetTokenUsedDomainEvent>
{
    private readonly ILogger<PasswordResetTokenUsedDomainEventHandler> _logger;

    public PasswordResetTokenUsedDomainEventHandler(ILogger<PasswordResetTokenUsedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(PasswordResetTokenUsedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Password reset token {TokenId} used successfully for user {UserId}",
            domainEvent.TokenId, domainEvent.UserId);

        // Here you could:
        // - Send password changed confirmation email
        // - Log security event
        // - Invalidate all user sessions
        // - etc.

        await Task.CompletedTask;
    }
} 