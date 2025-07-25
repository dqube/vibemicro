# API Service Layer Generation Prompt

## Overview
Generate a comprehensive API layer for a microservice using **Minimal APIs** and leveraging the BuildingBlocks.API library. This layer provides HTTP endpoints, request/response handling, authentication, authorization, and API documentation following modern ASP.NET Core patterns.

## Project Configuration

### Simplified Project File
```xml
<Project Sdk="Microsoft.NET.Sdk.Web"/>
```

**That's it!** All configuration is handled automatically by the centralized build system:

- **Target Framework**: .NET 8.0 (from `Directory.Build.props`)
- **BuildingBlocks Reference**: `BuildingBlocks.API` automatically referenced
- **Infrastructure Reference**: Service infrastructure project automatically referenced
- **Framework References**: `Microsoft.AspNetCore.App` automatically included
- **Package Management**: All necessary packages automatically included via feature flags

### Feature-Based Package Inclusion
Enable only the API features you need:

```xml
<!-- Optional in individual .csproj to enable specific features -->
<PropertyGroup>
  <!-- API Documentation -->
  <IncludeSwagger>true</IncludeSwagger>                         <!-- Swashbuckle, Scalar API docs -->
  <IncludeApiVersioning>true</IncludeApiVersioning>             <!-- API versioning support -->
  
  <!-- Security -->
  <IncludeAuthentication>true</IncludeAuthentication>           <!-- JWT, API Key authentication -->
  <IncludeRateLimiting>true</IncludeRateLimiting>               <!-- Rate limiting middleware -->
  <IncludeSecurity>true</IncludeSecurity>                       <!-- CORS, HTTPS, data protection -->
  
  <!-- Validation & Serialization -->
  <IncludeValidation>true</IncludeValidation>                   <!-- FluentValidation integration -->
  <IncludeSerialization>true</IncludeSerialization>             <!-- JSON, XML serialization -->
  
  <!-- Monitoring & Health -->
  <IncludeHealthChecks>true</IncludeHealthChecks>               <!-- Health check endpoints -->
  <IncludeMonitoring>true</IncludeMonitoring>                   <!-- OpenTelemetry, metrics -->
  
  <!-- HTTP Features -->
  <IncludeHttpClient>true</IncludeHttpClient>                   <!-- HTTP client, Polly -->
</PropertyGroup>
```

## Service API Structure

