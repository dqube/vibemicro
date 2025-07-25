using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Responses.Base;

/// <summary>
/// Base API response without data
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Gets or sets whether the operation was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the response message
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the error details
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Gets or sets the timestamp
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the trace identifier
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    /// Initializes a new instance of the ApiResponse class
    /// </summary>
    public ApiResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ApiResponse class
    /// </summary>
    /// <param name="success">Whether the operation was successful</param>
    /// <param name="message">The response message</param>
    /// <param name="errors">The error details</param>
    public ApiResponse(bool success, string? message = null, Dictionary<string, string[]>? errors = null)
    {
        Success = success;
        Message = message;
        Errors = errors;
    }

    /// <summary>
    /// Creates a successful response
    /// </summary>
    /// <param name="message">The success message</param>
    /// <returns>The API response</returns>
    public static ApiResponse Success(string? message = null)
    {
        return new ApiResponse(true, message);
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="errors">The validation errors</param>
    /// <returns>The API response</returns>
    public static ApiResponse Error(string message, Dictionary<string, string[]>? errors = null)
    {
        return new ApiResponse(false, message, errors);
    }

    /// <summary>
    /// Creates a not found response
    /// </summary>
    /// <param name="message">The not found message</param>
    /// <returns>The API response</returns>
    public static ApiResponse NotFound(string message = "Resource not found")
    {
        return new ApiResponse(false, message);
    }

    /// <summary>
    /// Creates a validation error response
    /// </summary>
    /// <param name="errors">The validation errors</param>
    /// <returns>The API response</returns>
    public static ApiResponse ValidationError(Dictionary<string, string[]> errors)
    {
        return new ApiResponse(false, "Validation failed", errors);
    }
}

/// <summary>
/// API response with data
/// </summary>
/// <typeparam name="T">The data type</typeparam>
public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// Gets or sets the response data
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    /// <summary>
    /// Initializes a new instance of the ApiResponse class
    /// </summary>
    public ApiResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ApiResponse class
    /// </summary>
    /// <param name="success">Whether the operation was successful</param>
    /// <param name="data">The response data</param>
    /// <param name="message">The response message</param>
    /// <param name="errors">The error details</param>
    public ApiResponse(bool success, T? data = default, string? message = null, Dictionary<string, string[]>? errors = null)
        : base(success, message, errors)
    {
        Data = data;
    }

    /// <summary>
    /// Creates a successful response with data
    /// </summary>
    /// <param name="data">The response data</param>
    /// <param name="message">The success message</param>
    /// <returns>The API response</returns>
    public static ApiResponse<T> Success(T data, string? message = null)
    {
        return new ApiResponse<T>(true, data, message);
    }

    /// <summary>
    /// Creates an error response with data type
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="errors">The validation errors</param>
    /// <returns>The API response</returns>
    public static new ApiResponse<T> Error(string message, Dictionary<string, string[]>? errors = null)
    {
        return new ApiResponse<T>(false, default, message, errors);
    }

    /// <summary>
    /// Creates a not found response with data type
    /// </summary>
    /// <param name="message">The not found message</param>
    /// <returns>The API response</returns>
    public static new ApiResponse<T> NotFound(string message = "Resource not found")
    {
        return new ApiResponse<T>(false, default, message);
    }

    /// <summary>
    /// Creates a validation error response with data type
    /// </summary>
    /// <param name="errors">The validation errors</param>
    /// <returns>The API response</returns>
    public static new ApiResponse<T> ValidationError(Dictionary<string, string[]> errors)
    {
        return new ApiResponse<T>(false, default, "Validation failed", errors);
    }
}

/// <summary>
/// Paged API response
/// </summary>
/// <typeparam name="T">The data type</typeparam>
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
    public PagedResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the PagedResponse class
    /// </summary>
    /// <param name="data">The response data</param>
    /// <param name="pagination">The pagination metadata</param>
    /// <param name="message">The response message</param>
    public PagedResponse(IEnumerable<T> data, PaginationMetadata pagination, string? message = null)
        : base(true, data, message)
    {
        Pagination = pagination;
    }

    /// <summary>
    /// Creates a successful paged response
    /// </summary>
    /// <param name="data">The response data</param>
    /// <param name="pageNumber">The current page number</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="totalCount">The total number of items</param>
    /// <param name="message">The success message</param>
    /// <returns>The paged API response</returns>
    public static PagedResponse<T> Success(
        IEnumerable<T> data, 
        int pageNumber, 
        int pageSize, 
        int totalCount, 
        string? message = null)
    {
        var pagination = new PaginationMetadata
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < (int)Math.Ceiling((double)totalCount / pageSize)
        };

        return new PagedResponse<T>(data, pagination, message);
    }
}

/// <summary>
/// Pagination metadata
/// </summary>
public class PaginationMetadata
{
    /// <summary>
    /// Gets or sets the current page number
    /// </summary>
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of items
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages
    /// </summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets or sets whether there is a previous page
    /// </summary>
    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Gets or sets whether there is a next page
    /// </summary>
    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage { get; set; }
} 