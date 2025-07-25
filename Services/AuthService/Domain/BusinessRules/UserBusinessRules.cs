using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.BusinessRules;
using BuildingBlocks.Domain.Common;

namespace AuthService.Domain.BusinessRules;

/// <summary>
/// Business rule to ensure username is unique
/// </summary>
public sealed class UsernameUniqueRule : BusinessRuleBase
{
    private readonly Username _username;
    private readonly Func<Username, Task<bool>> _checkUniqueness;

    public override string Message => $"Username '{_username}' is already taken";

    public UsernameUniqueRule(Username username, Func<Username, Task<bool>> checkUniqueness)
    {
        _username = username ?? throw new ArgumentNullException(nameof(username));
        _checkUniqueness = checkUniqueness ?? throw new ArgumentNullException(nameof(checkUniqueness));
    }

    public override bool IsBroken()
    {
        // This is a simplified synchronous check - in real implementation,
        // you'd use a domain service for async repository calls
        return !_checkUniqueness(_username).GetAwaiter().GetResult();
    }
}

/// <summary>
/// Business rule to ensure email is unique
/// </summary>
public sealed class EmailUniqueRule : BusinessRuleBase
{
    private readonly Email _email;
    private readonly Func<Email, Task<bool>> _checkUniqueness;

    public override string Message => $"Email '{_email}' is already registered";

    public EmailUniqueRule(Email email, Func<Email, Task<bool>> checkUniqueness)
    {
        _email = email ?? throw new ArgumentNullException(nameof(email));
        _checkUniqueness = checkUniqueness ?? throw new ArgumentNullException(nameof(checkUniqueness));
    }

    public override bool IsBroken()
    {
        return !_checkUniqueness(_email).GetAwaiter().GetResult();
    }
}

/// <summary>
/// Business rule to ensure user account is active
/// </summary>
public sealed class UserMustBeActiveRule : BusinessRuleBase
{
    private readonly User _user;

    public override string Message => "User account is not active";

    public UserMustBeActiveRule(User user)
    {
        _user = user ?? throw new ArgumentNullException(nameof(user));
    }

    public override bool IsBroken()
    {
        return !_user.IsActive;
    }
}

/// <summary>
/// Business rule to ensure user account is not locked out
/// </summary>
public sealed class UserMustNotBeLockedOutRule : BusinessRuleBase
{
    private readonly User _user;

    public override string Message => $"User account is locked out until {_user.LockoutEnd:yyyy-MM-dd HH:mm:ss}";

    public UserMustNotBeLockedOutRule(User user)
    {
        _user = user ?? throw new ArgumentNullException(nameof(user));
    }

    public override bool IsBroken()
    {
        return _user.IsLockedOut();
    }
}

/// <summary>
/// Business rule to ensure password meets complexity requirements
/// </summary>
public sealed class PasswordComplexityRule : BusinessRuleBase
{
    private readonly string _password;
    private readonly int _minLength;
    private readonly bool _requireUppercase;
    private readonly bool _requireLowercase;
    private readonly bool _requireDigit;
    private readonly bool _requireSpecialChar;

    public override string Message => "Password does not meet complexity requirements";

    public PasswordComplexityRule(
        string password,
        int minLength = 8,
        bool requireUppercase = true,
        bool requireLowercase = true,
        bool requireDigit = true,
        bool requireSpecialChar = true)
    {
        _password = password ?? throw new ArgumentNullException(nameof(password));
        _minLength = minLength;
        _requireUppercase = requireUppercase;
        _requireLowercase = requireLowercase;
        _requireDigit = requireDigit;
        _requireSpecialChar = requireSpecialChar;
    }

    public override bool IsBroken()
    {
        if (_password.Length < _minLength)
            return true;

        if (_requireUppercase && !_password.Any(char.IsUpper))
            return true;

        if (_requireLowercase && !_password.Any(char.IsLower))
            return true;

        if (_requireDigit && !_password.Any(char.IsDigit))
            return true;

        if (_requireSpecialChar && !_password.Any(c => !char.IsLetterOrDigit(c)))
            return true;

        return false;
    }
}

/// <summary>
/// Business rule to ensure token is valid for the operation
/// </summary>
public sealed class TokenValidForOperationRule : BusinessRuleBase
{
    private readonly RegistrationToken _token;
    private readonly TokenType _expectedType;

    public override string Message => "Token is not valid for this operation";

    public TokenValidForOperationRule(RegistrationToken token, TokenType expectedType)
    {
        _token = token ?? throw new ArgumentNullException(nameof(token));
        _expectedType = expectedType ?? throw new ArgumentNullException(nameof(expectedType));
    }

    public override bool IsBroken()
    {
        return !_token.IsValid() || !_token.TokenType.Equals(_expectedType);
    }
}

/// <summary>
/// Business rule to ensure user can have the specified role assigned
/// </summary>
public sealed class UserCanHaveRoleRule : BusinessRuleBase
{
    private readonly User _user;
    private readonly Role _role;

    public override string Message => $"User cannot be assigned role '{_role.Name}'";

    public UserCanHaveRoleRule(User user, Role role)
    {
        _user = user ?? throw new ArgumentNullException(nameof(user));
        _role = role ?? throw new ArgumentNullException(nameof(role));
    }

    public override bool IsBroken()
    {
        // Example business logic - you might have specific rules about role assignment
        // For now, just check if user already has the role
        return _user.HasRole(_role);
    }
}

/// <summary>
/// Business rule to ensure maximum number of failed login attempts
/// </summary>
public sealed class MaxFailedLoginAttemptsRule : BusinessRuleBase
{
    private readonly User _user;
    private readonly int _maxAttempts;

    public override string Message => $"Maximum number of failed login attempts ({_maxAttempts}) exceeded";

    public MaxFailedLoginAttemptsRule(User user, int maxAttempts = 5)
    {
        _user = user ?? throw new ArgumentNullException(nameof(user));
        _maxAttempts = maxAttempts;
    }

    public override bool IsBroken()
    {
        return _user.FailedLoginAttempts >= _maxAttempts;
    }
} 