### Project Organization
```
{ServiceName}.API/
├── {ServiceName}.API.csproj                  # Simple project file
├── Program.cs                                # Application entry point
├── Endpoints/                                # API endpoints organized by feature
│   ├── {Feature}/                           # Feature-based endpoint grouping
│   │   ├── {Feature}Endpoints.cs           # Main endpoints for feature
│   │   ├── {Feature}RequestModels.cs       # Request/response models
│   │   ├── {Feature}ValidationExtensions.cs # Endpoint validation
│   │   └── {Feature}EndpointExtensions.cs  # Extension methods
│   ├── Common/                             # Common endpoints
│   │   ├── HealthEndpoints.cs              # Health check endpoints
│   │   ├── MetricsEndpoints.cs             # Metrics endpoints
│   │   └── StatusEndpoints.cs              # Status endpoints
├── Middleware/                              # Custom middleware
│   ├── {ServiceName}ExceptionMiddleware.cs  # Service-specific exception handling
│   ├── RequestLoggingMiddleware.cs         # Request/response logging
│   ├── CorrelationIdMiddleware.cs          # Correlation ID handling
│   ├── RateLimitingMiddleware.cs           # Custom rate limiting
│   └── AuthenticationMiddleware.cs         # Custom authentication logic
├── Filters/                                 # Endpoint filters
│   ├── ValidationFilter.cs                # Input validation filter
│   ├── AuthorizationFilter.cs             # Authorization filter
│   ├── CachingFilter.cs                   # Response caching filter
│   └── AuditingFilter.cs                  # Request auditing filter
├── Models/                                  # API models and DTOs
│   ├── Requests/                          # Request models
│   │   ├── {Feature}/                     # Feature-specific requests
│   │   │   ├── Create{Entity}Request.cs   # Create operation requests
│   │   │   ├── Update{Entity}Request.cs   # Update operation requests
│   │   │   └── {Entity}QueryRequest.cs    # Query operation requests
│   │   └── Common/                        # Common request models
│   │       ├── PaginationRequest.cs       # Pagination parameters
│   │       ├── SortingRequest.cs          # Sorting parameters
│   │       └── FilterRequest.cs           # Filtering parameters
│   ├── Responses/                         # Response models
│   │   ├── {Feature}/                     # Feature-specific responses
│   │   │   ├── {Entity}Response.cs        # Entity response models
│   │   │   └── {Entity}SummaryResponse.cs # Summary response models
│   │   └── Common/                        # Common response models
│   │       ├── ApiResponse.cs             # Standard API response
│   │       ├── PagedResponse.cs           # Paginated response
│   │       ├── ErrorResponse.cs           # Error response
│   │       └── ValidationErrorResponse.cs  # Validation error response
│   └── Validators/                        # Request validators
│       ├── {Feature}Validators.cs         # Feature-specific validators
│       └── CommonValidators.cs            # Shared validators
├── Configuration/                          # API configuration
│   ├── {ServiceName}ApiConfiguration.cs   # Service-specific API config
│   ├── SwaggerConfiguration.cs            # Swagger/OpenAPI configuration
│   ├── AuthenticationConfiguration.cs     # Authentication configuration
│   ├── CorsConfiguration.cs               # CORS configuration
│   └── ApiVersioningConfiguration.cs      # API versioning configuration
├── Security/                               # Security implementations
│   ├── ApiKeyAuthenticationHandler.cs     # API key authentication
│   ├── JwtAuthenticationHandler.cs        # JWT authentication handler
│   ├── AuthorizationPolicies.cs           # Authorization policies
│   ├── SecurityHeaders.cs                 # Security headers middleware
│   └── RoleBasedAuthorization.cs          # Role-based authorization
├── Documentation/                          # API documentation
│   ├── SwaggerDocumentFilter.cs           # Swagger document customization
│   ├── SwaggerOperationFilter.cs          # Swagger operation customization
│   ├── ApiDocumentationExtensions.cs      # Documentation extensions
│   └── Examples/                          # Request/response examples
│       ├── {Feature}Examples.cs           # Feature-specific examples
│       └── CommonExamples.cs              # Common examples
├── Mapping/                                # Request/response mapping
│   ├── {Feature}MappingProfile.cs         # Feature-specific mapping
│   ├── ApiMappingExtensions.cs            # Mapping extensions
│   └── ModelConversionExtensions.cs       # Model conversion helpers
├── Health/                                 # Health check implementations
│   ├── {ServiceName}HealthCheck.cs        # Service-specific health checks
│   ├── DependencyHealthChecks.cs          # Dependency health checks
│   └── CustomHealthCheckExtensions.cs     # Health check extensions
├── Monitoring/                             # Monitoring and observability
│   ├── ApiMetrics.cs                      # API-specific metrics
│   ├── RequestTracking.cs                 # Request tracking
│   ├── PerformanceMonitoring.cs           # Performance monitoring
│   └── CustomTelemetry.cs                 # Custom telemetry
└── Extensions/                             # Extensions and configuration
    ├── ServiceCollectionExtensions.cs     # DI registration
    ├── WebApplicationExtensions.cs        # Application configuration
    ├── EndpointExtensions.cs              # Endpoint mapping extensions
    ├── ValidationExtensions.cs            # Validation extensions
    └── {ServiceName}ApiExtensions.cs       # Service-specific extensions
```

## Implementation Guidelines

### 1. Application Entry Point (Program.cs)
Configure the application with BuildingBlocks integration:

```csharp
// Example: Program.cs with BuildingBlocks integration
using AuthService.Application.Extensions;
using AuthService.Infrastructure.Extensions;
using AuthService.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddAuthDomain();
builder.Services.AddAuthApplication(builder.Configuration);
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddAuthApi(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthApi();

// Map API endpoints
app.MapAuthApiEndpoints();

app.Run();
```

