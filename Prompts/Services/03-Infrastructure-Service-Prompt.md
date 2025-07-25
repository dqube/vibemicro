# Infrastructure Service Layer Generation Prompt

## Overview
Generate a comprehensive Infrastructure layer for a microservice following Clean Architecture principles and leveraging the BuildingBlocks.Infrastructure library. This layer provides concrete implementations for data access, external integrations, messaging, caching, and other infrastructure concerns.

## Project Configuration

### Simplified Project File
```xml
<Project Sdk="Microsoft.NET.Sdk"/>
```

**That's it!** All configuration is handled automatically by the centralized build system:

- **Target Framework**: .NET 8.0 (from `Directory.Build.props`)
- **BuildingBlocks Reference**: `BuildingBlocks.Infrastructure` automatically referenced
- **Application Reference**: Service application project automatically referenced
- **Framework References**: `Microsoft.AspNetCore.App` automatically included
- **Package Management**: All necessary packages automatically included via feature flags

### Feature-Based Package Inclusion
Enable only the infrastructure features you need:

```xml
<!-- Optional in individual .csproj to enable specific features -->
<PropertyGroup>
  <!-- Core Infrastructure -->
  <IncludeEntityFramework>true</IncludeEntityFramework>        <!-- EF Core, migrations, interceptors -->
  <IncludeCaching>true</IncludeCaching>                        <!-- Memory, Redis, distributed caching -->
  
  <!-- Authentication & Security -->
  <IncludeAuthentication>true</IncludeAuthentication>          <!-- JWT, OAuth, API keys -->
  <IncludeSecurity>true</IncludeSecurity>                      <!-- Encryption, hashing, data protection -->
  
  <!-- External Services -->
  <IncludeMessaging>true</IncludeMessaging>                    <!-- Service Bus, RabbitMQ, MassTransit -->
  <IncludeEmailServices>true</IncludeEmailServices>            <!-- SMTP, SendGrid, email templates -->
  <IncludeHttpClient>true</IncludeHttpClient>                  <!-- HTTP clients, Polly, resilience -->
  <IncludeCloudStorage>true</IncludeCloudStorage>              <!-- Azure Blob, AWS S3, file storage -->
  
  <!-- Observability -->
  <IncludeMonitoring>true</IncludeMonitoring>                  <!-- OpenTelemetry, health checks, metrics -->
  
  <!-- Background Processing -->
  <IncludeBackgroundServices>true</IncludeBackgroundServices>  <!-- Hangfire, background jobs -->
</PropertyGroup>
```

## Service Infrastructure Structure

