using BuildingBlocks.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;
using AuthService.Domain.DomainEvents;

namespace AuthService.Domain.Entities;

/// <summary>
/// User entity representing an authenticated user in the system
/// </summary>
public class User : GuidAggregateRoot<UserId>, IAuditableEntity
{
    private readonly List<RoleId> _roleIds = new();

    /// <summary>
    /// Gets the username
    /// </summary>
    public Username Username { get; private set; }

    /// <summary>
    /// Gets the email address
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Gets the password hash
    /// </summary>
    public PasswordHash PasswordHash { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the user is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the number of failed login attempts
    /// </summary>
    public int FailedLoginAttempts { get; private set; }

    /// <summary>
    /// Gets the lockout end time (null if not locked out)
    /// </summary>
    public DateTime? LockoutEnd { get; private set; }

    /// <summary>
    /// Gets the role IDs assigned to this user
    /// </summary>
    public IReadOnlyList<RoleId> RoleIds => _roleIds.AsReadOnly();

    /// <summary>
    /// Gets the creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the user who created this entity
    /// </summary>
    public UserId? CreatedBy { get; private set; }

    /// <summary>
    /// Gets the last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the user who last updated this entity
    /// </summary>
    public UserId? UpdatedBy { get; private set; }

    /// <summary>
    /// Private constructor for Entity Framework
    /// </summary>
    private User() : base(UserId.Empty)
    {
        Username = null!;
        Email = null!;
        PasswordHash = null!;
    }

    /// <summary>
    /// Initializes a new instance of the User class
    /// </summary>
    /// <param name="id">The user identifier</param>
    /// <param name="username">The username</param>
    /// <param name="email">The email address</param>
    /// <param name="passwordHash">The password hash</param>
    /// <param name="createdBy">The user who created this entity</param>
    public User(UserId id, Username username, Email email, PasswordHash passwordHash, UserId? createdBy = null)
        : base(id)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        IsActive = true;
        FailedLoginAttempts = 0;
        LockoutEnd = null;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        // Raise domain event
        AddDomainEvent(new UserCreatedDomainEvent(Id, Username, Email));
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="email">The email address</param>
    /// <param name="password">The plain text password</param>
    /// <param name="createdBy">The user who created this entity</param>
    /// <returns>A new User instance</returns>
    public static User Create(Username username, Email email, string password, UserId? createdBy = null)
    {
        var passwordHash = PasswordHash.CreateHash(password);
        var user = new User(UserId.New(), username, email, passwordHash, createdBy);
        return user;
    }

    /// <summary>
    /// Updates the user's email address
    /// </summary>
    /// <param name="email">The new email address</param>
    /// <param name="updatedBy">The user who updated this entity</param>
    public void UpdateEmail(Email email, UserId? updatedBy = null)
    {
        if (Email.Equals(email))
            return;

        var oldEmail = Email;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new UserEmailChangedDomainEvent(Id, oldEmail, Email));
    }

    /// <summary>
    /// Changes the user's password
    /// </summary>
    /// <param name="currentPassword">The current password for verification</param>
    /// <param name="newPassword">The new password</param>
    /// <param name="updatedBy">The user who updated this entity</param>
    /// <exception cref="InvalidOperationException">Thrown when the current password is incorrect</exception>
    public void ChangePassword(string currentPassword, string newPassword, UserId? updatedBy = null)
    {
        if (!PasswordHash.VerifyPassword(currentPassword))
            throw new InvalidOperationException("Current password is incorrect.");

        PasswordHash = PasswordHash.CreateHash(newPassword);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        // Reset failed login attempts when password is changed
        ResetFailedLoginAttempts(updatedBy);

        AddDomainEvent(new UserPasswordChangedDomainEvent(Id));
    }

    /// <summary>
    /// Resets the user's password (admin operation)
    /// </summary>
    /// <param name="newPassword">The new password</param>
    /// <param name="updatedBy">The user who updated this entity</param>
    public void ResetPassword(string newPassword, UserId updatedBy)
    {
        PasswordHash = PasswordHash.CreateHash(newPassword);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        // Reset failed login attempts and unlock account
        ResetFailedLoginAttempts(updatedBy);
        Unlock(updatedBy);

        AddDomainEvent(new UserPasswordResetDomainEvent(Id, updatedBy));
    }

