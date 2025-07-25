namespace BuildingBlocks.Application.CQRS.Queries;

/// <summary>
/// Base class for queries with sorting capabilities
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query</typeparam>
public abstract class SortingQuery<TResult> : QueryBase<TResult>
{
    /// <summary>
    /// Gets or sets the field to sort by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Gets or sets the sort direction
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    /// <summary>
    /// Validates the sorting parameters
    /// </summary>
    /// <param name="allowedSortFields">The fields that are allowed for sorting</param>
    /// <exception cref="ArgumentException">Thrown when sort field is not allowed</exception>
    protected virtual void ValidateSorting(params string[] allowedSortFields)
    {
        if (!string.IsNullOrEmpty(SortBy) && allowedSortFields.Length > 0)
        {
            if (!allowedSortFields.Contains(SortBy, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Sort field '{SortBy}' is not allowed. Allowed fields: {string.Join(", ", allowedSortFields)}", nameof(SortBy));
            }
        }
    }
}

/// <summary>
/// Enumeration for sort directions
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Ascending order
    /// </summary>
    Ascending = 0,

    /// <summary>
    /// Descending order
    /// </summary>
    Descending = 1
} 