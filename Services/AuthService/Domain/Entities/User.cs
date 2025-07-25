using AuthService.Domain.DomainEvents;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Extensions;

namespace AuthService.Domain.Entities;

/// <summary>
/// Represents a user in the authentication system
/// </summary>
public sealed class User : GuidAggregateRoot<UserId>, IAuditableEntity
{
    private readonly List<Role> _roles = new();
    private readonly List<RegistrationToken> _registrationTokens = new();

    /// <summary>
    /// Gets the username
    /// </summary>
    public Username Username { get; private set; } = null!;

    /// <summary>
    /// Gets the email address
    /// </summary>
    public Email Email { get; private set; } = null!;

    /// <summary>
    /// Gets the password hash
    /// </summary>
    public PasswordHash PasswordHash { get; private set; } = null!;

    /// <summary>
    /// Gets whether the user account is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the number of failed login attempts
    /// </summary>
    public int FailedLoginAttempts { get; private set; }

    /// <summary>
    /// Gets when the account lockout ends
    /// </summary>
    public DateTime? LockoutEnd { get; private set; }

    /// <summary>
    /// Gets when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets who created the user
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets when the user was last modified
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// Gets who last modified the user
    /// </summary>
    public string? LastModifiedBy { get; set; }

    /// <summary>
    /// Gets the roles assigned to this user
    /// </summary>
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    /// <summary>
    /// Gets the registration tokens for this user
    /// </summary>
    public IReadOnlyCollection<RegistrationToken> RegistrationTokens => _registrationTokens.AsReadOnly();

    /// <summary>
    /// Private constructor for ORM
    /// </summary>
    private User() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the User class
    /// </summary>
    /// <param name="id">The user identifier</param>
    /// <param name="username">The username</param>
    /// <param name="email">The email address</param>
    /// <param name="passwordHash">The password hash</param>
    /// <param name="createdBy">Who created the user</param>
    public User(UserId id, Username username, Email email, PasswordHash passwordHash, string createdBy)
        : base(id)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        FailedLoginAttempts = 0;