### Project Organization
```
{ServiceName}.Infrastructure/
├── {ServiceName}.Infrastructure.csproj       # Simple project file
├── Data/                                     # Data access layer
│   ├── Context/                             # Database contexts
│   │   ├── {ServiceName}DbContext.cs       # Main database context
│   │   ├── DbContextFactory.cs             # Context factory for migrations
│   │   └── DesignTimeDbContextFactory.cs   # Design-time factory
│   ├── Configurations/                     # Entity configurations
│   │   ├── {Aggregate}Configuration.cs     # EF Core entity configurations
│   │   └── ValueObjectConfigurations.cs    # Value object configurations
│   ├── Repositories/                       # Repository implementations
│   │   ├── {Aggregate}Repository.cs        # Aggregate-specific repositories
│   │   └── UnitOfWork.cs                   # Unit of work implementation
│   ├── Migrations/                         # Database migrations
│   │   └── (Auto-generated EF migrations)
│   ├── Interceptors/                       # EF Core interceptors
│   │   ├── {ServiceName}AuditInterceptor.cs # Audit trail
│   │   ├── DomainEventInterceptor.cs       # Domain event publishing
│   │   └── SoftDeleteInterceptor.cs        # Soft delete handling
│   └── Seeding/                            # Data seeding
│       ├── {ServiceName}DataSeeder.cs      # Data seeding implementation
│       └── SeedData/                       # Seed data files
├── Caching/                                 # Caching implementations
│   ├── {ServiceName}CacheService.cs        # Service-specific caching
│   ├── CacheStrategies/                    # Caching strategies
│   │   ├── {Aggregate}CacheStrategy.cs     # Entity-specific caching
│   │   └── QueryCacheStrategy.cs           # Query result caching
│   └── Invalidation/                       # Cache invalidation
│       └── CacheInvalidationService.cs     # Cache invalidation logic
├── Authentication/                          # Authentication services
│   ├── JWT/                                # JWT implementation
│   │   ├── JwtTokenService.cs              # JWT token generation/validation
│   │   ├── JwtConfiguration.cs             # JWT configuration
│   │   └── JwtSecurityTokenHandler.cs      # Custom token handler
│   ├── ApiKey/                             # API key authentication
│   │   ├── ApiKeyAuthenticationService.cs  # API key validation
│   │   └── ApiKeyConfiguration.cs          # API key configuration
│   └── OAuth/                              # OAuth integration
│       ├── OAuthService.cs                 # OAuth service implementation
│       └── OAuthConfiguration.cs           # OAuth configuration
├── Authorization/                           # Authorization services
│   ├── {ServiceName}AuthorizationService.cs # Authorization logic
│   ├── Policies/                           # Authorization policies
│   │   ├── {Resource}AuthorizationPolicy.cs # Resource-specific policies
│   │   └── RoleBasedPolicies.cs            # Role-based authorization
│   └── Handlers/                           # Authorization handlers
│       └── {Requirement}AuthorizationHandler.cs # Custom authorization handlers
├── Messaging/                               # Message bus implementations
│   ├── EventBus/                           # Event bus
│   │   ├── {ServiceName}EventBus.cs        # Service-specific event bus
│   │   ├── InMemoryEventBus.cs             # In-memory implementation
│   │   ├── ServiceBusEventBus.cs           # Azure Service Bus
│   │   └── RabbitMQEventBus.cs             # RabbitMQ implementation
│   ├── Publishers/                         # Message publishers
│   │   ├── IntegrationEventPublisher.cs    # Integration event publisher
│   │   └── DomainEventPublisher.cs         # Domain event publisher
│   ├── Subscribers/                        # Message subscribers
│   │   ├── {Event}Subscriber.cs            # Event-specific subscribers
│   │   └── DeadLetterSubscriber.cs         # Dead letter handling
│   ├── Serialization/                      # Message serialization
│   │   ├── JsonMessageSerializer.cs        # JSON serialization
│   │   └── ProtobufMessageSerializer.cs    # Protobuf serialization
│   └── Configuration/                      # Messaging configuration
│       ├── MessageBusConfiguration.cs      # Message bus config
│       └── TopicConfiguration.cs           # Topic/queue configuration
├── ExternalServices/                        # External service integrations
│   ├── Http/                               # HTTP client services
│   │   ├── {ExternalService}HttpClient.cs  # Service-specific HTTP clients
│   │   ├── HttpClientConfiguration.cs      # HTTP client configuration
│   │   └── ResilienceStrategies.cs         # Polly resilience strategies
│   ├── Email/                              # Email services
│   │   ├── SmtpEmailService.cs             # SMTP email service
│   │   ├── SendGridEmailService.cs         # SendGrid implementation
│   │   ├── EmailTemplates/                 # Email templates
│   │   └── EmailConfiguration.cs           # Email configuration
│   ├── Storage/                            # Storage services
│   │   ├── AzureBlobStorageService.cs      # Azure Blob storage
│   │   ├── AwsS3StorageService.cs          # AWS S3 storage
│   │   └── LocalFileStorageService.cs      # Local file storage
│   └── ThirdParty/                         # Third-party integrations
│       ├── {ThirdPartyService}Client.cs    # Third-party API clients
│       └── {ThirdPartyService}Configuration.cs # Configuration
├── BackgroundServices/                      # Background processing
│   ├── {ServiceName}BackgroundService.cs   # Main background service
│   ├── Jobs/                               # Background jobs
│   │   ├── {JobName}Job.cs                 # Specific job implementations
│   │   └── RecurringJobs.cs                # Recurring job definitions
│   ├── Processors/                         # Message processors
│   │   ├── OutboxProcessor.cs              # Outbox pattern processor
│   │   ├── InboxProcessor.cs               # Inbox pattern processor
│   │   └── DomainEventProcessor.cs         # Domain event processor
│   └── Queues/                             # Queue implementations
│       ├── InMemoryQueue.cs                # In-memory queue
│       └── ServiceBusQueue.cs              # Service bus queue
├── Health/                                  # Health check implementations
│   ├── {ServiceName}HealthChecks.cs        # Service-specific health checks
│   ├── DatabaseHealthCheck.cs              # Database health check
│   ├── ExternalServiceHealthCheck.cs       # External service health checks
│   └── CacheHealthCheck.cs                 # Cache health check
├── Monitoring/                              # Monitoring and observability
│   ├── Metrics/                            # Custom metrics
│   │   ├── {ServiceName}Metrics.cs         # Service-specific metrics
│   │   └── BusinessMetrics.cs              # Business metrics
│   ├── Tracing/                            # Distributed tracing
│   │   ├── ActivitySources.cs              # Activity sources for tracing
│   │   └── TracingEnrichment.cs            # Tracing enrichment
│   └── Logging/                            # Structured logging
│       ├── {ServiceName}Logger.cs          # Service-specific logger
│       └── LoggingEnrichment.cs            # Logging enrichment
├── Security/                                # Security implementations
│   ├── Encryption/                         # Encryption services
│   │   ├── AesEncryptionService.cs         # AES encryption
│   │   └── RsaEncryptionService.cs         # RSA encryption
│   ├── Hashing/                            # Hashing services
│   │   ├── PasswordHashingService.cs       # Password hashing
│   │   └── HashingService.cs               # General hashing
│   └── DataProtection/                     # Data protection
│       ├── PersonalDataEncryption.cs       # Personal data encryption
│       └── TokenDataProtection.cs          # Token data protection
├── Configuration/                           # Configuration management
│   ├── {ServiceName}Configuration.cs       # Service configuration
│   ├── DatabaseConfiguration.cs            # Database configuration
│   ├── CacheConfiguration.cs               # Cache configuration
│   └── IntegrationConfiguration.cs         # Integration configuration
└── Extensions/                              # Extensions and DI registration
    ├── ServiceCollectionExtensions.cs      # Main DI registration
    ├── DataExtensions.cs                   # Data layer extensions
    ├── CachingExtensions.cs                # Caching extensions
    ├── MessagingExtensions.cs              # Messaging extensions
    ├── AuthenticationExtensions.cs         # Authentication extensions
    └── MonitoringExtensions.cs             # Monitoring extensions
```

