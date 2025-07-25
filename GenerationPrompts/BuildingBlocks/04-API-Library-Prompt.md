# BuildingBlocks.API Library Generation Prompt

## Context
Generate a comprehensive .NET 8.0 API layer library for the BuildingBlocks architecture using **Minimal APIs** (not controllers). This library provides web API infrastructure, endpoints, and HTTP concerns following modern ASP.NET Core patterns.

## Project Configuration

### Simplified Project File
```xml
<Project Sdk="Microsoft.NET.Sdk.Web"/>
```

**That's it!** All configuration is handled automatically by the centralized build system:

- **Target Framework**: .NET 8.0 (from `Directory.Build.props`)
- **Language Features**: Implicit usings, nullable reference types, warnings as errors
- **Documentation**: XML documentation generation enabled
- **Project References**: `BuildingBlocks.Infrastructure` automatically referenced (gets Application & Domain transitively)
- **Package Management**: All packages automatically included via `Directory.Build.targets`
- **Framework References**: `Microsoft.AspNetCore.App` automatically included
- **Versioning**: Automatic versioning with Git integration
- **Analysis**: Code quality rules from `BuildingBlocks.ruleset`

### Automatic Package Inclusion
The following packages are automatically included via the centralized build system:

**Core API Packages:**
- Microsoft.AspNetCore.OpenApi, Authentication.JwtBearer, Authorization, Cors
- FluentValidation.AspNetCore, FluentValidation.DependencyInjectionExtensions
- Microsoft.Extensions.Diagnostics.HealthChecks
- Serilog.AspNetCore, Microsoft.AspNetCore.ResponseCompression
- Microsoft.AspNetCore.Mvc.Core, Microsoft.AspNetCore.Authorization

**Feature-Based Package Inclusion:**
All additional packages are included based on feature flags:

```xml
<!-- Optional in individual .csproj to enable specific features -->
<PropertyGroup>
  <!-- API Versioning -->
  <IncludeApiVersioning>true</IncludeApiVersioning>             <!-- Asp.Versioning.Http, Asp.Versioning.Mvc.ApiExplorer -->
  
  <!-- OpenAPI/Swagger -->
  <IncludeSwagger>true</IncludeSwagger>                         <!-- Swashbuckle.AspNetCore, Scalar.AspNetCore -->
  
  <!-- Validation -->
  <IncludeValidation>true</IncludeValidation>                   <!-- FluentValidation.AspNetCore -->
  
  <!-- Rate Limiting -->
  <IncludeRateLimiting>true</IncludeRateLimiting>               <!-- AspNetCoreRateLimit -->
  
  <!-- Health Checks -->
  <IncludeHealthChecks>true</IncludeHealthChecks>               <!-- Health check UI, in-memory storage -->
  
  <!-- Monitoring -->
  <IncludeMonitoring>true</IncludeMonitoring>                   <!-- OpenTelemetry, Application Insights -->
  
  <!-- Security -->
  <IncludeSecurity>true</IncludeSecurity>                       <!-- Data Protection, HTTPS Policy -->
  
  <!-- Serialization -->
  <IncludeSerialization>true</IncludeSerialization>             <!-- JSON, Newtonsoft.Json -->
  
  <!-- HTTP Client -->
  <IncludeHttpClient>true</IncludeHttpClient>                   <!-- Polly, HTTP extensions -->
</PropertyGroup>
```

## Project Structure

```
BuildingBlocks.API/
├── Endpoints/
│   └── Base/
│       ├── CrudEndpoints.cs      # Minimal API CRUD extension methods
│       └── QueryEndpoints.cs     # Minimal API query extension methods
├── Extensions/
│   ├── EndpointExtensions.cs     # Common endpoint mapping patterns
│   ├── EndpointRouteBuilderExtensions.cs
│   ├── ServiceCollectionExtensions.cs
│   └── WebApplicationExtensions.cs
├── Middleware/
│   ├── ErrorHandling/
│   │   ├── GlobalExceptionMiddleware.cs
│   │   ├── ProblemDetailsMiddleware.cs    # Custom RFC 7807 implementation
│   │   ├── ErrorResponse.cs
│   │   └── ProblemDetailsFactory.cs
│   └── Logging/
│       ├── RequestLoggingMiddleware.cs
│       └── CorrelationIdMiddleware.cs
├── Responses/
│   └── Base/
│       ├── ApiResponse.cs
│       ├── PagedResponse.cs
│       └── Builders/
│           └── ApiResponseBuilder.cs
├── Configuration/
│   └── Options/
│       └── ApiOptions.cs
└── Utilities/
    ├── ApiConstants.cs
    └── Constants/
        └── ApiConstants.cs
```

## Key Requirements

