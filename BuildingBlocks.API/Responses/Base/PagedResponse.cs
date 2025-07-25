using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Responses.Base;

/// <summary>
/// Represents a paged API response
/// </summary>
/// <typeparam name="T">The type of items in the response</typeparam>
public class PagedResponse<T> : ApiResponse<IEnumerable<T>>
{
    /// <summary>
    /// Gets or sets the pagination metadata
    /// </summary>
    [JsonPropertyName("pagination")]
    public PaginationMetadata Pagination { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the PagedResponse class
    /// </summary>
    public PagedResponse() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the PagedResponse class
    /// </summary>
    /// <param name="items">The items for this page</param>
    /// <param name="pageNumber">The current page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="totalCount">The total number of items</param>
    /// <param name="message">Optional success message</param>
    public PagedResponse(
        IEnumerable<T> items,
        int pageNumber,
        int pageSize,
        long totalCount,
        string? message = null) : base(items, message)
    {
        Pagination = new PaginationMetadata
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    /// <summary>
    /// Creates a successful paged response
    /// </summary>
    /// <param name="items">The items for this page</param>
    /// <param name="pageNumber">The current page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="totalCount">The total number of items</param>
    /// <param name="message">Optional success message</param>
    /// <returns>A successful paged response</returns>
    public static PagedResponse<T> Success(
        IEnumerable<T> items,
        int pageNumber,
        int pageSize,
        long totalCount,
        string? message = null)
    {
        return new PagedResponse<T>(items, pageNumber, pageSize, totalCount, message);
    }

    /// <summary>
    /// Creates an empty paged response
    /// </summary>
    /// <param name="pageNumber">The current page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="message">Optional message</param>
    /// <returns>An empty paged response</returns>
    public static PagedResponse<T> Empty(int pageNumber = 1, int pageSize = 10, string? message = null)
    {
        return new PagedResponse<T>(Enumerable.Empty<T>(), pageNumber, pageSize, 0, message ?? "No items found");
    }

    /// <summary>
    /// Creates a failed paged response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="errors">Optional error details</param>
    /// <returns>A failed paged response</returns>
    public static new PagedResponse<T> Failure(string message, object? errors = null)
    {
        return new PagedResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors,
            Data = Enumerable.Empty<T>(),
            Pagination = new PaginationMetadata()
        };
    }

    /// <summary>
    /// Creates a paged response from a page result
    /// </summary>
    /// <param name="pageResult">The page result</param>
    /// <param name="message">Optional success message</param>
    /// <returns>A paged response</returns>
    public static PagedResponse<T> FromPageResult<TSource>(
        PageResult<TSource> pageResult,
        Func<TSource, T> mapper,
        string? message = null)
    {
        var mappedItems = pageResult.Items.Select(mapper);
        return Success(mappedItems, pageResult.PageNumber, pageResult.PageSize, pageResult.TotalCount, message);
    }

    /// <summary>
    /// Creates a paged response from items with the same type
    /// </summary>
    /// <param name="pageResult">The page result</param>
    /// <param name="message">Optional success message</param>
    /// <returns>A paged response</returns>
    public static PagedResponse<T> FromPageResult(PageResult<T> pageResult, string? message = null)
    {
        return Success(pageResult.Items, pageResult.PageNumber, pageResult.PageSize, pageResult.TotalCount, message);
    }
}

/// <summary>
/// Represents pagination metadata
/// </summary>
public class PaginationMetadata
{
    /// <summary>
    /// Gets or sets the current page number (1-based)
    /// </summary>
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the total number of items across all pages
    /// </summary>
    [JsonPropertyName("totalCount")]
    public long TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages
    /// </summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets whether there is a previous page
    /// </summary>
    [JsonPropertyName("hasPrevious")]
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// Gets whether there is a next page
    /// </summary>
    [JsonPropertyName("hasNext")]
    public bool HasNext => PageNumber < TotalPages;

    /// <summary>
    /// Gets the first item number on the current page
    /// </summary>
    [JsonPropertyName("firstItemOnPage")]
    public long FirstItemOnPage => TotalCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1;

    /// <summary>
    /// Gets the last item number on the current page
    /// </summary>
    [JsonPropertyName("lastItemOnPage")]
    public long LastItemOnPage => Math.Min(PageNumber * PageSize, TotalCount);

    /// <summary>
    /// Gets whether this is the first page
    /// </summary>
    [JsonPropertyName("isFirstPage")]
    public bool IsFirstPage => PageNumber == 1;

    /// <summary>
    /// Gets whether this is the last page
    /// </summary>
    [JsonPropertyName("isLastPage")]
    public bool IsLastPage => PageNumber == TotalPages;
}

/// <summary>
/// Represents a page result used for creating paged responses
/// </summary>
/// <typeparam name="T">The type of items in the page</typeparam>
public class PageResult<T>
{
    /// <summary>
    /// Gets or sets the items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Gets or sets the current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the total number of items
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// Gets the total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Gets whether there is a previous page
    /// </summary>
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// Gets whether there is a next page
    /// </summary>
    public bool HasNext => PageNumber < TotalPages;
} 