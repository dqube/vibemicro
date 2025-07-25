# BuildingBlocks.Infrastructure Library Generation Prompt

## Overview
Generate a comprehensive Infrastructure Layer library implementing external concerns and concrete implementations for .NET 8.0 microservices following Clean Architecture and DDD principles.

## Project Configuration

### Basic Setup
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>

  <!-- Layer References -->
  <ItemGroup>
    <ProjectReference Include="..\BuildingBlocks.Domain\BuildingBlocks.Domain.csproj" />
    <ProjectReference Include="..\BuildingBlocks.Application\BuildingBlocks.Application.csproj" />
  </ItemGroup>

  <!-- Infrastructure Dependencies -->
  <ItemGroup>
    <!-- Entity Framework Core -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
    
    <!-- Caching -->
    <PackageReference Include="Microsoft.Extensions.Caching.Memory" />
    <PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" />
    <PackageReference Include="Microsoft.Extensions.Caching.SqlServer" />
    
    <!-- Configuration -->
    <PackageReference Include="Microsoft.Extensions.Configuration" />
    <PackageReference Include="Microsoft.Extensions.Options.ConfigurationExtensions" />
    
    <!-- Logging -->
    <PackageReference Include="Microsoft.Extensions.Logging" />
    <PackageReference Include="Serilog.Extensions.Hosting" />
    <PackageReference Include="Serilog.Sinks.Console" />
    <PackageReference Include="Serilog.Sinks.File" />
    
    <!-- Messaging -->
    <PackageReference Include="Azure.ServiceBus" />
    <PackageReference Include="RabbitMQ.Client" />
    
    <!-- Authentication -->
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
    
    <!-- Storage -->
    <PackageReference Include="Azure.Storage.Blobs" />
    <PackageReference Include="Azure.Storage.Files.Shares" />
    
    <!-- Communication -->
    <PackageReference Include="MailKit" />
    <PackageReference Include="SendGrid" />
    
    <!-- Monitoring -->
    <PackageReference Include="OpenTelemetry" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" />
    <PackageReference Include="ApplicationInsights.AspNetCore" />
    
    <!-- Background Jobs -->
    <PackageReference Include="Hangfire.Core" />
    <PackageReference Include="Hangfire.SqlServer" />
    <PackageReference Include="Quartz" />
    
    <!-- HTTP Clients -->
    <PackageReference Include="Microsoft.Extensions.Http" />
    <PackageReference Include="Polly" />
    
    <!-- Security -->
    <PackageReference Include="Microsoft.AspNetCore.DataProtection" />
    <PackageReference Include="System.Security.Cryptography" />
    
    <!-- Validation -->
    <PackageReference Include="FluentValidation" />
    
    <!-- Serialization -->
    <PackageReference Include="System.Text.Json" />
    <PackageReference Include="Newtonsoft.Json" />
  </ItemGroup>
