# BuildingBlocks Minimal API Usage

This document demonstrates how to use the BuildingBlocks library with ASP.NET Core Minimal APIs.

## Overview

The BuildingBlocks library is fully configured to work with Minimal APIs, providing extension methods for common patterns like CRUD operations, queries, and commands while maintaining strongly typed ID support and proper error handling.

## Key Components

### 1. CrudEndpoints Extension Methods
Automatically maps CRUD operations for entities using Minimal APIs.

### 2. QueryEndpoints Extension Methods  
Maps query operations including pagination, search, and reporting.

### 3. EndpointExtensions
Provides helpers for mapping commands and configuring endpoint groups.

### 4. Strongly Typed ID Support
Automatic JSON serialization/deserialization in Minimal API parameters and responses.

### 5. Problem Details Middleware
RFC 7807 compliant error responses for all endpoints.

## Basic Setup

### Program.cs Configuration

```csharp
using BuildingBlocks.Domain.Extensions;
using BuildingBlocks.Application.Extensions;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add BuildingBlocks layers
builder.Services.AddDomain();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

// Configure pipeline for Minimal APIs
if (app.Environment.IsDevelopment())
{
    app.UseDevelopmentApi();
}

app.UseApi(app.Environment);

// Map your endpoints here
app.MapUserEndpoints();

app.Run();
```

## CRUD Endpoints Example

### Entity and DTOs

```csharp
// Strongly typed ID
[StronglyTypedId(typeof(Guid))]
public readonly struct UserId : IStronglyTypedId<Guid>
{
    public Guid Value { get; }
    public UserId(Guid value) => Value = value;
    // ... other implementations
}

// Entity
public class User
{
    public UserId Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

// DTOs
public class UserDto
{
    public UserId Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class UpdateUserRequest
{
    public string? Email { get; set; }
    public string? Name { get; set; }
}
```

### Commands and Queries

```csharp
// Commands
public class CreateUserCommand : ICommand<UserDto>
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class UpdateUserCommand : ICommand<UserDto>
{
    public UserId Id { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
}

public class DeleteUserCommand : ICommand<bool>
{
    public UserId Id { get; set; }
}

// Queries
public class GetUserByIdQuery : IQuery<UserDto?>
{
    public UserId Id { get; set; }
}

public class GetUsersQuery : PagedQuery<UserDto>
{
    public string? SearchTerm { get; set; }
}
```

### Endpoint Mapping

```csharp
public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapApiGroup("/api/v1/users", ConfigureUserEndpoints, "Users");
    }

    private static void ConfigureUserEndpoints(RouteGroupBuilder group)
    {
        // GET /api/v1/users/{id}
        group.MapGet("/{id}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .Produces<ApiResponse<UserDto>>(200)
            .Produces<ApiResponse<UserDto>>(404);

        // GET /api/v1/users
        group.MapGet("/", GetUsers)
            .WithName("GetUsers")
            .WithSummary("Get paginated list of users")
            .Produces<PagedResponse<UserDto>>(200);

        // POST /api/v1/users
        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create new user")
            .Produces<ApiResponse<UserDto>>(201)
            .Produces<ApiResponse<UserDto>>(400);

        // PUT /api/v1/users/{id}
        group.MapPut("/{id}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Update user")
            .Produces<ApiResponse<UserDto>>(200)
            .Produces<ApiResponse<UserDto>>(400)
            .Produces<ApiResponse<UserDto>>(404);

        // DELETE /api/v1/users/{id}
        group.MapDelete("/{id}", DeleteUser)
            .WithName("DeleteUser")
            .WithSummary("Delete user")
            .Produces(204)
            .Produces<ApiResponse<object>>(404);

        // GET /api/v1/users/search
        group.MapGet("/search", SearchUsers)
            .WithName("SearchUsers")
            .WithSummary("Search users")
            .Produces<PagedResponse<UserDto>>(200);
    }

    private static async Task<IResult> GetUserById(
        UserId id, // Automatically converted from route parameter
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery { Id = id };
        var result = await mediator.QueryAsync<GetUserByIdQuery, UserDto?>(query, cancellationToken);
        
        return result != null 
            ? Results.Ok(ApiResponse.Success(result))
            : Results.NotFound(ApiResponse.NotFound<UserDto>($"User with ID {id} not found"));
    }

    private static async Task<IResult> GetUsers(
        [AsParameters] PaginationQuery pagination,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery
        {
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            SortBy = pagination.SortBy,
            SortDescending = pagination.SortDescending
        };
        
        var result = await mediator.QueryAsync<GetUsersQuery, PagedResult<UserDto>>(query, cancellationToken);
        
        return Results.Ok(PagedResponse.Success(result.Items, result.TotalCount, result.PageNumber, result.PageSize));
    }

    private static async Task<IResult> CreateUser(
        CreateUserRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand
        {
            Email = request.Email,
            Name = request.Name
        };
        
        var result = await mediator.CommandAsync<CreateUserCommand, UserDto>(command, cancellationToken);
        
        return Results.Created($"/api/v1/users/{result.Id}", ApiResponse.Success(result));
    }

    private static async Task<IResult> UpdateUser(
        UserId id, // Automatically converted from route parameter
        UpdateUserRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand
        {
            Id = id,
            Email = request.Email,
            Name = request.Name
        };
        
        var result = await mediator.CommandAsync<UpdateUserCommand, UserDto>(command, cancellationToken);
        
        return Results.Ok(ApiResponse.Success(result));
    }

    private static async Task<IResult> DeleteUser(
        UserId id, // Automatically converted from route parameter
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand { Id = id };
        var result = await mediator.CommandAsync<DeleteUserCommand, bool>(command, cancellationToken);
        
        return result ? Results.NoContent() : Results.NotFound(ApiResponse.NotFound<object>($"User with ID {id} not found"));
    }

    private static async Task<IResult> SearchUsers(
        string? searchTerm,
        [AsParameters] PaginationQuery pagination,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery
        {
            SearchTerm = searchTerm,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            SortBy = pagination.SortBy,
            SortDescending = pagination.SortDescending
        };
        
        var result = await mediator.QueryAsync<GetUsersQuery, PagedResult<UserDto>>(query, cancellationToken);
        
        return Results.Ok(PagedResponse.Success(result.Items, result.TotalCount, result.PageNumber, result.PageSize));
    }
}
```

