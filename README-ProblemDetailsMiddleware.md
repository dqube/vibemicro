# Custom Problem Details Middleware

This document explains how to use the custom Problem Details middleware that replaces the Hellang.Middleware.ProblemDetails package.

## Overview

The custom `ProblemDetailsMiddleware` provides RFC 7807 compliant error responses for both exceptions and HTTP status codes, with full control over the implementation and no external dependencies.

## Features

- **RFC 7807 Compliance**: Generates standard Problem Details responses
- **Exception Mapping**: Configurable exception to status code mappings
- **Status Code Handling**: Converts 4xx/5xx status codes to Problem Details
- **Development Support**: Includes exception details in development environments
- **Correlation ID Support**: Automatically includes correlation/trace IDs
- **Strongly Typed ID Support**: Compatible with the BuildingBlocks JSON converters
- **Customizable**: Fully configurable exception mappings and response format

## Configuration

### 1. Service Registration

```csharp
// In Program.cs or Startup.cs
services.AddProblemDetailsMiddleware(options =>
{
    // Include exception details only in development
    options.IncludeExceptionDetails = app.Environment.IsDevelopment();
    
    // Map standard exceptions
    options.MapStandardExceptions();
    
    // Add custom exception mappings
    options.Map<CustomBusinessException>(422, "Business Rule Violation");
    options.Map<CustomValidationException>(400, "Validation Failed");
});
```

### 2. Middleware Registration

```csharp
// In Program.cs - Configure the pipeline
var app = builder.Build();

// Problem Details should be early in the pipeline
app.UseProblemDetailsMiddleware();

// ... other middleware
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

### 3. Standard Exception Mappings

The middleware includes standard exception mappings:

```csharp
options.MapStandardExceptions(); // Includes:

// ArgumentException -> 400 Bad Request
// ArgumentNullException -> 400 Bad Request  
// InvalidOperationException -> 400 Bad Request
// UnauthorizedAccessException -> 401 Unauthorized
// SecurityException -> 403 Forbidden
// KeyNotFoundException -> 404 Not Found
// NotSupportedException -> 405 Method Not Allowed
// TimeoutException -> 408 Request Timeout
// TaskCanceledException -> 499 Client Closed Request
// OperationCanceledException -> 499 Client Closed Request
// NotImplementedException -> 501 Not Implemented
```

## Response Format

### Exception Response Example

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request", 
  "status": 400,
  "detail": "The request could not be understood by the server due to malformed syntax.",
  "instance": "/api/users/123",
  "correlationId": "550e8400-e29b-41d4-a716-446655440000",
  "timestamp": "2023-10-15T10:30:00.000Z",
  "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01"
}
```

### Development Response (with exception details)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "Object reference not set to an instance of an object.",
  "instance": "/api/users/123",
  "correlationId": "550e8400-e29b-41d4-a716-446655440000", 
  "timestamp": "2023-10-15T10:30:00.000Z",
  "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
  "exceptionType": "NullReferenceException",
  "stackTrace": "   at MyService.GetUser(Int32 id) in /src/MyService.cs:line 42\n   at UserController.Get(Int32 id) in /src/UserController.cs:line 15",
  "innerException": {
    "type": "ArgumentNullException",
    "message": "Value cannot be null. (Parameter 'user')"
  }
}
```

## Custom Exception Mapping

### Basic Mapping

```csharp
services.AddProblemDetailsMiddleware(options =>
{
    options.Map<MyCustomException>(422, "Custom Business Error", 
        type: "https://myapi.com/errors/business-rule",
        detail: "A business rule was violated");
});
```

### Domain Exception Examples

```csharp
// Business rule violations
options.Map<BusinessRuleValidationException>(422, "Business Rule Violation");

// Domain validation errors
options.Map<DomainValidationException>(400, "Domain Validation Failed");

// Aggregate not found
options.Map<AggregateNotFoundException>(404, "Resource Not Found");

// Concurrency conflicts
options.Map<ConcurrencyException>(409, "Conflict");
```

## Advanced Configuration

### Custom Status Code Handling

```csharp
services.AddProblemDetailsMiddleware(options =>
{
    // Only handle server errors (5xx)
    options.ShouldHandleStatusCode = statusCode => statusCode >= 500;
    
    // Or handle all errors except specific ones
    options.ShouldHandleStatusCode = statusCode => 
        statusCode >= 400 && statusCode != 401 && statusCode != 403;
});
```

### Environment-Specific Configuration

```csharp
services.AddProblemDetailsMiddleware(options =>
{
    options.IncludeExceptionDetails = builder.Environment.IsDevelopment();
    
    if (builder.Environment.IsProduction())
    {
        // More restrictive in production
        options.ShouldHandleStatusCode = statusCode => statusCode >= 500;
    }
});
```

## Integration with Other Middleware

### Correct Middleware Order

```csharp
var app = builder.Build();

// 1. Security headers (if any)
app.UseSecurityHeaders();

// 2. Problem Details (early for exception handling)
app.UseProblemDetailsMiddleware();

// 3. HTTPS redirection
app.UseHttpsRedirection();

// 4. Correlation ID
app.UseCorrelationId();

// 5. Request logging
app.UseRequestLogging();

// 6. Routing
app.UseRouting();

// 7. CORS
app.UseCors();

// 8. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 9. Controllers/Endpoints
app.MapControllers();
```

### Correlation ID Integration

The middleware automatically detects correlation IDs from:
- `X-Correlation-ID` header
- `CorrelationId` header  
- `HttpContext.TraceIdentifier` (fallback)

## Comparison with Hellang.Middleware.ProblemDetails

| Feature | Custom Implementation | Hellang Package |
|---------|----------------------|-----------------|
| **Dependencies** | Zero external dependencies | Requires package reference |
| **RFC 7807 Compliance** | ✅ Full compliance | ✅ Full compliance |
| **Exception Mapping** | ✅ Fully customizable | ✅ Configurable |
| **Status Code Handling** | ✅ Configurable predicate | ✅ Built-in |
| **Development Support** | ✅ Conditional exception details | ✅ Built-in |
| **Strongly Typed IDs** | ✅ Native support | ❌ Requires custom serialization |
| **Correlation ID** | ✅ Auto-detection | ✅ Configurable |
| **Customization** | ✅ Full source control | ❌ Limited to package features |
| **Performance** | ✅ Optimized for use case | ✅ General purpose |

## Benefits of Custom Implementation

1. **No External Dependencies**: Reduces package dependencies and potential security vulnerabilities
2. **Full Control**: Complete control over error handling logic and response format
3. **Integration**: Seamless integration with other BuildingBlocks components
4. **Performance**: Optimized specifically for your use cases
5. **Maintainability**: Source code is part of your codebase
6. **Customization**: Unlimited customization possibilities

This custom implementation provides all the functionality of the Hellang package while giving you complete control over the error handling pipeline. 