### 2. Feature-Based Endpoints
Implement endpoints using BuildingBlocks CRUD patterns:

```csharp
// Example: User Endpoints
public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/users")
            .WithTags("Users")
            .RequireAuthorization();

        // Use BuildingBlocks CRUD endpoints
        group.MapCrudEndpoints<User, UserId, UserResponse, CreateUserRequest, UpdateUserRequest>();

        // Add custom endpoints
        group.MapCustomUserEndpoints();
    }

    private static void MapCustomUserEndpoints(this RouteGroupBuilder group)
    {
        // Get user by username
        group.MapGet("/by-username/{username}", GetUserByUsernameAsync)
            .WithName("GetUserByUsername")
            .WithSummary("Get user by username")
            .WithDescription("Retrieves a user by their username")
            .Produces<ApiResponse<UserResponse>>()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);

        // Search users
        group.MapPost("/search", SearchUsersAsync)
            .WithName("SearchUsers")
            .WithSummary("Search users")
            .WithDescription("Search users with filters and pagination")
            .Produces<PagedResponse<UserSummaryResponse>>()
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);

        // Change password
        group.MapPost("/{id}/change-password", ChangePasswordAsync)
            .WithName("ChangeUserPassword")
            .WithSummary("Change user password")
            .WithDescription("Change a user's password")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status404NotFound)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);

        // Get user roles
        group.MapGet("/{id}/roles", GetUserRolesAsync)
            .WithName("GetUserRoles")
            .WithSummary("Get user roles")
            .WithDescription("Get all roles assigned to a user")
            .Produces<ApiResponse<IEnumerable<RoleResponse>>>()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound);

        // Assign role to user
        group.MapPost("/{id}/roles", AssignRoleToUserAsync)
            .WithName("AssignRoleToUser")
            .WithSummary("Assign role to user")
            .WithDescription("Assign a role to a user")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status404NotFound)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetUserByUsernameAsync(
        string username,
        IAuthApplicationService authService,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await authService.GetUserByUsernameAsync(username, cancellationToken);
            
            return user != null 
                ? Results.Ok(ApiResponse.Success(user))
                : Results.NotFound(ApiResponse.NotFound<UserResponse>($"User with username '{username}' not found"));
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(ValidationErrorResponse.FromValidationException(ex));
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message);
        }
    }

    private static async Task<IResult> SearchUsersAsync(
        UserSearchRequest request,
        IAuthApplicationService authService,
        IValidator<UserSearchRequest> validator,
        CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(ValidationErrorResponse.FromValidationResult(validationResult));
        }

        try
        {
            var query = request.ToQuery();
            var result = await authService.SearchUsersAsync(query, cancellationToken);
            
            var response = new PagedResponse<UserSummaryResponse>(
                result.Items.Select(u => u.ToSummaryResponse()),
                result.TotalCount,
                result.Page,
                result.PageSize);

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message);
        }
    }

    private static async Task<IResult> ChangePasswordAsync(
        Guid id,
        ChangePasswordRequest request,
        IAuthApplicationService authService,
        IValidator<ChangePasswordRequest> validator,
        CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(ValidationErrorResponse.FromValidationResult(validationResult));
        }

        try
        {
            await authService.ChangePasswordAsync(id, request.CurrentPassword, request.NewPassword, cancellationToken);
            return Results.Ok(ApiResponse.Success("Password changed successfully"));
        }
        catch (UserNotFoundException)
        {
            return Results.NotFound(ApiResponse.NotFound($"User with ID '{id}' not found"));
        }
        catch (InvalidPasswordException ex)
        {
            return Results.BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message);
        }
    }

    private static async Task<IResult> GetUserRolesAsync(
        Guid id,
        IAuthApplicationService authService,
        CancellationToken cancellationToken)
    {
        try
        {
            var roles = await authService.GetUserRolesAsync(id, cancellationToken);
            var response = roles.Select(r => r.ToResponse());
            
            return Results.Ok(ApiResponse.Success(response));
        }
        catch (UserNotFoundException)
        {
            return Results.NotFound(ApiResponse.NotFound($"User with ID '{id}' not found"));
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message);
        }
    }

    private static async Task<IResult> AssignRoleToUserAsync(
        Guid id,
        AssignRoleRequest request,
        IAuthApplicationService authService,
        IValidator<AssignRoleRequest> validator,
        CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(ValidationErrorResponse.FromValidationResult(validationResult));
        }

        try
        {
            await authService.AssignRoleToUserAsync(id, request.RoleId, cancellationToken);
            return Results.Ok(ApiResponse.Success("Role assigned successfully"));
        }
        catch (UserNotFoundException)
        {
            return Results.NotFound(ApiResponse.NotFound($"User with ID '{id}' not found"));
        }
        catch (RoleNotFoundException)
        {
            return Results.NotFound(ApiResponse.NotFound($"Role with ID '{request.RoleId}' not found"));
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message);
        }
    }
}

// Example: Authentication Endpoints
public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/auth")
            .WithTags("Authentication")
            .AllowAnonymous();

        // Login endpoint
        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithSummary("User login")
            .WithDescription("Authenticate user and return access and refresh tokens")
            .Produces<ApiResponse<LoginResponse>>()
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized);

        // Register endpoint
        group.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .WithSummary("User registration")
            .WithDescription("Register a new user account")
            .Produces<ApiResponse<UserResponse>>(StatusCodes.Status201Created)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);

        // Refresh token endpoint
        group.MapPost("/refresh", RefreshTokenAsync)
            .WithName("RefreshToken")
            .WithSummary("Refresh access token")
            .WithDescription("Refresh an expired access token using a refresh token")
            .Produces<ApiResponse<LoginResponse>>()
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized);

        // Logout endpoint
        group.MapPost("/logout", LogoutAsync)
            .WithName("Logout")
            .WithSummary("User logout")
            .WithDescription("Logout user and invalidate tokens")
            .RequireAuthorization()
            .Produces<ApiResponse>(StatusCodes.Status200OK);

        // Forgot password endpoint
        group.MapPost("/forgot-password", ForgotPasswordAsync)
            .WithName("ForgotPassword")
            .WithSummary("Forgot password")
            .WithDescription("Send password reset email")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);

        // Reset password endpoint
        group.MapPost("/reset-password", ResetPasswordAsync)
            .WithName("ResetPassword")
            .WithSummary("Reset password")
            .WithDescription("Reset password using reset token")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        IAuthApplicationService authService,
        IValidator<LoginRequest> validator,
        CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(ValidationErrorResponse.FromValidationResult(validationResult));
        }

        try
        {
            var loginResult = await authService.LoginAsync(request.Username, request.Password, cancellationToken);
            
            var response = new LoginResponse
            {
                AccessToken = loginResult.AccessToken,
                RefreshToken = loginResult.RefreshToken,
                ExpiresIn = loginResult.ExpiresIn,
                TokenType = "Bearer",
                User = loginResult.User.ToResponse()
            };

            return Results.Ok(ApiResponse.Success(response));
        }
        catch (InvalidCredentialsException)
        {
            return Results.Unauthorized();
        }
        catch (UserInactiveException ex)
        {
            return Results.Unauthorized(ApiResponse.Unauthorized(ex.Message));
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message);
        }
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        IAuthApplicationService authService,
        IValidator<RegisterRequest> validator,
        CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(ValidationErrorResponse.FromValidationResult(validationResult));
        }

        try
        {
            var createUserRequest = new CreateUserRequest
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var user = await authService.RegisterUserAsync(createUserRequest, cancellationToken);
            var response = user.ToResponse();

            return Results.Created($"/api/v1/users/{user.Id}", ApiResponse.Success(response));
        }
        catch (UsernameAlreadyExistsException ex)
        {
            return Results.BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (EmailAlreadyExistsException ex)
        {
            return Results.BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message);
        }
    }

    // Additional authentication methods...
}
```