## Implementation Guidelines

### 1. Database Context and Configuration
Implement EF Core context with BuildingBlocks patterns:

```csharp
// Example: Service Database Context
public class AuthDbContext : DbContextBase, IDbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    // DbSets for aggregates
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

        // Configure strongly-typed IDs
        ConfigureStronglyTypedIds(modelBuilder);

        // Configure value objects
        ConfigureValueObjects(modelBuilder);

        // Configure audit properties
        ConfigureAuditProperties(modelBuilder);
    }

    private void ConfigureStronglyTypedIds(ModelBuilder modelBuilder)
    {
        // User ID conversion
        modelBuilder.Entity<User>()
            .Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        // Role ID conversion
        modelBuilder.Entity<Role>()
            .Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                value => new RoleId(value));
    }

    private void ConfigureValueObjects(ModelBuilder modelBuilder)
    {
        // Username value object
        modelBuilder.Entity<User>()
            .Property(e => e.Username)
            .HasConversion(
                username => username.Value,
                value => new Username(value))
            .HasMaxLength(50);

        // Email value object
        modelBuilder.Entity<User>()
            .Property(e => e.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasMaxLength(320);

        // Password hash value object
        modelBuilder.Entity<User>()
            .OwnsOne(e => e.PasswordHash, ph =>
            {
                ph.Property(p => p.Value)
                  .HasColumnName("PasswordHash")
                  .HasMaxLength(500);
            });
    }

    private void ConfigureAuditProperties(ModelBuilder modelBuilder)
    {
        // Configure auditable entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>(nameof(IAuditableEntity.CreatedAt))
                    .HasDefaultValueSql("GETUTCDATE()");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string?>(nameof(IAuditableEntity.CreatedBy))
                    .HasMaxLength(100);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime?>(nameof(IAuditableEntity.UpdatedAt));

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string?>(nameof(IAuditableEntity.UpdatedBy))
                    .HasMaxLength(100);
            }

            // Configure soft delete
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<bool>(nameof(ISoftDeletable.IsDeleted))
                    .HasDefaultValue(false);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime?>(nameof(ISoftDeletable.DeletedAt));

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string?>(nameof(ISoftDeletable.DeletedBy))
                    .HasMaxLength(100);

                // Global query filter for soft delete
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var propertyMethodInfo = typeof(EF).GetMethod(nameof(EF.Property))!
                    .MakeGenericMethod(typeof(bool));
                var isDeletedProperty = Expression.Call(
                    propertyMethodInfo,
                    parameter,
                    Expression.Constant(nameof(ISoftDeletable.IsDeleted)));
                var compareExpression = Expression.MakeBinary(
                    ExpressionType.Equal,
                    isDeletedProperty,
                    Expression.Constant(false));
                var lambda = Expression.Lambda(compareExpression, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}

// Example: Entity Configuration
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table configuration
        builder.ToTable("Users");
        
        // Primary key
        builder.HasKey(u => u.Id);
        
        // Properties
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Indexes
        builder.HasIndex(u => u.Username)
            .IsUnique();
            
        builder.HasIndex(u => u.Email)
            .IsUnique();

        // Relationships
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### 2. Repository Implementations
Implement repositories using BuildingBlocks patterns:

```csharp
// Example: User Repository Implementation
public class UserRepository : Repository<User, UserId, Guid>, IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(Username username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        var spec = new ActiveUsersSpecification();
        return await GetAsync(spec, cancellationToken);
    }

    public async Task<PagedResult<User>> GetUsersPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var spec = new ActiveUsersSpecification();
        return await GetPagedAsync(spec, page, pageSize, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.UserRoles.Any(ur => ur.RoleId == roleId))
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetUserWithRolesAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
}

