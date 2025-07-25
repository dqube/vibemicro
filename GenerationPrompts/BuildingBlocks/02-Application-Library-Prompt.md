# BuildingBlocks.Application Library Generation Prompt

## Overview
Generate a comprehensive Application Layer library following Clean Architecture, Domain-Driven Design (DDD), and CQRS patterns for .NET 8.0 microservices.

## Project Configuration

### Simplified Project File
```xml
<Project Sdk="Microsoft.NET.Sdk"/>
```

**That's it!** All configuration is handled automatically by the centralized build system:

- **Target Framework**: .NET 8.0 (from `Directory.Build.props`)
- **Language Features**: Implicit usings, nullable reference types, warnings as errors
- **Documentation**: XML documentation generation enabled
- **Project References**: `BuildingBlocks.Domain` automatically referenced
- **Package Management**: All packages automatically included via `Directory.Build.targets`
- **Versioning**: Automatic versioning with Git integration
- **Analysis**: Code quality rules from `BuildingBlocks.ruleset`

### Automatic Package Inclusion
The following packages are automatically included via the centralized build system:
- **Core packages**: Microsoft Extensions (DI, Logging, Hosting, Caching abstractions)
- **Application-specific**: `Scrutor` for assembly scanning
- **Optional features** (can be enabled via feature flags):
  - `FluentValidation` and extensions (if `IncludeValidation` is true)
  - `MediatR` and extensions (if `IncludeMediatR` is true)

### Feature Control
Application projects can enable additional features:
```xml
<!-- Optional in individual .csproj if needed -->
<PropertyGroup>
  <IncludeValidation>true</IncludeValidation>        <!-- Adds FluentValidation -->
  <IncludeMediatR>true</IncludeMediatR>              <!-- Adds MediatR (if using external mediator) -->
  <IncludeMonitoring>true</IncludeMonitoring>        <!-- Adds OpenTelemetry, Health Checks -->
  <IncludeMessaging>true</IncludeMessaging>          <!-- Adds messaging abstractions -->
</PropertyGroup>
```

## Architecture Principles

### Clean Architecture
- **Application Services**: Orchestrate domain operations
- **Use Cases**: Business operations implementations
- **Interfaces**: Define contracts for infrastructure
- **DTOs**: Data transfer between layers
- **Behaviors**: Cross-cutting concerns

### CQRS Implementation
- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations (Get, List, Search)
- **Handlers**: Command and Query processors
- **Events**: Domain and Integration events
- **Mediator**: Request/Response coordination

### Key Patterns
- **Custom Mediator** (not MediatR) for request handling
- **Pipeline Behaviors** for cross-cutting concerns
- **Repository Pattern** abstractions
- **Unit of Work** for transaction management
- **Outbox/Inbox** patterns for reliable messaging

## Folder Structure

