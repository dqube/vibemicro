using BuildingBlocks.Application.CQRS.Queries;
using AuthService.Domain.Repositories;
using AuthService.Domain.StronglyTypedIds;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Queries.User;

/// <summary>
/// Handler for get user by ID query
/// </summary>
public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<UserResponse?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user by ID: {UserId}", query.UserId);

        var user = await _userRepository.GetByIdAsync(UserId.From(query.UserId), cancellationToken);
        if (user == null)
        {
            _logger.LogInformation("User not found with ID: {UserId}", query.UserId);
            return null;
        }

        // Get user roles
        var roles = await _roleRepository.GetRolesByIdsAsync(user.RoleIds, cancellationToken);
        var roleResponses = roles.Select(r => new RoleResponse(r.Id.Value, r.Name, r.Description)).ToList();

        return new UserResponse(
            user.Id.Value,
            user.Username.Value,
            user.Email.Value,
            user.IsActive,
            roleResponses,
            user.CreatedAt,
            user.IsLockedOut(),
            user.LockoutEnd);
    }
} 