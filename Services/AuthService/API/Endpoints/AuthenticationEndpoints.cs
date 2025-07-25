using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Application.CQRS.Mediator;
using AuthService.Application.Commands.Authentication;
using AuthService.Application.Queries.User;
using BuildingBlocks.API.Responses.Base;

namespace AuthService.API.Endpoints;

/// <summary>
/// Authentication endpoints
/// </summary>
public static class AuthenticationEndpoints
{
    /// <summary>
    /// Maps authentication endpoints
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication MapAuthenticationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithSummary("Authenticate user with username/email and password")
            .Produces<ApiResponse<LoginResponse>>(200)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(401);

        group.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .WithSummary("Register a new user account")
            .Produces<ApiResponse<RegisterResponse>>(201)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(409);

        group.MapPost("/change-password", ChangePasswordAsync)
            .WithName("ChangePassword")
            .WithSummary("Change user password")
            .RequireAuthorization()
            .Produces<ApiResponse<ChangePasswordResponse>>(200)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(401);

        return app;
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.Unauthorized(ApiResponse.Failure(result.ErrorMessage ?? "Authentication failed"));
        }

        return Results.Ok(ApiResponse.Success(result, "Login successful"));
    }

    private static async Task<IResult> RegisterAsync(
        [FromBody] RegisterCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(ApiResponse.Failure(result.ErrorMessage ?? "Registration failed"));
        }

        return Results.Created($"/api/users/{result.UserId}", 
            ApiResponse.Success(result, "Registration successful"));
    }

    private static async Task<IResult> ChangePasswordAsync(
        [FromBody] ChangePasswordCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(ApiResponse.Failure(result.ErrorMessage ?? "Password change failed"));
        }

        return Results.Ok(ApiResponse.Success(result, "Password changed successfully"));
    }
} 