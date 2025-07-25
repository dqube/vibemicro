# BuildingBlocks.Infrastructure Library Generation Prompt

## Overview
Generate a comprehensive Infrastructure layer library implementing data access patterns, caching, persistence, and infrastructure concerns. This library provides concrete implementations for domain and application abstractions using Entity Framework Core, distributed caching, and modern .NET infrastructure patterns.

## Project Configuration

### Target Framework & Features
- **.NET 8.0** (`net8.0`)
- **Implicit Usings**: Enabled
- **Nullable Reference Types**: Enabled
- **Treat Warnings as Errors**: Enabled
- **Generate Documentation File**: Enabled

### Package Dependencies
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Options.ConfigurationExtensions" Version="8.0.0" />
<PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
```

### Project References
```xml
<ProjectReference Include="../BuildingBlocks.Domain/BuildingBlocks.Domain.csproj" />
<ProjectReference Include="../BuildingBlocks.Application/BuildingBlocks.Application.csproj" />
```

## Architecture & Patterns

### Core Infrastructure Concepts
1. **Entity Framework Core**: Object-relational mapping with modern patterns
2. **Repository Pattern**: Concrete implementations for domain abstractions
3. **Unit of Work**: Transaction management and change tracking
4. **Interceptors**: Cross-cutting concerns for data operations
5. **Caching**: Multi-level caching with memory and distributed options
6. **Configuration**: Strongly-typed configuration patterns

### Access Modifier Strategy
- **Public**: Core infrastructure interfaces and extension methods
- **Internal**: Implementation details and EF Core specific code

## Folder Structure & Components

### `/Data` - Data Access Layer
```
Data/
├── Context/
│   ├── IDbContext.cs              # Database context interface
│   ├── IDbContextFactory.cs       # Context factory interface
│   ├── DbContextBase.cs           # Base EF Core context
│   └── ApplicationDbContext.cs    # Main application context
├── Interceptors/
│   ├── AuditInterceptor.cs        # Audit trail interceptor
│   ├── DomainEventInterceptor.cs  # Domain event interceptor
│   └── SoftDeleteInterceptor.cs   # Soft deletion interceptor
├── Repositories/
│   ├── Repository.cs              # Generic repository implementation
│   └── ReadOnlyRepository.cs      # Read-only repository implementation
└── UnitOfWork/
    └── UnitOfWork.cs              # Unit of work implementation
```

**IDbContext.cs** - Database Context Interface:
```csharp
public interface IDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    void ChangeTracker_DetectChanges();
    
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
}
```

**DbContextBase.cs** - Base EF Core Context:
```csharp
public abstract class DbContextBase : DbContext, IDbContext
{
    private readonly ICurrentUser _currentUser;
    private readonly IDomainEventService _domainEventService;
    
