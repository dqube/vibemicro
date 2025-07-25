# BuildingBlocks.API Library Generation Prompt

## Overview
Generate a comprehensive API layer library that provides foundational building blocks for modern ASP.NET Core microservices. This library implements clean architecture principles, minimal APIs, comprehensive middleware, and modern development patterns.

## Project Configuration

### Target Framework & Features
- **.NET 8.0** (`net8.0`)
- **Project SDK**: `Microsoft.NET.Sdk.Web`
- **Implicit Usings**: Enabled
- **Nullable Reference Types**: Enabled
- **Treat Warnings as Errors**: Enabled
- **Generate Documentation File**: Enabled

### Package Dependencies
```xml
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

<!-- Security & CORS -->
<PackageReference Include="Microsoft.AspNetCore.Cors" />
<PackageReference Include="Microsoft.AspNetCore.DataProtection" />
<PackageReference Include="Microsoft.AspNetCore.HttpsPolicy" />

<!-- Problem Details -->
<PackageReference Include="Hellang.Middleware.ProblemDetails" />

<!-- HTTP Client -->
<PackageReference Include="Microsoft.Extensions.Http" />
<PackageReference Include="Polly.Extensions.Http" />

<!-- Serialization -->
<PackageReference Include="System.Text.Json" />
<PackageReference Include="Newtonsoft.Json" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" />
```

### Project References
```xml
<ProjectReference Include="../BuildingBlocks.Domain/BuildingBlocks.Domain.csproj" />
<ProjectReference Include="../BuildingBlocks.Application/BuildingBlocks.Application.csproj" />
<ProjectReference Include="../BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj" />
```

## Architecture & Patterns

### Core API Concepts
1. **Minimal APIs**: Fast, lightweight endpoint definitions
2. **Clean Architecture**: Clear separation of concerns
3. **CQRS Integration**: Command/Query segregation through MediatR
4. **Standardized Responses**: Consistent API response format
5. **Comprehensive Middleware**: Error handling, logging, authentication
6. **Configuration-Driven**: Options pattern for all settings
7. **Observability**: Health checks, metrics, tracing

### Access Modifier Strategy
- **Public**: Core abstractions, configuration options, response models
- **Internal**: Implementation details, extension methods

## Folder Structure & Components

### `/Configuration` - API Configuration
```
Configuration/
└── Options/
    ├── ApiOptions.cs              # Core API settings
    ├── AuthenticationOptions.cs   # JWT/Auth configuration
    ├── AuthorizationOptions.cs    # Authorization policies
    ├── CorsOptions.cs             # CORS configuration
    ├── RateLimitingOptions.cs     # Rate limiting settings
    ├── SwaggerOptions.cs          # OpenAPI/Swagger settings
    └── HealthCheckOptions.cs      # Health check configuration
```

**ApiOptions.cs** - Core API Configuration:
```csharp
public class ApiOptions
{
    public const string SectionName = "Api";
    
    public string Title { get; set; } = "BuildingBlocks API";
    public string Version { get; set; } = "1.0";
    public string Description { get; set; } = "A comprehensive API built with BuildingBlocks";
    public bool EnableDetailedErrors { get; set; } = false;
    public bool EnableRequestLogging { get; set; } = true;
    public bool EnableSwaggerInProduction { get; set; } = false;
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public long MaxRequestBodySize { get; set; } = 1024 * 1024 * 10; // 10MB
}
```

### `/Responses` - Standardized API Responses
```
Responses/
└── Base/
    ├── ApiResponse.cs         # Base response without data
    ├── ApiResponse{T}.cs      # Generic response with data
    ├── PagedApiResponse.cs    # Paginated response
    ├── ErrorResponse.cs       # Error-specific response
    └── ValidationResponse.cs  # Validation error response
```

**ApiResponse.cs** - Standard Response Format:
```csharp
public class ApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }
    
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
    
    // Factory methods for creating responses
    public static ApiResponse Success(string? message = null);
    public static ApiResponse Error(string message, Dictionary<string, string[]>? errors = null);
    public static ApiResponse ValidationError(Dictionary<string, string[]> errors);
}

public class ApiResponse<T> : ApiResponse
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }
    
    // Typed factory methods
    public static ApiResponse<T> Success(T data, string? message = null);
    public static ApiResponse<T> Error(string message, Dictionary<string, string[]>? errors = null);
}
```

