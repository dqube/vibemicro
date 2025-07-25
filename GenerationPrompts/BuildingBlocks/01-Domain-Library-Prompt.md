# BuildingBlocks.Domain Library Generation Prompt

## Overview
Generate a comprehensive Domain layer library following Domain-Driven Design (DDD) principles. This library provides foundational building blocks for microservices architecture using modern C# features including records and readonly structs.

## Project Configuration

### Target Framework & Features
- **.NET 8.0** (`net8.0`)
- **Implicit Usings**: Enabled
- **Nullable Reference Types**: Enabled
- **Treat Warnings as Errors**: Enabled
- **Generate Documentation File**: Enabled

### Package Dependencies
```xml
<PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
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
└── StringId.cs            # String-based ID struct
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
└── DateRange.cs            # Date range value object
```

### `/Exceptions` - Domain-Specific Errors
```
Exceptions/
├── DomainException.cs                    # Base domain exception
├── BusinessRuleValidationException.cs   # Rule violation
├── AggregateNotFoundException.cs        # Entity not found
├── ConcurrencyException.cs              # Optimistic concurrency
└── InvalidOperationDomainException.cs   # Invalid operations
```

### `/Extensions` - Utility Methods
```
Extensions/
└── DomainExtensions.cs     # Domain object extensions
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
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be empty");
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

Generate this library with full implementations, comprehensive XML documentation, and adherence to these architectural principles. 