    protected DbContextBase(
        DbContextOptions options,
        ICurrentUser currentUser,
        IDomainEventService domainEventService) : base(options)
    {
        _currentUser = currentUser;
        _domainEventService = domainEventService;
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Handle audit fields
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedAudit(_currentUser.UserName ?? "System");
                    break;
                case EntityState.Modified:
                    entry.Entity.SetModifiedAudit(_currentUser.UserName ?? "System");
                    break;
            }
        }
        
        // Collect domain events before saving
        var domainEvents = ChangeTracker.Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
        
        var result = await base.SaveChangesAsync(cancellationToken);
        
        // Dispatch domain events after successful save
        foreach (var domainEvent in domainEvents)
        {
            await _domainEventService.PublishAsync(domainEvent, cancellationToken);
        }
        
        return result;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Configure strongly-typed IDs
        ConfigureStronglyTypedIds(modelBuilder);
        
        // Configure soft delete global filter
        ConfigureSoftDeleteFilter(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
    
    private static void ConfigureStronglyTypedIds(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties()
                .Where(p => p.PropertyType.IsValueType && 
                           p.PropertyType.GetInterfaces().Any(i => 
                               i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStronglyTypedId<>)));
            
            foreach (var property in properties)
            {
                var valueProperty = property.PropertyType.GetProperty("Value");
                if (valueProperty != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasConversion(
                            v => valueProperty.GetValue(v),
                            v => Activator.CreateInstance(property.PropertyType, v));
                }
            }
        }
    }
    
    private static void ConfigureSoftDeleteFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(DbContextBase)
                    .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                
                var filter = method.Invoke(null, Array.Empty<object>());
                entityType.SetQueryFilter((LambdaExpression)filter!);
            }
        }
    }
    
    private static LambdaExpression GetSoftDeleteFilter<TEntity>()
        where TEntity : class, ISoftDeletable
    {
        Expression<Func<TEntity, bool>> filter = e => !e.IsDeleted;
        return filter;
    }
}
```

**Repository.cs** - Generic Repository:
```csharp
public class Repository<TEntity, TId, TIdValue> : ReadOnlyRepository<TEntity, TId, TIdValue>, IRepository<TEntity, TId, TIdValue>
    where TEntity : Entity<TId, TIdValue>
    where TId : struct, IStronglyTypedId<TIdValue>
    where TIdValue : IEquatable<TIdValue>
{
    public Repository(IDbContext context) : base(context) { }
    
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entry.Entity;
    }
    
    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().Update(entity);
        return Task.FromResult(entity);
    }
    
    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.Delete();
            Context.Set<TEntity>().Update(entity);
        }
        else
        {
            Context.Set<TEntity>().Remove(entity);
        }
        
        return Task.CompletedTask;
    }
    
    public async Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }
}
```

**AuditInterceptor.cs** - Audit Trail Interceptor:
```csharp
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser;
    
    public AuditInterceptor(ICurrentUser currentUser)
        => _currentUser = currentUser;
    
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyAuditInformation(eventData.Context);
        return result;
    }
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation(eventData.Context);
        return new ValueTask<InterceptionResult<int>>(result);
    }
    
    private void ApplyAuditInformation(DbContext? context)
    {
        if (context == null) return;
        
        var currentUser = _currentUser.UserName ?? "System";
        var utcNow = DateTime.UtcNow;
        
        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = currentUser;
                    entry.Entity.CreatedOn = utcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedBy = currentUser;
                    entry.Entity.ModifiedOn = utcNow;
                    break;
            }
        }
    }
}
```

**DomainEventInterceptor.cs** - Domain Event Interceptor:
```csharp
public sealed class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventService _domainEventService;
    
    public DomainEventInterceptor(IDomainEventService domainEventService)
        => _domainEventService = domainEventService;
    
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, 
        int result, 
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            await PublishDomainEventsAsync(eventData.Context, cancellationToken);
        }
        
        return result;
    }
    
    private async Task PublishDomainEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var domainEvents = context.ChangeTracker.Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
        
        foreach (var domainEvent in domainEvents)
        {
            await _domainEventService.PublishAsync(domainEvent, cancellationToken);
        }
        
        // Clear events after publishing
        foreach (var entry in context.ChangeTracker.Entries<Entity>())
        {
            entry.Entity.ClearDomainEvents();
        }
    }
}
```

### `/Caching` - Caching Implementation
```
Caching/
├── ICacheService.cs           # Cache service interface (from Application)
├── MemoryCacheService.cs      # In-memory cache implementation
└── CacheConfiguration.cs      # Cache configuration options
```

**MemoryCacheService.cs** - Memory Cache Implementation:
```csharp
public sealed class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;
    
    public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }
    
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue(key, out var value) && value is T typedValue)
        {
            _logger.LogDebug("Cache hit for key: {CacheKey}", key);
            return Task.FromResult<T?>(typedValue);
        }
        
        _logger.LogDebug("Cache miss for key: {CacheKey}", key);
        return Task.FromResult<T?>(default);
    }
    
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }
        else
        {
            options.SlidingExpiration = TimeSpan.FromMinutes(30); // Default sliding expiration
        }
        
        _memoryCache.Set(key, value, options);
        _logger.LogDebug("Cached value for key: {CacheKey} with expiration: {Expiration}", key, expiration);
        
        return Task.CompletedTask;
    }
    
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        _logger.LogDebug("Removed cache entry for key: {CacheKey}", key);
        return Task.CompletedTask;
    }
    
    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // Memory cache doesn't support pattern-based removal efficiently
        // This would require maintaining a separate index of keys
        _logger.LogWarning("Pattern-based cache removal not efficiently supported by MemoryCache: {Pattern}", pattern);
        return Task.CompletedTask;
    }
}
```

**CacheConfiguration.cs** - Cache Configuration:
```csharp
public sealed class CacheConfiguration
{
    public const string SectionName = "Cache";
    
