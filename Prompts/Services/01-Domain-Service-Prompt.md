# Domain Service Layer Generation Prompt

## Overview
Generate a comprehensive Domain layer for a microservice following Domain-Driven Design (DDD) principles and leveraging the BuildingBlocks.Domain library. This service domain contains the core business logic, entities, value objects, and domain rules specific to the service's bounded context.

## Project Configuration

### Simplified Project File
```xml
<Project Sdk="Microsoft.NET.Sdk"/>
```

**That's it!** All configuration is handled automatically by the centralized build system:

- **Target Framework**: .NET 8.0 (from `Directory.Build.props`)
- **BuildingBlocks Reference**: `BuildingBlocks.Domain` automatically referenced
- **Package Management**: All necessary packages automatically included
- **Feature Flags**: Optional domain helpers available (`IncludeDomainHelpers`)

### Automatic Capabilities
The centralized build system automatically provides:
- **Strong-Typed IDs**: JSON serialization, route binding, EF Core conversion
- **Value Objects**: Immutable records with validation
- **Domain Events**: Event-driven architecture support
- **Business Rules**: Validation and invariant enforcement
- **Specifications**: Query logic encapsulation
- **Audit Support**: Creatable, updatable, soft-deletable entities

## Service Domain Structure

### Project Organization
```
{ServiceName}.Domain/
├── {ServiceName}.Domain.csproj              # Simple project file
├── Aggregates/                              # Aggregate roots and related entities
│   ├── {AggregateName}/                    # One folder per aggregate
│   │   ├── {AggregateName}.cs              # Aggregate root entity
│   │   ├── {ChildEntity}.cs                # Child entities (if any)
│   │   ├── {AggregateName}Id.cs            # Strongly-typed ID
│   │   ├── {AggregateName}DomainEvents.cs  # Domain events for this aggregate
│   │   ├── {AggregateName}Specifications.cs # Query specifications
│   │   └── {AggregateName}BusinessRules.cs # Business rules
├── ValueObjects/                            # Service-specific value objects
│   ├── {ValueObject1}.cs                  # Immutable value objects
│   ├── {ValueObject2}.cs                  # Using BuildingBlocks patterns
│   └── {Enumeration}.cs                   # Rich enumerations
├── DomainServices/                          # Domain services for complex logic
│   ├── I{ServiceName}DomainService.cs      # Domain service interface
│   └── {ServiceName}DomainService.cs       # Domain service implementation
├── Repositories/                            # Repository contracts
│   ├── I{Aggregate}Repository.cs           # Repository interfaces
│   └── IUnitOfWork.cs                      # Unit of work interface
├── Events/                                  # Integration events
│   ├── {ServiceName}IntegrationEvents.cs   # Events published to other services
│   └── ExternalIntegrationEvents.cs        # Events from other services
├── Exceptions/                              # Domain-specific exceptions
│   ├── {ServiceName}DomainException.cs     # Service-specific exceptions
│   └── {Entity}NotFoundException.cs        # Specific exception types
├── Enums/                                   # Service enumerations
│   ├── {Enumeration1}.cs                  # Standard enums
│   └── {Enumeration2}.cs                  # Rich enumerations
└── Extensions/                              # Domain extensions
    ├── {ServiceName}DomainExtensions.cs    # Service-specific extensions
    └── ServiceCollectionExtensions.cs      # DI registration
```

## Implementation Guidelines

### 1. Strongly-Typed IDs
Use BuildingBlocks strongly-typed ID patterns:

```csharp
// Example: UserId for an AuthService
public readonly struct UserId : IStronglyTypedId<Guid>
{
    public Guid Value { get; }
    
    public UserId(Guid value) => Value = value;
    
    public static UserId New() => new(Guid.NewGuid());
    public static UserId Empty => new(Guid.Empty);
    
    // Implicit conversion to Guid
    public static implicit operator Guid(UserId id) => id.Value;
    
    // Explicit conversion from Guid
    public static explicit operator UserId(Guid value) => new(value);
    
    // Equality and comparison
    public bool Equals(UserId other) => Value.Equals(other.Value);
    public override bool Equals(object? obj) => obj is UserId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
    
    // Operators
    public static bool operator ==(UserId left, UserId right) => left.Equals(right);
    public static bool operator !=(UserId left, UserId right) => !left.Equals(right);
}
```