### 1. Minimal APIs Focus
- **NO Controllers**: Use Minimal APIs exclusively
- Extension methods for mapping endpoints
- Route grouping and organization
- Parameter binding from routes, query strings, and JSON bodies
- Strongly typed ID support in route parameters

### 2. CRUD Endpoint Extensions
```csharp
// Extension method approach
public static class CrudEndpoints
{
    public static IEndpointRouteBuilder MapCrudEndpoints<TEntity, TId, TDto, TCreateDto, TUpdateDto>(
        this IEndpointRouteBuilder endpoints,
        string routePrefix,
        string? tag = null)
        where TId : IStronglyTypedId
    {
        var group = endpoints.MapGroup(routePrefix);
        
        // GET /{id} - Get by ID
        group.MapGet("/{id}", async (TId id, IMediator mediator, CancellationToken ct) => 
        {
            var query = new GetByIdQuery<TEntity, TId> { Id = id };
            var result = await mediator.QueryAsync<GetByIdQuery<TEntity, TId>, TDto?>(query, ct);
            
            return result != null 
                ? Results.Ok(ApiResponse.Success(result))
                : Results.NotFound(ApiResponse.NotFound<TDto>($"Entity with ID {id} not found"));
        })
        .WithName($"Get{typeof(TEntity).Name}ById")
        .WithTags(tag ?? typeof(TEntity).Name)
        .Produces<ApiResponse<TDto>>()
        .Produces<ApiResponse<TDto>>(StatusCodes.Status404NotFound);
        
        // GET / - Get all with pagination
        group.MapGet("/", async ([AsParameters] PaginationQuery pagination, IMediator mediator, CancellationToken ct) => 
        {
            var query = new GetPagedQuery<TEntity> 
            { 
                Page = pagination.Page, 
                PageSize = pagination.PageSize,
                SortBy = pagination.SortBy,
                SortDirection = pagination.SortDirection
            };
            
            var result = await mediator.QueryAsync<GetPagedQuery<TEntity>, PagedResult<TDto>>(query, ct);
            return Results.Ok(PagedResponse.Success(result.Items, result.TotalCount, pagination.Page, pagination.PageSize));
        })
        .WithName($"Get{typeof(TEntity).Name}List")
        .WithTags(tag ?? typeof(TEntity).Name)
        .Produces<PagedResponse<TDto>>();
        
        // POST / - Create
        group.MapPost("/", async (TCreateDto createDto, IMediator mediator, CancellationToken ct) =>
        {
            var command = new CreateCommand<TEntity, TCreateDto> { Data = createDto };
            var result = await mediator.SendAsync<CreateCommand<TEntity, TCreateDto>, TDto>(command, ct);
            
            return Results.Created($"/{routePrefix}/{result.Id}", ApiResponse.Success(result));
        })
        .WithName($"Create{typeof(TEntity).Name}")
        .WithTags(tag ?? typeof(TEntity).Name)
        .Produces<ApiResponse<TDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<TDto>>(StatusCodes.Status400BadRequest);
        
        // PUT /{id} - Update
        group.MapPut("/{id}", async (TId id, TUpdateDto updateDto, IMediator mediator, CancellationToken ct) =>
        {
            var command = new UpdateCommand<TEntity, TId, TUpdateDto> { Id = id, Data = updateDto };
            var result = await mediator.SendAsync<UpdateCommand<TEntity, TId, TUpdateDto>, TDto>(command, ct);
            
            return Results.Ok(ApiResponse.Success(result));
        })
        .WithName($"Update{typeof(TEntity).Name}")
        .WithTags(tag ?? typeof(TEntity).Name)
        .Produces<ApiResponse<TDto>>()
        .Produces<ApiResponse<TDto>>(StatusCodes.Status404NotFound);
        
        // DELETE /{id} - Delete
        group.MapDelete("/{id}", async (TId id, IMediator mediator, CancellationToken ct) =>
        {
            var command = new DeleteCommand<TEntity, TId> { Id = id };
            await mediator.SendAsync(command, ct);
            
            return Results.NoContent();
        })
        .WithName($"Delete{typeof(TEntity).Name}")
        .WithTags(tag ?? typeof(TEntity).Name)
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
        
        return endpoints;
    }
}
```

