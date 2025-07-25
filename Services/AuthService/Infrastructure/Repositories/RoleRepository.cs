using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Infrastructure.Data;
using BuildingBlocks.Infrastructure.Data.Repositories;

namespace AuthService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Role entities
/// </summary>
public class RoleRepository : ReadOnlyRepository<Role, RoleId>, IRoleRepository
{
    public RoleRepository(AuthDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Role>()
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetRolesByIdsAsync(IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default)
    {
        var ids = roleIds.Select(r => r.Value).ToList();
        return await Context.Set<Role>()
            .Where(r => ids.Contains(r.Id.Value))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Role>()
            .AnyAsync(r => r.Id == roleId, cancellationToken);
    }

    public async Task<Dictionary<RoleId, bool>> ExistAsync(IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default)
    {
        var ids = roleIds.Select(r => r.Value).ToList();
        var existingIds = await Context.Set<Role>()
            .Where(r => ids.Contains(r.Id.Value))
            .Select(r => r.Id.Value)
            .ToListAsync(cancellationToken);

        return roleIds.ToDictionary(
            roleId => roleId,
            roleId => existingIds.Contains(roleId.Value));
    }
} 