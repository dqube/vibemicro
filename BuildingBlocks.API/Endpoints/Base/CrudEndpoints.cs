using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Responses.Base;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.API.Endpoints.Base;

/// <summary>
/// Extension methods for mapping CRUD endpoints using Minimal APIs
/// </summary>
public static class CrudEndpoints
{
    /// <summary>
    /// Maps CRUD endpoints for an entity type
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TId">The entity identifier type</typeparam>
    /// <typeparam name="TDto">The DTO type</typeparam>
    /// <typeparam name="TCreateDto">The create DTO type</typeparam>
    /// <typeparam name="TUpdateDto">The update DTO type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="routePrefix">The route prefix (e.g., "api/v1/users")</param>
    /// <param name="tag">The OpenAPI tag for grouping endpoints</param>
    /// <returns>The endpoint route builder</returns>
    public static IEndpointRouteBuilder MapCrudEndpoints<TEntity, TId, TDto, TCreateDto, TUpdateDto>(
        this IEndpointRouteBuilder endpoints,
        string routePrefix,
        string? tag = null)
        where TEntity : class
        where TId : notnull
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        var group = endpoints.MapGroup(routePrefix);
        
        if (!string.IsNullOrEmpty(tag))
        {
            group = group.WithTags(tag);
        }

        // GET /{id} - Get by ID
        group.MapGet("/{id}",
            async (TId id, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var query = CreateGetByIdQuery<TEntity, TId, TDto>(id);
                var result = await mediator.QueryAsync<IQuery<TDto?>, TDto?>(query, cancellationToken);
                
                return result != null 
                    ? Results.Ok(ApiResponse.Success(result))
                    : Results.NotFound(ApiResponse.NotFound<TDto>($"{typeof(TEntity).Name} with ID {id} not found"));
            })
            .WithName($"Get{typeof(TEntity).Name}ById")
            .WithSummary($"Get {typeof(TEntity).Name} by ID")
            .Produces<ApiResponse<TDto>>(200)
            .Produces<ApiResponse<TDto>>(404);

        // GET / - Get all with pagination
        group.MapGet("/",
            async ([AsParameters] PaginationQuery pagination, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var query = CreateGetAllQuery<TEntity, TDto>(pagination.PageNumber, pagination.PageSize, pagination.SortBy, pagination.SortDescending);
                var result = await mediator.QueryAsync<IQuery<PagedResult<TDto>>, PagedResult<TDto>>(query, cancellationToken);
                
                return Results.Ok(PagedResponse.Success(result.Items, result.TotalCount, result.PageNumber, result.PageSize));
            })
            .WithName($"Get{typeof(TEntity).Name}List")
            .WithSummary($"Get paginated list of {typeof(TEntity).Name}")
            .Produces<PagedResponse<TDto>>(200);

        // POST / - Create
        group.MapPost("/",
            async (TCreateDto createDto, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var command = CreateCommand<TEntity, TId, TDto, TCreateDto>(createDto);
                var result = await mediator.CommandAsync<ICommand<TDto>, TDto>(command, cancellationToken);
                
                return Results.Created($"/{routePrefix}/{GetEntityId(result)}", ApiResponse.Success(result));
            })
            .WithName($"Create{typeof(TEntity).Name}")
            .WithSummary($"Create new {typeof(TEntity).Name}")
            .Produces<ApiResponse<TDto>>(201)
            .Produces<ApiResponse<TDto>>(400);

        // PUT /{id} - Update
        group.MapPut("/{id}",
            async (TId id, TUpdateDto updateDto, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var command = CreateUpdateCommand<TEntity, TId, TDto, TUpdateDto>(id, updateDto);
                var result = await mediator.CommandAsync<ICommand<TDto>, TDto>(command, cancellationToken);
                
                return Results.Ok(ApiResponse.Success(result));
            })
            .WithName($"Update{typeof(TEntity).Name}")
            .WithSummary($"Update {typeof(TEntity).Name}")
            .Produces<ApiResponse<TDto>>(200)
            .Produces<ApiResponse<TDto>>(400)
            .Produces<ApiResponse<TDto>>(404);

