using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.DomainEvents;

namespace AuthService.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a registration token is created
/// </summary>
public sealed record RegistrationTokenCreatedDomainEvent(
    TokenId TokenId,
    UserId UserId,
    TokenType TokenType
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        TokenId = TokenId.Value,
        UserId = UserId.Value,
        TokenType = TokenType.Name,
        Timestamp = OccurredOn
    };
}

/// <summary>
/// Domain event raised when a registration token is used
/// </summary>
public sealed record RegistrationTokenUsedDomainEvent(
    TokenId TokenId,
    UserId UserId,
    TokenType TokenType
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        TokenId = TokenId.Value,
        UserId = UserId.Value,
        TokenType = TokenType.Name,
        Timestamp = OccurredOn
    };
}

/// <summary>
/// Domain event raised when a registration token expiration is extended
/// </summary>
public sealed record RegistrationTokenExtendedDomainEvent(
    TokenId TokenId,
    UserId UserId,
    DateTime NewExpiration
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        TokenId = TokenId.Value,
        UserId = UserId.Value,
        NewExpiration,
        Timestamp = OccurredOn
    };
}

/// <summary>
/// Domain event raised when a registration token expires
/// </summary>
public sealed record RegistrationTokenExpiredDomainEvent(
    TokenId TokenId,
    UserId UserId,
    TokenType TokenType
) : DomainEventBase
{
    /// <summary>
    /// Gets additional event data
    /// </summary>
    public object GetEventData() => new
    {
        TokenId = TokenId.Value,
        UserId = UserId.Value,
        TokenType = TokenType.Name,
        Timestamp = OccurredOn
    };
} 