using BuildingBlocks.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;

namespace AuthService.Domain.Entities;

/// <summary>
/// Role entity representing a user role in the system
/// </summary>
public class Role : IntEntity<RoleId>, IAuditableEntity
{
    public const int MaxNameLength = 20;
    public const int MaxDescriptionLength = 255;

    /// <summary>
    /// Gets the role name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the role description
    /// </summary>
    public string? Description { get; private set; }

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
    private Role() : base(RoleId.Zero)
    {
        Name = null!;
    }

    /// <summary>
    /// Initializes a new instance of the Role class
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <param name="name">The role name</param>
    /// <param name="description">The role description</param>
    /// <param name="createdBy">The user who created this entity</param>
    public Role(RoleId id, string name, string? description = null, UserId? createdBy = null)
        : base(id)
    {
        Name = ValidateName(name);
        Description = ValidateDescription(description);
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Creates a new role
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <param name="name">The role name</param>
    /// <param name="description">The role description</param>
    /// <param name="createdBy">The user who created this entity</param>
    /// <returns>A new Role instance</returns>
    public static Role Create(RoleId id, string name, string? description = null, UserId? createdBy = null)
    {
        return new Role(id, name, description, createdBy);
    }

    /// <summary>
    /// Updates the role description
    /// </summary>
    /// <param name="description">The new description</param>
    /// <param name="updatedBy">The user who updated this entity</param>
    public void UpdateDescription(string? description, UserId? updatedBy = null)
    {
        if (Description == description)
            return;

        Description = ValidateDescription(description);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Validates the role name
    /// </summary>
    /// <param name="name">The role name to validate</param>
    /// <returns>The validated role name</returns>
    /// <exception cref="ArgumentException">Thrown when the name is invalid</exception>
    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be null, empty, or whitespace.", nameof(name));

        var trimmed = name.Trim();

        if (trimmed.Length > MaxNameLength)
            throw new ArgumentException($"Role name cannot exceed {MaxNameLength} characters.", nameof(name));

        return trimmed;
    }

    /// <summary>
    /// Validates the role description
    /// </summary>
    /// <param name="description">The role description to validate</param>
    /// <returns>The validated role description</returns>
    /// <exception cref="ArgumentException">Thrown when the description is invalid</exception>
    private static string? ValidateDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return null;

        var trimmed = description.Trim();

        if (trimmed.Length > MaxDescriptionLength)
            throw new ArgumentException($"Role description cannot exceed {MaxDescriptionLength} characters.", nameof(description));

        return trimmed;
    }

    // Predefined roles based on database seeding
    public static class WellKnownRoles
    {
        public static readonly Role Cashier = new(RoleId.Cashier, "Cashier", "Can process sales and returns");
        public static readonly Role Supervisor = new(RoleId.Supervisor, "Supervisor", "Can override transactions and manage registers");
        public static readonly Role Manager = new(RoleId.Manager, "Manager", "Full store operations access");
        public static readonly Role Admin = new(RoleId.Admin, "Admin", "System administration access");
        public static readonly Role Inventory = new(RoleId.Inventory, "Inventory", "Inventory management access");
        public static readonly Role Reporting = new(RoleId.Reporting, "Reporting", "Reporting and analytics access");
    }
} 