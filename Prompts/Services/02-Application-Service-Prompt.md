# Application Service Layer Generation Prompt

## Overview
Generate a comprehensive Application layer for a microservice following Clean Architecture, CQRS, and Domain-Driven Design principles, leveraging the BuildingBlocks.Application library. This layer orchestrates business workflows, handles use cases, and coordinates between the domain and infrastructure layers.

## Project Configuration

### Simplified Project File
```xml
<Project Sdk="Microsoft.NET.Sdk"/>
```

**That's it!** All configuration is handled automatically by the centralized build system:

- **Target Framework**: .NET 8.0 (from `Directory.Build.props`)
- **BuildingBlocks Reference**: `BuildingBlocks.Application` automatically referenced
- **Domain Reference**: Service domain project automatically referenced  
- **Package Management**: All necessary packages automatically included
- **Feature Flags**: Optional packages available (validation, MediatR, monitoring)

### Automatic Capabilities
The centralized build system automatically provides:
- **Custom Mediator**: Request/response handling with pipeline behaviors
- **CQRS Implementation**: Commands, queries, and handlers
- **Pipeline Behaviors**: Logging, validation, caching, transactions
- **Event Handling**: Domain and integration event processing
- **Outbox/Inbox**: Reliable messaging patterns
- **Validation**: FluentValidation integration
- **Caching**: Application-level caching support

## Service Application Structure

### Project Organization
```
{ServiceName}.Application/
├── {ServiceName}.Application.csproj           # Simple project file
├── Commands/                                  # Command operations (write)
│   ├── {Aggregate}/                          # Commands grouped by aggregate
│   │   ├── Create{Aggregate}/                # Command grouping by operation
│   │   │   ├── Create{Aggregate}Command.cs   # Command definition
│   │   │   ├── Create{Aggregate}CommandHandler.cs # Handler implementation
│   │   │   └── Create{Aggregate}CommandValidator.cs # Validation rules
│   │   ├── Update{Aggregate}/
│   │   │   ├── Update{Aggregate}Command.cs
│   │   │   ├── Update{Aggregate}CommandHandler.cs
│   │   │   └── Update{Aggregate}CommandValidator.cs
│   │   └── Delete{Aggregate}/
│   │       ├── Delete{Aggregate}Command.cs
│   │       └── Delete{Aggregate}CommandHandler.cs
├── Queries/                                   # Query operations (read)
│   ├── {Aggregate}/                          # Queries grouped by aggregate
│   │   ├── Get{Aggregate}ById/               # Query grouping by operation
│   │   │   ├── Get{Aggregate}ByIdQuery.cs    # Query definition
│   │   │   └── Get{Aggregate}ByIdQueryHandler.cs # Handler implementation
│   │   ├── Get{Aggregate}List/
│   │   │   ├── Get{Aggregate}ListQuery.cs
│   │   │   └── Get{Aggregate}ListQueryHandler.cs
│   │   └── Search{Aggregate}/
│   │       ├── Search{Aggregate}Query.cs
│   │       └── Search{Aggregate}QueryHandler.cs
├── Events/                                    # Event handling
│   ├── DomainEventHandlers/                  # Domain event handlers
│   │   ├── {Aggregate}DomainEventHandlers.cs # Handlers for domain events
│   │   └── CrossAggregateDomainEventHandlers.cs # Cross-aggregate handlers
│   ├── IntegrationEventHandlers/             # Integration event handlers
│   │   ├── External{Event}Handler.cs         # Handlers for external events
│   │   └── {ServiceName}IntegrationEventHandlers.cs # Service integration events
├── DTOs/                                      # Data Transfer Objects
│   ├── {Aggregate}/                          # DTOs grouped by aggregate
│   │   ├── {Aggregate}Dto.cs                 # Read model DTO
│   │   ├── Create{Aggregate}Request.cs       # Create request DTO
│   │   ├── Update{Aggregate}Request.cs       # Update request DTO
│   │   └── {Aggregate}SummaryDto.cs          # Summary/list DTO
│   ├── Common/                               # Common DTOs
│   │   ├── PaginatedRequest.cs               # Pagination request
│   │   ├── SortingRequest.cs                 # Sorting request
│   │   └── FilterRequest.cs                  # Filtering request
├── Services/                                  # Application services
│   ├── I{ServiceName}ApplicationService.cs   # Main service interface
│   ├── {ServiceName}ApplicationService.cs    # Main service implementation
│   ├── {Concern}ApplicationService.cs        # Specialized services
│   └── BackgroundServices/                   # Background processing
│       ├── {ServiceName}OutboxService.cs     # Outbox processing
│       └── {ServiceName}InboxService.cs      # Inbox processing
├── Behaviors/                                 # Pipeline behaviors
│   ├── {ServiceName}ValidationBehavior.cs    # Service-specific validation
│   ├── {ServiceName}CachingBehavior.cs       # Service-specific caching
│   └── {ServiceName}LoggingBehavior.cs       # Service-specific logging
├── Mapping/                                   # Object mapping
│   ├── {Aggregate}MappingProfile.cs          # AutoMapper profiles
│   └── DtoMappingExtensions.cs               # Manual mapping extensions
├── Validation/                                # Validation rules
│   ├── {Aggregate}Validators.cs              # Aggregate-specific validators
│   ├── CommonValidators.cs                   # Shared validation rules
│   └── ValidationExtensions.cs               # Validation helpers
├── Specifications/                            # Application specifications
│   ├── {Aggregate}ApplicationSpecs.cs        # Application-level specs
│   └── SearchSpecifications.cs               # Search and filter specs
├── Caching/                                   # Caching strategies
│   ├── {ServiceName}CacheKeys.cs             # Cache key definitions
│   ├── {Aggregate}CacheStrategies.cs         # Caching strategies
│   └── CacheInvalidationHandlers.cs          # Cache invalidation
├── Security/                                  # Security concerns
│   ├── {ServiceName}AuthorizationHandlers.cs # Authorization handlers
│   ├── {ServiceName}SecurityPolicies.cs      # Security policies
│   └── CurrentUserService.cs                 # Current user implementation
├── Contracts/                                 # External service contracts
│   ├── I{ExternalService}Client.cs           # External service interfaces
│   └── IntegrationEventContracts.cs          # Event contracts
└── Extensions/                                # Extensions and configuration
    ├── ServiceCollectionExtensions.cs        # DI registration
    ├── MappingExtensions.cs                   # Mapping helpers
    └── {ServiceName}ApplicationExtensions.cs  # Application-specific extensions
```