## Request/Response Examples

### Create User
```http
POST /api/v1/users
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "name": "John Doe"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "john.doe@example.com",
    "name": "John Doe"
  },
  "message": "Success",
  "timestamp": "2023-10-15T10:30:00Z"
}
```

### Get User by ID
```http
GET /api/v1/users/550e8400-e29b-41d4-a716-446655440000
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "john.doe@example.com",
    "name": "John Doe"
  },
  "message": "Success",
  "timestamp": "2023-10-15T10:30:00Z"
}
```

### Get Users with Pagination
```http
GET /api/v1/users?pageNumber=1&pageSize=10&sortBy=name&sortDescending=false
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "john.doe@example.com",
      "name": "John Doe"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalPages": 1,
    "totalCount": 1,
    "hasPrevious": false,
    "hasNext": false
  },
  "message": "Success",
  "timestamp": "2023-10-15T10:30:00Z"
}
```

## Error Handling

All endpoints automatically handle errors through the Problem Details middleware:

### Validation Error Example
```http
POST /api/v1/users
Content-Type: application/json

{
  "email": "invalid-email",
  "name": ""
}
```

**Response (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "One or more validation errors occurred",
  "instance": "/api/v1/users",
  "correlationId": "550e8400-e29b-41d4-a716-446655440000",
  "timestamp": "2023-10-15T10:30:00Z",
  "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
  "errors": {
    "Email": ["Email must be a valid email address"],
    "Name": ["Name is required"]
  }
}
```

### Not Found Example
```http
GET /api/v1/users/00000000-0000-0000-0000-000000000000
```

**Response (404 Not Found):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "User with ID 00000000-0000-0000-0000-000000000000 not found",
  "instance": "/api/v1/users/00000000-0000-0000-0000-000000000000",
  "correlationId": "550e8400-e29b-41d4-a716-446655440000",
  "timestamp": "2023-10-15T10:30:00Z",
  "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01"
}
```

## Benefits of This Approach

1. **Strongly Typed IDs**: Automatic conversion in route parameters and JSON
2. **Consistent Responses**: Standardized `ApiResponse<T>` and `PagedResponse<T>` formats
3. **Error Handling**: RFC 7807 compliant Problem Details for all errors
4. **Performance**: Minimal API overhead with BuildingBlocks abstractions
5. **Maintainability**: Clean separation of concerns with CQRS pattern
6. **Documentation**: Automatic OpenAPI/Swagger documentation generation
7. **Validation**: Integrated validation with clear error messages

This approach gives you all the benefits of the BuildingBlocks architecture while leveraging the performance and simplicity of Minimal APIs. 