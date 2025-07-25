using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.DomainEvents;

namespace AuthService.Domain.DomainEvents;

/// <summary>
/// Domain event raised when an email verification token is created
/// </summary>
/// <param name="TokenId">The token identifier</param>
/// <param name="UserId">The user identifier</param>
/// <param name="Expiration">When the token expires</param>
public sealed record EmailVerificationTokenCreatedDomainEvent(
    TokenId TokenId,
    UserId UserId,
    DateTime Expiration) : DomainEventBase;

/// <summary>
/// Domain event raised when a password reset token is created
/// </summary>
/// <param name="TokenId">The token identifier</param>
/// <param name="UserId">The user identifier</param>
/// <param name="Expiration">When the token expires</param>
public sealed record PasswordResetTokenCreatedDomainEvent(
    TokenId TokenId,
    UserId UserId,
    DateTime Expiration) : DomainEventBase;

/// <summary>
/// Domain event raised when an email verification token is used
/// </summary>
/// <param name="TokenId">The token identifier</param>
/// <param name="UserId">The user identifier</param>
public sealed record EmailVerificationTokenUsedDomainEvent(
    TokenId TokenId,
    UserId UserId) : DomainEventBase;

/// <summary>
/// Domain event raised when a password reset token is used
/// </summary>
/// <param name="TokenId">The token identifier</param>
/// <param name="UserId">The user identifier</param>
public sealed record PasswordResetTokenUsedDomainEvent(
    TokenId TokenId,
    UserId UserId) : DomainEventBase; 