### 2. Aggregate Roots
Leverage BuildingBlocks Entity base classes:

```csharp
// Example: User aggregate for AuthService
public class User : AggregateRoot<UserId, Guid>, IAuditableEntity, ISoftDeletable
{
    // Private constructor for EF Core
    private User() { }
    
    // Factory method for creation
    public static User Create(
        Username username, 
        Email email, 
        PasswordHash passwordHash)
    {
        var user = new User
        {
            Id = UserId.New(),
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        
        // Validate business rules
        user.CheckRule(new UniqueUsernameRule(username));
        user.CheckRule(new ValidEmailFormatRule(email));
        
        // Raise domain event
        user.AddDomainEvent(new UserCreatedDomainEvent(user.Id, username, email));
        
        return user;
    }
    
    // Properties
    public Username Username { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public UserStatus Status { get; private set; }
    
    // Audit properties (from IAuditableEntity)
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Soft delete properties (from ISoftDeletable)
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    
    // Domain methods
    public void ChangePassword(PasswordHash newPasswordHash)
    {
        CheckRule(new PasswordComplexityRule(newPasswordHash));
        
        PasswordHash = newPasswordHash;
        AddDomainEvent(new UserPasswordChangedDomainEvent(Id));
    }
    
    public void UpdateEmail(Email newEmail)
    {
        CheckRule(new ValidEmailFormatRule(newEmail));
        
        var oldEmail = Email;
        Email = newEmail;
        
        AddDomainEvent(new UserEmailChangedDomainEvent(Id, oldEmail, newEmail));
    }
    
    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        AddDomainEvent(new UserDeactivatedDomainEvent(Id));
    }
    
    // Soft delete override
    public void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        
        AddDomainEvent(new UserDeletedDomainEvent(Id));
    }
}
```

### 3. Value Objects
Use BuildingBlocks value object patterns:

```csharp
// Example: Username value object
public sealed record Username : SingleValueObject<string>
{
    public Username(string value) : base(value)
    {
    }
    
    protected override string ValidateValue(string value)
    {
        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        Guard.Against.InvalidFormat(value, nameof(value), @"^[a-zA-Z0-9_]{3,50}$", "Username must be 3-50 characters, alphanumeric and underscore only");
        
        return value.ToLowerInvariant();
    }
    
    public static implicit operator string(Username username) => username.Value;
    public static explicit operator Username(string value) => new(value);
}

// Example: Complex value object
public sealed record Address : ValueObject
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    
    public Address(string street, string city, string state, string postalCode, string country)
    {
        Street = Guard.Against.NullOrWhiteSpace(street, nameof(street));
        City = Guard.Against.NullOrWhiteSpace(city, nameof(city));
        State = Guard.Against.NullOrWhiteSpace(state, nameof(state));
        PostalCode = Guard.Against.NullOrWhiteSpace(postalCode, nameof(postalCode));
        Country = Guard.Against.NullOrWhiteSpace(country, nameof(country));
        
        Validate();
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }
    
    protected override void Validate()
    {
        // Additional validation logic
        if (PostalCode.Length < 5)
            throw new ArgumentException("Postal code must be at least 5 characters");
    }
}
```

### 4. Domain Events
Leverage BuildingBlocks event patterns:

```csharp
// Example: Domain events for User aggregate
public static class UserDomainEvents
{
    public sealed record UserCreatedDomainEvent(
        UserId UserId,
        Username Username,
        Email Email
    ) : DomainEventBase(nameof(UserCreatedDomainEvent));
    
    public sealed record UserPasswordChangedDomainEvent(
        UserId UserId
    ) : DomainEventBase(nameof(UserPasswordChangedDomainEvent));
    
    public sealed record UserEmailChangedDomainEvent(
        UserId UserId,
        Email OldEmail,
        Email NewEmail
    ) : DomainEventBase(nameof(UserEmailChangedDomainEvent));
    
    public sealed record UserDeactivatedDomainEvent(
        UserId UserId
    ) : DomainEventBase(nameof(UserDeactivatedDomainEvent));
    
    public sealed record UserDeletedDomainEvent(
        UserId UserId
    ) : DomainEventBase(nameof(UserDeletedDomainEvent));
}
```

### 5. Business Rules
Use BuildingBlocks business rule patterns:

