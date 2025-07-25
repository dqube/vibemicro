using BuildingBlocks.Domain.BusinessRules;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.StronglyTypedIds;

namespace AuthService.Domain.BusinessRules;

/// <summary>
/// Business rule ensuring username uniqueness
/// </summary>
public sealed class UsernameMustBeUniqueRule : IBusinessRule
{
    private readonly Username _username;
    private readonly Func<Username, Task<bool>> _isUsernameUnique;

    public UsernameMustBeUniqueRule(Username username, Func<Username, Task<bool>> isUsernameUnique)
    {
        _username = username;
        _isUsernameUnique = isUsernameUnique;
    }

    public string Message => $"Username '{_username}' is already taken.";

    public async Task<bool> IsBrokenAsync()
    {
        return !await _isUsernameUnique(_username);
    }
}

/// <summary>
/// Business rule ensuring email uniqueness
/// </summary>
public sealed class EmailMustBeUniqueRule : IBusinessRule
{
    private readonly string _email;
    private readonly UserId? _excludeUserId;
    private readonly Func<string, UserId?, Task<bool>> _isEmailUnique;

    public EmailMustBeUniqueRule(string email, Func<string, UserId?, Task<bool>> isEmailUnique, UserId? excludeUserId = null)
    {
        _email = email;
        _isEmailUnique = isEmailUnique;
        _excludeUserId = excludeUserId;
    }

    public string Message => $"Email '{_email}' is already in use.";

    public async Task<bool> IsBrokenAsync()
    {
        return !await _isEmailUnique(_email, _excludeUserId);
    }
}

/// <summary>
/// Business rule ensuring password strength requirements
/// </summary>
public sealed class PasswordMustMeetStrengthRequirementsRule : BusinessRuleBase
{
    private readonly string _password;

    public PasswordMustMeetStrengthRequirementsRule(string password)
    {
        _password = password;
    }

    public override string Message => "Password does not meet strength requirements.";

    public override bool IsBroken()
    {
        if (string.IsNullOrWhiteSpace(_password))
            return true;

        // Minimum length requirement
        if (_password.Length < 8)
            return true;

        // Must contain at least one uppercase letter
        if (!_password.Any(char.IsUpper))
            return true;

        // Must contain at least one lowercase letter
        if (!_password.Any(char.IsLower))
            return true;

        // Must contain at least one digit
        if (!_password.Any(char.IsDigit))
            return true;

        // Must contain at least one special character
        var specialCharacters = "!@#$%^&*(),.?\":{}|<>";
        if (!_password.Any(c => specialCharacters.Contains(c)))
            return true;

        return false;
    }
}

/// <summary>
/// Business rule ensuring user can only be assigned valid roles
/// </summary>
public sealed class UserCanOnlyHaveValidRolesRule : BusinessRuleBase
{
    private readonly RoleId _roleId;
    private readonly Func<RoleId, bool> _isValidRole;

    public UserCanOnlyHaveValidRolesRule(RoleId roleId, Func<RoleId, bool> isValidRole)
    {
        _roleId = roleId;
        _isValidRole = isValidRole;
    }

    public override string Message => $"Role with ID '{_roleId}' is not valid or does not exist.";

    public override bool IsBroken()
    {
        return !_isValidRole(_roleId);
    }
}

/// <summary>
/// Business rule ensuring user cannot be locked out indefinitely without proper authority
/// </summary>
public sealed class UserLockoutMustHaveReasonableTimeoutRule : BusinessRuleBase
{
    private readonly TimeSpan _lockoutDuration;
    private readonly TimeSpan _maxAllowedDuration;

    public UserLockoutMustHaveReasonableTimeoutRule(TimeSpan lockoutDuration, TimeSpan? maxAllowedDuration = null)
    {
        _lockoutDuration = lockoutDuration;
        _maxAllowedDuration = maxAllowedDuration ?? TimeSpan.FromDays(30); // Default max 30 days
    }

    public override string Message => $"Lockout duration of {_lockoutDuration} exceeds maximum allowed duration of {_maxAllowedDuration}.";

    public override bool IsBroken()
    {
        return _lockoutDuration > _maxAllowedDuration;
    }
}

/// <summary>
/// Business rule ensuring token expiration is reasonable
/// </summary>
public sealed class TokenExpirationMustBeReasonableRule : BusinessRuleBase
{
    private readonly DateTime _expiration;
    private readonly TokenType _tokenType;
    private readonly TimeSpan _maxAllowedDuration;

    public TokenExpirationMustBeReasonableRule(DateTime expiration, TokenType tokenType)
    {
        _expiration = expiration;
        _tokenType = tokenType;
        
        // Set different max durations based on token type
        _maxAllowedDuration = tokenType.IsEmailVerification 
            ? TimeSpan.FromDays(7)  // Email verification: max 7 days
            : TimeSpan.FromHours(24); // Password reset: max 24 hours
    }

    public override string Message => $"Token expiration time exceeds maximum allowed duration for {_tokenType} tokens.";

    public override bool IsBroken()
    {
        var duration = _expiration - DateTime.UtcNow;
        return duration > _maxAllowedDuration;
    }
}

/// <summary>
/// Business rule ensuring active user cannot be assigned to critical operations
/// </summary>
public sealed class OnlyActiveUsersCanPerformCriticalOperationsRule : BusinessRuleBase
{
    private readonly bool _isUserActive;
    private readonly bool _isUserLockedOut;

    public OnlyActiveUsersCanPerformCriticalOperationsRule(bool isUserActive, bool isUserLockedOut)
    {
        _isUserActive = isUserActive;
        _isUserLockedOut = isUserLockedOut;
    }

    public override string Message => "Only active, non-locked users can perform critical operations.";

    public override bool IsBroken()
    {
        return !_isUserActive || _isUserLockedOut;
    }
} 