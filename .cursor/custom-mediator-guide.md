# Custom Mediator Usage Guide

## Overview
BuildingBlocks uses a custom mediator implementation instead of MediatR. This provides clear separation between Commands, Queries, and Events with type-safe method signatures.

## Mediator Interface

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

## Handler Interfaces

### Command Handlers
```csharp
// For commands without return value
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

// For commands with return value
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
```

### Query Handlers
```csharp
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
```

### Event Handlers
```csharp
public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent eventItem, CancellationToken cancellationToken = default);
}
```

## Usage Examples

### 1. Commands

#### Define Commands
```csharp
// Command without return value
public sealed record DeleteUserCommand(UserId UserId) : CommandBase;

// Command with return value
public sealed record CreateUserCommand(
    string Username, 
    string Email
) : CommandBase<UserDto>;
```

#### Implement Command Handlers
```csharp
// Void command handler
public sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    public async Task HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        // Implementation
    }
}

// Command handler with return value
public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Implementation
        return userDto;
    }
}
```

#### Use Commands in Controllers/Endpoints
```csharp
// Void command
await _mediator.SendAsync(new DeleteUserCommand(userId));

// Command with return value
var result = await _mediator.SendAsync<CreateUserCommand, UserDto>(command);
```

### 2. Queries

#### Define Queries
```csharp
public sealed record GetUserQuery(UserId UserId) : QueryBase<UserDto>;

public sealed record GetUsersQuery(
    int Page = 1, 
    int PageSize = 10
) : PagedQuery<UserDto>;
```

#### Implement Query Handlers
```csharp
public sealed class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
{
    public async Task<UserDto> HandleAsync(GetUserQuery query, CancellationToken cancellationToken)
    {
        // Implementation
        return userDto;
    }
}
```

#### Use Queries in Controllers/Endpoints
```csharp
var user = await _mediator.QueryAsync<GetUserQuery, UserDto>(query);

var users = await _mediator.QueryAsync<GetUsersQuery, PagedResult<UserDto>>(query);
```

### 3. Events

#### Define Events
```csharp
public sealed record UserCreatedEvent(
    UserId UserId,
    string Username,
    DateTime CreatedAt
) : DomainEventBase(nameof(UserCreatedEvent));
```

#### Implement Event Handlers
```csharp
public sealed class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public async Task HandleAsync(UserCreatedEvent eventItem, CancellationToken cancellationToken)
    {
        // Implementation (e.g., send welcome email)
    }
}

// Multiple handlers for same event
public sealed class UserCreatedAuditHandler : IEventHandler<UserCreatedEvent>
{
    public async Task HandleAsync(UserCreatedEvent eventItem, CancellationToken cancellationToken)
    {
        // Implementation (e.g., audit logging)
    }
}
```

#### Publish Events
```csharp
await _mediator.PublishAsync(new UserCreatedEvent(userId, username, DateTime.UtcNow));
```

## Key Differences from MediatR

| Aspect | MediatR | Custom Mediator |
|--------|---------|-----------------|
| Command Method | `Send()` | `SendAsync()` |
| Query Method | `Send()` | `QueryAsync()` |
| Event Method | `Publish()` | `PublishAsync()` |
| Handler Method | `Handle()` | `HandleAsync()` |
| Type Safety | Single `Send()` method | Separate methods for Commands/Queries |
| Async by Default | ✅ | ✅ |
| Multiple Event Handlers | ✅ | ✅ |

## Benefits of Custom Mediator

1. **Clear Intent**: Separate methods make it obvious whether you're sending a command or query
2. **Type Safety**: Explicit type parameters prevent runtime errors
3. **IntelliSense**: Better IDE support with specific method signatures
4. **Performance**: No reflection overhead for method resolution
5. **Async First**: All methods are async by design
6. **Simplicity**: Fewer dependencies and simpler implementation

## Code Snippets Reference

Use these Cursor snippets for rapid development:

**CQRS & Application Layer:**
- `bb-commandhandler` - Command handler with return value
- `bb-voidcommandhandler` - Command handler without return value
- `bb-queryhandler` - Query handler
- `bb-eventhandler` - Event handler
- `bb-command` - Command definition
- `bb-query` - Query definition
- `bb-domainevent` - Domain event definition

**Domain Layer Components:**
- `bb-domainservice` - Domain service interface and implementation
- `bb-factory` - Factory interface and implementation
- `bb-policy` - Policy interface and implementation
- `bb-guard` - Guard clause for null checks
- `bb-guard-string` - Guard clause for string validation
- `bb-extendedvalueobject` - Extended value object with validation
- `bb-invariant` - Domain invariant implementation
- `bb-domainvalidator` - Domain validator

## Registration Example

```csharp
// In ServiceCollectionExtensions
services.AddScoped<IMediator, Mediator>();

// Register handlers
services.AddScoped<ICommandHandler<CreateUserCommand, UserDto>, CreateUserCommandHandler>();
services.AddScoped<IQueryHandler<GetUserQuery, UserDto>, GetUserQueryHandler>();
services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
```

This custom mediator provides a clean, type-safe, and performant CQRS implementation tailored specifically for BuildingBlocks architecture. 