// Example: Unit of Work Implementation
public class AuthUnitOfWork : UnitOfWork, IAuthUnitOfWork
{
    private readonly AuthDbContext _context;
    private IUserRepository? _userRepository;
    private IRoleRepository? _roleRepository;

    public AuthUnitOfWork(AuthDbContext context) : base(context)
    {
        _context = context;
    }

    public IUserRepository Users => _userRepository ??= new UserRepository(_context);
    public IRoleRepository Roles => _roleRepository ??= new RoleRepository(_context);

    public async Task<int> SaveChangesAsync(string? userId = null, CancellationToken cancellationToken = default)
    {
        // Set audit properties before saving
        SetAuditProperties(userId);
        
        // Dispatch domain events
        await DispatchDomainEventsAsync(cancellationToken);
        
        return await _context.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditProperties(string? userId)
    {
        var entries = _context.ChangeTracker.Entries<IAuditableEntity>();
        
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = userId;
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEventEntities = _context.ChangeTracker.Entries<AggregateRoot>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Any())
            .ToArray();

        var domainEvents = domainEventEntities
            .SelectMany(x => x.DomainEvents)
            .ToArray();

        domainEventEntities.ToList().ForEach(entity => entity.ClearDomainEvents());

        var mediator = _context.GetService<IMediator>();
        if (mediator != null)
        {
            foreach (var domainEvent in domainEvents)
            {
                await mediator.PublishAsync(domainEvent, cancellationToken);
            }
        }
    }
}
```

### 3. Caching Implementation
Implement caching strategies:

```csharp
// Example: Service-specific Cache Service
public class AuthCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<AuthCacheService> _logger;
    private readonly AuthCacheConfiguration _configuration;

    public AuthCacheService(
        IDistributedCache distributedCache,
        IMemoryCache memoryCache,
        ILogger<AuthCacheService> logger,
        IOptions<AuthCacheConfiguration> configuration)
    {
        _distributedCache = distributedCache;
        _memoryCache = memoryCache;
        _logger = logger;
        _configuration = configuration.Value;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        // Try memory cache first (L1)
        if (_memoryCache.TryGetValue(key, out T? memoryValue))
        {
            _logger.LogDebug("Cache hit (memory): {Key}", key);
            return memoryValue;
        }

        // Try distributed cache (L2)
        var distributedValue = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(distributedValue))
        {
            var value = JsonSerializer.Deserialize<T>(distributedValue);
            
            // Store in memory cache for faster subsequent access
            var memoryExpiry = TimeSpan.FromMinutes(_configuration.MemoryCacheExpiryMinutes);
            _memoryCache.Set(key, value, memoryExpiry);
            
            _logger.LogDebug("Cache hit (distributed): {Key}", key);
            return value;
        }

        _logger.LogDebug("Cache miss: {Key}", key);
        return default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var actualExpiry = expiry ?? TimeSpan.FromMinutes(_configuration.DefaultExpiryMinutes);
        
        // Set in distributed cache
        var serializedValue = JsonSerializer.Serialize(value);
        var distributedOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = actualExpiry
        };
        
        await _distributedCache.SetStringAsync(key, serializedValue, distributedOptions, cancellationToken);
        
        // Set in memory cache with shorter expiry
        var memoryExpiry = TimeSpan.FromMinutes(Math.Min(actualExpiry.TotalMinutes, _configuration.MemoryCacheExpiryMinutes));
        _memoryCache.Set(key, value, memoryExpiry);
        
        _logger.LogDebug("Cache set: {Key} (expires in {Expiry})", key, actualExpiry);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        // Remove from both caches
        _memoryCache.Remove(key);
        await _distributedCache.RemoveAsync(key, cancellationToken);
        
        _logger.LogDebug("Cache removed: {Key}", key);
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // For Redis, you could use SCAN command to find keys matching pattern
        // For now, this is a simplified implementation
        _logger.LogWarning("RemoveByPatternAsync not fully implemented for pattern: {Pattern}", pattern);
    }
}

