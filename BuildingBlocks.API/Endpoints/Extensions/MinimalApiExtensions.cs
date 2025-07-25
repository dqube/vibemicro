using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Responses.Base;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BuildingBlocks.API.Endpoints.Extensions;

/// <summary>
/// Extension methods for minimal API endpoint registration
/// </summary>
public static class MinimalApiExtensions
{
    /// <summary>
    /// Maps a GET endpoint for a query with no parameters
    /// </summary>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="app">The web application</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="queryFactory">Factory function to create the query</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapQuery<TQuery, TResult>(
        this WebApplication app,
        string pattern,
        Func<TQuery> queryFactory)
        where TQuery : IQuery<TResult>
    {
        return app.MapGet(pattern, async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = queryFactory();
            var result = await mediator.QueryAsync<TQuery, TResult>(query, cancellationToken);
            return Results.Ok(ApiResponse<TResult>.Success(result));
        });
    }

    /// <summary>
    /// Maps a GET endpoint for a query with a single parameter
    /// </summary>
    /// <typeparam name="TParam">The parameter type</typeparam>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="app">The web application</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="queryFactory">Factory function to create the query</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapQuery<TParam, TQuery, TResult>(
        this WebApplication app,
        string pattern,
        Func<TParam, TQuery> queryFactory)
        where TQuery : IQuery<TResult>
    {
        return app.MapGet(pattern, async (TParam param, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = queryFactory(param);
            var result = await mediator.QueryAsync<TQuery, TResult>(query, cancellationToken);
            return Results.Ok(ApiResponse<TResult>.Success(result));
        });
    }

    /// <summary>
    /// Maps a GET endpoint for a nullable query result
    /// </summary>
    /// <typeparam name="TParam">The parameter type</typeparam>
    /// <typeparam name="TQuery">The query type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="app">The web application</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="queryFactory">Factory function to create the query</param>
    /// <param name="notFoundMessage">Optional not found message</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapNullableQuery<TParam, TQuery, TResult>(
        this WebApplication app,
        string pattern,
        Func<TParam, TQuery> queryFactory,
        string? notFoundMessage = null)
        where TQuery : IQuery<TResult?>
        where TResult : class
    {
        return app.MapGet(pattern, async (TParam param, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = queryFactory(param);
            var result = await mediator.QueryAsync<TQuery, TResult?>(query, cancellationToken);
            
            if (result == null)
            {
                var message = notFoundMessage ?? "Resource not found";
                return Results.NotFound(ApiResponse<TResult>.NotFound(message));
            }

            return Results.Ok(ApiResponse<TResult>.Success(result));
        });
    }

    /// <summary>
    /// Maps a POST endpoint for a command without return value
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <param name="app">The web application</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="commandFactory">Factory function to create the command</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapCommand<TRequest, TCommand>(
        this WebApplication app,
        string pattern,
        Func<TRequest, TCommand> commandFactory)
        where TCommand : ICommand
    {
        return app.MapPost(pattern, async ([FromBody] TRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = commandFactory(request);
            await mediator.SendAsync(command, cancellationToken);
            return Results.Ok(ApiResponse<object>.Success(null, "Command executed successfully"));
        });
    }

    /// <summary>
    /// Maps a POST endpoint for a command with return value
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="app">The web application</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="commandFactory">Factory function to create the command</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapCommand<TRequest, TCommand, TResult>(
        this WebApplication app,
        string pattern,
        Func<TRequest, TCommand> commandFactory)
        where TCommand : ICommand<TResult>
    {
        return app.MapPost(pattern, async ([FromBody] TRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = commandFactory(request);
            var result = await mediator.SendAsync<TCommand, TResult>(command, cancellationToken);
            return Results.Ok(ApiResponse<TResult>.Success(result));
        });
    }

    /// <summary>
    /// Maps a PUT endpoint for an update command
    /// </summary>
    /// <typeparam name="TId">The identifier type</typeparam>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="app">The web application</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="commandFactory">Factory function to create the command</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapUpdateCommand<TId, TRequest, TCommand, TResult>(
        this WebApplication app,
        string pattern,
        Func<TId, TRequest, TCommand> commandFactory)
        where TCommand : ICommand<TResult>
    {
        return app.MapPut(pattern, async (TId id, [FromBody] TRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = commandFactory(id, request);
            var result = await mediator.SendAsync<TCommand, TResult>(command, cancellationToken);
            return Results.Ok(ApiResponse<TResult>.Success(result));
        });
    }

    /// <summary>
    /// Maps a DELETE endpoint for a delete command
    /// </summary>
    /// <typeparam name="TId">The identifier type</typeparam>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <param name="app">The web application</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="commandFactory">Factory function to create the command</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapDeleteCommand<TId, TCommand>(
        this WebApplication app,
        string pattern,
        Func<TId, TCommand> commandFactory)
        where TCommand : ICommand
    {
        return app.MapDelete(pattern, async (TId id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = commandFactory(id);
            await mediator.SendAsync(command, cancellationToken);
            return Results.NoContent();
        });
    }

    /// <summary>
    /// Maps CRUD endpoints for an entity
    /// </summary>
    /// <typeparam name="TId">The identifier type</typeparam>
    /// <typeparam name="TDto">The DTO type</typeparam>
    /// <typeparam name="TCreateRequest">The create request type</typeparam>
    /// <typeparam name="TUpdateRequest">The update request type</typeparam>
    /// <param name="app">The web application</param>
    /// <param name="baseRoute">The base route pattern</param>
    /// <param name="crudHandlers">CRUD operation handlers</param>
    /// <returns>The route group builder</returns>
    public static RouteGroupBuilder MapCrudEndpoints<TId, TDto, TCreateRequest, TUpdateRequest>(
        this WebApplication app,
        string baseRoute,
        CrudHandlers<TId, TDto, TCreateRequest, TUpdateRequest> crudHandlers)
    {
        var group = app.MapGroup(baseRoute);

        // GET /items
        group.MapGet("", async (IMediator mediator, [AsParameters] PaginationRequest pagination, CancellationToken cancellationToken) =>
        {
            var query = crudHandlers.GetAllQueryFactory(pagination);
            var result = await mediator.QueryAsync(query, cancellationToken);
            return Results.Ok(ApiResponse<PagedResult<TDto>>.Success(result));
        });

        // GET /items/{id}
        group.MapGet("{id}", async (TId id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = crudHandlers.GetByIdQueryFactory(id);
            var result = await mediator.QueryAsync(query, cancellationToken);
            
            if (result == null)
            {
                return Results.NotFound(ApiResponse<TDto>.NotFound($"Item with ID {id} not found"));
            }

            return Results.Ok(ApiResponse<TDto>.Success(result));
        });

        // POST /items
        group.MapPost("", async ([FromBody] TCreateRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = crudHandlers.CreateCommandFactory(request);
            var result = await mediator.SendAsync(command, cancellationToken);
            return Results.Created($"{baseRoute}/{crudHandlers.GetId(result)}", ApiResponse<TDto>.Success(result));
        });

        // PUT /items/{id}
        group.MapPut("{id}", async (TId id, [FromBody] TUpdateRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = crudHandlers.UpdateCommandFactory(id, request);
            var result = await mediator.SendAsync(command, cancellationToken);
            return Results.Ok(ApiResponse<TDto>.Success(result));
        });

        // DELETE /items/{id}
        group.MapDelete("{id}", async (TId id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = crudHandlers.DeleteCommandFactory(id);
            await mediator.SendAsync(command, cancellationToken);
            return Results.NoContent();
        });

        return group;
    }

    /// <summary>
    /// Adds common endpoint metadata (OpenAPI, authorization, etc.)
    /// </summary>
    /// <param name="builder">The route handler builder</param>
    /// <param name="summary">The endpoint summary</param>
    /// <param name="description">The endpoint description</param>
    /// <param name="tags">The endpoint tags</param>
    /// <param name="requiresAuth">Whether the endpoint requires authentication</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder WithApiMetadata(
        this RouteHandlerBuilder builder,
        string summary,
        string? description = null,
        string[]? tags = null,
        bool requiresAuth = false)
    {
        builder.WithSummary(summary);
        
        if (!string.IsNullOrEmpty(description))
        {
            builder.WithDescription(description);
        }

        if (tags?.Length > 0)
        {
            builder.WithTags(tags);
        }

        if (requiresAuth)
        {
            builder.RequireAuthorization();
        }

        return builder;
    }
}

/// <summary>
/// CRUD operation handlers for minimal API endpoints
/// </summary>
/// <typeparam name="TId">The identifier type</typeparam>
/// <typeparam name="TDto">The DTO type</typeparam>
/// <typeparam name="TCreateRequest">The create request type</typeparam>
/// <typeparam name="TUpdateRequest">The update request type</typeparam>
public class CrudHandlers<TId, TDto, TCreateRequest, TUpdateRequest>
{
    public required Func<PaginationRequest, IQuery<PagedResult<TDto>>> GetAllQueryFactory { get; init; }
    public required Func<TId, IQuery<TDto?>> GetByIdQueryFactory { get; init; }
    public required Func<TCreateRequest, ICommand<TDto>> CreateCommandFactory { get; init; }
    public required Func<TId, TUpdateRequest, ICommand<TDto>> UpdateCommandFactory { get; init; }
    public required Func<TId, ICommand> DeleteCommandFactory { get; init; }
    public required Func<TDto, TId> GetId { get; init; }
}

/// <summary>
/// Pagination request parameters
/// </summary>
public class PaginationRequest
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
    /// Gets or sets the search term
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Gets or sets the sort field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Gets or sets the sort order
    /// </summary>
    public string? SortOrder { get; set; }
} 