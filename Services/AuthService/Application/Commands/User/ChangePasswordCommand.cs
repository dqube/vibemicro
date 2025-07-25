using BuildingBlocks.Application.CQRS.Commands;

namespace AuthService.Application.Commands.User;

/// <summary>
/// Command to change a user's password
/// </summary>
/// <param name="UserId">The user's ID</param>
/// <param name="CurrentPassword">Current password</param>
/// <param name="NewPassword">New password</param>
/// <param name="ConfirmNewPassword">New password confirmation</param>
public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword) : ICommand<ChangePasswordResponse>;

/// <summary>
/// Response for change password command
/// </summary>
/// <param name="IsSuccess">Whether password change was successful</param>
/// <param name="ErrorMessage">Error message if change failed</param>
public sealed record ChangePasswordResponse(
    bool IsSuccess,
    string? ErrorMessage = null)
{
    public static ChangePasswordResponse Success() => new(true);
    public static ChangePasswordResponse Failure(string errorMessage) => new(false, errorMessage);
} 