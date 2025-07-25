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
/// Repository implementation for RegistrationToken entities
/// </summary>
public class RegistrationTokenRepository : Repository<RegistrationToken, TokenId>, IRegistrationTokenRepository
{
    public RegistrationTokenRepository(AuthDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<RegistrationToken>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var spec = new TokensByUserSpecification(userId);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<IEnumerable<RegistrationToken>> GetByTypeAsync(TokenType tokenType, CancellationToken cancellationToken = default)
    {
        var spec = new TokensByTypeSpecification(tokenType);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<IEnumerable<RegistrationToken>> GetValidTokensByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userSpec = new TokensByUserSpecification(userId);
        var validSpec = new ValidTokensSpecification();
        var combinedSpec = userSpec.And(validSpec);
        
        return await GetBySpecificationAsync(combinedSpec, cancellationToken);
    }

    public async Task<RegistrationToken?> GetValidEmailVerificationTokenAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var spec = new ValidEmailVerificationTokenForUserSpecification(userId);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<RegistrationToken?> GetValidPasswordResetTokenAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var spec = new ValidPasswordResetTokenForUserSpecification(userId);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<IEnumerable<RegistrationToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var spec = new ExpiredTokensSpecification();
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<IEnumerable<RegistrationToken>> GetUsedTokensAsync(CancellationToken cancellationToken = default)
    {
        var spec = new UsedTokensSpecification();
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<IEnumerable<RegistrationToken>> GetTokensForCleanupAsync(TimeSpan? retentionPeriod = null, CancellationToken cancellationToken = default)
    {
        var spec = new TokensForCleanupSpecification(retentionPeriod);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<int> CleanupExpiredTokensAsync(TimeSpan? retentionPeriod = null, CancellationToken cancellationToken = default)
    {
        var spec = new TokensForCleanupSpecification(retentionPeriod);
        var tokensToDelete = await GetBySpecificationAsync(spec, cancellationToken);
        
        var count = tokensToDelete.Count();
        if (count > 0)
        {
            Context.Set<RegistrationToken>().RemoveRange(tokensToDelete);
            await Context.SaveChangesAsync(cancellationToken);
        }
        
        return count;
    }

    public async Task<int> InvalidateTokensForUserAsync(UserId userId, TokenType? tokenType = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<RegistrationToken>()
            .Where(t => t.UserId == userId && !t.IsUsed);

        if (tokenType != null)
        {
            query = query.Where(t => t.TokenType == tokenType);
        }

        var tokensToInvalidate = await query.ToListAsync(cancellationToken);
        
        foreach (var token in tokensToInvalidate)
        {
            // We can't call the domain method here as EF Core doesn't track domain events properly
            // Instead, we'll update the property directly
            Context.Entry(token).Property("IsUsed").CurrentValue = true;
        }

        await Context.SaveChangesAsync(cancellationToken);
        return tokensToInvalidate.Count;
    }
} 