### `/Endpoints` - Endpoint Base Classes
```
Endpoints/
└── Base/
    ├── EndpointBase.cs           # Base endpoint class
    ├── CommandEndpointBase.cs    # Command-specific endpoint
    ├── QueryEndpointBase.cs      # Query-specific endpoint
    └── IEndpoint.cs              # Endpoint interface
```

**EndpointBase.cs** - Base Endpoint Implementation:
```csharp
public abstract class EndpointBase
{
    protected IMediator Mediator { get; }
    protected ILogger Logger { get; }
    
    protected EndpointBase(IMediator mediator, ILogger logger)
    {
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    // Response helper methods
    protected static ApiResponse<T> Success<T>(T data, string? message = null);
    protected static ApiResponse Success(string? message = null);
    protected static ApiResponse<T> Error<T>(string message, Dictionary<string, string[]>? errors = null);
    protected static ApiResponse Error(string message, Dictionary<string, string[]>? errors = null);
    
    // Async response helpers
    protected async Task<IResult> OkAsync<T>(Task<T> dataTask, string? message = null);
    protected async Task<IResult> CreatedAsync<T>(Task<T> dataTask, string location, string? message = null);
    protected async Task<IResult> NoContentAsync(Task task);
    
    // Error response helpers
    protected IResult BadRequest(string message, Dictionary<string, string[]>? errors = null);
    protected IResult NotFound(string message = "Resource not found");
    protected IResult Conflict(string message, Dictionary<string, string[]>? errors = null);
    protected IResult Unauthorized(string message = "Unauthorized access");
    protected IResult Forbidden(string message = "Access denied");
}
```

### `/Middleware` - Request Processing Pipeline
```
Middleware/
├── ErrorHandling/
│   ├── GlobalExceptionMiddleware.cs      # Global exception handling
│   ├── ProblemDetailsMiddleware.cs       # RFC 7807 problem details
│   └── ValidationExceptionMiddleware.cs  # Validation error handling
├── Logging/
│   ├── RequestLoggingMiddleware.cs       # Structured request logging
│   ├── CorrelationMiddleware.cs          # Correlation ID handling
│   └── PerformanceMiddleware.cs          # Performance monitoring
├── Security/
│   ├── ApiKeyMiddleware.cs               # API key authentication
│   ├── RateLimitingMiddleware.cs         # Rate limiting
│   └── SecurityHeadersMiddleware.cs      # Security headers
└── Caching/
    ├── ResponseCachingMiddleware.cs      # HTTP response caching
    └── ETagMiddleware.cs                 # ETag handling
```

**GlobalExceptionMiddleware.cs** - Exception Handling:
```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly ApiOptions _options;
    
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
        var response = exception switch
        {
            ValidationException validationEx => CreateValidationErrorResponse(validationEx),
            BusinessRuleValidationException businessEx => CreateBusinessRuleErrorResponse(businessEx),
            DomainException domainEx => CreateDomainErrorResponse(domainEx),
            UnauthorizedAccessException => CreateUnauthorizedResponse(),
            ArgumentException argumentEx => CreateBadRequestResponse(argumentEx),
            _ => CreateInternalServerErrorResponse(exception)
        };
        
        context.Response.StatusCode = GetStatusCode(exception);
        context.Response.ContentType = "application/json";
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

### `/Extensions` - Service Configuration
```
Extensions/
├── ServiceCollectionExtensions.cs     # DI container configuration
├── WebApplicationExtensions.cs        # Application pipeline setup
└── EndpointRouteBuilderExtensions.cs  # Endpoint routing extensions
```

**ServiceCollectionExtensions.cs** - Service Registration:
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiCore(configuration)
               .AddApiAuthentication(configuration)
               .AddApiAuthorization(configuration)
               .AddApiVersioning(configuration)
               .AddOpenApi(configuration)
               .AddApiCors(configuration)
               .AddApiRateLimiting(configuration)
               .AddApiHealthChecks(configuration)
               .AddApiObservability(configuration);
        
        return services;
    }
    
    public static IServiceCollection AddApiCore(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure API options
        services.Configure<ApiOptions>(configuration.GetSection(ApiOptions.SectionName));
        
        // Add controllers and minimal APIs
        services.AddControllers()
               .AddJsonOptions(options => ConfigureJsonOptions(options.JsonSerializerOptions))
               .AddFluentValidation();
        
        // Add problem details
        services.AddProblemDetails();
        
        // Add HTTP context accessor
        services.AddHttpContextAccessor();
        
        return services;
    }
    
    // Additional extension methods for specific concerns
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration);
    public static IServiceCollection AddApiAuthorization(this IServiceCollection services, IConfiguration configuration);
    public static IServiceCollection AddApiVersioning(this IServiceCollection services, IConfiguration configuration);
    public static IServiceCollection AddOpenApi(this IServiceCollection services, IConfiguration configuration);
    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration);
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration configuration);
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services, IConfiguration configuration);
    public static IServiceCollection AddApiObservability(this IServiceCollection services, IConfiguration configuration);
}
```