## Implementation Guidelines

### 1. Commands (Write Operations)
Implement commands using BuildingBlocks patterns:

```csharp
// Example: Create User Command
public sealed record CreateUserCommand : CommandBase<UserDto>
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}

// Command Handler
public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthDomainService _authDomainService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IAuthDomainService authDomainService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _authDomainService = authDomainService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user with username: {Username}", command.Username);

        // Create domain objects
        var username = new Username(command.Username);
        var email = new Email(command.Email);

        // Use domain service for complex logic
        var user = await _authDomainService.CreateUserAsync(username, email, command.Password, cancellationToken);

        // Save to repository
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);

        // Map to DTO
        return _mapper.Map<UserDto>(user);
    }
}

// Command Validator
public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 50)
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Username must be 3-50 characters, alphanumeric and underscore only");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one lowercase, uppercase, digit, and special character");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
```

### 2. Queries (Read Operations)
Implement queries using BuildingBlocks patterns:

```csharp
// Example: Get User By ID Query
public sealed record GetUserByIdQuery : QueryBase<UserDto?>
{
    public UserId UserId { get; init; }
    
    public GetUserByIdQuery(UserId userId)
    {
        UserId = userId;
    }
}

// Query Handler
public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<UserDto?> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = AuthCacheKeys.UserById(query.UserId);
        
        // Try cache first
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey, cancellationToken);
        if (cachedUser != null)
        {
            _logger.LogDebug("User found in cache: {UserId}", query.UserId);
            return cachedUser;
        }

        // Get from repository
        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", query.UserId);
            return null;
        }

        // Map to DTO
        var userDto = _mapper.Map<UserDto>(user);

        // Cache the result
        await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(30), cancellationToken);

        _logger.LogDebug("User retrieved from database: {UserId}", query.UserId);
        return userDto;
    }
}

// Example: Paginated Query
public sealed record GetUsersListQuery : PagedQuery<UserDto>
{
    public string? SearchTerm { get; init; }
    public UserStatus? Status { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public DateTime? CreatedBefore { get; init; }
}

// Paginated Query Handler
public sealed class GetUsersListQueryHandler : IQueryHandler<GetUsersListQuery, PagedResult<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUsersListQueryHandler> _logger;

    public GetUsersListQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUsersListQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<UserDto>> HandleAsync(GetUsersListQuery query, CancellationToken cancellationToken)
    {
        // Build specifications
        var spec = new ActiveUsersSpecification();
        
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            spec = spec.And(new UserSearchSpecification(query.SearchTerm));
        }

        if (query.Status.HasValue)
        {
            spec = spec.And(new UsersByStatusSpecification(query.Status.Value));
        }

        if (query.CreatedAfter.HasValue || query.CreatedBefore.HasValue)
        {
            spec = spec.And(new UsersCreatedBetweenSpecification(
                query.CreatedAfter ?? DateTime.MinValue,
                query.CreatedBefore ?? DateTime.MaxValue));
        }

        // Get paginated results
        var pagedUsers = await _userRepository.GetPagedAsync(
            spec,
            query.Page,
            query.PageSize,
            query.SortBy,
            query.SortDirection,
            cancellationToken);

        // Map to DTOs
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(pagedUsers.Items);

        return new PagedResult<UserDto>(
            userDtos,
            pagedUsers.TotalCount,
            query.Page,
            query.PageSize);
    }
}
```

