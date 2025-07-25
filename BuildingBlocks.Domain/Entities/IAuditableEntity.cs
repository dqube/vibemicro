namespace BuildingBlocks.Domain.Entities;

/// <summary>
/// Interface for entities that support audit tracking
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Gets or sets when the entity was created
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets who created the entity
    /// </summary>
    string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets when the entity was last modified
    /// </summary>
    DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets who last modified the entity
    /// </summary>
    string? LastModifiedBy { get; set; }
} 