using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.Outbox;
using AuthService.Domain.DomainEvents;
using AuthService.Application.Events;
using AuthService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AuthService.Application.Events;

/// <summary>
/// Publishes integration events when domain events occur
/// </summary>
public class UserDomainToIntegrationEventHandler : 
    IEventHandler<UserCreatedDomainEvent>,
    IEventHandler<UserActivatedDomainEvent>,
    IEventHandler<UserDeactivatedDomainEvent>,
    IEventHandler<UserRoleAddedDomainEvent>,
    IEventHandler<UserRoleRemovedDomainEvent>,
    IEventHandler<UserLockedOutDomainEvent>
{
    private readonly IOutboxService _outboxService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<UserDomainToIntegrationEventHandler> _logger;

    public UserDomainToIntegrationEventHandler(
        IOutboxService outboxService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILogger<UserDomainToIntegrationEventHandler> logger)
    {
        _outboxService = outboxService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing UserRegisteredIntegrationEvent for user {UserId}", domainEvent.UserId);

        var integrationEvent = new UserRegisteredIntegrationEvent(
            domainEvent.UserId.Value,
            domainEvent.Username.Value,
            domainEvent.Email.Value,
            null, // FullName not available in current domain model
            DateTime.UtcNow
        );

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            MessageType = nameof(UserRegisteredIntegrationEvent),
            Content = JsonSerializer.Serialize(integrationEvent, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            }),
            CorrelationId = domainEvent.UserId.Value.ToString(),
            Metadata = new Dictionary<string, object>
            {
                ["UserId"] = domainEvent.UserId.Value,
                ["EventSource"] = "AuthService"
            }
        };

        await _outboxService.AddMessageAsync(outboxMessage, cancellationToken);
    }

    public async Task Handle(UserActivatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing UserEmailVerifiedIntegrationEvent for user {UserId}", domainEvent.UserId);

        var user = await _userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
        if (user == null) return;

        var integrationEvent = new UserEmailVerifiedIntegrationEvent(
            domainEvent.UserId.Value,
            user.Email.Value,
            DateTime.UtcNow
        );

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            MessageType = nameof(UserEmailVerifiedIntegrationEvent),
            Content = JsonSerializer.Serialize(integrationEvent, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            }),
            CorrelationId = domainEvent.UserId.Value.ToString(),
            Metadata = new Dictionary<string, object>
            {
                ["UserId"] = domainEvent.UserId.Value,
                ["EventSource"] = "AuthService"
            }
        };

        await _outboxService.AddMessageAsync(outboxMessage, cancellationToken);
    }

    public async Task Handle(UserDeactivatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing UserDeactivatedIntegrationEvent for user {UserId}", domainEvent.UserId);

        var user = await _userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
        if (user == null) return;

        var integrationEvent = new UserDeactivatedIntegrationEvent(
            domainEvent.UserId.Value,
            user.Username.Value,
            DateTime.UtcNow,
            "User deactivated"
        );

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            MessageType = nameof(UserDeactivatedIntegrationEvent),
            Content = JsonSerializer.Serialize(integrationEvent, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            }),
            CorrelationId = domainEvent.UserId.Value.ToString(),
            Metadata = new Dictionary<string, object>
            {
                ["UserId"] = domainEvent.UserId.Value,
                ["EventSource"] = "AuthService"
            }
        };

        await _outboxService.AddMessageAsync(outboxMessage, cancellationToken);
    }

    public async Task Handle(UserRoleAddedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing UserRoleChangedIntegrationEvent for user {UserId}, role {RoleId}", 
            domainEvent.UserId, domainEvent.RoleId);

        var user = await _userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
        var role = await _roleRepository.GetByIdAsync(domainEvent.RoleId, cancellationToken);
        
        if (user == null || role == null) return;

        var integrationEvent = new UserRoleChangedIntegrationEvent(
            domainEvent.UserId.Value,
            user.Username.Value,
            domainEvent.RoleId.Value,
            role.Name,
            "Added",
            DateTime.UtcNow
        );

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            MessageType = nameof(UserRoleChangedIntegrationEvent),
            Content = JsonSerializer.Serialize(integrationEvent, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            }),
            CorrelationId = domainEvent.UserId.Value.ToString(),
            Metadata = new Dictionary<string, object>
            {
                ["UserId"] = domainEvent.UserId.Value,
                ["RoleId"] = domainEvent.RoleId.Value,
                ["EventSource"] = "AuthService"
            }
        };

        await _outboxService.AddMessageAsync(outboxMessage, cancellationToken);
    }

    public async Task Handle(UserRoleRemovedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing UserRoleChangedIntegrationEvent for user {UserId}, role {RoleId}", 
            domainEvent.UserId, domainEvent.RoleId);

        var user = await _userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
        var role = await _roleRepository.GetByIdAsync(domainEvent.RoleId, cancellationToken);
        
        if (user == null || role == null) return;

        var integrationEvent = new UserRoleChangedIntegrationEvent(
            domainEvent.UserId.Value,
            user.Username.Value,
            domainEvent.RoleId.Value,
            role.Name,
            "Removed",
            DateTime.UtcNow
        );

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            MessageType = nameof(UserRoleChangedIntegrationEvent),
            Content = JsonSerializer.Serialize(integrationEvent, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            }),
            CorrelationId = domainEvent.UserId.Value.ToString(),
            Metadata = new Dictionary<string, object>
            {
                ["UserId"] = domainEvent.UserId.Value,
                ["RoleId"] = domainEvent.RoleId.Value,
                ["EventSource"] = "AuthService"
            }
        };

        await _outboxService.AddMessageAsync(outboxMessage, cancellationToken);
    }

    public async Task Handle(UserLockedOutDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing UserAccountLockedIntegrationEvent for user {UserId}", domainEvent.UserId);

        var user = await _userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
        if (user == null) return;

        var integrationEvent = new UserAccountLockedIntegrationEvent(
            domainEvent.UserId.Value,
            user.Username.Value,
            domainEvent.LockoutEnd,
            $"Account locked after {domainEvent.FailedAttempts} failed login attempts",
            DateTime.UtcNow
        );

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            MessageType = nameof(UserAccountLockedIntegrationEvent),
            Content = JsonSerializer.Serialize(integrationEvent, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            }),
            CorrelationId = domainEvent.UserId.Value.ToString(),
            Metadata = new Dictionary<string, object>
            {
                ["UserId"] = domainEvent.UserId.Value,
                ["EventSource"] = "AuthService"
            }
        };

        await _outboxService.AddMessageAsync(outboxMessage, cancellationToken);
    }
} 