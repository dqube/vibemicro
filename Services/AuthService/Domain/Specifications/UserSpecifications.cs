using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;

namespace AuthService.Domain.Specifications;

/// <summary>
/// Specification to find users by username
/// </summary>
public sealed class UserByUsernameSpecification : Specification<User>
{
    private readonly Username _username;

    public UserByUsernameSpecification(Username username)
    {
        _username = username ?? throw new ArgumentNullException(nameof(username));
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Username.Equals(_username);
    }
}

/// <summary>
/// Specification to find users by email
/// </summary>
public sealed class UserByEmailSpecification : Specification<User>
{
    private readonly Email _email;

    public UserByEmailSpecification(Email email)
    {
        _email = email ?? throw new ArgumentNullException(nameof(email));
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Email.Equals(_email);
    }
}

/// <summary>
/// Specification to find active users
/// </summary>
public sealed class ActiveUsersSpecification : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.IsActive;
    }
}

/// <summary>
/// Specification to find inactive users
/// </summary>
public sealed class InactiveUsersSpecification : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => !user.IsActive;
    }
}

/// <summary>
/// Specification to find locked out users
/// </summary>
public sealed class LockedOutUsersSpecification : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        return user => user.LockoutEnd.HasValue && user.LockoutEnd.Value > now;
    }
}

/// <summary>
/// Specification to find users with failed login attempts
/// </summary>
public sealed class UsersWithFailedAttemptsSpecification : Specification<User>
{
    private readonly int _minFailedAttempts;

    public UsersWithFailedAttemptsSpecification(int minFailedAttempts = 1)
    {
        _minFailedAttempts = minFailedAttempts;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.FailedLoginAttempts >= _minFailedAttempts;
    }
}

/// <summary>
/// Specification to find users by role
/// </summary>
public sealed class UsersByRoleSpecification : Specification<User>
{
    private readonly RoleId _roleId;

    public UsersByRoleSpecification(RoleId roleId)
    {
        _roleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Roles.Any(role => role.Id.Equals(_roleId));
    }
}

/// <summary>
/// Specification to find users created within a date range
/// </summary>
public sealed class UsersCreatedBetweenSpecification : Specification<User>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public UsersCreatedBetweenSpecification(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be before end date", nameof(startDate));

        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.CreatedAt >= _startDate && user.CreatedAt <= _endDate;
    }
}

/// <summary>
/// Specification to find users with usernames matching a pattern
/// </summary>
public sealed class UsernameContainsSpecification : Specification<User>
{
    private readonly string _pattern;

    public UsernameContainsSpecification(string pattern)
    {
        _pattern = !string.IsNullOrWhiteSpace(pattern) 
            ? pattern.ToLowerInvariant() 
            : throw new ArgumentException("Pattern cannot be null or empty", nameof(pattern));
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Username.Value.Contains(_pattern);
    }
}

/// <summary>
/// Specification to find users with emails from a specific domain
/// </summary>
public sealed class UsersByEmailDomainSpecification : Specification<User>
{
    private readonly string _domain;

    public UsersByEmailDomainSpecification(string domain)
    {
        _domain = !string.IsNullOrWhiteSpace(domain) 
            ? domain.ToLowerInvariant() 
            : throw new ArgumentException("Domain cannot be null or empty", nameof(domain));
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Email.Domain.ToLower() == _domain;
    }
} 