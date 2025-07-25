# BuildingBlocks.Domain Library Generation Prompt

## Overview
Generate a comprehensive Domain layer library following Domain-Driven Design (DDD) principles. This library provides foundational building blocks for microservices architecture using modern C# features including records and readonly structs.

## Project Configuration

### Simplified Project File
```xml
<Project Sdk="Microsoft.NET.Sdk"/>
```

**That's it!** All configuration is handled automatically by the centralized build system:

- **Target Framework**: .NET 8.0 (from `Directory.Build.props`)
- **Language Features**: Implicit usings, nullable reference types, warnings as errors
- **Documentation**: XML documentation generation enabled
- **Package Management**: All packages automatically included via `Directory.Build.targets`
- **Versioning**: Automatic versioning with Git integration
- **Analysis**: Code quality rules from `BuildingBlocks.ruleset`

### Automatic Package Inclusion
The following packages are automatically included via the centralized build system:
- `System.Text.Json` (Domain-specific package)
- Core Microsoft Extensions (DI, Logging, Configuration abstractions)
- `System.ComponentModel.Annotations`
- Source Link for debugging (in non-test projects)
- Nullable reference types analyzer

### Feature Control
Domain projects can optionally enable additional features:
```xml
<!-- Optional in individual .csproj if needed -->
<PropertyGroup>
  <IncludeDomainHelpers>true</IncludeDomainHelpers>  <!-- Adds Ardalis.GuardClauses, CSharpFunctionalExtensions -->
</PropertyGroup>
```

## Architecture & Patterns

### Core DDD Concepts
1. **Value Objects**: Immutable objects defined by their attributes using `record` types
2. **Entities**: Objects with identity and lifecycle using strongly-typed IDs
3. **Aggregate Roots**: Domain entry points that maintain consistency
4. **Domain Events**: Represent significant business occurrences
5. **Business Rules**: Encapsulated validation and business logic
6. **Specifications**: Encapsulated query logic
7. **Strongly-Typed IDs**: Type-safe identifiers using `readonly struct`

### Access Modifier Strategy
- **Public**: Core abstractions (base classes, interfaces) used by microservices
- **Internal**: Implementation details that should be encapsulated

## Folder Structure & Components

### `/ValueObjects` - Immutable Value Types
```
ValueObjects/
├── ValueObject.cs          # Base record for multi-property value objects
├── SingleValueObject.cs    # Base record for single-value wrappers
└── Enumeration.cs         # Rich enumeration pattern (class-based)
```

**ValueObject.cs** - Abstract Record Base:
```csharp
public abstract record ValueObject
{
    protected virtual IEnumerable<object?> GetEqualityComponents() { yield break; }
    protected virtual void Validate() { }
}
```

**SingleValueObject.cs** - Single Property Record:
```csharp
public abstract record SingleValueObject<T>(T Value) : ValueObject
{
    public T Value { get; init; } = ValidateValue(Value);
    protected static virtual T ValidateValue(T value) => value;
    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }
}
```

### `/StronglyTypedIds` - Type-Safe Identifiers
```
StronglyTypedIds/
├── IStronglyTypedId.cs     # Base interface
├── GuidId.cs              # GUID-based ID struct
├── IntId.cs               # Integer-based ID struct
├── LongId.cs              # Long-based ID struct
├── StringId.cs            # String-based ID struct
├── IdJsonConverter.cs     # JSON serialization support
└── StronglyTypedIdExtensions.cs # Extension methods
```

**Implementation Pattern (GuidId.cs)**:
```csharp
public readonly struct GuidId<T> : IStronglyTypedId<Guid>, IEquatable<GuidId<T>>
    where T : GuidId<T>
{
    public Guid Value { get; }
    
    // Constructors, factory methods, operators, equality
    public static T New() => (T)Activator.CreateInstance(typeof(T), Guid.NewGuid())!;
    public static T Empty => (T)Activator.CreateInstance(typeof(T), Guid.Empty)!;
    
    // Implicit/explicit operators for seamless conversion
    public static implicit operator Guid(GuidId<T> id) => id.Value;
    public static explicit operator GuidId<T>(Guid value) => (T)Activator.CreateInstance(typeof(T), value)!;
}
```

