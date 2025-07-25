# Using BuildingBlocks with Centralized Package Management

## Overview
This guide shows how to create new microservices using the BuildingBlocks foundation with automatic package management, feature flags, and intelligent project type detection. No manual package references needed!

## 📋 Table of Contents
- [🚀 Quick Start](#-quick-start)
- [🏗️ Project Creation Workflows](#-project-creation-workflows)
- [🎯 Feature Configuration](#-feature-configuration)
- [🤖 AI-Assisted Development](#-ai-assisted-development)
- [📦 Centralized Package Management](#-centralized-package-management)
- [🛠️ Advanced Configuration](#-advanced-configuration)
- [📚 Best Practices](#-best-practices)

---

## 🚀 Quick Start

### 1. Copy Build Configuration Files
Copy these files from BuildingBlocks to your new solution:
```
YourSolution/
├── Directory.Build.props       # Global properties & feature flags
├── Directory.Build.targets     # Centralized package management
├── Directory.Packages.props    # Central Package Management (optional)
├── global.json                 # .NET SDK version
├── BuildingBlocks.ruleset      # Code analysis rules
└── coverlet.runsettings        # Test coverage settings
```

### 2. Reference BuildingBlocks Projects
Add project references to BuildingBlocks layers:
```xml
<!-- In your Domain project -->
<ItemGroup>
  <ProjectReference Include="..\..\BuildingBlocks\BuildingBlocks.Domain\BuildingBlocks.Domain.csproj" />
</ItemGroup>

<!-- In your Application project -->
<ItemGroup>
  <ProjectReference Include="..\..\BuildingBlocks\BuildingBlocks.Application\BuildingBlocks.Application.csproj" />
  <ProjectReference Include="..\YourService.Domain\YourService.Domain.csproj" />
</ItemGroup>

<!-- In your Infrastructure project -->
<ItemGroup>
  <ProjectReference Include="..\..\BuildingBlocks\BuildingBlocks.Infrastructure\BuildingBlocks.Infrastructure.csproj" />
  <ProjectReference Include="..\YourService.Application\YourService.Application.csproj" />
</ItemGroup>

<!-- In your API project -->
<ItemGroup>
  <ProjectReference Include="..\..\BuildingBlocks\BuildingBlocks.API\BuildingBlocks.API.csproj" />
  <ProjectReference Include="..\YourService.Infrastructure\YourService.Infrastructure.csproj" />
</ItemGroup>
```

### 3. That's It! 
No package references needed. The build system automatically includes packages based on project type and enabled features.

---

## 🏗️ Project Creation Workflows

### **A. New Microservice Structure**
```
YourService/
├── src/
│   ├── YourService.API/           # 🌐 API Layer (auto-detects API packages)
│   ├── YourService.Application/   # ⚙️ Application Layer (auto-detects app packages)
│   ├── YourService.Domain/        # 🏛️ Domain Layer (minimal packages)
│   └── YourService.Infrastructure/ # 🔧 Infrastructure Layer (full feature set)
├── tests/
│   ├── YourService.UnitTests/     # 🧪 Unit Tests (auto-detects test packages)
│   └── YourService.IntegrationTests/ # 🔬 Integration Tests
└── docker/
```

### **B. Project Type Auto-Detection**
The build system automatically detects project types:

| Project Pattern | Detected As | Gets Packages |
|----------------|-------------|---------------|
| `*.Domain.*` | Domain | Core packages only |
| `*.Application.*` | Application | Core + hosting, caching, validation |
| `*.Infrastructure.*` | Infrastructure | Full feature set (EF, Redis, etc.) |
| `*.API.*` | API | ASP.NET Core, Swagger, health checks |
| `*Test*` | Test | xUnit, Coverlet, test utilities |
| `*Benchmark*` | Benchmark | BenchmarkDotNet |

---

## 🎯 Feature Configuration

### **Global Feature Control**
Control features across your entire solution:

```xml
<!-- In Directory.Build.props or individual .csproj -->
<PropertyGroup>
  <!-- Disable features you don't need -->
  <IncludeEntityFramework>false</IncludeEntityFramework>
  <IncludeMessaging>false</IncludeMessaging>
  <IncludeSwagger>false</IncludeSwagger>
  <IncludeCaching>false</IncludeCaching>
</PropertyGroup>
```

### **Available Feature Flags**

#### 🌟 Global Features
```xml
<IncludeValidation>true</IncludeValidation>           <!-- FluentValidation -->
<IncludeSerialization>true</IncludeSerialization>     <!-- JSON, Protobuf, MessagePack -->
<IncludeHttpClient>true</IncludeHttpClient>           <!-- HTTP client with Polly -->
<IncludeSecurity>true</IncludeSecurity>               <!-- Security packages -->
<IncludeTestFramework>true</IncludeTestFramework>     <!-- xUnit and test packages -->
```

#### 🔧 Infrastructure Features
```xml
<IncludeEntityFramework>true</IncludeEntityFramework> <!-- EF Core + SQL Server -->
<IncludeCaching>true</IncludeCaching>                 <!-- Memory + Redis caching -->
<IncludeAuthentication>true</IncludeAuthentication>   <!-- JWT authentication -->
<IncludeMapping>true</IncludeMapping>                 <!-- AutoMapper + Mapster -->
<IncludeBackgroundServices>true</IncludeBackgroundServices> <!-- Hangfire -->
<IncludeMessaging>true</IncludeMessaging>             <!-- Service Bus + RabbitMQ -->
<IncludeMonitoring>true</IncludeMonitoring>           <!-- OpenTelemetry + Health Checks -->
<IncludeCloudStorage>true</IncludeCloudStorage>       <!-- Azure Blobs + AWS S3 -->
<IncludeEmailServices>true</IncludeEmailServices>     <!-- MailKit + MimeKit -->
```

#### 🌐 API Features
```xml
<IncludeSwagger>true</IncludeSwagger>                 <!-- OpenAPI documentation -->
<IncludeHealthChecks>true</IncludeHealthChecks>       <!-- Health check endpoints -->
<IncludeApiVersioning>true</IncludeApiVersioning>     <!-- API versioning -->
<IncludeRateLimiting>true</IncludeRateLimiting>       <!-- Rate limiting -->
```

#### 🛠️ Tooling Features
```xml
<IsSourceLinkSupported>true</IsSourceLinkSupported>   <!-- Source Link debugging -->
<UseMinVer>true</UseMinVer>                           <!-- MinVer versioning -->
```

---

## 🤖 AI-Assisted Development

### **A. Domain Layer Generation**
```prompt
Using BuildingBlocks.Domain as the foundation, create a [YourDomain] microservice domain layer with:

Domain Requirements:
- Entities: [Entity1], [Entity2], [Entity3]
- Value Objects: [ValueObject1], [ValueObject2]
- Business Rules: [Rule1], [Rule2]
- Domain Events: [Event1], [Event2]

Follow BuildingBlocks patterns:
- Use strongly-typed IDs (Entity1Id, Entity2Id)
- Inherit from Entity<TId, TIdValue> or AggregateRoot<TId, TIdValue>
- Create immutable value objects inheriting from ValueObject
- Implement business rules as separate classes inheriting from BusinessRuleBase
- Create domain events inheriting from DomainEventBase
- Use repository interfaces (IEntity1Repository)

The build system will automatically include the right packages. Generate complete domain model with:
1. Strongly-typed ID structs with JSON converters
2. Entity classes with proper encapsulation
3. Value object classes with validation
4. Domain event classes for state changes
5. Business rule classes with validation logic
6. Repository interfaces
7. Domain service interfaces if needed

Include comprehensive validation and follow DDD principles.
```

### **B. Application Layer Generation**
```prompt
Using BuildingBlocks.Application patterns, create the application layer for [YourDomain] with:

CQRS Requirements:
- Commands: Create[Entity], Update[Entity], Delete[Entity]
- Queries: Get[Entity]ById, List[Entity]s, Search[Entity]s
- DTOs: [Entity]Dto, Create[Entity]Dto, Update[Entity]Dto
- Validators: FluentValidation for all commands/queries

Follow BuildingBlocks patterns:
- Commands inherit from CommandBase
- Queries inherit from QueryBase<TResponse>
- Use PagedQuery<T> for list operations
- Implement ICommandHandler<TCommand> and IQueryHandler<TQuery, TResponse>
- Create proper DTOs inheriting from BaseDto
- Use comprehensive FluentValidation validators
- Include pipeline behaviors (logging, validation, performance)

The build system automatically includes:
- FluentValidation packages (IncludeValidation=true)
- Hosting abstractions for dependency injection
- Caching abstractions for response caching

Generate complete CQRS implementation with:
1. Command/Query classes with validation attributes
2. Handler classes with proper error handling
3. DTO classes with mapping logic
4. FluentValidation validator classes
5. Application service classes if needed
6. Integration event handlers

Include comprehensive error handling and validation.
```

### **C. Infrastructure Layer Generation**
```prompt
Using BuildingBlocks.Infrastructure, create the infrastructure layer for [YourDomain] with:

Infrastructure Requirements:
- DbContext: [YourDomain]DbContext with entity configurations
- Repositories: Concrete implementations of domain repositories
- Entity Configurations: EF Core type configurations
- Migrations: Initial database structure
- Services: Infrastructure service implementations

Follow BuildingBlocks patterns:
- DbContext inherits from DbContextBase
- Repositories inherit from Repository<TEntity, TId>
- Entity configurations inherit from EntityConfigurationBase
- Use proper audit and soft delete interceptors
- Configure strongly-typed IDs with value converters

The build system automatically includes (based on feature flags):
- Entity Framework Core (IncludeEntityFramework=true)
- Caching (Redis/Memory) (IncludeCaching=true)
- Authentication (JWT) (IncludeAuthentication=true)
- Mapping (AutoMapper/Mapster) (IncludeMaping=true)
- Monitoring (OpenTelemetry) (IncludeMonitoring=true)

Generate complete data access layer with:
1. DbContext with proper entity configurations
2. Repository implementations using UnitOfWork
3. EF Core entity type configurations
4. Database migration files
5. Infrastructure service implementations
6. Dependency injection registration

Include proper connection string configuration and health checks.
```

### **D. API Layer Generation**
```prompt
Using BuildingBlocks.API, create the API layer for [YourDomain] with:

API Requirements:
- Controllers: [Entity]Controller with full CRUD operations
- Endpoints: Minimal API endpoints as alternative
- DTOs: API-specific request/response models
- Validation: Comprehensive request validation
- Documentation: OpenAPI/Swagger documentation

Follow BuildingBlocks patterns:
- Controllers inherit from EndpointBase (if using base controller)
- All responses use ApiResponse<T> wrapper
- Proper HTTP status codes (200, 201, 400, 404, 500)
- Comprehensive input validation
- Rate limiting and caching where appropriate

The build system automatically includes (based on feature flags):
- ASP.NET Core packages for APIs
- Swagger/OpenAPI (IncludeSwagger=true)
- Health checks (IncludeHealthChecks=true)
- Rate limiting (IncludeRateLimiting=true)
- API versioning (IncludeApiVersioning=true)

Generate complete API with:
1. Controller classes with CRUD operations
2. Minimal API endpoint definitions
3. Request/response DTO classes
4. Input validation attributes
5. OpenAPI documentation attributes
6. Global exception handling
7. Health check endpoints

Include proper error handling, logging, and security.
```

---

## 📦 Centralized Package Management

### **How It Works**
1. **Project Type Detection**: Build system automatically detects project types
2. **Feature-Based Inclusion**: Packages included based on enabled features
3. **Smart Defaults**: Sensible defaults for each project type
4. **Override Capability**: Can disable features per project

### **Package Categories by Project Type**

#### 🏛️ Domain Projects
```
✅ Always Included:
- Microsoft.Extensions.DependencyInjection.Abstractions
- Microsoft.Extensions.Logging.Abstractions
- Microsoft.Extensions.Configuration.Abstractions
- System.ComponentModel.Annotations
```

#### ⚙️ Application Projects
```
✅ Always Included:
- Core packages (from Domain)
- Microsoft.Extensions.Hosting.Abstractions
- Microsoft.Extensions.Caching.Abstractions

🎛️ Feature-Based:
- FluentValidation (IncludeValidation=true)
- FluentValidation.DependencyInjectionExtensions
```

#### 🔧 Infrastructure Projects
```
✅ Always Included:
- Core packages
- Microsoft.Extensions.Options
- Microsoft.Extensions.Http

🎛️ Feature-Based (examples):
- EF Core packages (IncludeEntityFramework=true)
- Redis caching (IncludeCaching=true)
- JWT authentication (IncludeAuthentication=true)
- AutoMapper/Mapster (IncludeMapping=true)
- Hangfire (IncludeBackgroundServices=true)
- Service Bus/RabbitMQ (IncludeMessaging=true)
- OpenTelemetry (IncludeMonitoring=true)
```

#### 🌐 API Projects
```
✅ Always Included:
- ASP.NET Core packages
- Microsoft.AspNetCore.OpenApi
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Authorization
- Microsoft.AspNetCore.Cors

🎛️ Feature-Based:
- Swagger packages (IncludeSwagger=true)
- Health checks (IncludeHealthChecks=true)
- Rate limiting (IncludeRateLimiting=true)
- API versioning (IncludeApiVersioning=true)
```

#### 🧪 Test Projects
```
✅ Always Included:
- coverlet.collector
- coverlet.msbuild

🎛️ Feature-Based:
- Microsoft.NET.Test.Sdk (IncludeTestFramework=true)
- xunit
- xunit.runner.visualstudio
```

---

## 🛠️ Advanced Configuration

### **A. Per-Project Feature Override**
```xml
<!-- In YourService.Infrastructure.csproj -->
<PropertyGroup>
  <!-- This infrastructure project doesn't need messaging -->
  <IncludeMessaging>false</IncludeMessaging>
  
  <!-- But we want cloud storage -->
  <IncludeCloudStorage>true</IncludeCloudStorage>
</PropertyGroup>
```

### **B. Solution-Wide Configuration**
```xml
<!-- In Directory.Build.props -->
<PropertyGroup>
  <!-- Disable for entire solution -->
  <IncludeCloudStorage>false</IncludeCloudStorage>
  <IncludeEmailServices>false</IncludeEmailServices>
  
  <!-- Enable modern features -->
  <IncludeMonitoring>true</IncludeMonitoring>
  <IncludeSecurity>true</IncludeSecurity>
</PropertyGroup>
```

### **C. Environment-Specific Configuration**
```xml
<!-- Different features for different environments -->
<PropertyGroup Condition="'$(Environment)' == 'Development'">
  <IncludeSwagger>true</IncludeSwagger>
  <IncludeHealthChecks>true</IncludeHealthChecks>
</PropertyGroup>

<PropertyGroup Condition="'$(Environment)' == 'Production'">
  <IncludeSwagger>false</IncludeSwagger>
  <IncludeMonitoring>true</IncludeMonitoring>
</PropertyGroup>
```

### **D. Custom Build Targets**
```xml
<!-- Add to Directory.Build.targets -->
<Target Name="CustomPackageSelection" BeforeTargets="CollectPackageReferences">
  <ItemGroup Condition="'$(MSBuildProjectName.Contains('Legacy'))">
    <!-- Legacy projects get minimal packages -->
    <PackageReference Include="LegacyCompatibility.Package" />
  </ItemGroup>
</Target>
```

---

## 📚 Best Practices

### **A. Project Naming Conventions**
```
✅ Good Naming (Auto-detected):
- YourService.Domain
- YourService.Application  
- YourService.Infrastructure
- YourService.API
- YourService.Tests
- YourService.IntegrationTests

❌ Avoid (Won't auto-detect):
- YourService.Core (use .Domain)
- YourService.Services (use .Application)
- YourService.Data (use .Infrastructure)
- YourService.Web (use .API)
```

### **B. Feature Flag Strategy**
```xml
<!-- Start with defaults, then disable what you don't need -->
<PropertyGroup>
  <!-- Common disables for lightweight services -->
  <IncludeMessaging>false</IncludeMessaging>
  <IncludeBackgroundServices>false</IncludeBackgroundServices>
  <IncludeCloudStorage>false</IncludeCloudStorage>
  <IncludeEmailServices>false</IncludeEmailServices>
</PropertyGroup>
```

### **C. Development Workflow**
1. **Create Project Structure**: Use naming conventions for auto-detection
2. **Configure Features**: Enable/disable features in Directory.Build.props
3. **Add BuildingBlocks References**: Reference the foundation projects
4. **Generate Code**: Use AI prompts with BuildingBlocks patterns
5. **Build & Test**: Packages are included automatically

### **D. Code Review Checklist**
- ✅ No manual package references in .csproj files
- ✅ Project names follow conventions for auto-detection
- ✅ Feature flags are used appropriately
- ✅ BuildingBlocks patterns are followed
- ✅ Code analysis rules are satisfied

### **E. Performance Optimization**
```xml
<!-- For high-performance services -->
<PropertyGroup>
  <!-- Disable heavy features -->
  <IncludeSwagger>false</IncludeSwagger>
  <IncludeMonitoring>false</IncludeMonitoring>
  
  <!-- Enable essentials only -->
  <IncludeCaching>true</IncludeCaching>
  <IncludeSerialization>true</IncludeSerialization>
</PropertyGroup>
```

---

## 🎯 Migration from Manual Package Management

### **Step 1: Backup Current State**
```bash
# Create backup branch
git checkout -b backup-before-centralized-packages
git add -A && git commit -m "Backup before centralized package migration"
```

### **Step 2: Copy Build Files**
Copy `Directory.Build.props`, `Directory.Build.targets`, and `BuildingBlocks.ruleset` from BuildingBlocks solution.

### **Step 3: Remove Manual Package References**
Remove all `<PackageReference>` elements from .csproj files (except project-specific ones).

### **Step 4: Configure Features**
Set feature flags in Directory.Build.props based on your needs.

### **Step 5: Test Build**
```bash
dotnet clean
dotnet restore
dotnet build
```

### **Step 6: Verify Functionality**
Run tests and verify all expected packages are included.

---

This centralized approach transforms BuildingBlocks into a powerful, maintainable development platform that automatically manages dependencies while providing fine-grained control over features and packages. 