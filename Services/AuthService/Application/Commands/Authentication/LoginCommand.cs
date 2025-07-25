using BuildingBlocks.Application.CQRS.Commands;

namespace AuthService.Application.Commands.Authentication;

/// <summary>
/// Command to authenticate a user with username/email and password
/// </summary>
/// <param name="LoginIdentifier">Username or email address</param>
/// <param name="Password">User password</param>
/// <param name="RememberMe">Whether to create a long-lived session</param>
public sealed record LoginCommand(
    string LoginIdentifier,
    string Password,
    bool RememberMe = false) : ICommand<LoginResponse>;

/// <summary>
/// Response for login command
/// </summary>
/// <param name="UserId">The authenticated user's ID</param>
/// <param name="Username">The username</param>
/// <param name="Email">The email address</param>
/// <param name="Roles">The user's roles</param>
/// <param name="Token">The JWT access token</param>
/// <param name="IsSuccess">Whether authentication was successful</param>
/// <param name="ErrorMessage">Error message if authentication failed</param>
public sealed record LoginResponse(
    Guid? UserId,
    string? Username,
    string? Email,
    IReadOnlyList<int> Roles,
    string? Token,
    bool IsSuccess,
    string? ErrorMessage = null)
{
    public static LoginResponse Success(Guid userId, string username, string email, IReadOnlyList<int> roles, string token) =>
        new(userId, username, email, roles, token, true);

    public static LoginResponse Failure(string errorMessage) =>
        new(null, null, null, Array.Empty<int>(), null, false, errorMessage);
} 