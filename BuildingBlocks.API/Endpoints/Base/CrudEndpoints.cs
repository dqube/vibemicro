using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Responses.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.API.Endpoints.Base;

/// <summary>
/// Base class for CRUD endpoints providing standard REST operations
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The entity identifier type</typeparam>
/// <typeparam name="TDto">The DTO type</typeparam>
/// <typeparam name="TCreateDto">The create DTO type</typeparam>
/// <typeparam name="TUpdateDto">The update DTO type</typeparam>
public abstract class CrudEndpoints<TEntity, TId, TDto, TCreateDto, TUpdateDto> : EndpointBase
    where TEntity : class
    where TId : notnull
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    /// <summary>
    /// Initializes a new instance of the CrudEndpoints class
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="logger">The logger instance</param>
    protected CrudEndpoints(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    /// <summary>
    /// Gets an entity by ID
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entity or not found</returns>
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<ApiResponse<TDto>>> GetByIdAsync(
        [FromRoute] TId id,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Getting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

        var query = CreateGetByIdQuery(id);
        var result = await Mediator.QueryAsync<IQuery<TDto?>, TDto?>(query, cancellationToken);

        if (result == null)
        {
            Logger.LogDebug("{EntityType} with ID {Id} not found", typeof(TEntity).Name, id);
            return NotFound(ApiResponse<TDto>.NotFound($"{typeof(TEntity).Name} with ID {id} not found"));
        }

        return Ok(ApiResponse<TDto>.Success(result));
    }

    /// <summary>
    /// Gets all entities with optional filtering and pagination
    /// </summary>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="search">Optional search term</param>
    /// <param name="sortBy">Optional sort field</param>
    /// <param name="sortOrder">Optional sort order (asc/desc)</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A paged collection of entities</returns>
    [HttpGet]
    public virtual async Task<ActionResult<ApiResponse<PagedResult<TDto>>>> GetAllAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Getting {EntityType} list - Page: {PageNumber}, Size: {PageSize}, Search: {Search}",
            typeof(TEntity).Name, pageNumber, pageSize, search);

        var query = CreateGetAllQuery(pageNumber, pageSize, search, sortBy, sortOrder);
        var result = await Mediator.QueryAsync<IQuery<PagedResult<TDto>>, PagedResult<TDto>>(query, cancellationToken);

        return Ok(ApiResponse<PagedResult<TDto>>.Success(result));
    }

    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="createDto">The entity creation data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created entity</returns>
    [HttpPost]
    public virtual async Task<ActionResult<ApiResponse<TDto>>> CreateAsync(
        [FromBody] TCreateDto createDto,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Creating new {EntityType}", typeof(TEntity).Name);

        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<TDto>.BadRequest("Invalid model state", GetModelStateErrors()));
        }

        var command = CreateCreateCommand(createDto);
        var result = await Mediator.SendAsync<ICommand<TDto>, TDto>(command, cancellationToken);

        Logger.LogInformation("Created {EntityType} successfully", typeof(TEntity).Name);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = GetEntityId(result) }, ApiResponse<TDto>.Success(result));
    }

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="updateDto">The entity update data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated entity</returns>
    [HttpPut("{id}")]
    public virtual async Task<ActionResult<ApiResponse<TDto>>> UpdateAsync(
        [FromRoute] TId id,
        [FromBody] TUpdateDto updateDto,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Updating {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<TDto>.BadRequest("Invalid model state", GetModelStateErrors()));
        }

        var command = CreateUpdateCommand(id, updateDto);
        var result = await Mediator.SendAsync<ICommand<TDto>, TDto>(command, cancellationToken);

        Logger.LogInformation("Updated {EntityType} with ID {Id} successfully", typeof(TEntity).Name, id);
        return Ok(ApiResponse<TDto>.Success(result));
    }

    /// <summary>
    /// Partially updates an existing entity
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="patchDto">The partial update data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated entity</returns>
    [HttpPatch("{id}")]
    public virtual async Task<ActionResult<ApiResponse<TDto>>> PatchAsync(
        [FromRoute] TId id,
        [FromBody] object patchDto,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Patching {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<TDto>.BadRequest("Invalid model state", GetModelStateErrors()));
        }

        var command = CreatePatchCommand(id, patchDto);
        var result = await Mediator.SendAsync<ICommand<TDto>, TDto>(command, cancellationToken);

        Logger.LogInformation("Patched {EntityType} with ID {Id} successfully", typeof(TEntity).Name, id);
        return Ok(ApiResponse<TDto>.Success(result));
    }

    /// <summary>
    /// Deletes an entity by ID
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    public virtual async Task<ActionResult<ApiResponse<object>>> DeleteAsync(
        [FromRoute] TId id,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

        var command = CreateDeleteCommand(id);
        await Mediator.SendAsync(command, cancellationToken);

        Logger.LogInformation("Deleted {EntityType} with ID {Id} successfully", typeof(TEntity).Name, id);
        return NoContent();
    }

    /// <summary>
    /// Creates a query for getting an entity by ID
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <returns>The get by ID query</returns>
    protected abstract IQuery<TDto?> CreateGetByIdQuery(TId id);

    /// <summary>
    /// Creates a query for getting all entities
    /// </summary>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="search">The search term</param>
    /// <param name="sortBy">The sort field</param>
    /// <param name="sortOrder">The sort order</param>
    /// <returns>The get all query</returns>
    protected abstract IQuery<PagedResult<TDto>> CreateGetAllQuery(int pageNumber, int pageSize, string? search, string? sortBy, string? sortOrder);

    /// <summary>
    /// Creates a command for creating an entity
    /// </summary>
    /// <param name="createDto">The creation data</param>
    /// <returns>The create command</returns>
    protected abstract ICommand<TDto> CreateCreateCommand(TCreateDto createDto);

    /// <summary>
    /// Creates a command for updating an entity
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="updateDto">The update data</param>
    /// <returns>The update command</returns>
    protected abstract ICommand<TDto> CreateUpdateCommand(TId id, TUpdateDto updateDto);

    /// <summary>
    /// Creates a command for partially updating an entity
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="patchDto">The patch data</param>
    /// <returns>The patch command</returns>
    protected virtual ICommand<TDto> CreatePatchCommand(TId id, object patchDto)
    {
        throw new NotSupportedException("Patch operations are not supported by default. Override this method to implement patch functionality.");
    }

    /// <summary>
    /// Creates a command for deleting an entity
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <returns>The delete command</returns>
    protected abstract ICommand CreateDeleteCommand(TId id);

    /// <summary>
    /// Gets the entity identifier from a DTO
    /// </summary>
    /// <param name="dto">The DTO</param>
    /// <returns>The entity identifier</returns>
    protected abstract TId GetEntityId(TDto dto);
}

/// <summary>
/// Represents a paged result for API responses
/// </summary>
/// <typeparam name="T">The item type</typeparam>
public class PagedResult<T>
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