        // DELETE /{id} - Delete
        group.MapDelete("/{id}",
            async (TId id, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var command = CreateDeleteCommand<TEntity, TId>(id);
                var result = await mediator.CommandAsync<ICommand<bool>, bool>(command, cancellationToken);
                
                return result 
                    ? Results.NoContent()
                    : Results.NotFound(ApiResponse.NotFound<object>($"{typeof(TEntity).Name} with ID {id} not found"));
            })
            .WithName($"Delete{typeof(TEntity).Name}")
            .WithSummary($"Delete {typeof(TEntity).Name}")
            .Produces(204)
            .Produces<ApiResponse<object>>(404);

        return endpoints;
    }

    /// <summary>
    /// Creates a get by ID query - should be implemented by the calling code
    /// </summary>
    private static IQuery<TDto?> CreateGetByIdQuery<TEntity, TId, TDto>(TId id)
        where TEntity : class
        where TId : notnull
        where TDto : class
    {
        // This is a placeholder - the actual implementation should be provided
        // via dependency injection or factory pattern
        throw new NotImplementedException($"CreateGetByIdQuery for {typeof(TEntity).Name} must be implemented");
    }

    /// <summary>
    /// Creates a get all query - should be implemented by the calling code
    /// </summary>
    private static IQuery<PagedResult<TDto>> CreateGetAllQuery<TEntity, TDto>(
        int pageNumber, int pageSize, string? sortBy, bool sortDescending)
        where TEntity : class
        where TDto : class
    {
        // This is a placeholder - the actual implementation should be provided
        // via dependency injection or factory pattern
        throw new NotImplementedException($"CreateGetAllQuery for {typeof(TEntity).Name} must be implemented");
    }

    /// <summary>
    /// Creates a create command - should be implemented by the calling code
    /// </summary>
    private static ICommand<TDto> CreateCommand<TEntity, TId, TDto, TCreateDto>(TCreateDto createDto)
        where TEntity : class
        where TId : notnull
        where TDto : class
        where TCreateDto : class
    {
        // This is a placeholder - the actual implementation should be provided
        // via dependency injection or factory pattern
        throw new NotImplementedException($"CreateCommand for {typeof(TEntity).Name} must be implemented");
    }

    /// <summary>
    /// Creates an update command - should be implemented by the calling code
    /// </summary>
    private static ICommand<TDto> CreateUpdateCommand<TEntity, TId, TDto, TUpdateDto>(TId id, TUpdateDto updateDto)
        where TEntity : class
        where TId : notnull
        where TDto : class
        where TUpdateDto : class
    {
        // This is a placeholder - the actual implementation should be provided
        // via dependency injection or factory pattern
        throw new NotImplementedException($"CreateUpdateCommand for {typeof(TEntity).Name} must be implemented");
    }

    /// <summary>
    /// Creates a delete command - should be implemented by the calling code
    /// </summary>
    private static ICommand<bool> CreateDeleteCommand<TEntity, TId>(TId id)
        where TEntity : class
        where TId : notnull
    {
        // This is a placeholder - the actual implementation should be provided
        // via dependency injection or factory pattern
        throw new NotImplementedException($"CreateDeleteCommand for {typeof(TEntity).Name} must be implemented");
    }

    /// <summary>
    /// Gets the entity ID from a DTO - should be implemented by the calling code
    /// </summary>
    private static object GetEntityId<TDto>(TDto dto) where TDto : class
    {
        // This is a placeholder - the actual implementation should extract the ID
        var idProperty = typeof(TDto).GetProperty("Id");
        return idProperty?.GetValue(dto) ?? string.Empty;
    }
}

/// <summary>
/// Pagination query parameters for Minimal API endpoints
/// </summary>
public class PaginationQuery
{
    /// <summary>
    /// Gets or sets the page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the sort field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Gets or sets whether to sort in descending order
    /// </summary>
    public bool SortDescending { get; set; } = false;
} 