    public bool UseRedis { get; set; } = false;
    public string? RedisConnectionString { get; set; }
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan DefaultSlidingExpiration { get; set; } = TimeSpan.FromMinutes(5);
    public string KeyPrefix { get; set; } = "BuildingBlocks";
}
```

### `/Extensions` - Infrastructure Extensions
```
Extensions/
└── ServiceCollectionExtensions.cs # DI registration
```

**ServiceCollectionExtensions.cs** - DI Registration:
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Entity Framework
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                options.UseInMemoryDatabase("BuildingBlocksDb");
            }
            
            // Add interceptors
            options.AddInterceptors(
                services.BuildServiceProvider().GetRequiredService<AuditInterceptor>(),
                services.BuildServiceProvider().GetRequiredService<DomainEventInterceptor>(),
                services.BuildServiceProvider().GetRequiredService<SoftDeleteInterceptor>());
        });
        
        // Register interceptors
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<DomainEventInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        
        // Register context interfaces
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IDbContextFactory<ApplicationDbContext>, DbContextFactory<ApplicationDbContext>>();
        
        // Register repositories
        services.AddScoped(typeof(IRepository<,,>), typeof(Repository<,,>));
        services.AddScoped(typeof(IReadOnlyRepository<,,>), typeof(ReadOnlyRepository<,,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Add caching
        services.AddCaching(configuration);
        
        return services;
    }
    
    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheConfig = configuration.GetSection(CacheConfiguration.SectionName).Get<CacheConfiguration>() 
                         ?? new CacheConfiguration();
        
        services.Configure<CacheConfiguration>(configuration.GetSection(CacheConfiguration.SectionName));
        
        if (cacheConfig.UseRedis && !string.IsNullOrEmpty(cacheConfig.RedisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheConfig.RedisConnectionString;
                options.InstanceName = cacheConfig.KeyPrefix;
            });
            
            // Register Redis-based cache service
            services.AddScoped<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();
        }
        
        return services;
    }
}
```

## Key Design Principles

### 1. Entity Framework Core Integration
- Strongly-typed ID conversion
- Automatic audit field management
- Domain event integration
- Soft delete support

### 2. Repository Pattern
- Generic implementations for common operations
- Specification pattern integration
- Async/await throughout
- Proper cancellation token support

### 3. Interceptors for Cross-Cutting Concerns
- Audit trail automatically applied
- Domain events published after successful saves
- Soft delete filters applied globally
- Consistent behavior across all entities

### 4. Multi-Level Caching
- Memory cache for development/small deployments
- Redis cache for distributed scenarios
- Configurable expiration policies
- Pattern-based cache invalidation

### 5. Configuration-Driven
- Environment-specific database connections
- Configurable caching strategies
- Feature toggles for different implementations
- Strongly-typed configuration objects

## Usage Examples

### Entity Configuration
```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));
        
        builder.OwnsOne(u => u.Username, username =>
        {
            username.Property(u => u.Value)
                .HasColumnName("Username")
                .HasMaxLength(50)
                .IsRequired();
        });
        
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired();
        });
    }
}
```

### Custom Repository
```csharp
public interface IUserRepository : IRepository<User, UserId, Guid>
{
    Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
}

public class UserRepository : Repository<User, UserId, Guid>, IUserRepository
{
    public UserRepository(IDbContext context) : base(context) { }
    
    public async Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }
    
    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .AnyAsync(u => u.Email == email, cancellationToken);
    }
}
```

### Caching Integration
```csharp
public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    
    public async Task<UserDto?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"user-{request.UserId}";
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey, cancellationToken);
        
        if (cachedUser != null)
            return cachedUser;
        
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return null;
        
        var userDto = _mapper.Map<UserDto>(user);
        await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(15), cancellationToken);
        
        return userDto;
    }
}
```

## Implementation Notes

1. **Database Migrations**: Use EF Core migrations for schema management
2. **Connection Resilience**: Implement retry policies for database connections
3. **Performance**: Use compiled queries for frequently executed operations
4. **Monitoring**: Add performance counters and health checks
5. **Testing**: Use in-memory database for integration tests
6. **Security**: Implement proper connection string management

Generate this library with full implementations, comprehensive XML documentation, and adherence to these architectural principles. 