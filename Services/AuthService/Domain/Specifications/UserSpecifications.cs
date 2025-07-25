using BuildingBlocks.Domain.Specifications;
using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using System.Linq.Expressions;

namespace AuthService.Domain.Specifications;

/// <summary>
/// Specification for finding users by username
/// </summary>
public sealed class UserByUsernameSpecification : Specification<User>
{
    private readonly Username _username;

    public UserByUsernameSpecification(Username username)
    {
        _username = username;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Username == _username;
    }
}

/// <summary>
/// Specification for finding users by email
/// </summary>
public sealed class UserByEmailSpecification : Specification<User>
{
    private readonly string _email;

    public UserByEmailSpecification(string email)
    {
        _email = email.ToLowerInvariant();
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.Email.Value.ToLower() == _email;
    }
}

/// <summary>
/// Specification for finding active users
/// </summary>
public sealed class ActiveUsersSpecification : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.IsActive;
    }
}

/// <summary>
/// Specification for finding inactive users
/// </summary>
public sealed class InactiveUsersSpecification : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => !user.IsActive;
    }
}

/// <summary>
/// Specification for finding locked out users
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
/// Specification for finding users with excessive failed login attempts
/// </summary>
public sealed class UsersWithExcessiveFailedAttemptsSpecification : Specification<User>
{
    private readonly int _threshold;

    public UsersWithExcessiveFailedAttemptsSpecification(int threshold = 3)
    {
        _threshold = threshold;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.FailedLoginAttempts >= _threshold;
    }
}

/// <summary>
/// Specification for finding users by role
/// </summary>
public sealed class UsersByRoleSpecification : Specification<User>
{
    private readonly RoleId _roleId;

    public UsersByRoleSpecification(RoleId roleId)
    {
        _roleId = roleId;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.RoleIds.Contains(_roleId);
    }
}

/// <summary>
/// Specification for finding users created within a date range
/// </summary>
public sealed class UsersCreatedBetweenSpecification : Specification<User>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public UsersCreatedBetweenSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.CreatedAt >= _startDate && user.CreatedAt <= _endDate;
    }
}

/// <summary>
/// Specification for finding users eligible for password reset (active and not locked out)
/// </summary>
public sealed class UsersEligibleForPasswordResetSpecification : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        return user => user.IsActive && (!user.LockoutEnd.HasValue || user.LockoutEnd.Value <= now);
    }
}

/// <summary>
/// Specification for searching users by username or email
/// </summary>
public sealed class UserSearchSpecification : Specification<User>
{
    private readonly string _searchTerm;

    public UserSearchSpecification(string searchTerm)
    {
        _searchTerm = searchTerm.ToLowerInvariant();
    }

    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => 
            user.Username.Value.ToLower().Contains(_searchTerm) ||
            user.Email.Value.ToLower().Contains(_searchTerm);
    }
} 