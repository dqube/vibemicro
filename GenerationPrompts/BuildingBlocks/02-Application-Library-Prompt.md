# BuildingBlocks.Application Library Generation Prompt

## Overview
Generate a comprehensive Application layer library implementing Clean Architecture principles with CQRS (Command Query Responsibility Segregation), Mediator pattern, and cross-cutting concerns through pipeline behaviors. This library orchestrates domain logic and provides use case implementations for microservices.

## Project Configuration

### Target Framework & Features
- **.NET 8.0** (`net8.0`)
- **Implicit Usings**: Enabled
- **Nullable Reference Types**: Enabled
- **Treat Warnings as Errors**: Enabled
- **Generate Documentation File**: Enabled

### Package Dependencies
```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Caching.Abstractions" Version="8.0.0" />
<PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
```

### Project References
```xml
<ProjectReference Include="../BuildingBlocks.Domain/BuildingBlocks.Domain.csproj" />
```

## Architecture & Patterns

### Core Application Concepts
1. **CQRS**: Separate models for commands (write) and queries (read)
2. **Mediator Pattern**: Decoupled communication between application layers
3. **Pipeline Behaviors**: Cross-cutting concerns (logging, validation, caching, etc.)
4. **Application Services**: Orchestrate domain operations
5. **DTOs**: Data transfer objects for layer boundaries
6. **Mapping**: Transform between domain and application models

### Access Modifier Strategy
- **Public**: Interfaces and abstractions used by API/Infrastructure layers
- **Internal**: Implementation details specific to application layer

## Folder Structure & Components

### `/CQRS` - Command Query Responsibility Segregation
```
CQRS/
├── Commands/
│   ├── ICommand.cs              # Command marker interface
│   ├── ICommandHandler.cs       # Command handler interface
│   └── CommandBase.cs           # Base command implementation
├── Queries/
│   ├── IQuery.cs               # Query interface
│   ├── IQueryHandler.cs        # Query handler interface
│   ├── QueryBase.cs            # Base query implementation
│   ├── PagedQuery.cs           # Pagination support
│   ├── PagedResult.cs          # Paginated results
│   └── SortingQuery.cs         # Sorting support
├── Events/
│   ├── IEvent.cs               # Event marker interface
│   ├── IEventHandler.cs        # Event handler interface
│   ├── DomainEventNotification.cs # Domain event wrapper
│   ├── IIntegrationEvent.cs    # Integration event interface
│   └── IntegrationEventBase.cs # Base integration event
├── Messages/
│   ├── IMessage.cs             # Base message interface
│   ├── IMessageContext.cs      # Message context interface
│   ├── IStreamMessage.cs       # Streaming message interface
│   └── MessageBase.cs          # Base message implementation
└── Mediator/
    ├── IMediator.cs            # Mediator interface
    └── Mediator.cs             # Mediator implementation
```

**ICommand.cs** - Command Interface:
```csharp
public interface ICommand : IMessage
{
}

public interface ICommand<out TResponse> : IMessage<TResponse>
{
}
```

**IQuery.cs** - Query Interface:
```csharp
public interface IQuery<out TResponse> : IMessage<TResponse>
{
}
```

**PagedQuery.cs** - Pagination Support:
```csharp
public abstract record PagedQuery : QueryBase
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int Skip => (PageNumber - 1) * PageSize;
    
    protected PagedQuery()
    {
        if (PageNumber < 1) PageNumber = 1;
        if (PageSize < 1) PageSize = 10;
        if (PageSize > 100) PageSize = 100;
    }
}
```

**PagedResult.cs** - Paginated Results:
```csharp
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    
    public static PagedResult<T> Empty => new(Array.Empty<T>(), 0, 1, 10);
    
    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
        => new(items, totalCount, pageNumber, pageSize);
}
```

### `/Behaviors` - Pipeline Cross-Cutting Concerns
```
Behaviors/
├── IPipelineBehavior.cs        # Behavior interface
├── LoggingBehavior.cs          # Request/Response logging
├── ValidationBehavior.cs       # Input validation
├── CachingBehavior.cs          # Response caching
├── PerformanceBehavior.cs      # Performance monitoring
├── RetryBehavior.cs            # Retry policies
└── TransactionBehavior.cs      # Database transactions
```

**IPipelineBehavior.cs** - Behavior Interface:
```csharp
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IMessage<TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
```

**LoggingBehavior.cs** - Request Logging:
```csharp
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid();
        
        _logger.LogInformation("Starting request {RequestName} with ID {RequestId}", requestName, requestId);
        
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next();
            stopwatch.Stop();
            
            _logger.LogInformation("Completed request {RequestName} with ID {RequestId} in {ElapsedMs}ms", 
                requestName, requestId, stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Request {RequestName} with ID {RequestId} failed after {ElapsedMs}ms", 
                requestName, requestId, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

**ValidationBehavior.cs** - Input Validation:
```csharp
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();
        
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
        
        if (failures.Any())
            throw new ValidationException(failures);
        
        return await next();
    }
}
```

**CachingBehavior.cs** - Response Caching:
```csharp
public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage<TResponse>, ICacheableQuery
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.BypassCache) return await next();
        
        var cacheKey = request.CacheKey;
        var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
        
        if (cachedResponse != null)
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return cachedResponse;
        }
        
        var response = await next();
        await _cacheService.SetAsync(cacheKey, response, request.CacheDuration, cancellationToken);
        
        return response;
    }
}
```

### `/Services` - Application Orchestration
```
Services/
├── IApplicationService.cs      # Base service interface
├── ApplicationServiceBase.cs   # Base service implementation
├── IServiceContext.cs          # Service context interface
├── ServiceContext.cs           # Service context implementation
├── IDomainEventService.cs      # Domain event service interface
└── DomainEventService.cs       # Domain event service implementation
```

**IApplicationService.cs** - Service Interface:
```csharp
public interface IApplicationService
{
    IServiceContext Context { get; }
}
```

**ApplicationServiceBase.cs** - Base Service:
```csharp
public abstract class ApplicationServiceBase : IApplicationService
{
    protected ApplicationServiceBase(IServiceContext context)
        => Context = context;
    