```
BuildingBlocks.Application/
├── Behaviors/                    # Pipeline behaviors
│   ├── IPipelineBehavior.cs     # Behavior interface
│   ├── LoggingBehavior.cs       # Request/response logging
│   ├── ValidationBehavior.cs    # Input validation
│   ├── CachingBehavior.cs       # Response caching
│   ├── TransactionBehavior.cs   # Database transactions
│   ├── PerformanceBehavior.cs   # Performance monitoring
│   └── RetryBehavior.cs         # Retry policies
├── CQRS/                        # CQRS implementation
│   ├── Commands/                # Command handling
│   │   ├── ICommand.cs         # Command interfaces
│   │   ├── ICommandHandler.cs  # Handler interfaces
│   │   └── CommandBase.cs      # Base command class
│   ├── Queries/                 # Query handling
│   │   ├── IQuery.cs           # Query interfaces
│   │   ├── IQueryHandler.cs    # Handler interfaces
│   │   ├── QueryBase.cs        # Base query class
│   │   ├── PagedQuery.cs       # Pagination support
│   │   ├── PagedResult.cs      # Paged result wrapper
│   │   └── SortingQuery.cs     # Sorting support
│   ├── Events/                  # Event handling
│   │   ├── IEvent.cs           # Event interfaces
│   │   ├── IEventHandler.cs    # Handler interfaces
│   │   ├── IIntegrationEvent.cs # Integration events
│   │   ├── IntegrationEventBase.cs # Base integration event
│   │   └── DomainEventNotification.cs # Domain event wrapper
│   ├── Messages/                # Message abstractions
│   │   ├── IMessage.cs         # Message interface
│   │   ├── IStreamMessage.cs   # Stream message interface
│   │   ├── MessageBase.cs      # Base message class
│   │   └── IMessageContext.cs  # Message context
│   └── Mediator/                # Custom mediator
│       ├── IMediator.cs        # Mediator interface
│       └── Mediator.cs         # Mediator implementation
├── Services/                    # Application services
│   ├── IApplicationService.cs  # Service marker interface
│   ├── ApplicationServiceBase.cs # Base service class
│   ├── IDomainEventService.cs  # Domain event service
│   ├── DomainEventService.cs   # Domain event implementation
│   ├── IServiceContext.cs      # Service context interface
│   ├── ServiceContext.cs       # Service context implementation
│   ├── OutboxBackgroundService.cs # Outbox pattern service
│   └── InboxBackgroundService.cs  # Inbox pattern service
├── DTOs/                        # Data Transfer Objects
│   ├── BaseDto.cs              # Base DTO class
│   ├── AuditableDto.cs         # Auditable DTO
│   └── PagedDto.cs             # Paged DTO wrapper
├── Validation/                  # Input validation
│   ├── IValidator.cs           # Validator interface
│   ├── IValidationRule.cs      # Validation rule interface
│   ├── ValidationResult.cs     # Validation result
│   ├── ValidationError.cs      # Validation error
│   ├── CompositeValidator.cs   # Composite validation
│   └── ValidatorBase.cs        # Base validator class
├── Caching/                     # Application caching
│   ├── ICacheService.cs        # Cache service interface
│   ├── ICacheKey.cs            # Cache key interface
│   ├── CacheKey.cs             # Cache key implementation
│   ├── CacheSettings.cs        # Cache configuration
│   └── CachePolicy.cs          # Cache policies
├── Messaging/                   # Message bus abstractions
│   ├── IMessageBus.cs          # Message bus interface
│   ├── IEventBus.cs            # Event bus interface
│   ├── IMessageHandler.cs      # Message handler interface
│   ├── IMessagePublisher.cs    # Message publisher interface
│   └── MessageMetadata.cs      # Message metadata
├── Security/                    # Security abstractions
│   ├── ICurrentUser.cs         # Current user interface
│   ├── IPermissionService.cs   # Permission service interface
│   ├── UserContext.cs          # User context
│   └── SecurityContext.cs      # Security context
├── Outbox/                      # Outbox pattern
│   ├── IOutboxService.cs       # Outbox service interface
│   ├── OutboxMessage.cs        # Outbox message entity
│   ├── OutboxMessageStatus.cs  # Message status enum
│   ├── IOutboxProcessor.cs     # Outbox processor interface
│   └── OutboxProcessor.cs      # Outbox processor implementation
├── Inbox/                       # Inbox pattern
│   ├── IInboxService.cs        # Inbox service interface
│   ├── InboxMessage.cs         # Inbox message entity
│   ├── InboxMessageStatus.cs   # Message status enum
│   ├── IInboxProcessor.cs      # Inbox processor interface
│   └── InboxProcessor.cs       # Inbox processor implementation
├── Dispatchers/                 # Message dispatching
│   ├── ICommandDispatcher.cs   # Command dispatcher interface
│   ├── CommandDispatcher.cs    # Command dispatcher implementation
│   ├── IQueryDispatcher.cs     # Query dispatcher interface
│   ├── QueryDispatcher.cs      # Query dispatcher implementation
│   ├── IEventDispatcher.cs     # Event dispatcher interface
│   ├── EventDispatcher.cs      # Event dispatcher implementation
│   ├── IMessageDispatcher.cs   # Message dispatcher interface
│   └── MessageDispatcher.cs    # Message dispatcher implementation
├── Sagas/                       # Long-running processes
│   ├── ISaga.cs                # Saga interface
│   ├── SagaBase.cs             # Base saga implementation
│   └── ISagaManager.cs         # Saga manager interface
├── Idempotency/                 # Idempotent operations
│   ├── IIdempotencyService.cs  # Idempotency service interface
│   ├── IdempotencyOptions.cs   # Idempotency configuration
│   └── IdempotencyRecord.cs    # Idempotency record entity
└── Extensions/                  # Extension methods
    ├── ServiceCollectionExtensions.cs # DI registration
    ├── ApplicationExtensions.cs # Application extensions
    ├── MediatorExtensions.cs    # Mediator extensions
    └── QueryableExtensions.cs   # Queryable extensions
```

## Key Components Implementation

### Custom Mediator
**IMediator.cs** - Mediator Interface:
```csharp
public interface IMediator
{
    // Commands without return value
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    // Commands with return value
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;

    // Queries
    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;

    // Events
    Task PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default)
        where TEvent : IEvent;
}
```

### CQRS Commands
**ICommand.cs** - Command Interfaces:
```csharp
// Command without return value
public interface ICommand : IMessage
{
}

// Command with return value
public interface ICommand<TResult> : IMessage
{
}

// Command handler without return value
public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

// Command handler with return value
public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
```

### CQRS Queries
**IQuery.cs** - Query Interfaces:
```csharp
// Query interface
public interface IQuery<TResult> : IMessage
{
}

// Query handler interface
public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
```