```csharp
// Example: Business rules for User aggregate
public static class UserBusinessRules
{
    public sealed class UniqueUsernameRule : BusinessRuleBase
    {
        private readonly Username _username;
        private readonly IUserRepository _userRepository;
        
        public UniqueUsernameRule(Username username, IUserRepository userRepository)
        {
            _username = username;
            _userRepository = userRepository;
        }
        
        public override string Message => $"Username '{_username}' is already taken";
        public override string Code => "USER_USERNAME_NOT_UNIQUE";
        
        public override bool IsBroken() => _userRepository.ExistsByUsernameAsync(_username).Result;
    }
    
    public sealed class ValidEmailFormatRule : BusinessRuleBase
    {
        private readonly Email _email;
        
        public ValidEmailFormatRule(Email email)
        {
            _email = email;
        }
        
        public override string Message => $"Email '{_email}' is not in valid format";
        public override string Code => "USER_EMAIL_INVALID_FORMAT";
        
        public override bool IsBroken()
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(_email.Value);
                return addr.Address != _email.Value;
            }
            catch
            {
                return true;
            }
        }
    }
    
    public sealed class PasswordComplexityRule : BusinessRuleBase
    {
        private readonly PasswordHash _passwordHash;
        
        public PasswordComplexityRule(PasswordHash passwordHash)
        {
            _passwordHash = passwordHash;
        }
        
        public override string Message => "Password does not meet complexity requirements";
        public override string Code => "USER_PASSWORD_COMPLEXITY_FAILED";
        
        public override bool IsBroken()
        {
            // Implement password complexity validation
            // This would check the original password before hashing
            return false; // Implementation details...
        }
    }
}
```

### 6. Specifications
Use BuildingBlocks specification patterns:

```csharp
// Example: Specifications for User queries
public static class UserSpecifications
{
    public sealed class ActiveUsersSpecification : Specification<User>
    {
        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.Status == UserStatus.Active && !user.IsDeleted;
        }
    }
    
    public sealed class UsersByEmailDomainSpecification : Specification<User>
    {
        private readonly string _emailDomain;
        
        public UsersByEmailDomainSpecification(string emailDomain)
        {
            _emailDomain = emailDomain;
        }
        
        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.Email.Value.EndsWith($"@{_emailDomain}");
        }
    }
    
    public sealed class UsersCreatedBetweenSpecification : Specification<User>
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        
        public UsersCreatedBetweenSpecification(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
        }
        
        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.CreatedAt >= _startDate && user.CreatedAt <= _endDate;
        }
    }
}
```

### 7. Repository Interfaces
Define repository contracts:

```csharp
// Example: User repository interface
public interface IUserRepository : IRepository<User, UserId, Guid>
{
    Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(Username username, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<User>> GetUsersPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
```

### 8. Domain Services
Implement complex domain logic:

```csharp
// Example: Domain service interface
public interface IAuthDomainService : IDomainService
{
    Task<bool> IsUsernameAvailableAsync(Username username, CancellationToken cancellationToken = default);
    Task<bool> IsEmailAvailableAsync(Email email, CancellationToken cancellationToken = default);
    Task<User> CreateUserAsync(Username username, Email email, string password, CancellationToken cancellationToken = default);
    Task<bool> ValidatePasswordAsync(User user, string password, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(User user, string newPassword, CancellationToken cancellationToken = default);
}

// Example: Domain service implementation
public class AuthDomainService : DomainServiceBase, IAuthDomainService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    
    public AuthDomainService(
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
    }
    
    public async Task<bool> IsUsernameAvailableAsync(Username username, CancellationToken cancellationToken = default)
    {
        return !await _userRepository.ExistsByUsernameAsync(username, cancellationToken);
    }
    
    public async Task<bool> IsEmailAvailableAsync(Email email, CancellationToken cancellationToken = default)
    {
        return !await _userRepository.ExistsByEmailAsync(email, cancellationToken);
    }
    
    public async Task<User> CreateUserAsync(Username username, Email email, string password, CancellationToken cancellationToken = default)
    {
        // Validate availability
        CheckRule(new UniqueUsernameRule(username, _userRepository));
        CheckRule(new UniqueEmailRule(email, _userRepository));
        
        // Hash password
        var passwordHash = new PasswordHash(_passwordHashingService.HashPassword(password));
        
        // Create user
        return User.Create(username, email, passwordHash);
    }
    
    public async Task<bool> ValidatePasswordAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        return _passwordHashingService.VerifyPassword(password, user.PasswordHash.Value);
    }
    
    public async Task ChangePasswordAsync(User user, string newPassword, CancellationToken cancellationToken = default)
    {
        var newPasswordHash = new PasswordHash(_passwordHashingService.HashPassword(newPassword));
        user.ChangePassword(newPasswordHash);
    }
}
```