**WebApplicationExtensions.cs** - Application Pipeline:
```csharp
public static class WebApplicationExtensions
{
    public static WebApplication ConfigureApi(this WebApplication app)
    {
        app.UseApiSecurity()
           .UseApiMiddleware()
           .UseApiRouting()
           .UseApiDocumentation()
           .UseApiHealthChecks();
        
        return app;
    }
    
    public static WebApplication UseApiSecurity(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseSecurityHeaders();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();
        
        return app;
    }
    
    public static WebApplication UseApiMiddleware(this WebApplication app)
    {
        app.UseGlobalExceptionHandling();
        app.UseRequestLogging();
        app.UseCorrelationId();
        app.UsePerformanceMonitoring();
        
        return app;
    }
    
    public static WebApplication UseApiRouting(this WebApplication app)
    {
        app.MapControllers();
        app.MapApiEndpoints();
        
        return app;
    }
    
    public static WebApplication UseApiDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Api:EnableSwaggerInProduction"))
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapScalarApiReference();
        }
        
        return app;
    }
    
    public static WebApplication UseApiHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/ready");
        app.MapHealthChecks("/health/live");
        
        return app;
    }
}
```

### `/Attributes` - Custom Attributes
```
Attributes/
├── ApiKeyAttribute.cs          # API key authorization
├── RateLimitAttribute.cs       # Rate limiting decoration
├── CacheAttribute.cs           # Response caching
└── ValidateModelAttribute.cs   # Model validation
```

### `/Filters` - Action Filters
```
Filters/
├── ValidationFilter.cs         # Automatic validation
├── CacheFilter.cs             # Response caching filter
├── AuditFilter.cs             # Request auditing
└── PerformanceFilter.cs       # Performance monitoring
```

### `/Validators` - Request Validation
```
Validators/
├── BaseValidator.cs           # Base validation class
├── PaginationValidator.cs     # Pagination parameter validation
└── QueryParameterValidator.cs # Query parameter validation
```

### `/Services` - API-Specific Services
```
Services/
├── IApiContext.cs             # API context interface
├── ApiContext.cs              # API context implementation
├── IResponseCache.cs          # Response caching interface
├── ResponseCache.cs           # Response caching implementation
├── IApiKeyService.cs          # API key service interface
└── ApiKeyService.cs           # API key service implementation
```

## Key Implementation Patterns

### 1. Minimal API Endpoints
```csharp
public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/users")
                      .WithTags("Users")
                      .WithOpenApi();
        
        group.MapGet("/", GetUsers)
             .WithName("GetUsers")
             .WithSummary("Get all users")
             .Produces<ApiResponse<PagedResult<UserDto>>>();
        
        group.MapGet("/{id:guid}", GetUser)
             .WithName("GetUser")
             .WithSummary("Get user by ID")
             .Produces<ApiResponse<UserDto>>()
             .Produces(404);
        
        group.MapPost("/", CreateUser)
             .WithName("CreateUser")
             .WithSummary("Create new user")
             .Produces<ApiResponse<UserDto>>(201)
             .Produces<ValidationResponse>(400);
    }
    
    private static async Task<IResult> GetUsers(
        [AsParameters] GetUsersQuery query,
        IMediator mediator,
        ILogger<UserEndpoints> logger)
    {
        var result = await mediator.Send(query);
        return Results.Ok(ApiResponse<PagedResult<UserDto>>.Success(result));
    }
}
```