// Example: Cache Configuration
public class AuthCacheConfiguration
{
    public int DefaultExpiryMinutes { get; set; } = 30;
    public int MemoryCacheExpiryMinutes { get; set; } = 5;
    public int UserCacheExpiryMinutes { get; set; } = 60;
    public int RoleCacheExpiryMinutes { get; set; } = 240; // 4 hours
    public bool EnableCaching { get; set; } = true;
}
```

### 4. Authentication Services
Implement authentication services:

```csharp
// Example: JWT Token Service
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtTokenService(
        IOptions<JwtConfiguration> configuration,
        ILogger<JwtTokenService> logger,
        IDateTimeProvider dateTimeProvider)
    {
        _configuration = configuration.Value;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public string GenerateAccessToken(User user, IEnumerable<Role> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new(ClaimTypes.Name, user.Username.Value),
            new(ClaimTypes.Email, user.Email.Value),
            new("sub", user.Id.Value.ToString()),
            new("jti", Guid.NewGuid().ToString()),
            new("iat", _dateTimeProvider.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
            
            // Add permission claims
            foreach (var permission in role.Permissions)
            {
                claims.Add(new Claim("permission", permission.Name));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration.Issuer,
            audience: _configuration.Audience,
            claims: claims,
            expires: _dateTimeProvider.UtcNow.AddMinutes(_configuration.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        _logger.LogDebug("Access token generated for user: {UserId}", user.Id);
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        var refreshToken = Convert.ToBase64String(randomNumber);
        _logger.LogDebug("Refresh token generated");
        
        return refreshToken;
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration.Issuer,
                ValidateAudience = true,
                ValidAudience = _configuration.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            if (validatedToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogDebug("Token validated successfully");
                return principal;
            }

            _logger.LogWarning("Token validation failed: Invalid algorithm");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return null;
        }
    }

    public bool IsTokenExpired(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            
            return jsonToken.ValidTo <= _dateTimeProvider.UtcNow;
        }
        catch
        {
            return true;
        }
    }
}

// Example: Password Hashing Service
public class PasswordHashingService : IPasswordHashingService
{
    private readonly ILogger<PasswordHashingService> _logger;
    private const int WorkFactor = 12;

    public PasswordHashingService(ILogger<PasswordHashingService> logger)
    {
        _logger = logger;
    }

    public string HashPassword(string password)
    {
        try
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
            _logger.LogDebug("Password hashed successfully");
            return hashedPassword;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to hash password");
            throw;
        }
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            var isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            _logger.LogDebug("Password verification: {IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify password");
            return false;
        }
    }
}
```

### 5. External Service Integrations
Implement external service clients:

```csharp
// Example: Email Service Implementation
public class SmtpEmailService : IEmailService
{
    private readonly SmtpConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IOptions<SmtpConfiguration> configuration,
        ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            using var emailMessage = new MimeMessage();
            
            emailMessage.From.Add(new MailboxAddress(_configuration.FromName, _configuration.FromEmail));
            emailMessage.To.AddRange(message.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Email)));
            
            if (message.CcAddresses?.Any() == true)
            {
                emailMessage.Cc.AddRange(message.CcAddresses.Select(x => new MailboxAddress(x.Name, x.Email)));
            }
            
            if (message.BccAddresses?.Any() == true)
            {
                emailMessage.Bcc.AddRange(message.BccAddresses.Select(x => new MailboxAddress(x.Name, x.Email)));
            }

            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder();
            if (message.IsHtml)
            {
                bodyBuilder.HtmlBody = message.Body;
            }
            else
            {
                bodyBuilder.TextBody = message.Body;
            }

            if (message.Attachments?.Any() == true)
            {
                foreach (var attachment in message.Attachments)
                {
                    bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content);
                }
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration.Host, _configuration.Port, _configuration.UseSsl, cancellationToken);
            
            if (!string.IsNullOrEmpty(_configuration.Username))
            {
                await client.AuthenticateAsync(_configuration.Username, _configuration.Password, cancellationToken);
            }
            
            await client.SendAsync(emailMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent successfully to {ToAddresses}", 
                string.Join(", ", message.ToAddresses.Select(x => x.Email)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToAddresses}", 
                string.Join(", ", message.ToAddresses.Select(x => x.Email)));
            throw;
        }
    }

    public async Task SendTemplateEmailAsync<T>(string templateName, T model, EmailAddress toAddress, CancellationToken cancellationToken = default)
    {
        // Load template and render with model
        var template = await LoadTemplateAsync(templateName, cancellationToken);
        var renderedContent = RenderTemplate(template, model);

        var message = new EmailMessage
        {
            ToAddresses = new[] { toAddress },
            Subject = renderedContent.Subject,
            Body = renderedContent.Body,
            IsHtml = true
        };

        await SendEmailAsync(message, cancellationToken);
    }

    private async Task<EmailTemplate> LoadTemplateAsync(string templateName, CancellationToken cancellationToken)
    {
        // Load email template from file system or database
        var templatePath = Path.Combine(_configuration.TemplatesPath, $"{templateName}.html");
        
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Email template not found: {templateName}");
        }

        var content = await File.ReadAllTextAsync(templatePath, cancellationToken);
        
        return new EmailTemplate
        {
            Name = templateName,
            Subject = ExtractSubjectFromTemplate(content),
            Body = content
        };
    }

    private RenderedTemplate RenderTemplate<T>(EmailTemplate template, T model)
    {
        // Simple template rendering - in production, use a proper template engine like Handlebars.NET
        var renderedBody = template.Body;
        var renderedSubject = template.Subject;

        // Replace placeholders with model properties
        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(model)?.ToString() ?? string.Empty;
            var placeholder = $"{{{{{property.Name}}}}}";
            
            renderedBody = renderedBody.Replace(placeholder, value);
            renderedSubject = renderedSubject.Replace(placeholder, value);
        }

        return new RenderedTemplate
        {
            Subject = renderedSubject,
            Body = renderedBody
        };
    }

    private string ExtractSubjectFromTemplate(string content)
    {
        // Extract subject from template header
        var match = Regex.Match(content, @"<!--\s*Subject:\s*(.+?)\s*-->", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : "No Subject";
    }
}

