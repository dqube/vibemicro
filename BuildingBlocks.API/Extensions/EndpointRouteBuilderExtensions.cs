using BuildingBlocks.API.Endpoints.Base;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.API.Extensions;

/// <summary>
/// Extension methods for IEndpointRouteBuilder to simplify endpoint registration
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps a GET endpoint
    /// </summary>
    /// <typeparam name="TEndpoint">The endpoint type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapGet<TEndpoint>(this IEndpointRouteBuilder endpoints, string pattern)
        where TEndpoint : class
    {
        return endpoints.MapGet(pattern, GetEndpointDelegate<TEndpoint>())
            .WithName(typeof(TEndpoint).Name)
            .WithTags(GetTagFromEndpointType<TEndpoint>());
    }

    /// <summary>
    /// Maps a POST endpoint
    /// </summary>
    /// <typeparam name="TEndpoint">The endpoint type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapPost<TEndpoint>(this IEndpointRouteBuilder endpoints, string pattern)
        where TEndpoint : class
    {
        return endpoints.MapPost(pattern, GetEndpointDelegate<TEndpoint>())
            .WithName(typeof(TEndpoint).Name)
            .WithTags(GetTagFromEndpointType<TEndpoint>());
    }

    /// <summary>
    /// Maps a PUT endpoint
    /// </summary>
    /// <typeparam name="TEndpoint">The endpoint type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapPut<TEndpoint>(this IEndpointRouteBuilder endpoints, string pattern)
        where TEndpoint : class
    {
        return endpoints.MapPut(pattern, GetEndpointDelegate<TEndpoint>())
            .WithName(typeof(TEndpoint).Name)
            .WithTags(GetTagFromEndpointType<TEndpoint>());
    }

    /// <summary>
    /// Maps a DELETE endpoint
    /// </summary>
    /// <typeparam name="TEndpoint">The endpoint type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapDelete<TEndpoint>(this IEndpointRouteBuilder endpoints, string pattern)
        where TEndpoint : class
    {
        return endpoints.MapDelete(pattern, GetEndpointDelegate<TEndpoint>())
            .WithName(typeof(TEndpoint).Name)
            .WithTags(GetTagFromEndpointType<TEndpoint>());
    }

    /// <summary>
    /// Maps a PATCH endpoint
    /// </summary>
    /// <typeparam name="TEndpoint">The endpoint type</typeparam>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder MapPatch<TEndpoint>(this IEndpointRouteBuilder endpoints, string pattern)
        where TEndpoint : class
    {
        return endpoints.MapPatch(pattern, GetEndpointDelegate<TEndpoint>())
            .WithName(typeof(TEndpoint).Name)
            .WithTags(GetTagFromEndpointType<TEndpoint>());
    }

    /// <summary>
    /// Maps a versioned endpoint group
    /// </summary>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="prefix">The route prefix</param>
    /// <param name="version">The API version</param>
    /// <returns>The versioned route group builder</returns>
    public static RouteGroupBuilder MapVersionedGroup(this IEndpointRouteBuilder endpoints, string prefix, string version)
    {
        return endpoints.MapGroup($"/api/v{version}/{prefix}")
            .WithTags(prefix.Capitalize())
            .WithOpenApi();
    }

    /// <summary>
    /// Maps an API group with common configuration
    /// </summary>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="prefix">The route prefix</param>
    /// <param name="configureGroup">Action to configure the group</param>
    /// <returns>The route group builder</returns>
    public static RouteGroupBuilder MapApiGroup(
        this IEndpointRouteBuilder endpoints, 
        string prefix, 
        Action<RouteGroupBuilder>? configureGroup = null)
    {
        var group = endpoints.MapGroup($"/api/{prefix}")
            .WithTags(prefix.Capitalize())
            .WithOpenApi();

        configureGroup?.Invoke(group);

        return group;
    }

    /// <summary>
    /// Adds common endpoint filters
    /// </summary>
    /// <param name="builder">The route handler builder</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder WithCommonFilters(this RouteHandlerBuilder builder)
    {
        return builder
            .AddEndpointFilter<ValidationFilter>()
            .AddEndpointFilter<CachingFilter>()
            .AddEndpointFilter<RateLimitingFilter>();
    }

    /// <summary>
    /// Adds validation to an endpoint
    /// </summary>
    /// <typeparam name="T">The type to validate</typeparam>
    /// <param name="builder">The route handler builder</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder)
        where T : class
    {
        return builder.AddEndpointFilter<ValidationFilter<T>>();
    }

    /// <summary>
    /// Adds caching to an endpoint
    /// </summary>
    /// <param name="builder">The route handler builder</param>
    /// <param name="duration">The cache duration</param>
    /// <returns>The route handler builder</returns>
    public static RouteHandlerBuilder WithCaching(this RouteHandlerBuilder builder, TimeSpan? duration = null)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            // Add cache headers or implement caching logic
            var response = await next(context);
            
            if (context.HttpContext.Response.StatusCode == 200)
            {
                var cacheControl = duration?.TotalSeconds.ToString() ?? "300";
                context.HttpContext.Response.Headers.CacheControl = $"public, max-age={cacheControl}";
            }
            
            return response;
        });
    }

    /// <summary>
    /// Gets the endpoint delegate for the specified endpoint type
    /// </summary>
    /// <typeparam name="TEndpoint">The endpoint type</typeparam>
    /// <returns>The endpoint delegate</returns>
    private static Delegate GetEndpointDelegate<TEndpoint>()
        where TEndpoint : class
    {
        return async (IServiceProvider serviceProvider, HttpContext context) =>
        {
            var endpoint = serviceProvider.GetRequiredService<TEndpoint>();
            
            // Use reflection to call the HandleAsync method
            var method = typeof(TEndpoint).GetMethod("HandleAsync");
            if (method == null)
            {
                throw new InvalidOperationException($"Endpoint {typeof(TEndpoint).Name} must have a HandleAsync method");
            }

            var parameters = method.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                
                if (paramType == typeof(CancellationToken))
                {
                    args[i] = context.RequestAborted;
                }
                else if (paramType == typeof(HttpContext))
                {
                    args[i] = context;
                }
                else
                {
                    // Try to get from DI container or request
                    args[i] = serviceProvider.GetService(paramType) ?? 
                              await context.Request.ReadFromJsonAsync(paramType, context.RequestAborted);
                }
            }

            var result = method.Invoke(endpoint, args);
            
            if (result is Task<IResult> taskResult)
            {
                return await taskResult;
            }
            
            if (result is Task task)
            {
                await task;
                return Results.Ok();
            }
            
            return result as IResult ?? Results.Ok(result);
        };
    }

    /// <summary>
    /// Gets the tag name from the endpoint type
    /// </summary>
    /// <typeparam name="TEndpoint">The endpoint type</typeparam>
    /// <returns>The tag name</returns>
    private static string GetTagFromEndpointType<TEndpoint>()
    {
        var typeName = typeof(TEndpoint).Name;
        
        // Remove common endpoint suffixes
        var suffixes = new[] { "Endpoint", "Handler", "Controller" };
        foreach (var suffix in suffixes)
        {
            if (typeName.EndsWith(suffix))
            {
                typeName = typeName[..^suffix.Length];
                break;
            }
        }
        
        return typeName;
    }
}