### 3. Query Endpoint Extensions
```csharp
public static class QueryEndpoints
{
    public static IEndpointRouteBuilder MapQuery<TQuery, TResult>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<HttpContext, TQuery> createQuery,
        string method = "GET")
        where TQuery : IQuery<TResult>
    {
        var routeBuilder = method.ToUpper() switch
        {
            "GET" => endpoints.MapGet(pattern, async (HttpContext context, IMediator mediator, CancellationToken ct) =>
            {
                var query = createQuery(context);
                var result = await mediator.QueryAsync<TQuery, TResult>(query, ct);
                return Results.Ok(ApiResponse.Success(result));
            }),
            "POST" => endpoints.MapPost(pattern, async (HttpContext context, IMediator mediator, CancellationToken ct) =>
            {
                var query = createQuery(context);
                var result = await mediator.QueryAsync<TQuery, TResult>(query, ct);
                return Results.Ok(ApiResponse.Success(result));
            }),
            _ => throw new ArgumentException($"Unsupported HTTP method: {method}")
        };

        return endpoints;
    }
    
    public static IEndpointRouteBuilder MapPagedQuery<TQuery, TResult>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<HttpContext, TQuery> createQuery)
        where TQuery : IQuery<PagedResult<TResult>>
    {
        endpoints.MapGet(pattern, async (HttpContext context, IMediator mediator, CancellationToken ct) =>
        {
            var query = createQuery(context);
            var result = await mediator.QueryAsync<TQuery, PagedResult<TResult>>(query, ct);
            
            // Extract pagination info from query or context
            var page = 1; // Extract from query
            var pageSize = 10; // Extract from query
            
            return Results.Ok(PagedResponse.Success(result.Items, result.TotalCount, page, pageSize));
        })
        .Produces<PagedResponse<TResult>>();

        return endpoints;
    }
    
    public static IEndpointRouteBuilder MapSearchEndpoint<TQuery, TResult, TFilter>(
        this IEndpointRouteBuilder endpoints,
        string pattern)
        where TQuery : IQuery<IEnumerable<TResult>>, new()
    {
        endpoints.MapPost(pattern, async (TFilter filter, IMediator mediator, CancellationToken ct) =>
        {
            var query = new TQuery(); // Map filter to query
            var result = await mediator.QueryAsync<TQuery, IEnumerable<TResult>>(query, ct);
            return Results.Ok(ApiResponse.Success(result));
        })
        .WithName($"Search{typeof(TResult).Name}")
        .Produces<ApiResponse<IEnumerable<TResult>>>();

        return endpoints;
    }
}
```

### 4. Strongly Typed ID Support
```csharp
// Automatic JSON conversion and route parameter binding
public static class StronglyTypedIdExtensions
{
    public static void ConfigureStronglyTypedIds(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new GuidIdJsonConverterFactory());
            options.SerializerOptions.Converters.Add(new IntIdJsonConverterFactory());
            options.SerializerOptions.Converters.Add(new StringIdJsonConverterFactory());
        });
    }
    
    public static bool TryParse<T>(string value, out T result) where T : IStronglyTypedId
    {
        // Implementation for route parameter binding
        result = default!;
        return false; // Implementation details...
    }
}
```

### 5. Custom Problem Details Middleware
```csharp
public class ProblemDetailsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ProblemDetailsMiddleware> _logger;
    private readonly ProblemDetailsOptions _options;

    public ProblemDetailsMiddleware(
        RequestDelegate next,
        ILogger<ProblemDetailsMiddleware> logger,
        IOptions<ProblemDetailsOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = _options.MapException(exception);
        
        // Add correlation ID
        if (context.TraceIdentifier != null)
        {
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        }

        context.Response.StatusCode = problemDetails.Status ?? 500;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        
        _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
    }
}
```

### 6. Service Registration
```csharp
public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
{
    // Add Minimal API services (NO AddControllers)
    services.AddEndpointsApiExplorer();
    
    // Add custom Problem Details
    services.AddProblemDetailsMiddleware(options => 
    {
        options.MapStandardExceptions();
        options.IncludeExceptionDetails = configuration.GetValue<bool>("ProblemDetails:IncludeExceptionDetails");
    });
    
    // Configure JSON for strongly typed IDs
    services.ConfigureStronglyTypedIds();
    
    // Add CORS
    services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
    
    // Add health checks (if enabled)
    if (IsFeatureEnabled("IncludeHealthChecks"))
    {
        services.AddHealthChecks();
    }
    
    // Add API versioning (if enabled)
    if (IsFeatureEnabled("IncludeApiVersioning"))
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader("version"),
                new HeaderApiVersionReader("X-Version")
            );
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }
    
    return services;
}
```

### 7. Pipeline Configuration
```csharp
public static WebApplication UseApi(this WebApplication app)
{
    // Problem Details middleware (first in pipeline)
    app.UseProblemDetailsMiddleware();
    
    // Request logging
    app.UseRequestLogging();
    app.UseCorrelationId();
    
    // Security
    app.UseHttpsRedirection();
    app.UseCors();
    
    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();
    
    // Health checks (if enabled)
    if (IsFeatureEnabled("IncludeHealthChecks"))
    {
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/ready");
        app.MapHealthChecks("/health/live");
    }
    
    // Swagger (if enabled and in development)
    if (IsFeatureEnabled("IncludeSwagger") && app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapScalarApiReference(); // Modern Swagger alternative
    }
    
    // NO MapControllers - endpoints mapped by consuming applications
    return app;
}
```

