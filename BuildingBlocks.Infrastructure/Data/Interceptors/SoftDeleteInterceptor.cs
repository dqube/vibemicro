using BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor for handling soft delete operations
/// </summary>
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts SaveChanges operation
    /// </summary>
    /// <param name="eventData">The event data</param>
    /// <param name="result">The result</param>
    /// <returns>The intercepted result</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ProcessSoftDeletes(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Intercepts SaveChangesAsync operation
    /// </summary>
    /// <param name="eventData">The event data</param>
    /// <param name="result">The result</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The intercepted result</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        ProcessSoftDeletes(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Processes soft delete operations
    /// </summary>
    /// <param name="context">The database context</param>
    private static void ProcessSoftDeletes(DbContext? context)
    {
        if (context == null)
            return;

        var entries = context.ChangeTracker.Entries<ISoftDeletable>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            // Convert hard delete to soft delete
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = DateTime.UtcNow;
            entry.Entity.DeletedBy = GetCurrentUser(); // TODO: Get from user context
        }
    }

    /// <summary>
    /// Gets the current user identifier
    /// </summary>
    /// <returns>The current user identifier</returns>
    private static string? GetCurrentUser()
    {
        // TODO: Implement current user resolution from HTTP context or service
        return "System"; // Placeholder
    }
} 