### `/Entities` - Domain Objects with Identity
```
Entities/
├── Entity.cs              # Base entity with strongly-typed ID
├── AggregateRoot.cs       # Aggregate root with domain events
├── IAuditableEntity.cs    # Audit fields interface
└── ISoftDeletable.cs      # Soft deletion interface
```

**Entity.cs** - Base Entity:
```csharp
public abstract class Entity<TId, TIdValue> : IEquatable<Entity<TId, TIdValue>>
    where TId : struct, IStronglyTypedId<TIdValue>
    where TIdValue : IEquatable<TIdValue>
{
    public TId Id { get; protected set; }
    protected List<IDomainEvent> _domainEvents = new();
    
    // Equality, GetHashCode, domain event management
    internal void ClearDomainEvents() => _domainEvents.Clear();
}
```

### `/DomainEvents` - Business Event System
```
DomainEvents/
├── IDomainEvent.cs           # Event interface
├── DomainEventBase.cs        # Base record for events
├── IDomainEventHandler.cs    # Event handler interface
├── IDomainEventDispatcher.cs # Dispatcher interface
└── DomainEventDispatcher.cs  # Default implementation
```

**DomainEventBase.cs** - Event Record:
```csharp
public abstract record DomainEventBase : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string EventName { get; init; }
    public int Version { get; init; } = 1;
    
    internal DomainEventBase(string eventName) => EventName = eventName;
}
```

### `/BusinessRules` - Domain Validation
```
BusinessRules/
├── IBusinessRule.cs         # Rule interface
├── BusinessRuleBase.cs      # Base implementation
└── CompositeBusinessRule.cs # Rule composition
```

**BusinessRuleBase.cs**:
```csharp
public abstract class BusinessRuleBase : IBusinessRule
{
    public abstract string Message { get; }
    public abstract string Code { get; }
    public abstract bool IsBroken();
    
    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
            throw new BusinessRuleValidationException(rule);
    }
}
```

### `/Specifications` - Query Logic
```
Specifications/
├── ISpecification.cs        # Specification interface
├── Specification.cs         # Base implementation
├── AndSpecification.cs      # Logical AND
├── OrSpecification.cs       # Logical OR
├── NotSpecification.cs      # Logical NOT
└── SpecificationEvaluator.cs # EF Core evaluation
```

### `/Repository` - Data Access Abstractions
```
Repository/
├── IRepository.cs           # Generic repository
├── IReadOnlyRepository.cs   # Read-only operations
├── IUnitOfWork.cs          # Transaction boundary
└── RepositoryBase.cs       # Base implementation
```

### `/Common` - Shared Value Objects
```
Common/
├── Email.cs                # Email value object
├── PhoneNumber.cs          # Phone value object
├── Address.cs              # Address value object
├── Money.cs                # Money with currency
├── Currency.cs             # Currency enumeration
├── DateRange.cs            # Date range value object
├── Percentage.cs           # Percentage value object
└── Url.cs                  # URL value object
```

### `/Exceptions` - Domain-Specific Errors
```
Exceptions/
├── DomainException.cs                    # Base domain exception
├── BusinessRuleValidationException.cs   # Rule violation
├── AggregateNotFoundException.cs        # Entity not found
├── ConcurrencyException.cs              # Optimistic concurrency
├── InvalidOperationDomainException.cs   # Invalid operations
└── InvariantViolationException.cs       # Domain invariant violations
```

### `/Guards` - Input Validation
```
Guards/
├── Guard.cs                # Guard clause implementations
└── GuardExtensions.cs      # Extension methods for validation
```

### `/Services` - Domain Services
```
Services/
├── IDomainService.cs       # Domain service marker interface
└── DomainServiceBase.cs    # Base domain service implementation
```

### `/Validation` - Domain Validation
```
Validation/
├── IDomainValidator.cs     # Domain validator interface
├── DomainValidatorBase.cs  # Base domain validator
├── ValidationError.cs      # Validation error representation
└── ValidationResult.cs     # Validation result with errors
```

