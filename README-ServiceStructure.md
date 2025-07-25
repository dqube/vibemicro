# Service Project Structure

This document outlines the standardized structure for service projects in the BuildingBlocks microservices architecture.

## 📁 Folder Structure

Each service follows the Clean Architecture pattern with standardized folder names and project naming conventions:

```
Services/
├── [ServiceName]/
│   ├── Domain/                          # 📁 Domain Layer Folder
│   │   ├── [ServiceName].Domain.csproj  # 📄 Domain Project File
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── StronglyTypedIds/
│   │   ├── DomainEvents/
│   │   ├── BusinessRules/
│   │   ├── Specifications/
│   │   ├── Repositories/
│   │   ├── Services/
│   │   └── Exceptions/
│   ├── Application/                              # 📁 Application Layer Folder
│   │   ├── [ServiceName].Application.csproj     # 📄 Application Project File
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Events/
│   │   ├── DTOs/
│   │   ├── Services/
│   │   ├── Behaviors/
│   │   ├── Validators/
│   │   └── Mapping/
│   ├── Infrastructure/                              # 📁 Infrastructure Layer Folder
│   │   ├── [ServiceName].Infrastructure.csproj     # 📄 Infrastructure Project File
│   │   ├── Data/
│   │   ├── Messaging/
│   │   ├── External/
│   │   ├── Configuration/
│   │   └── Services/
│   └── API/                              # 📁 API Layer Folder
│       ├── [ServiceName].API.csproj     # 📄 API Project File
│       ├── Endpoints/
│       ├── Middleware/
│       ├── Configuration/
│       └── Program.cs
```

## 🎯 Example: AuthService

```
Services/
├── AuthService/
│   ├── Domain/                          # Clean folder name
│   │   └── AuthService.Domain.csproj    # Descriptive project name
│   ├── Application/
│   │   └── AuthService.Application.csproj
│   ├── Infrastructure/
│   │   └── AuthService.Infrastructure.csproj
│   └── API/
│       └── AuthService.API.csproj
```

## 🔧 Benefits of This Structure

### 1. Clean Navigation
- ✅ Consistent folder names across all services
- ✅ Easy to find specific layers: `/Domain`, `/Application`, `/Infrastructure`, `/API`
- ✅ Predictable structure for developers

### 2. Clear Project References
```xml
<!-- In AuthService.Application.csproj -->
<ProjectReference Include="..\Domain\AuthService.Domain.csproj" />

<!-- In AuthService.Infrastructure.csproj -->
<ProjectReference Include="..\Domain\AuthService.Domain.csproj" />
<ProjectReference Include="..\Application\AuthService.Application.csproj" />

<!-- In AuthService.API.csproj -->
<ProjectReference Include="..\Domain\AuthService.Domain.csproj" />
<ProjectReference Include="..\Application\AuthService.Application.csproj" />
<ProjectReference Include="..\Infrastructure\AuthService.Infrastructure.csproj" />
```

### 3. Build Target Automation
```xml
<!-- In Directory.Build.targets -->

<!-- Apply to all Domain projects -->
<ItemGroup Condition="$(MSBuildProjectName.EndsWith('.Domain'))">
  <PackageReference Include="BuildingBlocks.Domain" />
</ItemGroup>

<!-- Apply to all Application projects -->
<ItemGroup Condition="$(MSBuildProjectName.EndsWith('.Application'))">
  <PackageReference Include="BuildingBlocks.Application" />
  <PackageReference Include="BuildingBlocks.Domain" />
</ItemGroup>

<!-- Apply to all Infrastructure projects -->
<ItemGroup Condition="$(MSBuildProjectName.EndsWith('.Infrastructure'))">
  <PackageReference Include="BuildingBlocks.Infrastructure" />
  <PackageReference Include="BuildingBlocks.Application" />
  <PackageReference Include="BuildingBlocks.Domain" />
</ItemGroup>

<!-- Apply to all API projects -->
<ItemGroup Condition="$(MSBuildProjectName.EndsWith('.API'))">
  <PackageReference Include="BuildingBlocks.API" />
  <PackageReference Include="BuildingBlocks.Infrastructure" />
  <PackageReference Include="BuildingBlocks.Application" />
  <PackageReference Include="BuildingBlocks.Domain" />
</ItemGroup>
```

