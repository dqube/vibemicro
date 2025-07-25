namespace BuildingBlocks.Application.CQRS.Queries;

/// <summary>
/// Represents a paged result
/// </summary>
/// <typeparam name="T">The type of items in the result</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Gets the items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();

    /// <summary>
    /// Gets the current page number (1-based)
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Gets the page size
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Gets the total number of items
    /// </summary>
    public int TotalCount { get; init; }

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

    /// <summary>
    /// Creates a new paged result
    /// </summary>
    /// <param name="items">The items in the current page</param>
    /// <param name="totalCount">The total number of items</param>
    /// <param name="pageNumber">The current page number</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>A new paged result</returns>
    public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        return new PagedResult<T>
        {
            Items = items ?? Enumerable.Empty<T>(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Creates an empty paged result
    /// </summary>
    /// <param name="pageNumber">The current page number</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>An empty paged result</returns>
    public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 10)
    {
        return new PagedResult<T>
        {
            Items = Enumerable.Empty<T>(),
            TotalCount = 0,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
} 