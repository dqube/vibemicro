using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Infrastructure.Data.Configurations;

/// <summary>
/// Base configuration for auditable entities
/// </summary>
/// <typeparam name="TEntity">The auditable entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public abstract class AuditableEntityConfiguration<TEntity, TId, TIdValue> : EntityConfigurationBase<TEntity, TId, TIdValue>
    where TEntity : Entity<TId, TIdValue>, IAuditableEntity
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    /// <summary>
    /// Configures entity properties including audit properties
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    protected override void ConfigureProperties(EntityTypeBuilder<TEntity> builder)
    {
        base.ConfigureProperties(builder);
        
        ConfigureAuditProperties(builder);
    }

    /// <summary>
    /// Configures audit properties
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    protected virtual void ConfigureAuditProperties(EntityTypeBuilder<TEntity> builder)
    {
        // Created properties
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256)
            .IsRequired(false);

        // Updated properties
        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256)
            .IsRequired(false);
    }

    /// <summary>
    /// Configures audit indexes
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    protected override void ConfigureIndexes(EntityTypeBuilder<TEntity> builder)
    {
        base.ConfigureIndexes(builder);
        
        ConfigureAuditIndexes(builder);
    }

    /// <summary>
    /// Configures indexes for audit properties
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    protected virtual void ConfigureAuditIndexes(EntityTypeBuilder<TEntity> builder)
    {
        // Index on CreatedAt for temporal queries
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_CreatedAt");

        // Index on CreatedBy for user-specific queries
        builder.HasIndex(e => e.CreatedBy)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_CreatedBy");

        // Composite index for created audit trail
        builder.HasIndex(e => new { e.CreatedAt, e.CreatedBy })
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_CreatedAudit");
    }
} 