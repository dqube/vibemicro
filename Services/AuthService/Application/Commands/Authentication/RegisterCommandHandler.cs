using BuildingBlocks.Application.CQRS.Commands;
using AuthService.Domain.Services;
using AuthService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Commands.Authentication;

/// <summary>
/// Handler for register command
/// </summary>
public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterResponse>
{
    private readonly IAuthDomainService _authDomainService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IAuthDomainService authDomainService,
        ILogger<RegisterCommandHandler> logger)
    {
        _authDomainService = authDomainService;
        _logger = logger;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing registration for username: {Username}, email: {Email}", 
            command.Username, command.Email);

        try
        {
            // Validate password confirmation
            if (command.Password != command.ConfirmPassword)
            {
                _logger.LogWarning("Registration failed: Password confirmation does not match for {Username}", command.Username);
                return RegisterResponse.Failure("Password confirmation does not match");
            }

            var registrationResult = await _authDomainService.RegisterUserAsync(
                Username.From(command.Username),
                command.Email,
                command.Password,
                cancellationToken);

            if (!registrationResult.IsSuccess || registrationResult.User == null || registrationResult.VerificationToken == null)
            {
                _logger.LogWarning("Registration failed for username: {Username}. Reason: {ErrorMessage}", 
                    command.Username, registrationResult.ErrorMessage);
                return RegisterResponse.Failure(registrationResult.ErrorMessage ?? "Registration failed");
            }

            var user = registrationResult.User;
            var token = registrationResult.VerificationToken;

            _logger.LogInformation("Registration successful for user: {UserId}", user.Id);

            return RegisterResponse.Success(
                user.Id.Value,
                user.Username.Value,
                user.Email.Value,
                token.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during registration for username: {Username}", command.Username);
            return RegisterResponse.Failure("An error occurred during registration");
        }
    }
} 