### 3. DTOs (Data Transfer Objects)
Define clean data contracts:

```csharp
// Example: User DTOs
public sealed record UserDto
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public UserStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed record UserSummaryDto
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public UserStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record CreateUserRequest
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}

public sealed record UpdateUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}
```

### 4. Event Handlers
Handle domain and integration events:

```csharp
// Example: Domain Event Handlers
public sealed class UserDomainEventHandlers :
    IDomainEventHandler<UserCreatedDomainEvent>,
    IDomainEventHandler<UserEmailChangedDomainEvent>,
    IDomainEventHandler<UserDeactivatedDomainEvent>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<UserDomainEventHandlers> _logger;

    public UserDomainEventHandlers(
        IEventBus eventBus,
        ILogger<UserDomainEventHandlers> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UserCreatedDomainEvent for user: {UserId}", domainEvent.UserId);

        // Publish integration event
        var integrationEvent = new UserRegisteredIntegrationEvent(
            domainEvent.UserId.Value,
            domainEvent.Username.Value,
            domainEvent.Email.Value,
            DateTime.UtcNow);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }

    public async Task HandleAsync(UserEmailChangedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UserEmailChangedDomainEvent for user: {UserId}", domainEvent.UserId);

        // Invalidate cache
        var cacheKey = AuthCacheKeys.UserById(domainEvent.UserId);
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        // Publish integration event
        var integrationEvent = new UserEmailChangedIntegrationEvent(
            domainEvent.UserId.Value,
            domainEvent.OldEmail.Value,
            domainEvent.NewEmail.Value,
            DateTime.UtcNow);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }

    public async Task HandleAsync(UserDeactivatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UserDeactivatedDomainEvent for user: {UserId}", domainEvent.UserId);

        // Invalidate cache
        var cacheKey = AuthCacheKeys.UserById(domainEvent.UserId);
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        // Additional cleanup logic...
    }
}

// Example: Integration Event Handler
public sealed class ProfileUpdatedIntegrationEventHandler : IEventHandler<ProfileUpdatedIntegrationEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProfileUpdatedIntegrationEventHandler> _logger;

    public ProfileUpdatedIntegrationEventHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProfileUpdatedIntegrationEventHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(ProfileUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ProfileUpdatedIntegrationEvent for user: {UserId}", integrationEvent.UserId);

        var userId = new UserId(integrationEvent.UserId);
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user != null)
        {
            // Update user profile information
            // user.UpdateProfile(integrationEvent.FirstName, integrationEvent.LastName);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Invalidate cache
            var cacheKey = AuthCacheKeys.UserById(userId);
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);
        }
    }
}
```

### 5. Application Services
Orchestrate complex workflows:

```csharp
// Example: Main Application Service
public interface IAuthApplicationService : IApplicationService
{
    Task<UserDto> RegisterUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResult<UserSummaryDto>> GetUsersAsync(GetUsersListQuery query, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
}

public sealed class AuthApplicationService : ApplicationServiceBase, IAuthApplicationService
{
    private readonly IMediator _mediator;

    public AuthApplicationService(
        IMediator mediator,
        ILogger<AuthApplicationService> logger,
        ICurrentUser currentUser)
        : base(mediator, logger, currentUser)
    {
        _mediator = mediator;
    }

    public async Task<UserDto> RegisterUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Registering user: {Username}", request.Username);

        var command = new CreateUserCommand
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        return await _mediator.SendAsync<CreateUserCommand, UserDto>(command, cancellationToken);
    }

    public async Task<UserDto?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(new UserId(userId));
        return await _mediator.QueryAsync<GetUserByIdQuery, UserDto?>(query, cancellationToken);
    }

    public async Task<PagedResult<UserSummaryDto>> GetUsersAsync(GetUsersListQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.QueryAsync<GetUsersListQuery, PagedResult<UserDto>>(query, cancellationToken);
        
        // Convert to summary DTOs
        var summaryDtos = result.Items.Select(user => new UserSummaryDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Status = user.Status,
            CreatedAt = user.CreatedAt
        });

        return new PagedResult<UserSummaryDto>(summaryDtos, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Updating user: {UserId}", userId);

        var command = new UpdateUserCommand
        {
            UserId = new UserId(userId),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        return await _mediator.SendAsync<UpdateUserCommand, UserDto>(command, cancellationToken);
    }

    public async Task DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Deactivating user: {UserId}", userId);

        var command = new DeactivateUserCommand { UserId = new UserId(userId) };
        await _mediator.SendAsync(command, cancellationToken);
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var query = new ValidateCredentialsQuery
        {
            Username = username,
            Password = password
        };

        return await _mediator.QueryAsync<ValidateCredentialsQuery, bool>(query, cancellationToken);
    }
}
```

### 6. Mapping Profiles
Configure object mapping:

```csharp
// Example: AutoMapper Profile
public sealed class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Domain to DTO mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.Value))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value));

        CreateMap<User, UserSummaryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.Value))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));

        // Request to Command mappings
        CreateMap<CreateUserRequest, CreateUserCommand>();
        CreateMap<UpdateUserRequest, UpdateUserCommand>();
    }
}

// Extension methods for manual mapping
public static class DtoMappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id.Value,
            Username = user.Username.Value,
            Email = user.Email.Value,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static CreateUserCommand ToCommand(this CreateUserRequest request)
    {
        return new CreateUserCommand
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
    }
}
```

### 7. Caching Strategies
Define caching keys and strategies:

```csharp
// Example: Cache Keys
public static class AuthCacheKeys
{
    private const string PREFIX = "auth";
    private const string USER_PREFIX = $"{PREFIX}:user";

    public static string UserById(UserId userId) => $"{USER_PREFIX}:id:{userId.Value}";
    public static string UserByUsername(Username username) => $"{USER_PREFIX}:username:{username.Value}";
    public static string UserByEmail(Email email) => $"{USER_PREFIX}:email:{email.Value}";
    public static string UserList(int page, int pageSize, string? searchTerm = null) 
        => $"{USER_PREFIX}:list:{page}:{pageSize}:{searchTerm ?? "all"}";
}

// Example: Cache Invalidation Handler
public sealed class UserCacheInvalidationHandler :
    IDomainEventHandler<UserCreatedDomainEvent>,
    IDomainEventHandler<UserUpdatedDomainEvent>,
    IDomainEventHandler<UserDeletedDomainEvent>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<UserCacheInvalidationHandler> _logger;

    public UserCacheInvalidationHandler(
        ICacheService cacheService,
        ILogger<UserCacheInvalidationHandler> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await InvalidateUserListCaches(cancellationToken);
    }

    public async Task HandleAsync(UserUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await InvalidateUserCaches(domainEvent.UserId, cancellationToken);
        await InvalidateUserListCaches(cancellationToken);
    }

    public async Task HandleAsync(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await InvalidateUserCaches(domainEvent.UserId, cancellationToken);
        await InvalidateUserListCaches(cancellationToken);
    }

    private async Task InvalidateUserCaches(UserId userId, CancellationToken cancellationToken)
    {
        var keys = new[]
        {
            AuthCacheKeys.UserById(userId)
            // Add other user-specific cache keys
        };

        foreach (var key in keys)
        {
            await _cacheService.RemoveAsync(key, cancellationToken);
        }

        _logger.LogDebug("Invalidated user caches for: {UserId}", userId);
    }

    private async Task InvalidateUserListCaches(CancellationToken cancellationToken)
    {
        // Invalidate list caches - you might want to use cache tagging for this
        _logger.LogDebug("Invalidated user list caches");
    }
}
```