## Centralized Build System Benefits

### 1. Automatic Package Management
- **Zero configuration**: All API packages included automatically
- **Feature flags**: Only include what you need (Swagger, versioning, etc.)
- **Consistent versions**: All API projects use same package versions
- **Security updates**: Centrally managed and automatically applied

### 2. Clean Architecture Integration
- **Infrastructure reference**: `BuildingBlocks.Infrastructure` automatically referenced
- **Transitive dependencies**: Gets Application and Domain layers automatically
- **Framework references**: ASP.NET Core Web framework automatically included
- **Architectural rules**: API-specific patterns enforced by custom analyzers

### 3. Modern ASP.NET Core Features
- **Minimal APIs**: Optimized for performance and simplicity
- **Strongly typed IDs**: Seamless route parameter binding and JSON serialization
- **Problem Details**: RFC 7807 compliant error responses
- **OpenAPI integration**: Automatic documentation generation

## Key Features

### 1. Minimal API Patterns
- Route grouping with `MapGroup()`
- Extension methods for common patterns
- Parameter binding with `[AsParameters]`
- OpenAPI integration with `WithTags()`, `WithSummary()`, `Produces<>()`

### 2. Strongly Typed ID Integration
- Automatic route parameter conversion: `/users/{userId}` where `UserId` is strongly typed
- JSON serialization/deserialization with custom converters
- Query string parameter binding support

### 3. Standardized Responses
- `ApiResponse<T>` for single items with success/error states
- `PagedResponse<T>` for paginated results with metadata
- Consistent error responses via Problem Details middleware

### 4. Error Handling
- Custom Problem Details middleware (no external dependencies)
- Exception to HTTP status mapping with extensible configuration
- Correlation ID tracking for request tracing
- Development vs production error details

### 5. OpenAPI/Swagger Integration
- Automatic documentation generation for all endpoints
- Type-safe endpoint definitions with proper status codes
- API versioning support with proper documentation

## Usage Pattern

```csharp
// In consuming application's Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add BuildingBlocks layers (automatic references)
builder.Services.AddDomain();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

// Configure pipeline
app.UseApi();

// Map endpoints using BuildingBlocks extensions
app.MapGroup("/api/v1")
   .MapCrudEndpoints<User, UserId, UserDto, CreateUserRequest, UpdateUserRequest>("users")
   .MapCrudEndpoints<Product, ProductId, ProductDto, CreateProductRequest, UpdateProductRequest>("products");

// Map custom endpoints
app.MapGroup("/api/v1/users")
   .MapGet("/search", async ([AsParameters] UserSearchQuery query, IMediator mediator) =>
   {
       var result = await mediator.QueryAsync<UserSearchQuery, IEnumerable<UserDto>>(query);
       return Results.Ok(ApiResponse.Success(result));
   })
   .WithTags("Users")
   .WithSummary("Search users by criteria");

app.Run();
```

## Integration with Build System

The API library integrates seamlessly with the centralized build system:

- **Package Metadata**: Automatically configured with proper PackageId and description
- **Dependencies**: Infrastructure layer automatically referenced (gets Application & Domain transitively)
- **Feature Flags**: Comprehensive set of optional API packages (Swagger, versioning, rate limiting)
- **Framework References**: ASP.NET Core Web framework automatically included
- **Code Quality**: API-specific architectural rules enforced
- **Documentation**: XML docs generated and published automatically
- **Modern Packages**: Uses latest ASP.NET Core packages (Asp.Versioning.*, Scalar.AspNetCore)

## Critical Requirements

1. **Minimal APIs Only**: No controllers, no MVC dependencies beyond what's required for JSON serialization
2. **Extension Method Pattern**: All endpoint mapping via extension methods for reusability
3. **Strongly Typed ID Support**: Seamless integration with BuildingBlocks strongly typed IDs
4. **Custom Problem Details**: No external Problem Details dependencies - custom RFC 7807 implementation
5. **Performance Focus**: Leverage Minimal API performance benefits over traditional controllers
6. **Clean Architecture**: Maintain separation of concerns with clear layer boundaries
7. **Feature Flags**: All optional features controlled via MSBuild properties
8. **Zero Configuration**: Everything works out of the box with centralized build system

Generate a production-ready API layer that leverages the full power of Minimal APIs while maintaining the BuildingBlocks architecture principles with zero configuration overhead. 