    public IServiceContext Context { get; }
    
    protected async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (DomainException)
        {
            throw; // Re-throw domain exceptions as-is
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An application error occurred", ex);
        }
    }
}
```

### `/DTOs` - Data Transfer Objects
```
DTOs/
└── BaseDto.cs                  # Base DTO with common properties
```

**BaseDto.cs** - Base DTO:
```csharp
public abstract record BaseDto
{
    public string? CreatedBy { get; init; }
    public DateTime? CreatedOn { get; init; }
    public string? ModifiedBy { get; init; }
    public DateTime? ModifiedOn { get; init; }
}
```

### `/Mapping` - Object Mapping
```
Mapping/
└── IMapper.cs                  # Mapping interface
```

**IMapper.cs** - Mapping Interface:
```csharp
public interface IMapper
{
    TDestination Map<TDestination>(object source);
    TDestination Map<TSource, TDestination>(TSource source);
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
}
```

### `/Validation` - Input Validation
```
Validation/
└── IValidator.cs               # Validator interface
```

**IValidator.cs** - Validator Interface:
```csharp
public interface IValidator<in T>
{
    Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellationToken = default);
}

public sealed record ValidationResult(IEnumerable<ValidationFailure> Errors)
{
    public bool IsValid => !Errors.Any();
    public static ValidationResult Success => new(Array.Empty<ValidationFailure>());
}

public sealed record ValidationFailure(string PropertyName, string ErrorMessage, object? AttemptedValue = null);
```

### `/Caching` - Caching Abstractions
```
Caching/
└── ICacheService.cs            # Cache service interface
```

**ICacheService.cs** - Cache Interface:
```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
}

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan CacheDuration { get; }
    bool BypassCache { get; }
}
```

### `/Messaging` - Message Bus
```
Messaging/
└── IMessageBus.cs              # Message bus interface
```

**IMessageBus.cs** - Message Bus Interface:
```csharp
public interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IIntegrationEvent;
    Task SendAsync<T>(T command, CancellationToken cancellationToken = default) where T : ICommand;
    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
```

### `/Security` - Security Context
```
Security/
└── ICurrentUser.cs             # Current user interface
```

**ICurrentUser.cs** - Current User Interface:
```csharp
public interface ICurrentUser
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    IDictionary<string, object> Claims { get; }
}
```

### `/Extensions` - Application Extensions
```
Extensions/
├── ServiceCollectionExtensions.cs # DI registration
└── QueryableExtensions.cs         # Query extensions
```

**ServiceCollectionExtensions.cs** - DI Registration:
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();
        
        // Register all handlers
        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        // Register behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        return services;
    }
}
```

**QueryableExtensions.cs** - Query Extensions:
```csharp
public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return PagedResult<T>.Create(items, totalCount, pageNumber, pageSize);
    }
    
    public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification.Criteria != null)
            query = query.Where(specification.Criteria);
        
        query = specification.Includes
            .Aggregate(query, (current, include) => current.Include(include));
        
        if (specification.OrderBy != null)
            query = query.OrderBy(specification.OrderBy);
        else if (specification.OrderByDescending != null)
            query = query.OrderByDescending(specification.OrderByDescending);
        
        return query;
    }
}
```

## Key Design Principles

### 1. CQRS Separation
- Clear separation between commands (write) and queries (read)
- Different models and validation for different operations
- Optimized for specific use cases

### 2. Mediator Pattern
- Decoupled communication between layers
- Single point of entry for requests
- Easy to add cross-cutting concerns

### 3. Pipeline Behaviors
- Cross-cutting concerns as composable behaviors
- Consistent application of concerns (logging, validation, etc.)
- Easy to add/remove behaviors

### 4. Domain Event Integration
- Bridge between domain events and integration events
- Consistent event handling patterns
- Support for eventual consistency

### 5. Modern C# Features
- Records for immutable DTOs
- Pattern matching for robust error handling
- Async/await throughout
- Nullable reference types

## Usage Examples

### Command Definition
```csharp
public sealed record CreateUserCommand(
    string Username,
    string Email,
    string Password
) : CommandBase<CreateUserResult>;

public sealed record CreateUserResult(
    Guid UserId,
    bool IsSuccess,
    string? ErrorMessage = null
);
```

### Query Definition
```csharp
public sealed record GetUsersQuery(
    string? SearchTerm = null,
    bool? IsActive = null
) : PagedQuery, IQuery<PagedResult<UserDto>>;
```

### Command Handler
```csharp
public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Business logic implementation
        var user = User.Create(request.Username, request.Email, request.Password);
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new CreateUserResult(user.Id.Value, true);
    }
}
```

### Query Handler with Caching
```csharp
public sealed record GetUsersQuery : PagedQuery, IQuery<PagedResult<UserDto>>, ICacheableQuery
{
    public string CacheKey => $"users-{PageNumber}-{PageSize}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
    public bool BypassCache => false;
}
```

## Implementation Notes

1. **Dependency Injection**: Register all handlers and behaviors automatically
2. **Error Handling**: Use result patterns for business errors
3. **Validation**: Implement comprehensive input validation
4. **Caching**: Cache expensive queries with appropriate invalidation
5. **Performance**: Monitor and log performance metrics
6. **Testing**: Create integration tests for complete request pipelines

Generate this library with full implementations, comprehensive XML documentation, and adherence to these architectural principles. 