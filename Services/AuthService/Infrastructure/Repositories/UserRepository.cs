using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Specifications;
using AuthService.Infrastructure.Data;
using BuildingBlocks.Infrastructure.Data.Repositories;

namespace AuthService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entities
/// </summary>
public class UserRepository : Repository<User, UserId>, IUserRepository
{
    public UserRepository(AuthDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default)
    {
        var spec = new UserByUsernameSpecification(username);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var spec = new UserByEmailSpecification(email);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<bool> IsUsernameUniqueAsync(Username username, UserId? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<User>().Where(u => u.Username == username);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, UserId? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<User>().Where(u => u.Email.Value.ToLower() == email.ToLower());
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        var spec = new ActiveUsersSpecification();
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        var spec = new UsersByRoleSpecification(roleId);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetLockedOutUsersAsync(CancellationToken cancellationToken = default)
    {
        var spec = new LockedOutUsersSpecification();
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersWithExcessiveFailedAttemptsAsync(int threshold = 3, CancellationToken cancellationToken = default)
    {
        var spec = new UsersWithExcessiveFailedAttemptsSpecification(threshold);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var spec = new UserSearchSpecification(searchTerm);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersCreatedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var spec = new UsersCreatedBetweenSpecification(startDate, endDate);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }
} 