using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;

namespace AuthService.Domain.Specifications;

/// <summary>
/// Specification to find tokens by user ID
/// </summary>
public sealed class TokensByUserIdSpecification : Specification<RegistrationToken>
{
    private readonly UserId _userId;

    public TokensByUserIdSpecification(UserId userId)
    {
        _userId = userId ?? throw new ArgumentNullException(nameof(userId));
    }

    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        return token => token.UserId.Equals(_userId);
    }
}

/// <summary>
/// Specification to find tokens by type
/// </summary>
public sealed class TokensByTypeSpecification : Specification<RegistrationToken>
{
    private readonly TokenType _tokenType;

    public TokensByTypeSpecification(TokenType tokenType)
    {
        _tokenType = tokenType ?? throw new ArgumentNullException(nameof(tokenType));
    }

    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        return token => token.TokenType.Equals(_tokenType);
    }
}

/// <summary>
/// Specification to find active (valid) tokens
/// </summary>
public sealed class ActiveTokensSpecification : Specification<RegistrationToken>
{
    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        return token => !token.IsUsed && token.Expiration > now;
    }
}

/// <summary>
/// Specification to find expired tokens
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
/// Specification to find used tokens
/// </summary>
public sealed class UsedTokensSpecification : Specification<RegistrationToken>
{
    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        return token => token.IsUsed;
    }
}

/// <summary>
/// Specification to find tokens expiring within a specific timeframe
/// </summary>
public sealed class TokensExpiringWithinSpecification : Specification<RegistrationToken>
{
    private readonly TimeSpan _timeSpan;

    public TokensExpiringWithinSpecification(TimeSpan timeSpan)
    {
        _timeSpan = timeSpan;
    }

    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        var cutoffTime = DateTime.UtcNow.Add(_timeSpan);
        return token => !token.IsUsed && token.Expiration <= cutoffTime && token.Expiration > DateTime.UtcNow;
    }
}

/// <summary>
/// Specification to find tokens created between specific dates
/// </summary>
public sealed class TokensCreatedBetweenSpecification : Specification<RegistrationToken>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public TokensCreatedBetweenSpecification(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be before end date", nameof(startDate));

        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        return token => token.CreatedAt >= _startDate && token.CreatedAt <= _endDate;
    }
}

/// <summary>
/// Specification to find active tokens for a specific user and type
/// </summary>
public sealed class ActiveTokensByUserAndTypeSpecification : Specification<RegistrationToken>
{
    private readonly UserId _userId;
    private readonly TokenType _tokenType;

    public ActiveTokensByUserAndTypeSpecification(UserId userId, TokenType tokenType)
    {
        _userId = userId ?? throw new ArgumentNullException(nameof(userId));
        _tokenType = tokenType ?? throw new ArgumentNullException(nameof(tokenType));
    }

    public override Expression<Func<RegistrationToken, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        return token => token.UserId.Equals(_userId) 
                     && token.TokenType.Equals(_tokenType)
                     && !token.IsUsed 
                     && token.Expiration > now;
    }
} 