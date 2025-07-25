using AuthService.Domain.StronglyTypedIds;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Extensions;

namespace AuthService.Domain.Entities;

/// <summary>
/// Represents a user role in the authentication system
/// </summary>
public sealed class Role : IntAggregateRoot<RoleId>, IAuditableEntity
{
    private readonly List<User> _users = new();

    /// <summary>
    /// Gets the role name
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the role description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets when the role was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets who created the role
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets when the role was last modified
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// Gets who last modified the role
    /// </summary>
    public string? LastModifiedBy { get; set; }

    /// <summary>
    /// Gets the users assigned to this role
    /// </summary>
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    /// <summary>
    /// Private constructor for ORM
    /// </summary>
    private Role() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Role class
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <param name="name">The role name</param>
    /// <param name="description">The role description</param>
    /// <param name="createdBy">Who created the role</param>
    public Role(RoleId id, string name, string? description, string createdBy)
        : base()
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        SetName(name);
        Description = description?.Trim();
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new role
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <param name="name">The role name</param>
    /// <param name="description">The role description</param>
    /// <param name="createdBy">Who created the role</param>
    /// <returns>A new role instance</returns>
    public static Role Create(RoleId id, string name, string? description, string createdBy)
    {
        return new Role(id, name, description, createdBy);
    }

    /// <summary>
    /// Updates the role information
    /// </summary>
    /// <param name="name">The new role name</param>
    /// <param name="description">The new role description</param>
    /// <param name="modifiedBy">Who modified the role</param>
    public void Update(string name, string? description, string modifiedBy)
    {
        SetName(name);
        Description = description?.Trim();
        LastModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
        LastModifiedAt = DateTime.UtcNow;
        MarkAsModified();
    }

    /// <summary>
    /// Adds a user to this role
    /// </summary>
    /// <param name="user">The user to add</param>
    internal void AddUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (!_users.Contains(user))
        {
            _users.Add(user);
            MarkAsModified();
        }
    }

    /// <summary>
    /// Removes a user from this role
    /// </summary>
    /// <param name="user">The user to remove</param>
    internal void RemoveUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (_users.Remove(user))
        {
            MarkAsModified();
        }
    }

    /// <summary>
    /// Checks if the role has any users assigned
    /// </summary>
    /// <returns>True if the role has users</returns>
    public bool HasUsers() => _users.Count > 0;

    /// <summary>
    /// Checks if the specified user has this role
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>True if the user has this role</returns>
    public bool HasUser(User user)
    {
        if (user == null)
            return false;

        return _users.Contains(user);
    }

    /// <summary>
    /// Sets the role name with validation
    /// </summary>
    /// <param name="name">The role name</param>
    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be null or empty", nameof(name));

        var trimmed = name.Trim();
        if (trimmed.Length > 20)
            throw new ArgumentException("Role name cannot exceed 20 characters", nameof(name));

        Name = trimmed;
    }

    /// <summary>
    /// Returns the string representation of the role
    /// </summary>
    public override string ToString()
    {
        return $"Role: {Name} (Id: {Id})";
    }
} 