        AddDomainEvent(new UserCreatedDomainEvent(Id, Username, Email));
    }

    /// <summary>
    /// Factory method to create a new user
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="email">The email address</param>
    /// <param name="passwordHash">The password hash</param>
    /// <param name="createdBy">Who created the user</param>
    /// <returns>A new user instance</returns>
    public static User Create(Username username, Email email, PasswordHash passwordHash, string createdBy)
    {
        var userId = UserId.New();
        return new User(userId, username, email, passwordHash, createdBy);
    }

    /// <summary>
    /// Updates the user's email address
    /// </summary>
    /// <param name="email">The new email address</param>
    /// <param name="modifiedBy">Who modified the user</param>
    public void UpdateEmail(Email email, string modifiedBy)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));

        if (!Email.Equals(email))
        {
            var oldEmail = Email;
            Email = email;
            LastModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
            LastModifiedAt = DateTime.UtcNow;
            MarkAsModified();

            AddDomainEvent(new UserEmailChangedDomainEvent(Id, oldEmail, Email));
        }
    }

    /// <summary>
    /// Updates the user's password
    /// </summary>
    /// <param name="passwordHash">The new password hash</param>
    /// <param name="modifiedBy">Who modified the user</param>
    public void UpdatePassword(PasswordHash passwordHash, string modifiedBy)
    {
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        LastModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
        LastModifiedAt = DateTime.UtcNow;
        FailedLoginAttempts = 0; // Reset failed attempts when password is changed
        LockoutEnd = null; // Clear any lockout when password is changed
        MarkAsModified();

        AddDomainEvent(new UserPasswordChangedDomainEvent(Id));
    }

    /// <summary>
    /// Activates the user account
    /// </summary>
    /// <param name="modifiedBy">Who activated the user</param>
    public void Activate(string modifiedBy)
    {
        if (!IsActive)
        {
            IsActive = true;
            LastModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
            LastModifiedAt = DateTime.UtcNow;
            MarkAsModified();

            AddDomainEvent(new UserActivatedDomainEvent(Id));
        }
    }

    /// <summary>
    /// Deactivates the user account
    /// </summary>
    /// <param name="modifiedBy">Who deactivated the user</param>
    public void Deactivate(string modifiedBy)
    {
        if (IsActive)
        {
            IsActive = false;
            LastModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
            LastModifiedAt = DateTime.UtcNow;
            MarkAsModified();

            AddDomainEvent(new UserDeactivatedDomainEvent(Id));
        }
    }

    /// <summary>
    /// Records a failed login attempt
    /// </summary>
    /// <param name="maxAttempts">Maximum allowed failed attempts before lockout</param>
    /// <param name="lockoutDuration">Duration of lockout</param>
    public void RecordFailedLoginAttempt(int maxAttempts = 5, TimeSpan? lockoutDuration = null)
    {
        FailedLoginAttempts++;
        MarkAsModified();

        if (FailedLoginAttempts >= maxAttempts)
        {
            var duration = lockoutDuration ?? TimeSpan.FromMinutes(30);
            LockoutEnd = DateTime.UtcNow.Add(duration);
            AddDomainEvent(new UserLockedOutDomainEvent(Id, LockoutEnd.Value));
        }
    }

    /// <summary>
    /// Records a successful login
    /// </summary>
    public void RecordSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
        MarkAsModified();
    }

    /// <summary>
    /// Checks if the user account is currently locked out
    /// </summary>
    /// <returns>True if the account is locked out</returns>
    public bool IsLockedOut()
    {
        return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Unlocks the user account
    /// </summary>
    /// <param name="modifiedBy">Who unlocked the user</param>
    public void Unlock(string modifiedBy)
    {
        if (IsLockedOut())
        {
            FailedLoginAttempts = 0;
            LockoutEnd = null;
            LastModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
            LastModifiedAt = DateTime.UtcNow;
            MarkAsModified();

            AddDomainEvent(new UserUnlockedDomainEvent(Id));
        }
    }

    /// <summary>
    /// Adds a role to the user
    /// </summary>
    /// <param name="role">The role to add</param>
    /// <param name="modifiedBy">Who added the role</param>
    public void AddRole(Role role, string modifiedBy)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));

        if (!_roles.Contains(role))
        {
            _roles.Add(role);
            role.AddUser(this);
            LastModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
            LastModifiedAt = DateTime.UtcNow;
            MarkAsModified();

            AddDomainEvent(new UserRoleAssignedDomainEvent(Id, role.Id));
        }
    }

    /// <summary>
    /// Removes a role from the user
    /// </summary>
    /// <param name="role">The role to remove</param>
    /// <param name="modifiedBy">Who removed the role</param>
    public void RemoveRole(Role role, string modifiedBy)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));

        if (_roles.Remove(role))
        {
            role.RemoveUser(this);
            LastModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
            LastModifiedAt = DateTime.UtcNow;
            MarkAsModified();

            AddDomainEvent(new UserRoleRemovedDomainEvent(Id, role.Id));
        }
    }

    /// <summary>
    /// Checks if the user has the specified role
    /// </summary>
    /// <param name="role">The role to check</param>
    /// <returns>True if the user has the role</returns>
    public bool HasRole(Role role)
    {
        if (role == null)
            return false;

        return _roles.Contains(role);
    }

    /// <summary>
    /// Checks if the user has any of the specified roles
    /// </summary>
    /// <param name="roles">The roles to check</param>
    /// <returns>True if the user has any of the roles</returns>
    public bool HasAnyRole(params Role[] roles)
    {
        if (roles == null || roles.Length == 0)
            return false;

        return roles.Any(role => _roles.Contains(role));
    }

    /// <summary>
    /// Adds a registration token to the user
    /// </summary>
    /// <param name="token">The token to add</param>
    internal void AddRegistrationToken(RegistrationToken token)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        if (!_registrationTokens.Contains(token))
        {
            _registrationTokens.Add(token);
            MarkAsModified();
        }
    }

    /// <summary>
    /// Gets active registration tokens of the specified type
    /// </summary>
    /// <param name="tokenType">The token type</param>
    /// <returns>Active tokens of the specified type</returns>
    public IEnumerable<RegistrationToken> GetActiveTokens(TokenType tokenType)
    {
        return _registrationTokens
            .Where(t => t.TokenType.Equals(tokenType) && !t.IsExpired() && !t.IsUsed)
            .OrderByDescending(t => t.CreatedAt);
    }

    /// <summary>
    /// Returns the string representation of the user
    /// </summary>
    public override string ToString()
    {
        return $"User: {Username} ({Email}) - Active: {IsActive}";
    }
} 