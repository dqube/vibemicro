using BuildingBlocks.Application.Services;
using BuildingBlocks.Application.CQRS.Mediator;
using AuthService.Application.Commands.Authentication;
using AuthService.Application.Commands.User;
using AuthService.Application.Queries.User;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Services;

/// <summary>
/// Application service for auth operations
/// </summary>
public class AuthApplicationService : ApplicationServiceBase, IAuthApplicationService
{
    public AuthApplicationService(
        IMediator mediator,
        ILogger<AuthApplicationService> logger) : base(mediator, logger)
    {
    }

    /// <summary>
    /// Authenticates a user
    /// </summary>
    /// <param name="command">The login command</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The login response</returns>
    public async Task<LoginResponse> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(LoginAsync), command);
        
        var response = await Mediator.Send(command, cancellationToken);
        
        LogOperationEnd(nameof(LoginAsync), response.IsSuccess);
        return response;
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="command">The register command</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The register response</returns>
    public async Task<RegisterResponse> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(RegisterAsync), new { command.Username, command.Email });
        
        var response = await Mediator.Send(command, cancellationToken);
        
        LogOperationEnd(nameof(RegisterAsync), response.IsSuccess);
        return response;
    }

    /// <summary>
    /// Changes a user's password
    /// </summary>
    /// <param name="command">The change password command</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The change password response</returns>
    public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(ChangePasswordAsync), new { command.UserId });
        
        var response = await Mediator.Send(command, cancellationToken);
        
        LogOperationEnd(nameof(ChangePasswordAsync), response.IsSuccess);
        return response;
    }

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="query">The get user query</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The user response</returns>
    public async Task<UserResponse?> GetUserAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        LogOperationStart(nameof(GetUserAsync), new { query.UserId });
        
        var response = await Mediator.Send(query, cancellationToken);
        
        LogOperationEnd(nameof(GetUserAsync), response != null);
        return response;
    }
}

/// <summary>
/// Interface for auth application service
/// </summary>
public interface IAuthApplicationService : IApplicationService
{
    /// <summary>
    /// Authenticates a user
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user
    /// </summary>
    Task<RegisterResponse> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes a user's password
    /// </summary>
    Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    Task<UserResponse?> GetUserAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default);
} 