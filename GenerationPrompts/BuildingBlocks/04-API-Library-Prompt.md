# BuildingBlocks.API Library Generation Prompt

## Context
Generate a comprehensive .NET 8.0 API layer library for the BuildingBlocks architecture using **Minimal APIs** (not controllers). This library provides web API infrastructure, endpoints, and HTTP concerns following modern ASP.NET Core patterns.

## Project Structure

```
BuildingBlocks.API/
├── BuildingBlocks.API.csproj
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
│       └── ApiResponseBuilder.cs
├── Configuration/
│   └── Options/
│       └── ApiOptions.cs
└── Utilities/
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
    {
        var group = endpoints.MapGroup(routePrefix);
        
        // GET /{id} - Get by ID
        group.MapGet("/{id}", async (TId id, IMediator mediator, CancellationToken ct) => 
        {
            // Implementation
        });
        
        // GET / - Get all with pagination
        group.MapGet("/", async ([AsParameters] PaginationQuery pagination, IMediator mediator, CancellationToken ct) => 
        {
            // Implementation
        });
        
        // POST / - Create
        // PUT /{id} - Update  
        // DELETE /{id} - Delete
        
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
    {
        // Implementation
    }
    
    public static IEndpointRouteBuilder MapPagedQuery<TQuery, TResult>(...)
    public static IEndpointRouteBuilder MapSearchEndpoint<TQuery, TResult, TFilter>(...)
}
```

### 4. Strongly Typed ID Support
- Automatic conversion in route parameters: `/users/{userId}` where `UserId` is strongly typed
- JSON serialization/deserialization support
- Query string parameter binding

### 5. Custom Problem Details Middleware
- **NO Hellang.Middleware.ProblemDetails dependency**
- Custom RFC 7807 compliant implementation
- Exception to status code mapping
- Development vs production error details
- Correlation ID support

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
    });
    
    // Configure JSON for strongly typed IDs
    services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.AddStronglyTypedIdConverters();
    });
    
    // Other services...
}
```

### 7. Pipeline Configuration
```csharp
public static IApplicationBuilder UseApi(this IApplicationBuilder app, IWebHostEnvironment environment)
{
    // Problem Details middleware
    app.UseProblemDetailsMiddleware();
    
    // Other middleware...
    
    // NO MapControllers - endpoints mapped by consuming applications
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/health");
        // Consuming apps will add: endpoints.MapUserEndpoints();
    });
}
```

## Package References

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>

  <ItemGroup>
    <!-- Core ASP.NET Core -->
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
    <PackageReference Include="Microsoft.AspNetCore.Authorization" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Versioning" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer" />

    <!-- OpenAPI/Swagger -->
    <PackageReference Include="Swashbuckle.AspNetCore" />
    <PackageReference Include="Swashbuckle.AspNetCore.Annotations" />
    <PackageReference Include="Swashbuckle.AspNetCore.Filters" />
    <PackageReference Include="Scalar.AspNetCore" />

    <!-- Validation -->
    <PackageReference Include="FluentValidation.AspNetCore" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" />

    <!-- Rate Limiting -->
    <PackageReference Include="Microsoft.AspNetCore.RateLimiting" />
    <PackageReference Include="AspNetCoreRateLimit" />

    <!-- Health Checks -->
    <PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks" />
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics.HealthChecks" />
    <PackageReference Include="AspNetCore.HealthChecks.UI" />
    <PackageReference Include="AspNetCore.HealthChecks.UI.InMemory.Storage" />

    <!-- Monitoring & Observability -->
    <PackageReference Include="OpenTelemetry.AspNetCore" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" />

    <!-- CORS -->
    <PackageReference Include="Microsoft.AspNetCore.Cors" />

    <!-- Security -->
    <PackageReference Include="Microsoft.AspNetCore.DataProtection" />
    <PackageReference Include="Microsoft.AspNetCore.HttpsPolicy" />

    <!-- Serialization -->
    <PackageReference Include="System.Text.Json" />
    <PackageReference Include="Newtonsoft.Json" />

    <!-- Utilities -->
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" />
    <PackageReference Include="Microsoft.Extensions.Options.ConfigurationExtensions" />

    <!-- HTTP Client -->
    <PackageReference Include="Microsoft.Extensions.Http" />
    <PackageReference Include="Polly.Extensions.Http" />

    <!-- Minimal APIs -->
    <PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../BuildingBlocks.Domain/BuildingBlocks.Domain.csproj" />
    <ProjectReference Include="../BuildingBlocks.Application/BuildingBlocks.Application.csproj" />
    <ProjectReference Include="../BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

## Key Features

### 1. Minimal API Patterns
- Route grouping with `MapGroup()`
- Extension methods for common patterns
- Parameter binding with `[AsParameters]`
- OpenAPI integration with `WithTags()`, `WithSummary()`, `Produces<>()`

### 2. Strongly Typed ID Integration
- Automatic route parameter conversion
- JSON serialization/deserialization
- Query string parameter binding

### 3. Standardized Responses
- `ApiResponse<T>` for single items
- `PagedResponse<T>` for paginated results
- Consistent error responses via Problem Details

### 4. Error Handling
- Custom Problem Details middleware (no external dependencies)
- Exception to HTTP status mapping
- Correlation ID tracking
- Development vs production error details

### 5. OpenAPI/Swagger Integration
- Automatic documentation generation
- Type-safe endpoint definitions
- Version support

## Usage Pattern

```csharp
// In consuming application's Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add BuildingBlocks
builder.Services.AddDomain();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

// Configure pipeline
app.UseApi(app.Environment);

// Map endpoints using BuildingBlocks extensions
app.MapApiGroup("/api/v1/users", group =>
{
    group.MapCrudEndpoints<User, UserId, UserDto, CreateUserRequest, UpdateUserRequest>();
    
    group.MapGet("/search", async ([AsParameters] UserSearchQuery query, IMediator mediator) =>
    {
        // Custom search endpoint
    });
});

app.Run();
```

## Critical Requirements

1. **Minimal APIs Only**: No controllers, no MVC dependencies beyond what's required for JSON serialization
2. **Extension Method Pattern**: All endpoint mapping via extension methods
3. **Strongly Typed ID Support**: Seamless integration with BuildingBlocks strongly typed IDs
4. **Custom Problem Details**: No external Problem Details dependencies
5. **Performance Focus**: Leverage Minimal API performance benefits
6. **Clean Architecture**: Maintain separation of concerns
7. **OpenAPI Integration**: Automatic documentation generation

Generate a production-ready API layer that leverages the full power of Minimal APIs while maintaining the BuildingBlocks architecture principles. 