### Pipeline Behaviors
**IPipelineBehavior.cs** - Behavior Interface:
```csharp
public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken);
}

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
```

### Application Services
**IApplicationService.cs** - Service Interface:
```csharp
public interface IApplicationService
{
    // Marker interface for application services
}

public abstract class ApplicationServiceBase : IApplicationService
{
    protected IMediator Mediator { get; }
    protected ILogger Logger { get; }
    protected ICurrentUser CurrentUser { get; }

    protected ApplicationServiceBase(
        IMediator mediator,
        ILogger logger,
        ICurrentUser currentUser)
    {
        Mediator = mediator;
        Logger = logger;
        CurrentUser = currentUser;
    }
}
```

### Outbox Pattern
**IOutboxService.cs** - Outbox Interface:
```csharp
public interface IOutboxService
{
    Task AddMessageAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent;
    
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(
        int batchSize = 50, 
        CancellationToken cancellationToken = default);
    
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default);
}
```

### Validation Framework
**IValidator.cs** - Validation Interface:
```csharp
public interface IValidator<T>
{
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}

public class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<ValidationError> Errors { get; }
    
    // Implementation details...
}
```

## Centralized Build System Benefits

### 1. Automatic Dependency Management
- **Domain reference**: `BuildingBlocks.Domain` automatically referenced
- **Package consistency**: All Application projects use same versions
- **Feature-based packages**: Only include what you need
- **Security updates**: Centrally managed and applied

### 2. Clean Architecture Enforcement
- **Layer dependencies**: Only references Domain layer automatically
- **Architectural rules**: Custom analyzers enforce patterns
- **Interface abstractions**: Clear contracts for Infrastructure

### 3. CQRS & Mediator Patterns
- **Custom mediator**: No external dependencies on MediatR
- **Pipeline behaviors**: Cross-cutting concerns implementation
- **Handler registration**: Automatic via assembly scanning
- **Type safety**: Compile-time validation of requests/responses

## Key Design Patterns

### 1. Custom Mediator Pattern
- **No MediatR dependency** - custom implementation
- **Type-safe request/response** handling
- **Pipeline behavior** support
- **Async/await** throughout

### 2. CQRS Implementation
- **Separate read/write models**
- **Command handlers** for write operations
- **Query handlers** for read operations
- **Event handlers** for domain events

### 3. Cross-Cutting Concerns
- **Logging behavior** for all requests
- **Validation behavior** with FluentValidation
- **Caching behavior** for queries
- **Transaction behavior** for commands
- **Performance monitoring**

### 4. Messaging Patterns
- **Outbox pattern** for reliable messaging
- **Inbox pattern** for message deduplication
- **Event sourcing** support
- **Saga pattern** for long-running processes

## Usage Examples

### Command Handler Example
```csharp
public class CreateUserCommand : CommandBase<User>
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = User.Create(command.Email, command.FirstName, command.LastName);
        
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return user;
    }
}
```

### Query Handler Example
```csharp
public class GetUserByIdQuery : QueryBase<User?>
{
    public UserId UserId { get; set; }
}

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, User?>
{
    private readonly IReadOnlyUserRepository _userRepository;

    public GetUserByIdQueryHandler(IReadOnlyUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
    }
}
```

## Registration and Configuration

### ServiceCollectionExtensions.cs
```csharp
public static IServiceCollection AddApplication(this IServiceCollection services, params Assembly[] assemblies)
{
    // Add mediator
    services.AddScoped<IMediator, Mediator>();
    
    // Add behaviors
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    
    // Register handlers from assemblies
    services.AddHandlers(assemblies);
    
    return services;
}
```

## Best Practices

### 1. Command Design
- **Immutable commands** with required data
- **Single responsibility** per command
- **Validation** at command level
- **Business logic** in domain layer

### 2. Query Design
- **Read-only operations** only
- **Projection** to DTOs
- **Caching** for expensive queries
- **Pagination** for large datasets

### 3. Event Handling
- **Async event processing**
- **Idempotent handlers**
- **Error handling** and retry logic
- **Event versioning** support

### 4. Error Handling
- **Structured exceptions**
- **Validation errors** collection
- **Logging** at appropriate levels
- **User-friendly** error messages

## Integration with Build System

The Application library integrates seamlessly with the centralized build system:

- **Package Metadata**: Automatically configured with proper PackageId and description
- **Dependencies**: Domain layer automatically referenced
- **Feature Flags**: Optional packages like validation and MediatR can be enabled
- **Code Quality**: Application-specific architectural rules enforced
- **Documentation**: XML docs generated and published automatically
- **Assembly Scanning**: Handlers automatically discovered and registered

This Application Layer provides a robust foundation for implementing business logic while maintaining clean separation of concerns and supporting modern architectural patterns with zero configuration overhead. 