// Example: HTTP Client Service with Resilience
public class UserProfileHttpClient : IUserProfileHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserProfileHttpClient> _logger;
    private readonly UserProfileClientConfiguration _configuration;

    public UserProfileHttpClient(
        HttpClient httpClient,
        ILogger<UserProfileHttpClient> logger,
        IOptions<UserProfileClientConfiguration> configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration.Value;
    }

    public async Task<UserProfile?> GetUserProfileAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting user profile for: {UserId}", userId);

            var response = await _httpClient.GetAsync($"/api/profiles/{userId.Value}", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("User profile not found: {UserId}", userId);
                return null;
            }

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var profile = JsonSerializer.Deserialize<UserProfile>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _logger.LogDebug("User profile retrieved successfully: {UserId}", userId);
            return profile;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error getting user profile: {UserId}", userId);
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout getting user profile: {UserId}", userId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting user profile: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> UpdateUserProfileAsync(UserId userId, UpdateUserProfileRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating user profile for: {UserId}", userId);

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/api/profiles/{userId.Value}", content, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("User profile not found for update: {UserId}", userId);
                return false;
            }

            response.EnsureSuccessStatusCode();
            
            _logger.LogDebug("User profile updated successfully: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile: {UserId}", userId);
            throw;
        }
    }
}
```

### 6. Background Services and Job Processing
Implement background processing:

```csharp
// Example: Outbox Processor Background Service
public class OutboxProcessorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorBackgroundService> _logger;
    private readonly OutboxConfiguration _configuration;

    public OutboxProcessorBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorBackgroundService> logger,
        IOptions<OutboxConfiguration> configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox processor background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
                await Task.Delay(_configuration.ProcessingIntervalSeconds * 1000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Outbox processor background service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
                await Task.Delay(5000, stoppingToken); // Wait 5 seconds before retrying
            }
        }

        _logger.LogInformation("Outbox processor background service stopped");
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var messages = await outboxService.GetUnprocessedMessagesAsync(_configuration.BatchSize, cancellationToken);

        if (!messages.Any())
        {
            return;
        }

        _logger.LogDebug("Processing {Count} outbox messages", messages.Count());

        foreach (var message in messages)
        {
            try
            {
                // Deserialize and publish the event
                var eventType = Type.GetType(message.EventType);
                if (eventType == null)
                {
                    _logger.LogError("Unknown event type: {EventType}", message.EventType);
                    await outboxService.MarkAsFailedAsync(message.Id, $"Unknown event type: {message.EventType}", cancellationToken);
                    continue;
                }

                var eventData = JsonSerializer.Deserialize(message.EventData, eventType);
                if (eventData == null)
                {
                    _logger.LogError("Failed to deserialize event: {EventId}", message.Id);
                    await outboxService.MarkAsFailedAsync(message.Id, "Failed to deserialize event", cancellationToken);
                    continue;
                }

                // Publish the event
                await eventBus.PublishAsync((IIntegrationEvent)eventData, cancellationToken);

                // Mark as processed
                await outboxService.MarkAsProcessedAsync(message.Id, cancellationToken);

                _logger.LogDebug("Outbox message processed successfully: {EventId}", message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process outbox message: {EventId}", message.Id);
                await outboxService.MarkAsFailedAsync(message.Id, ex.Message, cancellationToken);
            }
        }
    }
}