</Project>
```

## Folder Structure

```
BuildingBlocks.Infrastructure/
├── Data/                        # Data access implementations
│   ├── Context/                 # Database contexts
│   │   ├── IDbContext.cs       # Context interface
│   │   ├── ApplicationDbContext.cs # Main application context
│   │   ├── DbContextBase.cs    # Base context class
│   │   └── IDbContextFactory.cs # Context factory interface
│   ├── Repositories/            # Repository implementations
│   │   ├── Repository.cs       # Generic repository
│   │   ├── ReadOnlyRepository.cs # Read-only repository
│   │   └── RepositoryBase.cs   # Base repository
│   ├── UnitOfWork/              # Transaction management
│   │   └── UnitOfWork.cs       # Unit of work implementation
│   ├── Migrations/              # Database migrations
│   │   ├── IMigrationRunner.cs # Migration runner interface
│   │   └── MigrationRunner.cs  # Migration runner implementation
│   ├── Seeding/                 # Data seeding
│   │   ├── IDataSeeder.cs      # Data seeder interface
│   │   ├── DataSeederBase.cs   # Base data seeder
│   │   └── SeedDataExtensions.cs # Seeding extensions
│   ├── Interceptors/            # EF Core interceptors
│   │   ├── AuditInterceptor.cs # Audit trail interceptor
│   │   ├── DomainEventInterceptor.cs # Domain event interceptor
│   │   └── SoftDeleteInterceptor.cs # Soft delete interceptor
│   └── Configurations/          # Entity configurations
│       ├── EntityConfigurationBase.cs # Base configuration
│       ├── AuditableEntityConfiguration.cs # Auditable entities
│       └── ValueObjectConfiguration.cs # Value object configs
├── Caching/                     # Caching implementations
│   ├── ICacheService.cs        # Cache service interface
│   ├── MemoryCacheService.cs   # In-memory cache
│   ├── DistributedCacheService.cs # Distributed cache
│   ├── RedisCacheService.cs    # Redis cache
│   ├── CacheKeyGenerator.cs    # Cache key generation
│   └── CacheConfiguration.cs   # Cache configuration
├── Messaging/                   # Message bus implementations
│   ├── MessageBus/              # Message bus providers
│   │   ├── IMessageBus.cs      # Message bus interface
│   │   ├── InMemoryMessageBus.cs # In-memory implementation
│   │   ├── ServiceBusMessageBus.cs # Azure Service Bus
│   │   └── RabbitMQMessageBus.cs # RabbitMQ implementation
│   ├── EventBus/                # Event bus providers
│   │   ├── IEventBus.cs        # Event bus interface
│   │   ├── InMemoryEventBus.cs # In-memory event bus
│   │   ├── ServiceBusEventBus.cs # Azure Service Bus events
│   │   └── RabbitMQEventBus.cs # RabbitMQ events
│   ├── Publishers/              # Message publishers
│   │   ├── IMessagePublisher.cs # Publisher interface
│   │   ├── MessagePublisherBase.cs # Base publisher
│   │   ├── ServiceBusPublisher.cs # Service Bus publisher
│   │   └── RabbitMQPublisher.cs # RabbitMQ publisher
│   ├── Subscribers/             # Message subscribers
│   │   ├── IMessageSubscriber.cs # Subscriber interface
│   │   ├── MessageSubscriberBase.cs # Base subscriber
│   │   ├── ServiceBusSubscriber.cs # Service Bus subscriber
│   │   └── RabbitMQSubscriber.cs # RabbitMQ subscriber
│   ├── Serialization/           # Message serialization
│   │   ├── IMessageSerializer.cs # Serializer interface
│   │   ├── JsonMessageSerializer.cs # JSON serializer
│   │   └── BinaryMessageSerializer.cs # Binary serializer
│   └── Configuration/           # Messaging configuration
│       ├── MessageBusConfiguration.cs # Bus configuration
│       ├── ServiceBusConfiguration.cs # Service Bus config
│       └── RabbitMQConfiguration.cs # RabbitMQ config
├── Logging/                     # Logging implementations
│   ├── ILoggerService.cs       # Logger service interface
│   ├── LoggerService.cs        # Logger service implementation
│   ├── OpenTelemetry/          # OpenTelemetry integration
│   ├── Structured/             # Structured logging
│   └── [Additional logging components]
├── Authentication/              # Authentication providers
│   ├── JWT/                    # JWT authentication
│   ├── OAuth/                  # OAuth authentication
│   ├── ApiKey/                 # API key authentication
│   └── Identity/               # Identity management
├── Authorization/               # Authorization implementations
│   ├── IAuthorizationService.cs # Authorization service
│   ├── AuthorizationService.cs # Authorization implementation
│   ├── Policies/               # Authorization policies
│   ├── Handlers/               # Authorization handlers
│   └── Requirements/           # Authorization requirements
├── Storage/                     # Storage implementations
│   ├── Files/                  # File storage services
│   ├── Blobs/                  # Blob storage services
│   └── Documents/              # Document storage services
├── Communication/               # Communication services
│   ├── Email/                  # Email services
│   ├── SMS/                    # SMS services
│   ├── Push/                   # Push notification services
│   └── Notifications/          # General notification services
├── Monitoring/                  # Monitoring implementations
│   ├── Health/                 # Health check services
│   ├── Metrics/                # Metrics collection
│   ├── Tracing/                # Distributed tracing
│   └── Performance/            # Performance monitoring
├── BackgroundServices/          # Background task processing
│   ├── IBackgroundTaskService.cs # Task service interface
│   ├── BackgroundTaskService.cs # Task service implementation
│   ├── Queues/                 # Background queues
│   ├── Jobs/                   # Job scheduling
│   └── Workers/                # Worker services
├── External/                    # External service integrations
│   ├── HttpClients/            # HTTP client services
│   ├── APIs/                   # External API integrations
│   └── ThirdParty/             # Third-party integrations
├── Security/                    # Security implementations
│   ├── Encryption/             # Encryption services
│   ├── Hashing/                # Hashing services
│   ├── KeyManagement/          # Key management
│   └── Secrets/                # Secrets management
├── Validation/                  # Validation implementations
│   ├── FluentValidation/       # FluentValidation implementation
│   ├── DataAnnotations/        # Data annotations validation
│   └── Custom/                 # Custom validation
├── Serialization/               # Serialization implementations
│   ├── Json/                   # JSON serialization
│   ├── Xml/                    # XML serialization
│   ├── Binary/                 # Binary serialization
│   └── Csv/                    # CSV serialization
├── Idempotency/                 # Idempotency implementations
│   ├── IdempotencyEntity.cs    # Idempotency entity
│   ├── IIdempotencyRepository.cs # Repository interface
│   ├── IdempotencyProcessor.cs # Idempotency processor
│   ├── IdempotencyMiddleware.cs # Idempotency middleware
│   └── IdempotencyConfiguration.cs # Configuration
├── Configuration/               # Configuration management
│   ├── IConfigurationService.cs # Configuration service
│   ├── ConfigurationService.cs # Configuration implementation
│   ├── Settings/               # Application settings
│   ├── Providers/              # Configuration providers
│   └── Validation/             # Configuration validation
└── Extensions/                  # Extension methods
    ├── ServiceCollectionExtensions.cs # DI registration
    ├── ApplicationBuilderExtensions.cs # App builder extensions
    ├── HostBuilderExtensions.cs # Host builder extensions
    └── InfrastructureExtensions.cs # General extensions