### 9. Integration Events
Define cross-service communication:

```csharp
// Example: Integration events for AuthService
public static class AuthIntegrationEvents
{
    public sealed record UserRegisteredIntegrationEvent(
        Guid UserId,
        string Username,
        string Email,
        DateTime RegisteredAt
    ) : IntegrationEventBase(nameof(UserRegisteredIntegrationEvent));
    
    public sealed record UserEmailChangedIntegrationEvent(
        Guid UserId,
        string OldEmail,
        string NewEmail,
        DateTime ChangedAt
    ) : IntegrationEventBase(nameof(UserEmailChangedIntegrationEvent));
    
    public sealed record UserDeactivatedIntegrationEvent(
        Guid UserId,
        string Username,
        DateTime DeactivatedAt
    ) : IntegrationEventBase(nameof(UserDeactivatedIntegrationEvent));
}

// Example: External events the service listens to
public static class ExternalIntegrationEvents
{
    public sealed record ProfileUpdatedIntegrationEvent(
        Guid UserId,
        string FirstName,
        string LastName,
        DateTime UpdatedAt
    ) : IntegrationEventBase(nameof(ProfileUpdatedIntegrationEvent));
    
    public sealed record PaymentProcessedIntegrationEvent(
        Guid UserId,
        decimal Amount,
        string Currency,
        DateTime ProcessedAt
    ) : IntegrationEventBase(nameof(PaymentProcessedIntegrationEvent));
}
```

### 10. Service Registration
Configure dependency injection:

```csharp
// Example: Domain service registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection Add{ServiceName}Domain(this IServiceCollection services)
    {
        // Register domain services
        services.AddScoped<IAuthDomainService, AuthDomainService>();
        
        // Register domain event handlers (if any)
        services.AddScoped<IDomainEventHandler<UserCreatedDomainEvent>, UserCreatedEventHandler>();
        
        // Register specifications
        services.AddTransient<ActiveUsersSpecification>();
        services.AddTransient<UsersByEmailDomainSpecification>();
        
        return services;
    }
}
```

## Key Principles

### 1. **Domain Purity**
- **No external dependencies** except BuildingBlocks.Domain
- **Pure business logic** without infrastructure concerns
- **Explicit interfaces** for all external dependencies

### 2. **Rich Domain Model**
- **Behavior-rich entities** with business methods
- **Immutable value objects** with validation
- **Strongly-typed IDs** for type safety
- **Business rules** as first-class citizens

### 3. **Event-Driven Design**
- **Domain events** for internal concerns
- **Integration events** for cross-service communication
- **Event sourcing** support when needed

### 4. **Specification Pattern**
- **Encapsulated query logic** in specifications
- **Composable queries** using And/Or/Not operations
- **Testable business queries**

### 5. **Clean Architecture**
- **Dependencies point inward** to domain
- **Repository abstractions** defined in domain
- **Domain services** for complex business logic

## Benefits of This Structure

### 1. **Automatic BuildingBlocks Integration**
- **Zero configuration** - everything works out of the box
- **Type safety** across all domain constructs
- **Consistent patterns** using proven building blocks

### 2. **Rich Domain Modeling**
- **Strongly-typed identifiers** prevent primitive obsession
- **Value objects** ensure data integrity
- **Business rules** enforce domain invariants
- **Domain events** enable reactive programming

### 3. **Clean Architecture Compliance**
- **Pure domain layer** with no external dependencies
- **Explicit contracts** for infrastructure needs
- **Testable business logic** with clear separation

### 4. **Event-Driven Architecture**
- **Domain events** for internal consistency
- **Integration events** for service communication
- **Eventual consistency** support

Generate a domain layer following these patterns and principles, ensuring rich business logic, type safety, and seamless integration with the BuildingBlocks architecture. 