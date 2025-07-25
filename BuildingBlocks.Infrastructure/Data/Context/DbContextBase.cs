using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;
using BuildingBlocks.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Data.Context;

/// <summary>
/// Base class for Entity Framework Core database contexts
/// </summary>
public abstract class DbContextBase : DbContext, IDbContext
{
    private readonly ILogger<DbContextBase> _logger;

    /// <summary>
    /// Initializes a new instance of the DbContextBase class
    /// </summary>
    /// <param name="options">The database context options</param>
    /// <param name="logger">The logger</param>
    protected DbContextBase(DbContextOptions options, ILogger<DbContextBase> logger)
        : base(options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Configures the model
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        // Configure strongly-typed IDs
        ConfigureStronglyTypedIds(modelBuilder);

        // Configure audit properties
        ConfigureAuditProperties(modelBuilder);

        // Configure soft delete
        ConfigureSoftDelete(modelBuilder);
    }

    /// <summary>
    /// Configures the database context options
    /// </summary>
    /// <param name="optionsBuilder">The options builder</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Add interceptors
        optionsBuilder.AddInterceptors(
            new AuditInterceptor(),
            new DomainEventInterceptor(),
            new SoftDeleteInterceptor()
        );

        // Enable sensitive data logging in development
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        // Enable detailed errors in development
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            optionsBuilder.EnableDetailedErrors();
        }
    }

    /// <summary>
    /// Saves changes with domain event handling
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of state entries written to the database</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Update audit properties before saving
            UpdateAuditProperties();

            // Save changes
            var result = await base.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Saved {Count} changes to database", result);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The database transaction</returns>
    public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Executes a SQL command
    /// </summary>
    /// <param name="sql">The SQL command</param>
    /// <param name="parameters">The parameters</param>
    /// <returns>The number of rows affected</returns>
    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return await Database.ExecuteSqlRawAsync(sql, parameters);
    }

    /// <summary>
    /// Executes a SQL command
    /// </summary>
    /// <param name="sql">The SQL command</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of rows affected</returns>
    public async Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken)
    {
        return await Database.ExecuteSqlRawAsync(sql, cancellationToken);
    }

    /// <summary>
    /// Configures strongly-typed IDs
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected virtual void ConfigureStronglyTypedIds(ModelBuilder modelBuilder)
    {
        // Configure strongly-typed ID conversions
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType
                .GetProperties()
                .Where(p => p.PropertyType.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStronglyTypedId<>)));

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                var valueType = propertyType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStronglyTypedId<>))
                    .GetGenericArguments()[0];

                var converterType = typeof(StronglyTypedIdConverter<,>).MakeGenericType(propertyType, valueType);
                var converter = Activator.CreateInstance(converterType);

                modelBuilder.Entity(entityType.ClrType)
                    .Property(property.Name)
                    .HasConversion((Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter)converter!);
            }
        }
    }

    /// <summary>
    /// Configures audit properties
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected virtual void ConfigureAuditProperties(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(IAuditableEntity.CreatedAt))
                    .HasDefaultValueSql("GETUTCDATE()");

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(IAuditableEntity.CreatedBy))
                    .HasMaxLength(256);

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(IAuditableEntity.LastModifiedBy))
                    .HasMaxLength(256);
            }
        }
    }

    /// <summary>
    /// Configures soft delete
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected virtual void ConfigureSoftDelete(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(ISoftDeletable.IsDeleted))
                    .HasDefaultValue(false);

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(ISoftDeletable.DeletedBy))
                    .HasMaxLength(256);

                // Add global filter for soft delete
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    /// <summary>
    /// Gets the soft delete filter expression
    /// </summary>
    /// <param name="entityType">The entity type</param>
    /// <returns>The filter expression</returns>
    protected virtual System.Linq.Expressions.LambdaExpression GetSoftDeleteFilter(Type entityType)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
        var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(condition, parameter);
    }

    /// <summary>
    /// Updates audit properties for tracked entities
    /// </summary>
    protected virtual void UpdateAuditProperties()
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    // CreatedBy should be set by the audit interceptor
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    // LastModifiedBy should be set by the audit interceptor
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    break;
            }
        }
    }
}

/// <summary>
/// Value converter for strongly-typed IDs
/// </summary>
/// <typeparam name="TStronglyTypedId">The strongly-typed ID type</typeparam>
/// <typeparam name="TValue">The underlying value type</typeparam>
public class StronglyTypedIdConverter<TStronglyTypedId, TValue> : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TStronglyTypedId, TValue>
    where TStronglyTypedId : IStronglyTypedId<TValue>
    where TValue : notnull
{
    /// <summary>
    /// Initializes a new instance of the StronglyTypedIdConverter class
    /// </summary>
    public StronglyTypedIdConverter() : base(
        id => id.Value,
        value => CreateStronglyTypedId(value))
    {
    }

    /// <summary>
    /// Creates a strongly-typed ID from a value
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>The strongly-typed ID</returns>
    private static TStronglyTypedId CreateStronglyTypedId(TValue value)
    {
        return (TStronglyTypedId)Activator.CreateInstance(typeof(TStronglyTypedId), value)!;
    }
} 