### 3. Request/Response Models
Define clean API contracts:

```csharp
// Example: Request Models
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

public sealed record UserSearchRequest : PaginationRequest
{
    public string? SearchTerm { get; init; }
    public UserStatus? Status { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public DateTime? CreatedBefore { get; init; }
    public string? Email { get; init; }
}

public sealed record LoginRequest
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public bool RememberMe { get; init; }
}

public sealed record ChangePasswordRequest
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}

// Example: Response Models
public sealed record UserResponse
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public UserStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed record UserSummaryResponse
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public UserStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public int ExpiresIn { get; init; }
    public string TokenType { get; init; } = string.Empty;
    public UserResponse User { get; init; } = null!;
}

public sealed record RoleResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IEnumerable<PermissionResponse> Permissions { get; init; } = Enumerable.Empty<PermissionResponse>();
}

// Common response wrappers (using BuildingBlocks)
public sealed record ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }

    public static ApiResponse<T> Success(T data, string? message = null)
        => new() { Success = true, Message = message ?? "Operation completed successfully", Data = data };

    public static ApiResponse<T> NotFound(string message = "Resource not found")
        => new() { Success = false, Message = message };
}

public sealed record ValidationErrorResponse : ApiResponse
{
    public Dictionary<string, string[]> Errors { get; init; } = new();

    public static ValidationErrorResponse FromValidationResult(ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationErrorResponse
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors
        };
    }

    public static ValidationErrorResponse FromValidationException(ValidationException validationException)
    {
        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationErrorResponse
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors
        };
    }
}
```