### `/Extensions` - Utility Methods
```
Extensions/
├── DomainExtensions.cs     # Domain object extensions
└── JsonExtensions.cs       # JSON serialization extensions
```

**Internal Extension Methods**:
```csharp
internal static class DomainExtensions
{
    internal static void AddDomainEventIfNotExists<T>(this Entity entity, T domainEvent) 
        where T : IDomainEvent;
    
    internal static void SetCreatedAudit(this IAuditableEntity entity, string createdBy);
    internal static void SetModifiedAudit(this IAuditableEntity entity, string modifiedBy);
}
```

## Centralized Build System Benefits

### 1. Automatic Package Management
- **No manual PackageReference entries** needed
- **Version consistency** across all projects
- **Feature-based inclusion** via property flags
- **Security updates** managed centrally

### 2. Clean Architecture Enforcement
- **No dependencies** on other layers (Domain is pure)
- **Automatic project structure** validation
- **Layered architecture** rules enforced

### 3. Modern .NET Features
- **Implicit usings** configured globally
- **Nullable reference types** enabled
- **File-scoped namespaces** supported
- **Records and pattern matching** optimized

### 4. Development Experience
- **XML documentation** generated automatically
- **Code analysis** with custom rules
- **Source Link** for debugging
- **Git integration** with version info

## Key Design Principles

### 1. Immutability & Records
- Value objects use `record` types for structural equality
- Strongly-typed IDs use `readonly struct` for performance
- Domain events are immutable records

### 2. Type Safety
- Eliminate primitive obsession with strongly-typed IDs
- Comprehensive validation in value objects
- Rich domain models with expressive APIs

### 3. Encapsulation
- Internal implementation details
- Public contracts for cross-service communication
- Clear separation of concerns

### 4. Performance
- Struct-based IDs for stack allocation
- Records for efficient equality comparisons
- Minimal allocations in hot paths

### 5. Modern C# Features
- Primary constructors for records
- Init-only properties
- Pattern matching
- Nullable reference types

## Usage Examples

### Creating Strongly-Typed IDs
```csharp
public readonly struct UserId : IStronglyTypedId<Guid>
{
    public Guid Value { get; }
    public UserId(Guid value) => Value = value;
    public static UserId New() => new(Guid.NewGuid());
    // Operators and equality members...
}
```

### Defining Value Objects
```csharp
internal sealed record Username(string Value) : SingleValueObject<string>(Value)
{
    protected static override string ValidateValue(string value)
    {
        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        return value.Trim().ToLowerInvariant();
    }
}
```

### Domain Events
```csharp
public sealed record UserCreatedDomainEvent(
    UserId UserId, 
    Username Username, 
    Email Email
) : DomainEventBase(nameof(UserCreatedDomainEvent));
```

### Business Rules
```csharp
public sealed class UsernameUniqueRule : BusinessRuleBase
{
    private readonly IUserRepository _userRepository;
    private readonly Username _username;
    
    public override string Message => $"Username '{_username}' is already taken";
    public override string Code => "USERNAME_NOT_UNIQUE";
    
    public override bool IsBroken() => _userRepository.ExistsByUsername(_username);
}
```

## Implementation Notes

1. **Dependency Injection**: Register domain services in DI container
2. **Event Handling**: Implement event dispatching in infrastructure layer
3. **Persistence**: Map domain objects to database entities
4. **Validation**: Fail fast with domain exceptions
5. **Testing**: Create builders for complex domain objects
6. **JSON Serialization**: Strongly-typed IDs automatically serialize/deserialize
7. **Build System**: Leverage centralized package management and feature flags

## Integration with Build System

The Domain library integrates seamlessly with the centralized build system:

- **Package Metadata**: Automatically configured with proper PackageId and description
- **Dependencies**: No external dependencies (pure domain layer)
- **Feature Flags**: Optional domain helpers can be enabled
- **Code Quality**: Domain-specific architectural rules enforced
- **Documentation**: XML docs generated and published automatically

Generate this library with full implementations, comprehensive XML documentation, and adherence to these architectural principles while leveraging the centralized build system for maximum developer productivity. 