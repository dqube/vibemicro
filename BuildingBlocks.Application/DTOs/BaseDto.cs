namespace BuildingBlocks.Application.DTOs;

/// <summary>
/// Base class for Data Transfer Objects
/// </summary>
public abstract class BaseDto
{
    /// <summary>
    /// Returns the string representation of the DTO
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name}";
    }
}

/// <summary>
/// Base class for DTOs with an identifier
/// </summary>
/// <typeparam name="TId">The identifier type</typeparam>
public abstract class BaseDto<TId> : BaseDto
{
    /// <summary>
    /// Gets or sets the identifier
    /// </summary>
    public TId Id { get; set; } = default!;

    /// <summary>
    /// Returns the string representation of the DTO
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} [Id: {Id}]";
    }
}

/// <summary>
/// Base class for auditable DTOs
/// </summary>
/// <typeparam name="TId">The identifier type</typeparam>
public abstract class AuditableDto<TId> : BaseDto<TId>
{
    /// <summary>
    /// Gets or sets when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets who created the entity
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets when the entity was last modified
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets who last modified the entity
    /// </summary>
    public string? ModifiedBy { get; set; }
}

/// <summary>
/// Base class for paged DTOs
/// </summary>
/// <typeparam name="T">The item type</typeparam>
public class PagedDto<T>
{
    /// <summary>
    /// Gets or sets the items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Gets or sets the current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of items
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets the total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Gets the number of items in the current page
    /// </summary>
    public int Count => Items.Count();
} 