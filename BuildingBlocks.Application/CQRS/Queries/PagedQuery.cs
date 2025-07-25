namespace BuildingBlocks.Application.CQRS.Queries;

/// <summary>
/// Base class for paged queries
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query</typeparam>
public abstract class PagedQuery<TResult> : QueryBase<TResult>
{
    /// <summary>
    /// Gets or sets the page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Validates the paging parameters
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when paging parameters are invalid</exception>
    protected virtual void ValidatePaging()
    {
        if (PageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0", nameof(PageNumber));
        
        if (PageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(PageSize));
        
        if (PageSize > 1000)
            throw new ArgumentException("Page size cannot exceed 1000", nameof(PageSize));
    }

    /// <summary>
    /// Gets the number of items to skip
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Gets the number of items to take
    /// </summary>
    public int Take => PageSize;
} 