using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.Caching;
using AuthService.Domain.Repositories;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Application.Caching;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Queries.User;

/// <summary>
/// Handler for get user by ID query with caching
/// </summary>
public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICacheService cacheService,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<UserResponse?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user by ID: {UserId}", query.UserId);

        var cacheKey = UserCacheKeys.GetUserById(query.UserId);
        
        // Try to get from cache first
        var cachedUser = await _cacheService.GetAsync<UserResponse>(cacheKey, cancellationToken);
        if (cachedUser != null)
        {
            _logger.LogDebug("User {UserId} retrieved from cache", query.UserId);
            return cachedUser;
        }

        // Get from database
        var user = await _userRepository.GetByIdAsync(UserId.From(query.UserId), cancellationToken);
        if (user == null)
        {
            _logger.LogInformation("User not found with ID: {UserId}", query.UserId);
            return null;
        }

        // Get user roles (cached separately)
        var rolesCacheKey = UserCacheKeys.GetAllRoles();
        var allRoles = await _cacheService.GetAsync<List<Role>>(rolesCacheKey, cancellationToken);
        
        if (allRoles == null)
        {
            var rolesFromDb = await _roleRepository.GetAllRolesAsync(cancellationToken);
            allRoles = rolesFromDb.ToList();
            await _cacheService.SetAsync(rolesCacheKey, allRoles, TimeSpan.FromHours(1), cancellationToken);
        }

        var userRoles = allRoles.Where(r => user.RoleIds.Contains(r.Id)).ToList();
        var roleResponses = userRoles.Select(r => new RoleResponse(r.Id.Value, r.Name, r.Description)).ToList();

        var response = new UserResponse(
            user.Id.Value,
            user.Username.Value,
            user.Email.Value,
            user.IsActive,
            roleResponses,
            user.CreatedAt,
            user.IsLockedOut(),
            user.LockoutEnd);

        // Cache the response for 30 minutes
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        _logger.LogDebug("User {UserId} cached for future requests", query.UserId);
        return response;
    }
} 