/// <summary>
/// Extension methods for string manipulation
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Capitalizes the first letter of a string
    /// </summary>
    /// <param name="value">The string to capitalize</param>
    /// <returns>The capitalized string</returns>
    public static string Capitalize(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        
        return char.ToUpper(value[0]) + (value.Length > 1 ? value[1..] : string.Empty);
    }
}

/// <summary>
/// Placeholder validation filter
/// </summary>
public class ValidationFilter : IEndpointFilter
{
    /// <summary>
    /// Invokes the filter
    /// </summary>
    /// <param name="context">The endpoint filter invocation context</param>
    /// <param name="next">The next filter in the pipeline</param>
    /// <returns>The filter result</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Implement validation logic here
        return await next(context);
    }
}

/// <summary>
/// Placeholder validation filter for specific types
/// </summary>
/// <typeparam name="T">The type to validate</typeparam>
public class ValidationFilter<T> : IEndpointFilter
    where T : class
{
    /// <summary>
    /// Invokes the filter
    /// </summary>
    /// <param name="context">The endpoint filter invocation context</param>
    /// <param name="next">The next filter in the pipeline</param>
    /// <returns>The filter result</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Implement type-specific validation logic here
        return await next(context);
    }
}

/// <summary>
/// Placeholder caching filter
/// </summary>
public class CachingFilter : IEndpointFilter
{
    /// <summary>
    /// Invokes the filter
    /// </summary>
    /// <param name="context">The endpoint filter invocation context</param>
    /// <param name="next">The next filter in the pipeline</param>
    /// <returns>The filter result</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Implement caching logic here
        return await next(context);
    }
}

/// <summary>
/// Placeholder rate limiting filter
/// </summary>
public class RateLimitingFilter : IEndpointFilter
{
    /// <summary>
    /// Invokes the filter
    /// </summary>
    /// <param name="context">The endpoint filter invocation context</param>
    /// <param name="next">The next filter in the pipeline</param>
    /// <returns>The filter result</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Implement rate limiting logic here
        return await next(context);
    }
} 