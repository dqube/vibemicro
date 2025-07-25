using BuildingBlocks.Domain.Specifications;
using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using System.Linq.Expressions;

namespace AuthService.Domain.Specifications;

/// <summary>
/// Specification for finding tokens by user ID
/// </summary>
public sealed class TokensByUserSpecification : Specification<RegistrationToken>
{
    private readonly UserId _userId;

    public TokensByUserSpecification(UserId userId)
    {
        _userId = userId;
    }

    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        return token => token.UserId == _userId;
    }
}

/// <summary>
/// Specification for finding tokens by type
/// </summary>
public sealed class TokensByTypeSpecification : Specification<RegistrationToken>
{
    private readonly TokenType _tokenType;

    public TokensByTypeSpecification(TokenType tokenType)
    {
        _tokenType = tokenType;
    }

    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        return token => token.TokenType == _tokenType;
    }
}

/// <summary>
/// Specification for finding valid (unused and not expired) tokens
/// </summary>
public sealed class ValidTokensSpecification : Specification<RegistrationToken>
{
    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        return token => !token.IsUsed && token.Expiration > now;
    }
}

/// <summary>
/// Specification for finding expired tokens
/// </summary>
public sealed class ExpiredTokensSpecification : Specification<RegistrationToken>
{
    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        return token => token.Expiration <= now;
    }
}

/// <summary>
/// Specification for finding used tokens
/// </summary>
public sealed class UsedTokensSpecification : Specification<RegistrationToken>
{
    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        return token => token.IsUsed;
    }
}

/// <summary>
/// Specification for finding tokens created within a date range
/// </summary>
public sealed class TokensCreatedBetweenSpecification : Specification<RegistrationToken>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public TokensCreatedBetweenSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        return token => token.CreatedAt >= _startDate && token.CreatedAt <= _endDate;
    }
}

/// <summary>
/// Specification for finding valid email verification tokens for a specific user
/// </summary>
public sealed class ValidEmailVerificationTokenForUserSpecification : Specification<RegistrationToken>
{
    private readonly UserId _userId;

    public ValidEmailVerificationTokenForUserSpecification(UserId userId)
    {
        _userId = userId;
    }

    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        return token => 
            token.UserId == _userId &&
            token.TokenType == TokenType.EmailVerification &&
            !token.IsUsed &&
            token.Expiration > now;
    }
}

/// <summary>
/// Specification for finding valid password reset tokens for a specific user
/// </summary>
public sealed class ValidPasswordResetTokenForUserSpecification : Specification<RegistrationToken>
{
    private readonly UserId _userId;

    public ValidPasswordResetTokenForUserSpecification(UserId userId)
    {
        _userId = userId;
    }

    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        return token => 
            token.UserId == _userId &&
            token.TokenType == TokenType.PasswordReset &&
            !token.IsUsed &&
            token.Expiration > now;
    }
}

/// <summary>
/// Specification for finding tokens that need cleanup (old, used, or expired)
/// </summary>
public sealed class TokensForCleanupSpecification : Specification<RegistrationToken>
{
    private readonly DateTime _cutoffDate;

    public TokensForCleanupSpecification(TimeSpan? retentionPeriod = null)
    {
        var retention = retentionPeriod ?? TimeSpan.FromDays(30);
        _cutoffDate = DateTime.UtcNow.Subtract(retention);
    }

    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        return token => 
            token.IsUsed || 
            token.Expiration <= now ||
            token.CreatedAt <= _cutoffDate;
    }
} 