```

## Key Implementation Examples

### Data Layer Implementation

**ApplicationDbContext.cs** - Main Database Context:
```csharp
public class ApplicationDbContext : DbContextBase, IDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Configure value objects
        ConfigureValueObjects(modelBuilder);
        
        // Configure strongly-typed IDs
        ConfigureStronglyTypedIds(modelBuilder);
    }

    private void ConfigureValueObjects(ModelBuilder modelBuilder)
    {
        // Configure value objects as owned entities
        // Implementation details...
    }

    private void ConfigureStronglyTypedIds(ModelBuilder modelBuilder)
    {
        // Configure strongly-typed ID conversions
        // Implementation details...
    }
}
```

**Repository.cs** - Generic Repository Implementation:
```csharp
public class Repository<TEntity, TId, TIdValue> : ReadOnlyRepository<TEntity, TId, TIdValue>, IRepository<TEntity, TId, TIdValue>
    where TEntity : Entity<TId, TIdValue>
    where TId : IStronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    public Repository(IDbContext context) : base(context)
    {
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public void Update(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

    public async Task RemoveAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            Remove(entity);
        }
    }
}
```

### Caching Implementation

**RedisCacheService.cs** - Redis Cache Implementation:
```csharp
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly CacheConfiguration _configuration;

    public RedisCacheService(
        IDistributedCache distributedCache,
        ILogger<RedisCacheService> logger,
        CacheConfiguration configuration)
    {
        _distributedCache = distributedCache;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _distributedCache.GetStringAsync(key, cancellationToken);
            
            if (string.IsNullOrEmpty(value))
                return default;

            return JsonSerializer.Deserialize<T>(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions();
            
            if (expiry.HasValue)
            {
                options.SetAbsoluteExpiration(expiry.Value);
            }
            else
            {
                options.SetAbsoluteExpiration(_configuration.DefaultExpiry);
            }

            var serializedValue = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serializedValue, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
        }
    }
}
```

### Messaging Implementation

**InMemoryMessageBus.cs** - In-Memory Message Bus:
```csharp
public class InMemoryMessageBus : IMessageBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InMemoryMessageBus> _logger;
    private readonly Channel<MessageEnvelope> _channel;

    public InMemoryMessageBus(IServiceProvider serviceProvider, ILogger<InMemoryMessageBus> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        var options = new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        };
        
        _channel = Channel.CreateBounded<MessageEnvelope>(options);
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage
    {
        var envelope = new MessageEnvelope
        {
            MessageId = Guid.NewGuid(),
            MessageType = typeof(T).Name,
            Payload = JsonSerializer.Serialize(message),
            Timestamp = DateTime.UtcNow
        };

        await _channel.Writer.WriteAsync(envelope, cancellationToken);
        _logger.LogDebug("Published message {MessageId} of type {MessageType}", envelope.MessageId, envelope.MessageType);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _ = Task.Run(async () =>
        {
            await foreach (var envelope in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                await ProcessMessageAsync(envelope, cancellationToken);
            }
        }, cancellationToken);
    }

    private async Task ProcessMessageAsync(MessageEnvelope envelope, CancellationToken cancellationToken)
    {
        try
        {
            // Process message using registered handlers
            // Implementation details...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message {MessageId}", envelope.MessageId);
        }
    }
}
```

### Authentication Implementation

**JwtAuthenticationService.cs** - JWT Authentication:
```csharp
public class JwtAuthenticationService : IAuthenticationService
{
    private readonly JwtConfiguration _configuration;
    private readonly ILogger<JwtAuthenticationService> _logger;

