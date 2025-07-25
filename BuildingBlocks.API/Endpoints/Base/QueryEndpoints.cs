using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Responses.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.API.Endpoints.Base;

/// <summary>
/// Base class for query-only endpoints providing read operations
/// </summary>
/// <typeparam name="TResult">The query result type</typeparam>
public abstract class QueryEndpoints<TResult> : EndpointBase
    where TResult : class
{
    /// <summary>
    /// Initializes a new instance of the QueryEndpoints class
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="logger">The logger instance</param>
    protected QueryEndpoints(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    /// <summary>
    /// Executes a query and returns the result
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <param name="query">The query to execute</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The query result</returns>
    protected async Task<ActionResult<ApiResponse<TResult>>> ExecuteQueryAsync<TQuery>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        Logger.LogDebug("Executing query: {QueryType}", typeof(TQuery).Name);

        try
        {
            var result = await Mediator.QueryAsync<TQuery, TResult>(query, cancellationToken);
            return Ok(ApiResponse<TResult>.Success(result));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing query {QueryType}: {Error}", typeof(TQuery).Name, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Executes a paged query and returns the paged result
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <typeparam name="TItem">The item type</typeparam>
    /// <param name="query">The query to execute</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The paged query result</returns>
    protected async Task<ActionResult<ApiResponse<PagedResult<TItem>>>> ExecutePagedQueryAsync<TQuery, TItem>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<PagedResult<TItem>>
        where TItem : class
    {
        Logger.LogDebug("Executing paged query: {QueryType}", typeof(TQuery).Name);

        try
        {
            var result = await Mediator.QueryAsync<TQuery, PagedResult<TItem>>(query, cancellationToken);
            return Ok(ApiResponse<PagedResult<TItem>>.Success(result));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing paged query {QueryType}: {Error}", typeof(TQuery).Name, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Executes a query that may return null and handles the null case
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <param name="query">The query to execute</param>
    /// <param name="notFoundMessage">The message to return when result is null</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The query result or not found</returns>
    protected async Task<ActionResult<ApiResponse<TResult>>> ExecuteNullableQueryAsync<TQuery>(
        TQuery query,
        string? notFoundMessage = null,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult?>
    {
        Logger.LogDebug("Executing nullable query: {QueryType}", typeof(TQuery).Name);

        try
        {
            var result = await Mediator.QueryAsync<TQuery, TResult?>(query, cancellationToken);
            
            if (result == null)
            {
                var message = notFoundMessage ?? "Resource not found";
                Logger.LogDebug("Query {QueryType} returned null result", typeof(TQuery).Name);
                return NotFound(ApiResponse<TResult>.NotFound(message));
            }

            return Ok(ApiResponse<TResult>.Success(result));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing nullable query {QueryType}: {Error}", typeof(TQuery).Name, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Executes a query with custom response handling
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <typeparam name="TQueryResult">The query result type</typeparam>
    /// <param name="query">The query to execute</param>
    /// <param name="responseBuilder">The response builder function</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The custom response</returns>
    protected async Task<ActionResult<ApiResponse<TResult>>> ExecuteQueryWithCustomResponseAsync<TQuery, TQueryResult>(
        TQuery query,
        Func<TQueryResult, ApiResponse<TResult>> responseBuilder,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQueryResult>
    {
        Logger.LogDebug("Executing query with custom response: {QueryType}", typeof(TQuery).Name);

        try
        {
            var queryResult = await Mediator.QueryAsync<TQuery, TQueryResult>(query, cancellationToken);
            var response = responseBuilder(queryResult);
            
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing query {QueryType}: {Error}", typeof(TQuery).Name, ex.Message);
            throw;
        }
    }
}

/// <summary>
/// Base class for search endpoints providing search functionality
/// </summary>
/// <typeparam name="TSearchResult">The search result type</typeparam>
public abstract class SearchEndpoints<TSearchResult> : QueryEndpoints<PagedResult<TSearchResult>>
    where TSearchResult : class
{
    /// <summary>
    /// Initializes a new instance of the SearchEndpoints class
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="logger">The logger instance</param>
    protected SearchEndpoints(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    /// <summary>
    /// Performs a search operation
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="filters">Optional additional filters</param>
    /// <param name="sortBy">Optional sort field</param>
    /// <param name="sortOrder">Optional sort order</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The search results</returns>
    [HttpGet("search")]
    public virtual async Task<ActionResult<ApiResponse<PagedResult<TSearchResult>>>> SearchAsync(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Dictionary<string, string>? filters = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Performing search - Term: {SearchTerm}, Page: {PageNumber}, Size: {PageSize}",
            searchTerm, pageNumber, pageSize);

        var query = CreateSearchQuery(searchTerm, pageNumber, pageSize, filters, sortBy, sortOrder);
        return await ExecutePagedQueryAsync<IQuery<PagedResult<TSearchResult>>, TSearchResult>(query, cancellationToken);
    }

    /// <summary>
    /// Creates a search query based on the provided parameters
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="filters">Additional filters</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortOrder">Sort order</param>
    /// <returns>The search query</returns>
    protected abstract IQuery<PagedResult<TSearchResult>> CreateSearchQuery(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        Dictionary<string, string>? filters,
        string? sortBy,
        string? sortOrder);
}

/// <summary>
/// Base class for reporting endpoints providing aggregated data
/// </summary>
/// <typeparam name="TReportResult">The report result type</typeparam>
public abstract class ReportEndpoints<TReportResult> : QueryEndpoints<TReportResult>
    where TReportResult : class
{
    /// <summary>
    /// Initializes a new instance of the ReportEndpoints class
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="logger">The logger instance</param>
    protected ReportEndpoints(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    /// <summary>
    /// Generates a report for the specified date range
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <param name="groupBy">Optional grouping field</param>
    /// <param name="filters">Optional filters</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The report result</returns>
    [HttpGet("report")]
    public virtual async Task<ActionResult<ApiResponse<TReportResult>>> GenerateReportAsync(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? groupBy = null,
        [FromQuery] Dictionary<string, string>? filters = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Generating report - Start: {StartDate}, End: {EndDate}, GroupBy: {GroupBy}",
            startDate, endDate, groupBy);

        var query = CreateReportQuery(startDate, endDate, groupBy, filters);
        return await ExecuteQueryAsync(query, cancellationToken);
    }

    /// <summary>
    /// Creates a report query based on the provided parameters
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <param name="groupBy">Grouping field</param>
    /// <param name="filters">Additional filters</param>
    /// <returns>The report query</returns>
    protected abstract IQuery<TReportResult> CreateReportQuery(
        DateTime? startDate,
        DateTime? endDate,
        string? groupBy,
        Dictionary<string, string>? filters);
} 