### 2. Controller-Based Endpoints
```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class UsersController : EndpointBase
{
    public UsersController(IMediator mediator, ILogger<UsersController> logger)
        : base(mediator, logger)
    {
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserDto>>), 200)]
    public async Task<IResult> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await Mediator.Send(query);
        return await OkAsync(Task.FromResult(result));
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IResult> GetUser(Guid id)
    {
        var query = new GetUserQuery(id);
        var result = await Mediator.Send(query);
        return await OkAsync(Task.FromResult(result));
    }
}
```

### 3. Structured Error Handling
```csharp
public static class ErrorHandler
{
    public static ApiResponse HandleException(Exception exception, bool includeDetails = false)
    {
        return exception switch
        {
            ValidationException validationEx => CreateValidationError(validationEx),
            BusinessRuleValidationException businessEx => CreateBusinessRuleError(businessEx),
            DomainException domainEx => CreateDomainError(domainEx),
            UnauthorizedAccessException => CreateUnauthorizedError(),
            ArgumentException argumentEx => CreateBadRequestError(argumentEx),
            _ => CreateInternalServerError(exception, includeDetails)
        };
    }
}
```

### 4. Health Check Configuration
```csharp
public static class HealthCheckExtensions
{
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddDbContextCheck<ApplicationDbContext>()
                .AddRedis(configuration.GetConnectionString("Redis"))
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddUrlGroup(new Uri("https://api.external.com/health"), "external-api");
        
        return services;
    }
}
```

## Configuration Examples

### appsettings.json Structure
```json
{
  "Api": {
    "Title": "My Microservice API",
    "Version": "1.0",
    "Description": "A comprehensive microservice API",
    "EnableDetailedErrors": false,
    "EnableRequestLogging": true,
    "EnableSwaggerInProduction": false,
    "RequestTimeout": "00:00:30",
    "MaxRequestBodySize": 10485760
  },
  "Authentication": {
    "Jwt": {
      "Issuer": "https://your-auth-server.com",
      "Audience": "your-api",
      "SecretKey": "your-secret-key",
      "ExpirationMinutes": 60
    }
  },
  "RateLimiting": {
    "EnableRateLimiting": true,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      }
    ]
  },
  "Cors": {
    "AllowedOrigins": ["https://localhost:3000"],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowedHeaders": ["*"],
    "AllowCredentials": true
  }
}
```

## Usage Examples

### Program.cs Setup
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddApi(builder.Configuration)
                .AddApplication(builder.Configuration)
                .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure pipeline
app.ConfigureApi();

// Map endpoints
app.MapUserEndpoints();
app.MapProductEndpoints();

app.Run();
```

### Custom Endpoint Implementation
```csharp
public class CreateUserEndpoint : EndpointBase
{
    public CreateUserEndpoint(IMediator mediator, ILogger<CreateUserEndpoint> logger)
        : base(mediator, logger)
    {
    }
    
    public async Task<IResult> HandleAsync(CreateUserCommand command)
    {
        var result = await Mediator.Send(command);
        return await CreatedAsync(Task.FromResult(result), $"/api/v1/users/{result.Id}");
    }
}
```

## Implementation Guidelines

1. **Consistency**: Use standardized response formats across all endpoints
2. **Security**: Implement comprehensive security middleware and validation
3. **Performance**: Include caching, rate limiting, and performance monitoring
4. **Observability**: Add structured logging, health checks, and metrics
5. **Documentation**: Generate comprehensive OpenAPI specifications
6. **Testing**: Support for integration and unit testing
7. **Extensibility**: Pluggable architecture for custom behaviors

Generate this library with full implementations, comprehensive XML documentation, and modern ASP.NET Core best practices. 