### 4. Request Validation
Implement comprehensive validation:

```csharp
// Example: Request Validators
public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private readonly IUserRepository _userRepository;

    public CreateUserRequestValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 50)
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Username must be 3-50 characters, alphanumeric and underscore only")
            .MustAsync(BeUniqueUsername)
            .WithMessage("Username is already taken");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320)
            .MustAsync(BeUniqueEmail)
            .WithMessage("Email is already in use");

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

    private async Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken)
    {
        return !await _userRepository.ExistsByUsernameAsync(new Username(username), cancellationToken);
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _userRepository.ExistsByEmailAsync(new Email(email), cancellationToken);
    }
}

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(500);
    }
}

public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("New password must contain at least one lowercase, uppercase, digit, and special character");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.NewPassword)
            .WithMessage("Password confirmation does not match new password");
    }
}

public sealed class UserSearchRequestValidator : AbstractValidator<UserSearchRequest>
{
    public UserSearchRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.CreatedAfter)
            .LessThan(x => x.CreatedBefore)
            .When(x => x.CreatedAfter.HasValue && x.CreatedBefore.HasValue)
            .WithMessage("Created after date must be before created before date");
    }
}
```

### 5. Endpoint Filters
Implement reusable endpoint filters:

```csharp
// Example: Validation Filter
public sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;
    private readonly ILogger<ValidationFilter<T>> _logger;

    public ValidationFilter(IValidator<T> validator, ILogger<ValidationFilter<T>> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();
        if (request == null)
        {
            return await next(context);
        }

        var validationResult = await _validator.ValidateAsync(request, context.HttpContext.RequestAborted);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for {RequestType}: {@Errors}", 
                typeof(T).Name, 
                validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

            return Results.BadRequest(ValidationErrorResponse.FromValidationResult(validationResult));
        }

        return await next(context);
    }
}

// Example: Authorization Filter
public sealed class AuthorizationFilter : IEndpointFilter
{
    private readonly string _requiredPermission;
    private readonly ILogger<AuthorizationFilter> _logger;

    public AuthorizationFilter(string requiredPermission, ILogger<AuthorizationFilter> logger)
    {
        _requiredPermission = requiredPermission;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated == true)
        {
            _logger.LogWarning("Unauthenticated access attempt to protected endpoint");
            return Results.Unauthorized();
        }

        if (!user.HasClaim("permission", _requiredPermission))
        {
            _logger.LogWarning("User {UserId} attempted to access endpoint requiring permission {Permission}", 
                user.GetUserId(), _requiredPermission);
            return Results.Forbid();
        }

        return await next(context);
    }
}

// Example: Caching Filter
public sealed class CachingFilter<T> : IEndpointFilter where T : class
{
    private readonly ICacheService _cacheService;
    private readonly TimeSpan _cacheDuration;
    private readonly ILogger<CachingFilter<T>> _logger;

    public CachingFilter(ICacheService cacheService, TimeSpan cacheDuration, ILogger<CachingFilter<T>> logger)
    {
        _cacheService = cacheService;
        _cacheDuration = cacheDuration;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var cacheKey = GenerateCacheKey(context);
        
        // Try to get from cache
        var cachedResult = await _cacheService.GetAsync<T>(cacheKey, context.HttpContext.RequestAborted);
        if (cachedResult != null)
        {
            _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
            return Results.Ok(ApiResponse.Success(cachedResult));
        }

        // Execute the endpoint
        var result = await next(context);
        
        // Cache the result if it's successful
        if (result is IResult httpResult && IsSuccessResult(httpResult))
        {
            if (TryExtractData(httpResult, out var data))
            {
                await _cacheService.SetAsync(cacheKey, data, _cacheDuration, context.HttpContext.RequestAborted);
                _logger.LogDebug("Cached result for key: {CacheKey}", cacheKey);
            }
        }

        return result;
    }

    private string GenerateCacheKey(EndpointFilterInvocationContext context)
    {
        var request = context.HttpContext.Request;
        var keyParts = new List<string>
        {
            request.Path.Value ?? "",
            request.QueryString.Value ?? ""
        };

        // Add user-specific caching if needed
        var userId = context.HttpContext.User.GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            keyParts.Add($"user:{userId}");
        }

        return string.Join(":", keyParts.Where(p => !string.IsNullOrEmpty(p)));
    }

    private bool IsSuccessResult(IResult result) => result is not null; // Simplified check

    private bool TryExtractData(IResult result, out T? data)
    {
        data = default;
        // Extract data from result - this is a simplified implementation
        return false;
    }
}

// Example: Auditing Filter
public sealed class AuditingFilter : IEndpointFilter
{
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditingFilter> _logger;

    public AuditingFilter(IAuditService auditService, ILogger<AuditingFilter> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.HttpContext.Request;
        var user = context.HttpContext.User;

        var auditEntry = new AuditEntry
        {
            UserId = user.GetUserId(),
            Action = $"{request.Method} {request.Path}",
            Timestamp = DateTime.UtcNow,
            IPAddress = request.HttpContext.GetClientIpAddress(),
            UserAgent = request.Headers.UserAgent.ToString()
        };

        try
        {
            var result = await next(context);
            
            stopwatch.Stop();
            auditEntry.Duration = stopwatch.ElapsedMilliseconds;
            auditEntry.Success = IsSuccessResult(result);
            
            await _auditService.LogAsync(auditEntry, context.HttpContext.RequestAborted);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            auditEntry.Duration = stopwatch.ElapsedMilliseconds;
            auditEntry.Success = false;
            auditEntry.ErrorMessage = ex.Message;
            
            await _auditService.LogAsync(auditEntry, context.HttpContext.RequestAborted);
            
            throw;
        }
    }

    private bool IsSuccessResult(object? result)
    {
        // Simplified success check
        return result is IResult;
    }
}
```

