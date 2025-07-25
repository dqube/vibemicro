# Strongly Typed ID JSON Converters

This document demonstrates how to use the strongly typed ID JSON converters in the BuildingBlocks library.

## Overview

The JSON converters provide seamless serialization and deserialization of strongly typed IDs to/from JSON, ensuring your APIs work correctly with these custom types.

## Components

### 1. StronglyTypedIdJsonConverter<TId, TValue>
Generic converter for specific strongly typed ID types.

### 2. StronglyTypedIdJsonConverterFactory
Factory that automatically creates converters for any strongly typed ID type.

### 3. StronglyTypedIdAttribute
Attribute to mark strongly typed IDs (optional but recommended).

### 4. JsonExtensions
Extension methods for easy configuration.

## Usage Examples

### 1. Basic Strongly Typed ID with JSON Support

```csharp
[StronglyTypedId(typeof(Guid))]
[JsonConverter(typeof(StronglyTypedIdJsonConverterFactory))]
public readonly struct UserId : IStronglyTypedId<Guid>
{
    public Guid Value { get; }
    
    public UserId(Guid value) => Value = value;
    
    public static UserId New() => new(Guid.NewGuid());
    public static UserId From(Guid value) => new(value);
    
    // ... other implementations
}
```

### 2. Configuring JSON Options (ASP.NET Core)

```csharp
// In Program.cs or Startup.cs
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.AddStronglyTypedIdConverters();
    });

// Or use the extension method
services.ConfigureJsonOptionsForStronglyTypedIds();
```

### 3. Manual JSON Serialization

```csharp
var user = new User
{
    Id = UserId.New(),
    Name = "John Doe"
};

// Serialize with strongly typed ID support
string json = JsonExtensions.SerializeWithStronglyTypedIds(user);
// Output: {"id":"550e8400-e29b-41d4-a716-446655440000","name":"John Doe"}

// Deserialize with strongly typed ID support
var deserializedUser = JsonExtensions.DeserializeWithStronglyTypedIds<User>(json);
```

### 4. API Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(UserId id) // Automatically converted from JSON
    {
        // id is properly deserialized as UserId
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }
    
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserRequest request)
    {
        var userId = UserId.New();
        // ... create user logic
        
        return CreatedAtAction(nameof(GetUser), new { id = userId }, userDto);
        // userId is automatically serialized as GUID string in JSON
    }
}
```

### 5. DTO Example

```csharp
public class UserDto
{
    public UserId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public RoleId RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
}

// JSON representation:
// {
//   "id": "550e8400-e29b-41d4-a716-446655440000",
//   "name": "John Doe", 
//   "roleId": "660e8400-e29b-41d4-a716-446655440000",
//   "createdAt": "2023-10-15T10:30:00Z"
// }
```

### 6. Configuration Options

```csharp
// Custom JSON options
services.ConfigureJsonOptionsForStronglyTypedIds(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.WriteIndented = true;
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Create custom options for manual serialization
var options = JsonExtensions.CreateOptionsForStronglyTypedIds(opt =>
{
    opt.WriteIndented = true;
});
```

### 7. Multiple ID Types

```csharp
// All these types work automatically with the converter factory:

[StronglyTypedId(typeof(Guid))]
public readonly struct UserId : IStronglyTypedId<Guid> { /* ... */ }

[StronglyTypedId(typeof(int))]
public readonly struct ProductId : IStronglyTypedId<int> { /* ... */ }

[StronglyTypedId(typeof(string))]
public readonly struct CategoryCode : IStronglyTypedId<string> { /* ... */ }

[StronglyTypedId(typeof(long))]
public readonly struct OrderNumber : IStronglyTypedId<long> { /* ... */ }
```

## Benefits

1. **Automatic Conversion**: No manual serialization code needed
2. **Type Safety**: Strongly typed IDs maintain their type safety in APIs
3. **Clean JSON**: IDs serialize as their underlying values (not wrapped objects)
4. **Performance**: Efficient conversion using compiled expressions
5. **Flexibility**: Works with any strongly typed ID implementation

## Registration in Domain Layer

The JSON converters are automatically registered when you call:

```csharp
// In Domain layer
services.AddDomain();

// Or manually
services.ConfigureJsonOptionsForStronglyTypedIds();
```

## Error Handling

The converters handle common scenarios:

- **Null Values**: Throws `JsonException` for non-nullable strongly typed IDs
- **Invalid Values**: Delegates to underlying type's parsing logic
- **Type Mismatches**: Provides clear error messages

## Performance Considerations

- Converters use reflection caching for optimal performance
- Factory pattern minimizes converter instance creation
- Compatible with System.Text.Json's high-performance serialization

This implementation ensures your strongly typed IDs work seamlessly with JSON serialization in APIs, maintaining both type safety and clean JSON representations. 