### 4. Pattern Matching for DevOps
```bash
# Find all Domain projects
find . -name "*.Domain.csproj"

# Find all API projects  
find . -name "*.API.csproj"

# Find all projects for specific service
find . -name "AuthService.*.csproj"

# PowerShell equivalent
Get-ChildItem -Recurse -Filter "*.Domain.csproj"
Get-ChildItem -Recurse -Filter "AuthService.*.csproj"
```

## 📋 Naming Conventions

### Folder Names
- ✅ **Domain** (not `Domain Layer` or `AuthService.Domain`)
- ✅ **Application** (not `App` or `AuthService.Application`)
- ✅ **Infrastructure** (not `Infra` or `AuthService.Infrastructure`)
- ✅ **API** (not `WebAPI` or `AuthService.API`)

### Project Names
- ✅ **[ServiceName].Domain.csproj**
- ✅ **[ServiceName].Application.csproj**
- ✅ **[ServiceName].Infrastructure.csproj**
- ✅ **[ServiceName].API.csproj**

### Namespace Convention
```csharp
// In AuthService.Domain project
namespace AuthService.Domain.Entities;
namespace AuthService.Domain.ValueObjects;

// In AuthService.Application project  
namespace AuthService.Application.Commands;
namespace AuthService.Application.Queries;

// In AuthService.Infrastructure project
namespace AuthService.Infrastructure.Data;
namespace AuthService.Infrastructure.Messaging;

// In AuthService.API project
namespace AuthService.API.Endpoints;
namespace AuthService.API.Configuration;
```

## 🚀 Creating a New Service

### 1. Create Folder Structure
```bash
mkdir -p Services/NewService/{Domain,Application,Infrastructure,API}
```

### 2. Create Projects
```bash
cd Services/NewService/Domain
dotnet new classlib -n NewService.Domain

cd ../Application  
dotnet new classlib -n NewService.Application

cd ../Infrastructure
dotnet new classlib -n NewService.Infrastructure

cd ../API
dotnet new web -n NewService.API
```

### 3. Add Project References
```bash
cd ../Application
dotnet add reference ../Domain/NewService.Domain.csproj

cd ../Infrastructure
dotnet add reference ../Domain/NewService.Domain.csproj
dotnet add reference ../Application/NewService.Application.csproj

cd ../API
dotnet add reference ../Domain/NewService.Domain.csproj
dotnet add reference ../Application/NewService.Application.csproj  
dotnet add reference ../Infrastructure/NewService.Infrastructure.csproj
```

### 4. Add BuildingBlocks References
```bash
# Domain
cd Domain
dotnet add reference ../../../BuildingBlocks.Domain/BuildingBlocks.Domain.csproj

# Application  
cd ../Application
dotnet add reference ../../../BuildingBlocks.Application/BuildingBlocks.Application.csproj

# Infrastructure
cd ../Infrastructure
dotnet add reference ../../../BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj

# API
cd ../API
dotnet add reference ../../../BuildingBlocks.API/BuildingBlocks.API.csproj
```

## ✅ Consistency Rules

1. **Folder names are always just the layer name**: `Domain`, `Application`, `Infrastructure`, `API`
2. **Project names always include the service prefix**: `[ServiceName].[Layer].csproj`
3. **All services follow identical structure** for consistency
4. **Build targets can use EndsWith patterns** for automation
5. **Project references use relative paths** to sibling layer folders

This structure provides the perfect balance of clean organization and powerful automation capabilities while maintaining consistency across all microservices. 