### 6. Custom Middleware
Implement service-specific middleware:

```csharp
// Example: Service-specific Exception Middleware
public sealed class AuthExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public AuthExceptionMiddleware(
        RequestDelegate next,
        ILogger<AuthExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
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
        var (statusCode, message, details) = MapException(exception);

        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = GetTitle(statusCode),
            Status = statusCode,
            Detail = message,
            Instance = context.Request.Path
        };

        if (_environment.IsDevelopment() && details != null)
        {
            problemDetails.Extensions.Add("details", details);
        }

        if (context.TraceIdentifier != null)
        {
            problemDetails.Extensions.Add("traceId", context.TraceIdentifier);
        }

        _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}, StatusCode: {StatusCode}", 
            context.TraceIdentifier, statusCode);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    private (int statusCode, string message, object? details) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validationEx => (400, "Validation failed", validationEx.Errors),
            UserNotFoundException => (404, "User not found", null),
            RoleNotFoundException => (404, "Role not found", null),
            InvalidCredentialsException => (401, "Invalid credentials", null),
            UserInactiveException => (401, "User account is inactive", null),
            UsernameAlreadyExistsException => (409, "Username already exists", null),
            EmailAlreadyExistsException => (409, "Email already exists", null),
            InvalidPasswordException => (400, "Invalid password", null),
            UnauthorizedAccessException => (403, "Access denied", null),
            TimeoutException => (408, "Request timeout", null),
            _ => (500, "An internal server error occurred", _environment.IsDevelopment() ? exception.ToString() : null)
        };
    }

    private string GetTitle(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        408 => "Request Timeout",
        409 => "Conflict",
        500 => "Internal Server Error",
        _ => "Error"
    };
}

// Example: Request Correlation Middleware
public sealed class CorrelationIdMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        
        // Add to response headers
        context.Response.Headers[CorrelationIdHeaderName] = correlationId;
        
        // Add to logging scope
        using var scope = context.RequestServices.GetRequiredService<ILogger<CorrelationIdMiddleware>>()
            .BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId });
        
        await _next(context);
    }

    private string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var existingCorrelationId) &&
            !string.IsNullOrEmpty(existingCorrelationId))
        {
            return existingCorrelationId!;
        }

        return Guid.NewGuid().ToString();
    }
}
```