    public JwtAuthenticationService(JwtConfiguration configuration, ILogger<JwtAuthenticationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(ClaimsPrincipal principal)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.SecretKey);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(principal.Claims),
            Expires = DateTime.UtcNow.AddMinutes(_configuration.ExpiryMinutes),
            Issuer = _configuration.Issuer,
            Audience = _configuration.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.SecretKey);
            
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

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return null;
        }
    }
}
```

### Background Services

**BackgroundTaskService.cs** - Background Task Processing:
```csharp
public class BackgroundTaskService : BackgroundService, IBackgroundTaskService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundTaskService> _logger;
    private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _queue;

    public BackgroundTaskService(IServiceProvider serviceProvider, ILogger<BackgroundTaskService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        var options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        
        _queue = Channel.CreateBounded<Func<IServiceProvider, CancellationToken, Task>>(options);
    }

    public async Task QueueBackgroundWorkItemAsync(Func<IServiceProvider, CancellationToken, Task> workItem)
    {
        if (workItem == null)
            throw new ArgumentNullException(nameof(workItem));

        await _queue.Writer.WriteAsync(workItem);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var workItem in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                await workItem(scope.ServiceProvider, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing background work item");
            }
        }
    }
}
```

## Service Registration

### ServiceCollectionExtensions.cs
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add data layer
        services.AddDataLayer(configuration);
        
        // Add caching
        services.AddCaching(configuration);
        
        // Add messaging
        services.AddMessaging(configuration);
        
        // Add authentication
        services.AddInfrastructureAuthentication(configuration);
        
        // Add background services
        services.AddBackgroundServices(configuration);
        
        return services;
    }

    public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<,,>), typeof(Repository<,,>));
        
        return services;
    }

    // Additional registration methods...
}
```

## Key Design Patterns

### 1. Repository Pattern
- **Generic repositories** with strongly-typed IDs
- **Unit of Work** for transaction management
- **Specification pattern** for complex queries
- **Read/Write separation** with different interfaces

### 2. Caching Strategy
- **Multiple providers** (Memory, Redis, SQL Server)
- **Configurable expiration** policies
- **Cache-aside pattern** implementation
- **Error handling** with fallback mechanisms

### 3. Messaging Architecture
- **Provider abstraction** (In-Memory, Service Bus, RabbitMQ)
- **Message serialization** with multiple formats
- **Reliable delivery** with retry mechanisms
- **Dead letter queues** for failed messages

### 4. Authentication & Authorization
- **Multiple auth providers** (JWT, OAuth, API Key)
- **Claims-based authorization**
- **Policy-based permissions**
- **Secure token handling**

### 5. Monitoring & Observability
- **Health checks** for all dependencies
- **Distributed tracing** with OpenTelemetry
- **Structured logging** with correlation IDs
- **Metrics collection** for performance monitoring

This Infrastructure Layer provides concrete implementations for all external concerns while maintaining clean interfaces and supporting multiple provider options for maximum flexibility. 