// Example: Domain Event Processing Background Service
public class DomainEventProcessorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventProcessorBackgroundService> _logger;
    private readonly Channel<IDomainEvent> _domainEventChannel;

    public DomainEventProcessorBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<DomainEventProcessorBackgroundService> logger,
        Channel<IDomainEvent> domainEventChannel)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _domainEventChannel = domainEventChannel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Domain event processor background service started");

        await foreach (var domainEvent in _domainEventChannel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessDomainEventAsync(domainEvent, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing domain event: {EventType} {EventId}", 
                    domainEvent.GetType().Name, domainEvent.Id);
            }
        }

        _logger.LogInformation("Domain event processor background service stopped");
    }

    private async Task ProcessDomainEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        _logger.LogDebug("Processing domain event: {EventType} {EventId}", 
            domainEvent.GetType().Name, domainEvent.Id);

        await mediator.PublishAsync(domainEvent, cancellationToken);

        _logger.LogDebug("Domain event processed successfully: {EventType} {EventId}", 
            domainEvent.GetType().Name, domainEvent.Id);
    }
}
```

### 7. Health Checks
Implement comprehensive health checks:

```csharp
// Example: Service-specific Health Checks
public class AuthHealthChecks : IHealthCheck
{
    private readonly AuthDbContext _dbContext;
    private readonly ICacheService _cacheService;
    private readonly IUserProfileHttpClient _userProfileClient;
    private readonly ILogger<AuthHealthChecks> _logger;

    public AuthHealthChecks(
        AuthDbContext dbContext,
        ICacheService cacheService,
        IUserProfileHttpClient userProfileClient,
        ILogger<AuthHealthChecks> logger)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
        _userProfileClient = userProfileClient;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, object>();
        var isHealthy = true;
        var errors = new List<string>();

        // Check database connectivity
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            results.Add("database", "healthy");
            _logger.LogDebug("Database health check passed");
        }
        catch (Exception ex)
        {
            isHealthy = false;
            errors.Add($"Database: {ex.Message}");
            results.Add("database", "unhealthy");
            _logger.LogError(ex, "Database health check failed");
        }

        // Check cache connectivity
        try
        {
            var testKey = "health_check_" + Guid.NewGuid();
            await _cacheService.SetAsync(testKey, "test", TimeSpan.FromMinutes(1), cancellationToken);
            var cachedValue = await _cacheService.GetAsync<string>(testKey, cancellationToken);
            await _cacheService.RemoveAsync(testKey, cancellationToken);

            if (cachedValue == "test")
            {
                results.Add("cache", "healthy");
                _logger.LogDebug("Cache health check passed");
            }
            else
            {
                isHealthy = false;
                errors.Add("Cache: Failed to retrieve test value");
                results.Add("cache", "unhealthy");
            }
        }
        catch (Exception ex)
        {
            isHealthy = false;
            errors.Add($"Cache: {ex.Message}");
            results.Add("cache", "unhealthy");
            _logger.LogError(ex, "Cache health check failed");
        }

        // Check external services
        try
        {
            // This is a simplified check - you might want to have a dedicated health endpoint
            var testUserId = UserId.New();
            var profile = await _userProfileClient.GetUserProfileAsync(testUserId, cancellationToken);
            // If no exception is thrown, the service is reachable
            results.Add("user_profile_service", "healthy");
            _logger.LogDebug("User profile service health check passed");
        }
        catch (Exception ex)
        {
            // External service failures might not be critical for the main service
            results.Add("user_profile_service", "degraded");
            _logger.LogWarning(ex, "User profile service health check failed");
        }

        var status = isHealthy ? HealthStatus.Healthy : HealthStatus.Unhealthy;
        var description = isHealthy ? "All systems operational" : string.Join("; ", errors);

        return new HealthCheckResult(status, description, null, results);
    }
}

