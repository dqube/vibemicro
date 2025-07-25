using BuildingBlocks.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Data.Configurations;

/// <summary>
/// Utilities for configuring value objects in Entity Framework Core
/// </summary>
public static class ValueObjectConfiguration
{
    /// <summary>
    /// Configures a value object as an owned entity
    /// </summary>
    /// <typeparam name="TEntity">The owning entity type</typeparam>
    /// <typeparam name="TValueObject">The value object type</typeparam>
    /// <param name="builder">The entity type builder</param>
    /// <param name="navigationExpression">The navigation expression to the value object</param>
    /// <param name="configureAction">Optional action to configure the owned entity</param>
    /// <returns>The owned navigation builder</returns>
    public static OwnedNavigationBuilder<TEntity, TValueObject> ConfigureValueObject<TEntity, TValueObject>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TValueObject?>> navigationExpression,
        Action<OwnedNavigationBuilder<TEntity, TValueObject>>? configureAction = null)
        where TEntity : class
        where TValueObject : class
    {
        var ownedBuilder = builder.OwnsOne(navigationExpression);
        configureAction?.Invoke(ownedBuilder);
        return ownedBuilder;
    }

    /// <summary>
    /// Configures a collection of value objects as owned entities
    /// </summary>
    /// <typeparam name="TEntity">The owning entity type</typeparam>
    /// <typeparam name="TValueObject">The value object type</typeparam>
    /// <param name="builder">The entity type builder</param>
    /// <param name="navigationExpression">The navigation expression to the value object collection</param>
    /// <param name="configureAction">Optional action to configure the owned entity</param>
    /// <returns>The owned navigation builder</returns>
    public static OwnedNavigationBuilder<TEntity, TValueObject> ConfigureValueObjectCollection<TEntity, TValueObject>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, IEnumerable<TValueObject>>> navigationExpression,
        Action<OwnedNavigationBuilder<TEntity, TValueObject>>? configureAction = null)
        where TEntity : class
        where TValueObject : class
    {
        var ownedBuilder = builder.OwnsMany(navigationExpression);
        configureAction?.Invoke(ownedBuilder);
        return ownedBuilder;
    }

    /// <summary>
    /// Configures a single-value object as a JSON column
    /// </summary>
    /// <typeparam name="TEntity">The owning entity type</typeparam>
    /// <typeparam name="TValue">The underlying value type</typeparam>
    /// <param name="builder">The entity type builder</param>
    /// <param name="propertyExpression">The property expression</param>
    /// <param name="columnName">Optional column name</param>
    /// <returns>The property builder</returns>
    public static PropertyBuilder<SingleValueObject<TValue>> ConfigureSingleValueObject<TEntity, TValue>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, SingleValueObject<TValue>>> propertyExpression,
        string? columnName = null)
        where TEntity : class
        where TValue : notnull
    {
        var propertyBuilder = builder.Property(propertyExpression)
            .HasConversion(
                vo => vo.Value,
                value => (SingleValueObject<TValue>)Activator.CreateInstance(typeof(SingleValueObject<TValue>), value)!);

        if (!string.IsNullOrEmpty(columnName))
        {
            propertyBuilder.HasColumnName(columnName);
        }

        return propertyBuilder;
    }

    /// <summary>
    /// Configures a complex value object as a JSON column
    /// </summary>
    /// <typeparam name="TEntity">The owning entity type</typeparam>
    /// <typeparam name="TValueObject">The value object type</typeparam>
    /// <param name="builder">The entity type builder</param>
    /// <param name="propertyExpression">The property expression</param>
    /// <param name="columnName">Optional column name</param>
    /// <param name="jsonOptions">Optional JSON serialization options</param>
    /// <returns>The property builder</returns>
    public static PropertyBuilder<TValueObject> ConfigureJsonValueObject<TEntity, TValueObject>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TValueObject>> propertyExpression,
        string? columnName = null,
        JsonSerializerOptions? jsonOptions = null)
        where TEntity : class
        where TValueObject : ValueObject
    {
        var options = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var propertyBuilder = builder.Property(propertyExpression)
            .HasConversion(
                vo => JsonSerializer.Serialize(vo, options),
                json => JsonSerializer.Deserialize<TValueObject>(json, options)!)
            .HasColumnType("nvarchar(max)");

        if (!string.IsNullOrEmpty(columnName))
        {
            propertyBuilder.HasColumnName(columnName);
        }

        return propertyBuilder;
    }

    /// <summary>
    /// Configures a nullable complex value object as a JSON column
    /// </summary>
    /// <typeparam name="TEntity">The owning entity type</typeparam>
    /// <typeparam name="TValueObject">The value object type</typeparam>
    /// <param name="builder">The entity type builder</param>
    /// <param name="propertyExpression">The property expression</param>
    /// <param name="columnName">Optional column name</param>
    /// <param name="jsonOptions">Optional JSON serialization options</param>
    /// <returns>The property builder</returns>
    public static PropertyBuilder<TValueObject?> ConfigureJsonValueObject<TEntity, TValueObject>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TValueObject?>> propertyExpression,
        string? columnName = null,
        JsonSerializerOptions? jsonOptions = null)
        where TEntity : class
        where TValueObject : ValueObject
    {
        var options = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var propertyBuilder = builder.Property(propertyExpression)
            .HasConversion(
                vo => vo == null ? null : JsonSerializer.Serialize(vo, options),
                json => string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<TValueObject>(json, options))
            .HasColumnType("nvarchar(max)");

        if (!string.IsNullOrEmpty(columnName))
        {
            propertyBuilder.HasColumnName(columnName);
        }

        return propertyBuilder;
    }

    /// <summary>
    /// Configures an enumeration as a string column
    /// </summary>
    /// <typeparam name="TEntity">The owning entity type</typeparam>
    /// <typeparam name="TEnumeration">The enumeration type</typeparam>
    /// <param name="builder">The entity type builder</param>
    /// <param name="propertyExpression">The property expression</param>
    /// <param name="maxLength">Maximum length for the string column</param>
    /// <param name="columnName">Optional column name</param>
    /// <returns>The property builder</returns>
    public static PropertyBuilder<TEnumeration> ConfigureEnumeration<TEntity, TEnumeration>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TEnumeration>> propertyExpression,
        int maxLength = 100,
        string? columnName = null)
        where TEntity : class
        where TEnumeration : Enumeration
    {
        var propertyBuilder = builder.Property(propertyExpression)
            .HasConversion(
                enumeration => enumeration.Name,
                name => Enumeration.FromName<TEnumeration>(name))
            .HasMaxLength(maxLength);

        if (!string.IsNullOrEmpty(columnName))
        {
            propertyBuilder.HasColumnName(columnName);
        }

        return propertyBuilder;
    }
} 