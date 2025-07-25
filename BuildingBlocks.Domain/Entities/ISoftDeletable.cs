namespace BuildingBlocks.Domain.Entities;

/// <summary>
/// Interface for entities that support soft deletion
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets or sets whether the entity is deleted
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets when the entity was deleted
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets who deleted the entity
    /// </summary>
    string? DeletedBy { get; set; }

    /// <summary>
    /// Marks the entity as deleted
    /// </summary>
    /// <param name="deletedBy">Who is deleting the entity</param>
    void Delete(string deletedBy);

    /// <summary>
    /// Restores a soft-deleted entity
    /// </summary>
    void Restore();
} 