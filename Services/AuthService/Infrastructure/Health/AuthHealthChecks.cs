using Microsoft.Extensions.Diagnostics.HealthChecks;
using AuthService.Infrastructure.Data;
using AuthService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Health;

/// <summary>
/// Health check for AuthService database
/// </summary>
public class AuthDatabaseHealthCheck : IHealthCheck
{
    private readonly AuthDbContext _context;

    public AuthDatabaseHealthCheck(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if we can connect to the database
            await _context.Database.CanConnectAsync(cancellationToken);

            // Check if we can query a simple table
            await _context.Roles.Take(1).ToListAsync(cancellationToken);

            return HealthCheckResult.Healthy("AuthService database is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("AuthService database is not accessible", ex);
        }
    }
}

/// <summary>
/// Health check for AuthService domain services
/// </summary>
public class AuthServiceHealthCheck : IHealthCheck
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public AuthServiceHealthCheck(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if we can query repositories
            var roleCount = (await _roleRepository.GetAllRolesAsync(cancellationToken)).Count();
            
            if (roleCount == 0)
            {
                return HealthCheckResult.Degraded("No roles found in the system");
            }

            return HealthCheckResult.Healthy($"AuthService is healthy. {roleCount} roles configured.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("AuthService domain services are not accessible", ex);
        }
    }
} 