### 7. API Configuration and Documentation
Configure Swagger and API documentation:

```csharp
// Example: Swagger Configuration
public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Auth Service API",
                Version = "v1",
                Description = "Authentication and User Management API",
                Contact = new OpenApiContact
                {
                    Name = "Development Team",
                    Email = "dev@company.com"
                }
            });

            // Add JWT authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Add operation filters
            options.OperationFilter<SwaggerOperationFilter>();
            options.DocumentFilter<SwaggerDocumentFilter>();

            // Add examples
            options.SchemaFilter<SwaggerExampleFilter>();
        });

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service API v1");
                options.RoutePrefix = "swagger";
                options.DocExpansion(DocExpansion.List);
                options.DefaultModelsExpandDepth(-1);
                options.DisplayRequestDuration();
                options.EnableDeepLinking();
                options.EnableFilter();
                options.ShowExtensions();
                options.EnableValidator();
            });

            // Add Scalar API documentation (modern alternative to Swagger UI)
            app.MapScalarApiReference(options =>
            {
                options.Title = "Auth Service API";
                options.Theme = ScalarTheme.Purple;
                options.ShowSidebar = true;
            });
        }

        return app;
    }
}

// Example: Swagger Operation Filter
public sealed class SwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add correlation ID header to all operations
        operation.Parameters ??= new List<OpenApiParameter>();
        
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Correlation-ID",
            In = ParameterLocation.Header,
            Description = "Correlation ID for request tracking",
            Required = false,
            Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
        });

        // Add common response types
        operation.Responses.TryAdd("400", new OpenApiResponse
        {
            Description = "Bad Request - Validation failed",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = context.SchemaGenerator.GenerateSchema(typeof(ValidationErrorResponse), context.SchemaRepository)
                }
            }
        });

        if (HasAuthorizeAttribute(context))
        {
            operation.Responses.TryAdd("401", new OpenApiResponse
            {
                Description = "Unauthorized - Authentication required"
            });

            operation.Responses.TryAdd("403", new OpenApiResponse
            {
                Description = "Forbidden - Insufficient permissions"
            });
        }
    }

    private bool HasAuthorizeAttribute(OperationFilterContext context)
    {
        return context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
               context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true;
    }
}
```

### 8. Service Registration and Pipeline Configuration
Configure the complete API layer:

