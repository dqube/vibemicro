using BuildingBlocks.Application.CQRS.Commands;
using AuthService.Domain.Services;
using AuthService.Domain.ValueObjects;
using AuthService.Infrastructure.Authentication;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Commands.Authentication;

/// <summary>
/// Handler for login command
/// </summary>
public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IAuthDomainService _authDomainService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IAuthDomainService authDomainService,
        IJwtTokenService jwtTokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _authDomainService = authDomainService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing login for identifier: {LoginIdentifier}", command.LoginIdentifier);

        try
        {
            // Determine if login identifier is email or username
            var isEmail = command.LoginIdentifier.Contains('@');
            
            var authResult = isEmail
                ? await _authDomainService.AuthenticateByEmailAsync(command.LoginIdentifier, command.Password, cancellationToken)
                : await _authDomainService.AuthenticateAsync(Username.From(command.LoginIdentifier), command.Password, cancellationToken);

            if (!authResult.IsSuccess || authResult.User == null)
            {
                _logger.LogWarning("Login failed for identifier: {LoginIdentifier}. Reason: {ErrorMessage}", 
                    command.LoginIdentifier, authResult.ErrorMessage);
                return LoginResponse.Failure(authResult.ErrorMessage ?? "Authentication failed");
            }

            var user = authResult.User;

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(user, command.RememberMe);

            _logger.LogInformation("Login successful for user: {UserId}", user.Id);

            return LoginResponse.Success(
                user.Id.Value,
                user.Username.Value,
                user.Email.Value,
                user.RoleIds.Select(r => r.Value).ToList(),
                token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for identifier: {LoginIdentifier}", command.LoginIdentifier);
            return LoginResponse.Failure("An error occurred during authentication");
        }
    }
} 