using BuildingBlocks.Domain.BusinessRules;
using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Exceptions;

namespace BuildingBlocks.Domain.Extensions;

/// <summary>
/// Extension methods for domain objects
/// </summary>
public static class DomainExtensions
{
    /// <summary>
    /// Checks a business rule and throws an exception if it's broken
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <param name="rule">The business rule to check</param>
    /// <exception cref="BusinessRuleValidationException">Thrown when the rule is broken</exception>
    public static void CheckRule(this Entity entity, IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }

    /// <summary>
    /// Checks multiple business rules and throws an exception if any are broken
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <param name="rules">The business rules to check</param>
    /// <exception cref="BusinessRuleValidationException">Thrown when any rule is broken</exception>
    public static void CheckRules(this Entity entity, params IBusinessRule[] rules)
    {
        foreach (var rule in rules)
        {
            entity.CheckRule(rule);
        }
    }

    /// <summary>
    /// Adds a domain event to an entity if it doesn't already exist
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <param name="domainEvent">The domain event to add</param>
    /// <typeparam name="T">The entity type</typeparam>
    internal static void AddDomainEventIfNotExists<T>(this T entity, IDomainEvent domainEvent) 
        where T : Entity
    {
        if (!entity.DomainEvents.Any(e => e.GetType() == domainEvent.GetType()))
        {
            entity.GetType()
                .GetMethod("AddDomainEvent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                .Invoke(entity, new object[] { domainEvent });
        }
    }

    /// <summary>
    /// Checks if an entity has any domain events of the specified type
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <typeparam name="TEvent">The domain event type</typeparam>
    /// <returns>True if the entity has events of the specified type</returns>
    public static bool HasDomainEvent<TEvent>(this Entity entity) 
        where TEvent : IDomainEvent
    {
        return entity.DomainEvents.OfType<TEvent>().Any();
    }

    /// <summary>
    /// Gets all domain events of the specified type from an entity
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <typeparam name="TEvent">The domain event type</typeparam>
    /// <returns>The domain events of the specified type</returns>
    public static IEnumerable<TEvent> GetDomainEvents<TEvent>(this Entity entity) 
        where TEvent : IDomainEvent
    {
        return entity.DomainEvents.OfType<TEvent>();
    }

    /// <summary>
    /// Checks if an aggregate is new (has no version or version is 0)
    /// </summary>
    /// <param name="aggregate">The aggregate root</param>
    /// <returns>True if the aggregate is new</returns>
    public static bool IsNew(this AggregateRoot aggregate)
    {
        return aggregate.Version == 0;
    }

    /// <summary>
    /// Checks if an entity implements IAuditableEntity
    /// </summary>
    /// <param name="entity">The entity to check</param>
    /// <returns>True if the entity is auditable</returns>
    public static bool IsAuditable(this Entity entity)
    {
        return entity is IAuditableEntity;
    }

    /// <summary>
    /// Checks if an entity implements ISoftDeletable
    /// </summary>
    /// <param name="entity">The entity to check</param>
    /// <returns>True if the entity supports soft deletion</returns>
    public static bool IsSoftDeletable(this Entity entity)
    {
        return entity is ISoftDeletable;
    }

    /// <summary>
    /// Checks if an entity is soft deleted
    /// </summary>
    /// <param name="entity">The entity to check</param>
    /// <returns>True if the entity is soft deleted</returns>
    public static bool IsSoftDeleted(this Entity entity)
    {
        return entity is ISoftDeletable softDeletable && softDeletable.IsDeleted;
    }

    /// <summary>
    /// Safely soft deletes an entity if it supports soft deletion
    /// </summary>
    /// <param name="entity">The entity to soft delete</param>
    /// <param name="deletedBy">Who is deleting the entity</param>
    /// <returns>True if the entity was soft deleted</returns>
    public static bool TrySoftDelete(this Entity entity, string deletedBy)
    {
        if (entity is ISoftDeletable softDeletable && !softDeletable.IsDeleted)
        {
            softDeletable.Delete(deletedBy);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Safely restores a soft deleted entity if it supports soft deletion
    /// </summary>
    /// <param name="entity">The entity to restore</param>
    /// <returns>True if the entity was restored</returns>
    public static bool TryRestore(this Entity entity)
    {
        if (entity is ISoftDeletable softDeletable && softDeletable.IsDeleted)
        {
            softDeletable.Restore();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the audit information from an entity if it's auditable
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <returns>The audit information or null if not auditable</returns>
    public static (DateTime CreatedAt, string CreatedBy, DateTime? LastModifiedAt, string? LastModifiedBy)? GetAuditInfo(this Entity entity)
    {
        if (entity is IAuditableEntity auditable)
        {
            return (auditable.CreatedAt, auditable.CreatedBy, auditable.LastModifiedAt, auditable.LastModifiedBy);
        }
        return null;
    }

    /// <summary>
    /// Sets the audit information for creation if the entity is auditable
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <param name="createdBy">Who created the entity</param>
    /// <param name="createdAt">When the entity was created (optional, defaults to now)</param>
    internal static void SetCreatedAudit(this Entity entity, string createdBy, DateTime? createdAt = null)
    {
        if (entity is IAuditableEntity auditable)
        {
            auditable.CreatedBy = createdBy;
            auditable.CreatedAt = createdAt ?? DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Sets the audit information for modification if the entity is auditable
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <param name="modifiedBy">Who modified the entity</param>
    /// <param name="modifiedAt">When the entity was modified (optional, defaults to now)</param>
    internal static void SetModifiedAudit(this Entity entity, string modifiedBy, DateTime? modifiedAt = null)
    {
        if (entity is IAuditableEntity auditable)
        {
            auditable.LastModifiedBy = modifiedBy;
            auditable.LastModifiedAt = modifiedAt ?? DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Gets all broken business rules from a collection of rules
    /// </summary>
    /// <param name="rules">The business rules to check</param>
    /// <returns>The broken business rules</returns>
    public static IEnumerable<IBusinessRule> GetBrokenRules(this IEnumerable<IBusinessRule> rules)
    {
        return rules.Where(rule => rule.IsBroken());
    }

    /// <summary>
    /// Checks if all business rules in a collection are satisfied
    /// </summary>
    /// <param name="rules">The business rules to check</param>
    /// <returns>True if all rules are satisfied</returns>
    public static bool AllSatisfied(this IEnumerable<IBusinessRule> rules)
    {
        return rules.All(rule => !rule.IsBroken());
    }

    /// <summary>
    /// Checks if any business rule in a collection is broken
    /// </summary>
    /// <param name="rules">The business rules to check</param>
    /// <returns>True if any rule is broken</returns>
    public static bool AnyBroken(this IEnumerable<IBusinessRule> rules)
    {
        return rules.Any(rule => rule.IsBroken());
    }

    /// <summary>
    /// Combines multiple business rules with AND logic
    /// </summary>
    /// <param name="rules">The business rules to combine</param>
    /// <returns>A composite business rule</returns>
    public static CompositeBusinessRule CombineWithAnd(this IEnumerable<IBusinessRule> rules)
    {
        return CompositeBusinessRule.And(rules.ToArray());
    }

    /// <summary>
    /// Combines multiple business rules with OR logic
    /// </summary>
    /// <param name="rules">The business rules to combine</param>
    /// <returns>A composite business rule</returns>
    public static CompositeBusinessRule CombineWithOr(this IEnumerable<IBusinessRule> rules)
    {
        return CompositeBusinessRule.Or(rules.ToArray());
    }

    /// <summary>
    /// Validates that a string is not null or empty, throwing an exception if it is
    /// </summary>
    /// <param name="value">The string to validate</param>
    /// <param name="parameterName">The parameter name for the exception</param>
    /// <param name="message">Custom error message</param>
    /// <returns>The validated string</returns>
    /// <exception cref="ArgumentException">Thrown when the string is null or empty</exception>
    public static string ThrowIfNullOrEmpty(this string? value, string parameterName, string? message = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException(message ?? "Value cannot be null or empty", parameterName);
        }
        return value;
    }

    /// <summary>
    /// Validates that a string is not null, empty, or whitespace, throwing an exception if it is
    /// </summary>
    /// <param name="value">The string to validate</param>
    /// <param name="parameterName">The parameter name for the exception</param>
    /// <param name="message">Custom error message</param>
    /// <returns>The validated string</returns>
    /// <exception cref="ArgumentException">Thrown when the string is null, empty, or whitespace</exception>
    public static string ThrowIfNullOrWhiteSpace(this string? value, string parameterName, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message ?? "Value cannot be null, empty, or whitespace", parameterName);
        }
        return value;
    }

    /// <summary>
    /// Validates that an object is not null, throwing an exception if it is
    /// </summary>
    /// <param name="value">The object to validate</param>
    /// <param name="parameterName">The parameter name for the exception</param>
    /// <param name="message">Custom error message</param>
    /// <returns>The validated object</returns>
    /// <exception cref="ArgumentNullException">Thrown when the object is null</exception>
    public static T ThrowIfNull<T>(this T? value, string parameterName, string? message = null) where T : class
    {
        return value ?? throw new ArgumentNullException(parameterName, message);
    }
} 