```csharp
// Example: API Service Registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Add BuildingBlocks API
        services.AddApi(configuration);

        // Configure API-specific services
        services.AddApiModels();
        services.AddApiValidation();
        services.AddApiFilters();
        services.AddApiDocumentation(configuration);
        services.AddApiSecurity(configuration);
        services.AddApiMonitoring();

        return services;
    }

    private static IServiceCollection AddApiModels(this IServiceCollection services)
    {
        // Register AutoMapper profiles
        services.AddAutoMapper(typeof(UserMappingProfile));

        return services;
    }

    private static IServiceCollection AddApiValidation(this IServiceCollection services)
    {
        // Register FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

        // Register validation filters
        services.AddScoped(typeof(ValidationFilter<>));

        return services;
    }

    private static IServiceCollection AddApiFilters(this IServiceCollection services)
    {
        services.AddScoped<AuthorizationFilter>();
        services.AddScoped(typeof(CachingFilter<>));
        services.AddScoped<AuditingFilter>();

        return services;
    }

    private static IServiceCollection AddApiDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerDocumentation(configuration);

        return services;
    }

    private static IServiceCollection AddApiSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        // Configure rate limiting
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;
    }

    private static IServiceCollection AddApiMonitoring(this IServiceCollection services)
    {
        // Add custom health checks
        services.AddHealthChecks()
            .AddCheck<AuthApiHealthCheck>("auth_api");

        // Add custom metrics
        services.AddSingleton<AuthApiMetrics>();

        return services;
    }
}

// Example: Application Pipeline Configuration
public static class WebApplicationExtensions
{
    public static WebApplication UseAuthApi(this WebApplication app)
    {
        // Use BuildingBlocks API pipeline
        app.UseApi();

        // Add service-specific middleware
        app.UseServiceSpecificMiddleware();

        return app;
    }

    private static WebApplication UseServiceSpecificMiddleware(this WebApplication app)
    {
        // Custom exception handling
        app.UseMiddleware<AuthExceptionMiddleware>();

        // Correlation ID
        app.UseMiddleware<CorrelationIdMiddleware>();

        // Request logging
        app.UseMiddleware<RequestLoggingMiddleware>();

        return app;
    }

    public static WebApplication MapAuthApiEndpoints(this WebApplication app)
    {
        // Map authentication endpoints
        app.MapAuthenticationEndpoints();

        // Map user management endpoints
        app.MapUserEndpoints();

        // Map role management endpoints
        app.MapRoleEndpoints();

        // Map health and monitoring endpoints
        app.MapHealthEndpoints();

        return app;
    }
}
```

## Key Principles

### 1. **Minimal APIs Excellence**
- **Performance optimized** over traditional controllers
- **Strongly-typed** route parameters and responses
- **Feature-based** endpoint organization
- **Extension method** patterns for reusability

### 2. **Clean API Design**
- **RESTful conventions** with proper HTTP verbs and status codes
- **Consistent response** formats using BuildingBlocks patterns
- **Comprehensive validation** at API boundary
- **OpenAPI documentation** auto-generated

### 3. **Security First**
- **Authentication** and **authorization** at endpoint level
- **Input validation** and **sanitization**
- **Rate limiting** and **CORS** protection
- **Security headers** and **audit logging**

### 4. **Observability Built-in**
- **Correlation ID** tracking across requests
- **Structured logging** with context
- **Health checks** for dependencies
- **Custom metrics** for business KPIs

### 5. **Developer Experience**
- **Auto-generated documentation** with examples
- **Type-safe** request/response models
- **Comprehensive error handling** with proper messages
- **Easy testing** with clear endpoint definitions

## Benefits of This Structure

### 1. **Automatic BuildingBlocks Integration**
- **Zero configuration** - leverages centralized build system
- **CRUD endpoints** with minimal code
- **Strongly-typed IDs** work seamlessly
- **Consistent error handling** across all endpoints

### 2. **Production-Ready API**
- **Security** best practices implemented
- **Performance** optimized with caching and validation
- **Monitoring** and **observability** built-in
- **Documentation** auto-generated and maintained

### 3. **Maintainable and Scalable**
- **Feature-based** organization scales with team size
- **Reusable filters** and middleware
- **Clear separation** of concerns
- **Testable** endpoint implementations

### 4. **Modern ASP.NET Core**
- **Minimal APIs** for maximum performance
- **Endpoint filters** for cross-cutting concerns
- **OpenAPI 3.0** specification compliance
- **Latest security** and performance features

Generate an API layer following these patterns and principles, ensuring high-performance endpoints, comprehensive security, excellent developer experience, and seamless integration with the BuildingBlocks architecture. 