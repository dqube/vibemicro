using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Responses.Base;

namespace BuildingBlocks.API.Extensions;

/// <summary>
/// Extension methods for mapping common endpoint patterns in Minimal APIs
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Maps a command endpoint
    /// </summary>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="method">HTTP method</param>
    /// <param name="name">Endpoint name</param>
    /// <param name="summary">Endpoint summary</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapCommand<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        string method = "POST",
        string? name = null,
        string? summary = null,
        params string[] tags)
        where TCommand : ICommand<TResult>
    {
        var handler = async (TCommand command, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.CommandAsync<TCommand, TResult>(command, cancellationToken);
            
            return method.ToUpperInvariant() switch
            {
                "POST" => Results.Created($"/{pattern}/{GetEntityId(result)}", ApiResponse.Success(result)),
                "PUT" or "PATCH" => Results.Ok(ApiResponse.Success(result)),
                "DELETE" => Results.NoContent(),
                _ => Results.Ok(ApiResponse.Success(result))
            };
        };

        var routeBuilder = method.ToUpperInvariant() switch
        {
            "GET" => endpoints.MapGet(pattern, handler),
            "POST" => endpoints.MapPost(pattern, handler),
            "PUT" => endpoints.MapPut(pattern, handler),
            "PATCH" => endpoints.MapPatch(pattern, handler),
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

        return routeBuilder
            .Produces<ApiResponse<TResult>>(GetSuccessStatusCode(method))
            .Produces<ApiResponse<TResult>>(400);
    }

    /// <summary>
    /// Maps a command endpoint with route parameters
    /// </summary>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="createCommand">Function to create command from route parameters and body</param>
    /// <param name="method">HTTP method</param>
    /// <param name="name">Endpoint name</param>
    /// <param name="summary">Endpoint summary</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapCommandWithRoute<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<HttpContext, TCommand> createCommand,
        string method = "POST",
        string? name = null,
        string? summary = null,
        params string[] tags)
        where TCommand : ICommand<TResult>
    {
        var handler = async (HttpContext context, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = createCommand(context);
            var result = await mediator.CommandAsync<TCommand, TResult>(command, cancellationToken);
            
            return method.ToUpperInvariant() switch
            {
                "POST" => Results.Created($"/{pattern}/{GetEntityId(result)}", ApiResponse.Success(result)),
                "PUT" or "PATCH" => Results.Ok(ApiResponse.Success(result)),
                "DELETE" => Results.NoContent(),
                _ => Results.Ok(ApiResponse.Success(result))
            };
        };

        var routeBuilder = method.ToUpperInvariant() switch
        {
            "GET" => endpoints.MapGet(pattern, handler),
            "POST" => endpoints.MapPost(pattern, handler),
            "PUT" => endpoints.MapPut(pattern, handler),
            "PATCH" => endpoints.MapPatch(pattern, handler),
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

        return routeBuilder
            .Produces<ApiResponse<TResult>>(GetSuccessStatusCode(method))
            .Produces<ApiResponse<TResult>>(400);
    }

    /// <summary>
    /// Maps an endpoint group with common configuration
    /// </summary>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="prefix">The route prefix</param>
    /// <param name="configureGroup">Action to configure the group</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <returns>The route group builder</returns>
    public static RouteGroupBuilder MapApiGroup(
        this IEndpointRouteBuilder endpoints,
        string prefix,
        Action<RouteGroupBuilder> configureGroup,
        params string[] tags)
    {
        var group = endpoints.MapGroup(prefix);

        if (tags.Length > 0)
        {
            group = group.WithTags(tags);
        }

        // Add common behaviors
        group.AddEndpointFilter(async (context, next) =>
        {
            // Add correlation ID if not present
            if (!context.HttpContext.Request.Headers.ContainsKey("X-Correlation-ID"))
            {
                context.HttpContext.Request.Headers["X-Correlation-ID"] = Guid.NewGuid().ToString();
            }

            return await next(context);
        });

        configureGroup(group);
        return group;
    }

    /// <summary>
    /// Adds versioning to an endpoint
    /// </summary>
    /// <param name="builder">The route handler builder</param>
    /// <param name="version">The API version</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder WithApiVersion(this RouteHandlerBuilder builder, string version)
    {
        return builder.WithMetadata(new { ApiVersion = version });
    }

    /// <summary>
    /// Adds authorization requirement to an endpoint
    /// </summary>
    /// <param name="builder">The route handler builder</param>
    /// <param name="policy">The authorization policy</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder RequireAuthorization(this RouteHandlerBuilder builder, string? policy = null)
    {
        return string.IsNullOrEmpty(policy) 
            ? builder.RequireAuthorization()
            : builder.RequireAuthorization(policy);
    }

    /// <summary>
    /// Gets the entity ID from a result object
    /// </summary>
    /// <param name="result">The result object</param>
    /// <returns>The entity ID</returns>
    private static object GetEntityId(object result)
    {
        if (result == null) return string.Empty;
        
        var idProperty = result.GetType().GetProperty("Id");
        return idProperty?.GetValue(result) ?? string.Empty;
    }

    /// <summary>
    /// Gets the success status code for an HTTP method
    /// </summary>
    /// <param name="method">The HTTP method</param>
    /// <returns>The success status code</returns>
    private static int GetSuccessStatusCode(string method)
    {
        return method.ToUpperInvariant() switch
        {
            "POST" => 201,
            "DELETE" => 204,
            _ => 200
        };
    }
} 