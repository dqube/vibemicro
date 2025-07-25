using BuildingBlocks.Application.CQRS.Commands;

namespace AuthService.Application.Commands.Authentication;

/// <summary>
/// Command to register a new user
/// </summary>
/// <param name="Username">Desired username</param>
/// <param name="Email">Email address</param>
/// <param name="Password">User password</param>
/// <param name="ConfirmPassword">Password confirmation</param>
public sealed record RegisterCommand(
    string Username,
    string Email,
    string Password,
    string ConfirmPassword) : ICommand<RegisterResponse>;

/// <summary>
/// Response for register command
/// </summary>
/// <param name="UserId">The created user's ID</param>
/// <param name="Username">The username</param>
/// <param name="Email">The email address</param>
/// <param name="VerificationTokenId">Email verification token ID</param>
/// <param name="IsSuccess">Whether registration was successful</param>
/// <param name="ErrorMessage">Error message if registration failed</param>
public sealed record RegisterResponse(
    Guid? UserId,
    string? Username,
    string? Email,
    Guid? VerificationTokenId,
    bool IsSuccess,
    string? ErrorMessage = null)
{
    public static RegisterResponse Success(Guid userId, string username, string email, Guid verificationTokenId) =>
        new(userId, username, email, verificationTokenId, true);

    public static RegisterResponse Failure(string errorMessage) =>
        new(null, null, null, null, false, errorMessage);
} 