    /// <summary>
    /// Activates the user account
    /// </summary>
    /// <param name="updatedBy">The user who updated this entity</param>
    public void Activate(UserId? updatedBy = null)
    {
        if (IsActive)
            return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new UserActivatedDomainEvent(Id));
    }

    /// <summary>
    /// Deactivates the user account
    /// </summary>
    /// <param name="updatedBy">The user who updated this entity</param>
    public void Deactivate(UserId? updatedBy = null)
    {
        if (!IsActive)
            return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new UserDeactivatedDomainEvent(Id));
    }

    /// <summary>
    /// Adds a role to the user
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <param name="updatedBy">The user who updated this entity</param>
    public void AddRole(RoleId roleId, UserId? updatedBy = null)
    {
        if (_roleIds.Contains(roleId))
            return;

        _roleIds.Add(roleId);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new UserRoleAddedDomainEvent(Id, roleId));
    }

    /// <summary>
    /// Removes a role from the user
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <param name="updatedBy">The user who updated this entity</param>
    public void RemoveRole(RoleId roleId, UserId? updatedBy = null)
    {
        if (!_roleIds.Remove(roleId))
            return;

        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new UserRoleRemovedDomainEvent(Id, roleId));
    }

    /// <summary>
    /// Records a failed login attempt
    /// </summary>
    /// <param name="maxAttempts">Maximum allowed failed attempts before lockout</param>
    /// <param name="lockoutDuration">Duration of lockout</param>
    public void RecordFailedLoginAttempt(int maxAttempts = 5, TimeSpan? lockoutDuration = null)
    {
        FailedLoginAttempts++;
        UpdatedAt = DateTime.UtcNow;

        if (FailedLoginAttempts >= maxAttempts)
        {
            var duration = lockoutDuration ?? TimeSpan.FromMinutes(30);
            LockoutEnd = DateTime.UtcNow.Add(duration);
            
            AddDomainEvent(new UserLockedOutDomainEvent(Id, FailedLoginAttempts, LockoutEnd.Value));
        }
    }

    /// <summary>
    /// Resets failed login attempts
    /// </summary>
    /// <param name="updatedBy">The user who updated this entity</param>
    public void ResetFailedLoginAttempts(UserId? updatedBy = null)
    {
        if (FailedLoginAttempts == 0)
            return;

        FailedLoginAttempts = 0;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Unlocks the user account
    /// </summary>
    /// <param name="updatedBy">The user who updated this entity</param>
    public void Unlock(UserId? updatedBy = null)
    {
        if (LockoutEnd == null)
            return;

        LockoutEnd = null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new UserUnlockedDomainEvent(Id));
    }

    /// <summary>
    /// Checks if the user is currently locked out
    /// </summary>
    /// <returns>True if the user is locked out</returns>
    public bool IsLockedOut()
    {
        return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Verifies the user's password
    /// </summary>
    /// <param name="password">The password to verify</param>
    /// <returns>True if the password is correct</returns>
    public bool VerifyPassword(string password)
    {
        return PasswordHash.VerifyPassword(password);
    }

    /// <summary>
    /// Checks if the user has a specific role
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <returns>True if the user has the role</returns>
    public bool HasRole(RoleId roleId)
    {
        return _roleIds.Contains(roleId);
    }

    /// <summary>
    /// Checks if the user has any of the specified roles
    /// </summary>
    /// <param name="roleIds">The role identifiers</param>
    /// <returns>True if the user has any of the roles</returns>
    public bool HasAnyRole(params RoleId[] roleIds)
    {
        return roleIds.Any(roleId => _roleIds.Contains(roleId));
    }

    /// <summary>
    /// Checks if the user has all of the specified roles
    /// </summary>
    /// <param name="roleIds">The role identifiers</param>
    /// <returns>True if the user has all of the roles</returns>
    public bool HasAllRoles(params RoleId[] roleIds)
    {
        return roleIds.All(roleId => _roleIds.Contains(roleId));
    }
} 