### 8. Pipeline Behaviors
Implement cross-cutting concerns:

```csharp
// Example: Service-specific validation behavior
public sealed class AuthValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<AuthValidationBehavior<TRequest, TResponse>> _logger;

    public AuthValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<AuthValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            _logger.LogWarning("Validation failed for {RequestType}: {@Errors}", 
                typeof(TRequest).Name, failures.Select(f => f.ErrorMessage));
            
            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

### 9. Service Registration
Configure dependency injection:

```csharp
// Example: Application service registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection Add{ServiceName}Application(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        // Add BuildingBlocks Application
        services.AddApplication(assemblies);

        // Register application services
        services.AddScoped<IAuthApplicationService, AuthApplicationService>();

        // Register pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthValidationBehavior<,>));

        // Register AutoMapper profiles
        services.AddAutoMapper(typeof(UserMappingProfile));

        // Register validators
        services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();

        // Register event handlers
        services.AddScoped<UserDomainEventHandlers>();
        services.AddScoped<ProfileUpdatedIntegrationEventHandler>();
        services.AddScoped<UserCacheInvalidationHandler>();

        // Register domain event handler mappings
        services.AddScoped<IDomainEventHandler<UserCreatedDomainEvent>>(
            provider => provider.GetRequiredService<UserDomainEventHandlers>());
        services.AddScoped<IDomainEventHandler<UserEmailChangedDomainEvent>>(
            provider => provider.GetRequiredService<UserDomainEventHandlers>());
        services.AddScoped<IDomainEventHandler<UserDeactivatedDomainEvent>>(
            provider => provider.GetRequiredService<UserDomainEventHandlers>());

        // Register cache invalidation handlers
        services.AddScoped<IDomainEventHandler<UserCreatedDomainEvent>>(
            provider => provider.GetRequiredService<UserCacheInvalidationHandler>());
        services.AddScoped<IDomainEventHandler<UserUpdatedDomainEvent>>(
            provider => provider.GetRequiredService<UserCacheInvalidationHandler>());
        services.AddScoped<IDomainEventHandler<UserDeletedDomainEvent>>(
            provider => provider.GetRequiredService<UserCacheInvalidationHandler>());

        return services;
    }
}
```

## Key Principles

### 1. **CQRS Implementation**
- **Commands** for write operations with validation
- **Queries** for read operations with caching
- **Separate models** for reads and writes
- **Event-driven** side effects

### 2. **Clean Architecture**
- **Application layer** orchestrates business workflows
- **Domain layer** contains business logic
- **Infrastructure concerns** abstracted via interfaces
- **Cross-cutting concerns** handled via pipeline behaviors

### 3. **Event-Driven Architecture**
- **Domain events** for internal consistency
- **Integration events** for service communication
- **Event handlers** for side effects
- **Outbox/Inbox** for reliable messaging

### 4. **Validation & Error Handling**
- **FluentValidation** for input validation
- **Pipeline behaviors** for cross-cutting validation
- **Structured exceptions** with proper error messages
- **Logging** at appropriate levels

### 5. **Performance & Scalability**
- **Caching strategies** for read operations
- **Async/await** throughout
- **Pagination** for large datasets
- **Specifications** for complex queries

## Benefits of This Structure

### 1. **Automatic BuildingBlocks Integration**
- **Zero configuration** - everything works out of the box
- **CQRS patterns** with custom mediator
- **Pipeline behaviors** for cross-cutting concerns
- **Event handling** infrastructure

### 2. **Clean Separation of Concerns**
- **Commands** separated from **queries**
- **Business logic** in domain layer
- **Orchestration** in application layer
- **Cross-cutting concerns** via behaviors

### 3. **Testability**
- **Unit testable** handlers with clear dependencies
- **Integration testable** workflows
- **Mockable interfaces** for external dependencies
- **Isolated concerns** via CQRS

### 4. **Performance & Scalability**
- **Caching** at appropriate levels
- **Async processing** throughout
- **Event-driven** architecture
- **Optimized queries** with specifications

Generate an application layer following these patterns and principles, ensuring clean orchestration, robust validation, comprehensive event handling, and seamless integration with the BuildingBlocks architecture. 