using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Responses.Base;

namespace BuildingBlocks.API.Endpoints.Base;

/// <summary>
/// Extension methods for mapping query endpoints using Minimal APIs
/// </summary>
public static class QueryEndpoints
{
    /// <summary>
    /// Maps a single query endpoint
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="createQuery">Function to create the query from route parameters</param>
    /// <param name="method">HTTP method (GET, POST, etc.)</param>
    /// <param name="name">Endpoint name for OpenAPI</param>
    /// <param name="summary">Endpoint summary for OpenAPI</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <returns>The endpoint route builder</returns>
    public static IEndpointRouteBuilder MapQuery<TQuery, TResult>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<HttpContext, TQuery> createQuery,
        string method = "GET",
        string? name = null,
        string? summary = null,
        params string[] tags)
        where TQuery : IQuery<TResult>
    {
        var handler = async (HttpContext context, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = createQuery(context);
            var result = await mediator.QueryAsync<TQuery, TResult>(query, cancellationToken);
            
            return Results.Ok(ApiResponse.Success(result));
        };

        var routeBuilder = method.ToUpperInvariant() switch
        {
            "GET" => endpoints.MapGet(pattern, handler),
            "POST" => endpoints.MapPost(pattern, handler),
            "PUT" => endpoints.MapPut(pattern, handler),
            "DELETE" => endpoints.MapDelete(pattern, handler),
            _ => throw new ArgumentException($"Unsupported HTTP method: {method}", nameof(method))
        };

        if (!string.IsNullOrEmpty(name))
        {
            routeBuilder = routeBuilder.WithName(name);
        }

        if (!string.IsNullOrEmpty(summary))
        {
            routeBuilder = routeBuilder.WithSummary(summary);
        }

        if (tags.Length > 0)
        {
            routeBuilder = routeBuilder.WithTags(tags);
        }

        return endpoints;
    }

    /// <summary>
    /// Maps a paged query endpoint
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="createQuery">Function to create the query from route parameters and pagination</param>
    /// <param name="name">Endpoint name for OpenAPI</param>
    /// <param name="summary">Endpoint summary for OpenAPI</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <returns>The endpoint route builder</returns>
    public static IEndpointRouteBuilder MapPagedQuery<TQuery, TResult>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<HttpContext, PaginationQuery, TQuery> createQuery,
        string? name = null,
        string? summary = null,
        params string[] tags)
        where TQuery : IQuery<PagedResult<TResult>>
    {
        var handler = async (
            HttpContext context,
            [AsParameters] PaginationQuery pagination,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = createQuery(context, pagination);
            var result = await mediator.QueryAsync<TQuery, PagedResult<TResult>>(query, cancellationToken);
            
            return Results.Ok(PagedResponse.Success(result.Items, result.TotalCount, result.PageNumber, result.PageSize));
        };

        var routeBuilder = endpoints.MapGet(pattern, handler);

        if (!string.IsNullOrEmpty(name))
        {
            routeBuilder = routeBuilder.WithName(name);
        }

        if (!string.IsNullOrEmpty(summary))
        {
            routeBuilder = routeBuilder.WithSummary(summary);
        }

        if (tags.Length > 0)
        {
            routeBuilder = routeBuilder.WithTags(tags);
        }

        routeBuilder
            .Produces<PagedResponse<TResult>>(200)
            .Produces<ApiResponse<PagedResult<TResult>>>(400);

        return endpoints;
    }

    /// <summary>
    /// Maps a search endpoint with filters
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <typeparam name="TFilter">The filter type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="createQuery">Function to create the query from filters and pagination</param>
    /// <param name="name">Endpoint name for OpenAPI</param>
    /// <param name="summary">Endpoint summary for OpenAPI</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <returns>The endpoint route builder</returns>
    public static IEndpointRouteBuilder MapSearchEndpoint<TQuery, TResult, TFilter>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<TFilter, PaginationQuery, TQuery> createQuery,
        string? name = null,
        string? summary = null,
        params string[] tags)
        where TQuery : IQuery<PagedResult<TResult>>
        where TFilter : class
    {
        var handler = async (
            [AsParameters] TFilter filter,
            [AsParameters] PaginationQuery pagination,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = createQuery(filter, pagination);
            var result = await mediator.QueryAsync<TQuery, PagedResult<TResult>>(query, cancellationToken);
            
            return Results.Ok(PagedResponse.Success(result.Items, result.TotalCount, result.PageNumber, result.PageSize));
        };

        var routeBuilder = endpoints.MapGet(pattern, handler);

        if (!string.IsNullOrEmpty(name))
        {
            routeBuilder = routeBuilder.WithName(name);
        }

        if (!string.IsNullOrEmpty(summary))
        {
            routeBuilder = routeBuilder.WithSummary(summary);
        }

        if (tags.Length > 0)
        {
            routeBuilder = routeBuilder.WithTags(tags);
        }

        routeBuilder
            .Produces<PagedResponse<TResult>>(200)
            .Produces<ApiResponse<PagedResult<TResult>>>(400);

        return endpoints;
    }

    /// <summary>
    /// Maps a report endpoint
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="createQuery">Function to create the query from route parameters</param>
    /// <param name="name">Endpoint name for OpenAPI</param>
    /// <param name="summary">Endpoint summary for OpenAPI</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <returns>The endpoint route builder</returns>
    public static IEndpointRouteBuilder MapReportEndpoint<TQuery, TResult>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<HttpContext, TQuery> createQuery,
        string? name = null,
        string? summary = null,
        params string[] tags)
        where TQuery : IQuery<TResult>
    {
        return endpoints.MapQuery<TQuery, TResult>(
            pattern, 
            createQuery, 
            "GET", 
            name, 
            summary, 
            tags);
    }
} 