// Example: Database Health Check
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AuthDbContext _dbContext;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(AuthDbContext dbContext, ILogger<DatabaseHealthCheck> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
            
            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy("Cannot connect to database");
            }

            // Test a simple query
            var userCount = await _dbContext.Users.CountAsync(cancellationToken);
            
            var data = new Dictionary<string, object>
            {
                { "user_count", userCount },
                { "connection_string", _dbContext.Database.GetConnectionString()?.Split(';')[0] ?? "unknown" }
            };

            _logger.LogDebug("Database health check passed. User count: {UserCount}", userCount);
            return HealthCheckResult.Healthy("Database is healthy", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy($"Database health check failed: {ex.Message}");
        }
    }
}
```

### 8. Service Registration and Configuration
Configure all infrastructure services:

```csharp
// Example: Main Infrastructure Service Registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add BuildingBlocks Infrastructure
        services.AddInfrastructure(configuration);

        // Configure and add database
        services.AddDatabase(configuration);

        // Configure and add caching
        services.AddCaching(configuration);

        // Configure and add authentication
        services.AddInfrastructureAuthentication(configuration);

        // Configure and add messaging
        services.AddMessaging(configuration);

        // Configure and add external services
        services.AddExternalServices(configuration);

        // Configure and add background services
        services.AddBackgroundServices(configuration);

        // Configure and add monitoring
        services.AddMonitoring(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Add database context
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(30);
                });

            // Add interceptors
            options.AddInterceptors(
                services.BuildServiceProvider().GetRequiredService<AuditInterceptor>(),
                services.BuildServiceProvider().GetRequiredService<DomainEventInterceptor>(),
                services.BuildServiceProvider().GetRequiredService<SoftDeleteInterceptor>());
        });

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();

        // Register interceptors
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<DomainEventInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();

        // Register data seeder
        services.AddScoped<IAuthDataSeeder, AuthDataSeeder>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("Caching:Enabled", true))
        {
            return services;
        }

        // Configure cache settings
        services.Configure<AuthCacheConfiguration>(configuration.GetSection("Caching"));

        // Add memory cache
        services.AddMemoryCache();

        // Add distributed cache (Redis)
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "AuthService";
            });
        }
        else
        {
            // Fallback to SQL Server distributed cache
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                options.SchemaName = "dbo";
                options.TableName = "DistributedCache";
            });
        }

        // Register cache service
        services.AddScoped<ICacheService, AuthCacheService>();

        return services;
    }

    private static IServiceCollection AddInfrastructureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure JWT settings
        services.Configure<JwtConfiguration>(configuration.GetSection("Jwt"));

        // Register authentication services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        // Configure JWT authentication
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtConfiguration>();
        if (jwtSettings != null)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                            logger.LogError(context.Exception, "JWT authentication failed");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                            logger.LogDebug("JWT token validated for user: {UserId}", context.Principal?.Identity?.Name);
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        return services;
    }

    private static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure HTTP clients
        services.AddHttpClient<IUserProfileHttpClient, UserProfileHttpClient>(client =>
        {
            var baseUrl = configuration.GetValue<string>("ExternalServices:UserProfile:BaseUrl");
            if (!string.IsNullOrEmpty(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl);
            }
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // Configure email service
        services.Configure<SmtpConfiguration>(configuration.GetSection("Email:Smtp"));
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure background service settings
        services.Configure<OutboxConfiguration>(configuration.GetSection("Outbox"));

        // Register domain event channel
        services.AddSingleton(provider =>
        {
            var options = new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = false,
                SingleWriter = false
            };
            return Channel.CreateBounded<IDomainEvent>(options);
        });

        // Register background services
        services.AddHostedService<OutboxProcessorBackgroundService>();
        services.AddHostedService<DomainEventProcessorBackgroundService>();

        return services;
    }

    private static IServiceCollection AddMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        // Add health checks
        services.AddHealthChecks()
            .AddCheck<AuthHealthChecks>("auth_service")
            .AddCheck<DatabaseHealthCheck>("database")
            .AddDbContextCheck<AuthDbContext>("auth_database");

        // Configure logging
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            builder.AddConsole();
            builder.AddDebug();
        });

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var logger = context.GetLogger();
                    logger?.LogWarning("Retry {RetryCount} after {Delay}ms", retryCount, timespan.TotalMilliseconds);
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, timespan) =>
                {
                    // Log circuit breaker opened
                },
                onReset: () =>
                {
                    // Log circuit breaker closed
                });
    }
}
```

## Key Principles

### 1. **Feature-Based Configuration**
- **Enable only what you need** via MSBuild properties
- **Zero unused dependencies** in final build
- **Consistent package versions** across services
- **Centralized configuration** via build system

### 2. **Clean Architecture Compliance**
- **Infrastructure depends on Application** (which depends on Domain)
- **External concerns** isolated in infrastructure
- **Interfaces defined in Application** layer
- **Concrete implementations** in Infrastructure layer

### 3. **Resilience and Reliability**
- **Retry policies** for external calls
- **Circuit breakers** for failing services
- **Health checks** for all dependencies
- **Background processing** for reliable operations

### 4. **Performance and Scalability**
- **Multi-level caching** (memory + distributed)
- **Async/await** throughout
- **Connection pooling** for databases
- **Bulk processing** for background tasks

### 5. **Observability**
- **Structured logging** with correlation IDs
- **Health checks** for monitoring
- **Metrics collection** for performance
- **Distributed tracing** for request flow

## Benefits of This Structure

### 1. **Automatic BuildingBlocks Integration**
- **Zero configuration** - leverages centralized build system
- **Feature flags** control package inclusion
- **Consistent patterns** across all infrastructure concerns
- **Type safety** with strongly-typed configurations

### 2. **Production-Ready Infrastructure**
- **Resilience patterns** built-in
- **Monitoring and observability** integrated
- **Security best practices** implemented
- **Performance optimizations** included

### 3. **Maintainable and Testable**
- **Clear separation** of concerns
- **Dependency injection** throughout
- **Interface-based** external dependencies
- **Comprehensive logging** for debugging

### 4. **Scalable Architecture**
- **Background processing** for heavy operations
- **Caching strategies** for performance
- **Message-driven** architecture support
- **Health monitoring** for reliability

Generate an infrastructure layer following these patterns and principles, ensuring robust external integrations, reliable data access, comprehensive monitoring, and seamless integration with the BuildingBlocks architecture. 