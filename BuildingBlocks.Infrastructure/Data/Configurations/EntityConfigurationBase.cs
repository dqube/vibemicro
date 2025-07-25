using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Infrastructure.Data.Configurations;

/// <summary>
/// Base class for entity configurations providing common functionality
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TIdValue">The underlying value type of the identifier</typeparam>
public abstract class EntityConfigurationBase<TEntity, TId, TIdValue> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    /// <summary>
    /// Configures the entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ConfigureEntity(builder);
        ConfigureProperties(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
        ConfigureConstraints(builder);
    }

    /// <summary>
    /// Configures basic entity settings (table name, schema, etc.)
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    protected virtual void ConfigureEntity(EntityTypeBuilder<TEntity> builder)
    {
        // Configure primary key
        builder.HasKey(e => e.Id);

        // Configure ID property with strongly-typed ID conversion
        builder.Property(e => e.Id)
            .HasConversion(
                v => v.Value,
                v => (TId)Activator.CreateInstance(typeof(TId), v)!)
            .ValueGeneratedNever();

        // Configure version for optimistic concurrency
        builder.Property(e => e.Version)
            .IsRowVersion();
    }

    /// <summary>
    /// Configures entity properties
    /// Override in derived classes for entity-specific properties
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    protected virtual void ConfigureProperties(EntityTypeBuilder<TEntity> builder)
    {
        // Override in derived classes
    }

    /// <summary>
    /// Configures entity relationships
    /// Override in derived classes for entity-specific relationships
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    protected virtual void ConfigureRelationships(EntityTypeBuilder<TEntity> builder)
    {
        // Override in derived classes
    }

    /// <summary>
    /// Configures entity indexes
    /// Override in derived classes for entity-specific indexes
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    protected virtual void ConfigureIndexes(EntityTypeBuilder<TEntity> builder)
    {
        // Override in derived classes
    }

    /// <summary>
    /// Configures entity constraints
    /// Override in derived classes for entity-specific constraints
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    protected virtual void ConfigureConstraints(EntityTypeBuilder<TEntity> builder)
    {
        // Override in derived classes
    }

    /// <summary>
    /// Configures string property with common settings
    /// </summary>
    /// <param name="builder">The property builder</param>
    /// <param name="maxLength">The maximum length</param>
    /// <param name="isRequired">Whether the property is required</param>
    /// <param name="isUnicode">Whether the property supports unicode</param>
    /// <returns>The property builder</returns>
    protected static PropertyBuilder<string> ConfigureStringProperty(
        PropertyBuilder<string> builder, 
        int? maxLength = null, 
        bool isRequired = true, 
        bool isUnicode = true)
    {
        if (isRequired)
            builder.IsRequired();

        if (maxLength.HasValue)
            builder.HasMaxLength(maxLength.Value);

        builder.IsUnicode(isUnicode);

        return builder;
    }

    /// <summary>
    /// Configures decimal property with precision and scale
    /// </summary>
    /// <param name="builder">The property builder</param>
    /// <param name="precision">The precision</param>
    /// <param name="scale">The scale</param>
    /// <returns>The property builder</returns>
    protected static PropertyBuilder<decimal> ConfigureDecimalProperty(
        PropertyBuilder<decimal> builder, 
        int precision = 18, 
        int scale = 2)
    {
        return builder.HasPrecision(precision, scale);
    }

    /// <summary>
    /// Configures decimal property with precision and scale (nullable)
    /// </summary>
    /// <param name="builder">The property builder</param>
    /// <param name="precision">The precision</param>
    /// <param name="scale">The scale</param>
    /// <returns>The property builder</returns>
    protected static PropertyBuilder<decimal?> ConfigureDecimalProperty(
        PropertyBuilder<decimal?> builder, 
        int precision = 18, 
        int scale = 2)
    {
        return builder.HasPrecision(precision, scale);
    }
} 