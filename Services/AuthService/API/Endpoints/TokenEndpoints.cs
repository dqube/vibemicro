using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Application.CQRS.Mediator;
using AuthService.Application.Commands.Token;
using BuildingBlocks.API.Responses.Base;

namespace AuthService.API.Endpoints;

/// <summary>
/// Token management endpoints
/// </summary>
public static class TokenEndpoints
{
    /// <summary>
    /// Maps token endpoints
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication MapTokenEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tokens")
            .WithTags("Tokens")
            .WithOpenApi();

        group.MapPost("/verify-email", VerifyEmailAsync)
            .WithName("VerifyEmail")
            .WithSummary("Verify user email with token")
            .Produces<ApiResponse>(200)
            .Produces<ApiResponse>(400);

        group.MapPost("/reset-password", ResetPasswordAsync)
            .WithName("ResetPassword")
            .WithSummary("Reset password with token")
            .Produces<ApiResponse>(200)
            .Produces<ApiResponse>(400);

        group.MapPost("/request-password-reset", RequestPasswordResetAsync)
            .WithName("RequestPasswordReset")
            .WithSummary("Request password reset token")
            .Produces<ApiResponse>(200)
            .Produces<ApiResponse>(400);

        group.MapPost("/resend-verification", ResendVerificationAsync)
            .WithName("ResendVerification")
            .WithSummary("Resend email verification token")
            .RequireAuthorization()
            .Produces<ApiResponse>(200)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(401);

        return app;
    }

    private static async Task<IResult> VerifyEmailAsync(
        [FromBody] VerifyEmailCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(ApiResponse.Failure(result.ErrorMessage ?? "Email verification failed"));
        }

        return Results.Ok(ApiResponse.Success("Email verified successfully"));
    }

    private static async Task<IResult> ResetPasswordAsync(
        [FromBody] ResetPasswordCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(ApiResponse.Failure(result.ErrorMessage ?? "Password reset failed"));
        }

        return Results.Ok(ApiResponse.Success("Password reset successfully"));
    }

    private static async Task<IResult> RequestPasswordResetAsync(
        [FromBody] RequestPasswordResetCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        // Always return success for security reasons (don't reveal if email exists)
        return Results.Ok(ApiResponse.Success("If the email exists, a password reset link has been sent"));
    }

    private static async Task<IResult> ResendVerificationAsync(
        [FromBody] ResendVerificationCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(ApiResponse.Failure(result.ErrorMessage ?? "Failed to resend verification"));
        }

        return Results.Ok(ApiResponse.Success("Verification email sent"));
    }
} 