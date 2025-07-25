using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor for handling domain events
/// </summary>
public class DomainEventInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts SaveChanges operation
    /// </summary>
    /// <param name="eventData">The event data</param>
    /// <param name="result">The result</param>
    /// <returns>The intercepted result</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ProcessDomainEvents(eventData.Context);
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
        ProcessDomainEvents(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Processes domain events from entities
    /// </summary>
    /// <param name="context">The database context</param>
    private static void ProcessDomainEvents(DbContext? context)
    {
        if (context == null)
            return;

        // Get all entities with domain events
        var entitiesWithEvents = context.ChangeTracker.Entries()
            .Where(e => e.Entity.GetType().GetInterfaces()
                .Any(i => i.IsGenericType && 
                     i.GetGenericTypeDefinition() == typeof(Entity<,>) ||
                     i.GetGenericTypeDefinition() == typeof(AggregateRoot<,>)))
            .Select(e => e.Entity)
            .Cast<dynamic>()
            .Where(e => e.DomainEvents != null && ((System.Collections.ICollection)e.DomainEvents).Count > 0)
            .ToList();

        if (!entitiesWithEvents.Any())
            return;

        // TODO: Store domain events for later processing
        // This could be done by:
        // 1. Adding them to an outbox table
        // 2. Publishing them immediately (not recommended in SaveChanges)
        // 3. Storing them in a domain event service for later processing

        // For now, we'll just clear them to prevent memory leaks
        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }
    }
} 