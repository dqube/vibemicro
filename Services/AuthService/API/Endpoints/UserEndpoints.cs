using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Application.CQRS.Mediator;
using AuthService.Application.Queries.User;
using AuthService.Application.Commands.User;
using BuildingBlocks.API.Responses.Base;
using System.Security.Claims;

namespace AuthService.API.Endpoints;

/// <summary>
/// User management endpoints
/// </summary>
public static class UserEndpoints
{
    /// <summary>
    /// Maps user endpoints
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetUserByIdAsync)
            .WithName("GetUserById")
            .WithSummary("Get a user by ID")
            .Produces<ApiResponse<UserResponse>>(200)
            .Produces<ApiResponse>(404)
            .Produces<ApiResponse>(401);

        group.MapGet("/me", GetCurrentUserAsync)
            .WithName("GetCurrentUser")
            .WithSummary("Get the current authenticated user")
            .Produces<ApiResponse<UserResponse>>(200)
            .Produces<ApiResponse>(401);

        return app;
    }

    private static async Task<IResult> GetUserByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return Results.NotFound(ApiResponse.Failure("User not found"));
        }

        return Results.Ok(ApiResponse.Success(result, "User retrieved successfully"));
    }

    private static async Task<IResult> GetCurrentUserAsync(
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        // Get user ID from JWT claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var query = new GetUserByIdQuery(userId);
        var result = await mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return Results.NotFound(ApiResponse.Failure("User not found"));
        }

        return Results.Ok(ApiResponse.Success(result, "Current user retrieved successfully"));
    }
} 