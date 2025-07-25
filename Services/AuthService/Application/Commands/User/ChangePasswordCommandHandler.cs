using BuildingBlocks.Application.CQRS.Commands;
using AuthService.Domain.Services;
using AuthService.Domain.StronglyTypedIds;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Commands.User;

/// <summary>
/// Handler for change password command
/// </summary>
public sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    private readonly IAuthDomainService _authDomainService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IAuthDomainService authDomainService,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _authDomainService = authDomainService;
        _logger = logger;
    }

    public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing password change for user: {UserId}", command.UserId);

        try
        {
            // Validate password confirmation
            if (command.NewPassword != command.ConfirmNewPassword)
            {
                _logger.LogWarning("Password change failed: New password confirmation does not match for user {UserId}", command.UserId);
                return ChangePasswordResponse.Failure("New password confirmation does not match");
            }

            var result = await _authDomainService.ChangePasswordAsync(
                UserId.From(command.UserId),
                command.CurrentPassword,
                command.NewPassword,
                cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Password change failed for user: {UserId}. Reason: {ErrorMessage}", 
                    command.UserId, result.ErrorMessage);
                return ChangePasswordResponse.Failure(result.ErrorMessage ?? "Password change failed");
            }

            _logger.LogInformation("Password change successful for user: {UserId}", command.UserId);
            return ChangePasswordResponse.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during password change for user: {UserId}", command.UserId);
            return ChangePasswordResponse.Failure("An error occurred during password change");
        }
    }
} 