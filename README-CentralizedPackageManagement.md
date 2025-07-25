# Centralized Package Management

This solution uses a centralized package management system through `Directory.Build.props` and `Directory.Build.targets` files. All package references are handled automatically based on project types and feature flags.

## Project Type Detection

Projects are automatically categorized based on their names and paths:

- **Domain Projects**: Contains `Domain` in name or path
- **Application Projects**: Contains `Application` in name or path  
- **Infrastructure Projects**: Contains `Infrastructure` in name or path
- **API Projects**: Contains `API` in name or path
- **Test Projects**: Contains `Test` in name or path
- **Benchmark Projects**: Contains `Benchmark` in name or path

## Package Categories

### Core Packages (All Projects)
- Microsoft.Extensions.DependencyInjection.Abstractions
- Microsoft.Extensions.Logging.Abstractions
- Microsoft.Extensions.Configuration.Abstractions
- System.ComponentModel.Annotations

### Domain Projects
- Only core packages (minimal dependencies)

### Application Projects
- Core packages
- Microsoft.Extensions.Hosting.Abstractions
- Microsoft.Extensions.Caching.Abstractions
- FluentValidation packages (if validation enabled)

### Infrastructure Projects
- Entity Framework Core (if enabled)
- Caching (Redis, Memory)
- Authentication & Authorization
- Mapping (AutoMapper, Mapster)
- Serialization (JSON, Protobuf, MessagePack)
- Background Services (Hangfire)
- Messaging (Azure Service Bus, RabbitMQ)
- Monitoring & Observability
- Cloud Storage (Azure, AWS)
- Email Services
- HTTP Client with Polly

### API Projects
- ASP.NET Core packages
- OpenAPI/Swagger
- Validation
- Rate Limiting
- Health Checks
- Monitoring
- Security
- Serialization

## Feature Control

You can enable/disable features by setting properties in individual project files:

```xml
<PropertyGroup>
  <!-- Disable specific features -->
  <IncludeSwagger>false</IncludeSwagger>
  <IncludeEntityFramework>false</IncludeEntityFramework>
  <IncludeCaching>false</IncludeCaching>
  <IncludeMessaging>false</IncludeMessaging>
</PropertyGroup>
```

## Available Feature Flags

### Global Features
- `IncludeValidation` - FluentValidation packages
- `IncludeSerialization` - JSON, Protobuf, MessagePack
- `IncludeHttpClient` - HTTP client with Polly
- `IncludeSecurity` - Security and cryptography packages
- `IncludeTestFramework` - xUnit and test packages

### Infrastructure Features
- `IncludeEntityFramework` - Entity Framework Core
- `IncludeCaching` - Memory and Redis caching
- `IncludeAuthentication` - JWT and authentication
- `IncludeMapping` - AutoMapper and Mapster
- `IncludeBackgroundServices` - Hangfire
- `IncludeMessaging` - Service Bus and RabbitMQ
- `IncludeMonitoring` - Health checks and OpenTelemetry
- `IncludeCloudStorage` - Azure Blobs and AWS S3
- `IncludeEmailServices` - MailKit and MimeKit

### API Features
- `IncludeSwagger` - OpenAPI documentation
- `IncludeHealthChecks` - Health check endpoints
- `IncludeApiVersioning` - API versioning
- `IncludeRateLimiting` - Rate limiting middleware

### Tooling Features
- `IsSourceLinkSupported` - Source Link for debugging
- `UseMinVer` - MinVer for versioning

## Benefits

1. **Consistency**: All projects use the same package versions
2. **Maintainability**: Update packages in one place
3. **Flexibility**: Enable/disable features per project
4. **Performance**: Only include packages you need
5. **Clean Project Files**: No package clutter in .csproj files

## Example: Creating a Minimal API Project

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    
    <!-- Disable unnecessary features -->
    <IncludeSwagger>false</IncludeSwagger>
    <IncludeHealthChecks>false</IncludeHealthChecks>
    <IncludeRateLimiting>false</IncludeRateLimiting>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="../MyApp.Application/MyApp.Application.csproj" />
  </ItemGroup>
</Project>
```

This project will automatically get the core API packages but skip Swagger, health checks, and rate limiting. 