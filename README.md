# 🏗️ BuildingBlocks - Microservices Foundation

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()
[![Coverage](https://img.shields.io/badge/coverage-85%25-green.svg)]()

A comprehensive, production-ready set of building blocks for developing microservices using **Clean Architecture**, **Domain-Driven Design (DDD)**, and **CQRS** patterns with .NET 8.

## 📋 Table of Contents

- [Overview](#-overview)
- [Architecture](#-architecture)
- [Features](#-features)
- [Quick Start](#-quick-start)
- [Project Structure](#-project-structure)
- [Usage Examples](#-usage-examples)
- [Configuration](#-configuration)
- [Testing](#-testing)
- [Contributing](#-contributing)
- [License](#-license)

## 🎯 Overview

**BuildingBlocks** provides a solid foundation for building enterprise-grade microservices with:

- **Clean Architecture** with clear separation of concerns
- **Domain-Driven Design** patterns and practices
- **CQRS** (Command Query Responsibility Segregation) implementation
- **Strongly-typed IDs** for enhanced type safety
- **Repository & Unit of Work** patterns
- **Domain Events** handling
- **Comprehensive API layer** with authentication, validation, and documentation
- **Caching**, **Logging**, **Health Checks**, and **Monitoring**

## 🏛️ Architecture

The solution follows **Clean Architecture** principles with four main layers:

```
┌─────────────────────────────────────────────────────────┐
│                      API Layer                          │
│  Controllers, Endpoints, Middleware, Authentication     │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│                 Application Layer                       │
│     CQRS, Mediator, Behaviors, Services, DTOs          │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│                Infrastructure Layer                     │
│   EF Core, Repositories, Caching, External Services    │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│                   Domain Layer                          │
│    Entities, Value Objects, Domain Events, Rules       │
└─────────────────────────────────────────────────────────┘
```

## ✨ Features

### 🏗️ **Domain Layer**
- **Entities & Aggregate Roots** with strongly-typed IDs
- **Value Objects** for complex domain concepts
- **Domain Events** for decoupled communication
- **Business Rules** validation
- **Specifications** pattern for complex queries
- **Repository** abstractions

### 🚀 **Application Layer**
- **CQRS** implementation with commands and queries
- **Mediator** pattern for request handling
- **Pipeline Behaviors** (Logging, Validation, Caching, Transactions)
- **Application Services** abstractions
- **Domain Event** handlers
- **Data Transfer Objects** (DTOs)

### 🏭 **Infrastructure Layer**
- **Entity Framework Core** with **Repository Pattern** and **Unit of Work**
- **Database Migrations** and **Seeding** infrastructure
- **OpenTelemetry** integration for **observability**

### 🌐 **API Layer**
- **Minimal APIs** with endpoint pattern
- **JWT Bearer** & **API Key** authentication
- **Role-based authorization**
- **API Versioning** (URL, Header, Query)
- **OpenAPI/Swagger** documentation
- **Rate Limiting** & **CORS**
- **Health Checks** & **Monitoring**
- **Global Exception Handling**
- **Request/Response Logging**

### 🔧 **Build & Development**
- **Centralized Package Management** (CPM)
- **Code Analysis** with StyleCop & SonarAnalyzer
- **EditorConfig** for consistent formatting
- **Custom Build Targets** for CI/CD
- **Docker** support (coming soon)

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/sql-server) (optional, can use SQLite/InMemory)

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/buildingblocks.git
cd buildingblocks
```

### 2. Build the Solution

```bash
# Restore packages
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test
```

### 3. Basic Usage

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

// Configure pipeline
app.UseApi();

// Map endpoints
app.MapVersionedApiEndpoints("1", endpoints =>
{
    endpoints.MapGet<GetUsersEndpoint>("/users");
    endpoints.MapPost<CreateUserEndpoint>("/users");
});

app.Run();
```

## 📁 Project Structure

```
📦 BuildingBlocks/
├── 📁 src/
│   ├── 📁 BuildingBlocks.Domain/
│   │   ├── 📁 Entities/           # Domain entities & aggregates
│   │   ├── 📁 ValueObjects/       # Value objects
│   │   ├── 📁 DomainEvents/       # Domain events
│   │   ├── 📁 Repository/         # Repository abstractions
│   │   ├── 📁 Specifications/     # Query specifications
│   │   └── 📁 StronglyTypedIds/   # Type-safe identifiers
│   │
│   ├── 📁 BuildingBlocks.Application/
│   │   ├── 📁 CQRS/              # Commands, queries, events
│   │   ├── 📁 Behaviors/         # Pipeline behaviors
│   │   ├── 📁 Services/          # Application services
│   │   ├── 📁 Validation/        # Validation logic
│   │   └── 📁 DTOs/              # Data transfer objects
│   │
│   ├── 📁 BuildingBlocks.Infrastructure/
│   │   ├── 📁 Data/              # EF Core, repositories
│   │   ├── 📁 Caching/           # Cache implementations
│   │   ├── 📁 Messaging/         # Message bus
│   │   └── 📁 External/          # External service integrations
│   │
│   └── 📁 BuildingBlocks.API/
│       ├── 📁 Endpoints/         # Minimal API endpoints
│       ├── 📁 Middleware/        # Custom middleware
│       ├── 📁 Authentication/    # Auth implementations
│       └── 📁 Configuration/     # API configuration
│
├── 📁 tests/                     # Unit & integration tests
├── 📁 docs/                      # Documentation
├── 📁 samples/                   # Usage examples
├── 📄 Directory.Build.props      # MSBuild properties
├── 📄 Directory.Packages.props   # Package versions
├── 📄 global.json               # .NET SDK version
└── 📄 .editorconfig             # Code formatting
```

## 💡 Usage Examples

### Creating a Domain Entity

```csharp
public class UserId : GuidId<UserId>
{
    public UserId(Guid value) : base(value) { }
    public static UserId New() => new(Guid.NewGuid());
}

public class User : GuidAggregateRoot<UserId>
{
    public string Name { get; private set; }
    public Email Email { get; private set; }

    private User() { } // EF Core

    public User(UserId id, string name, Email email) : base(id)
    {
        Name = Guard.Against.NullOrEmpty(name);
        Email = Guard.Against.Null(email);
        
        AddDomainEvent(new UserCreatedEvent(Id, Name, Email));
    }
}
```

### CQRS Command & Handler

```csharp
public record CreateUserCommand(string Name, string Email) : ICommand<UserId>;

public class CreateUserHandler : ICommandHandler<CreateUserCommand, UserId>
{
    private readonly IRepository<User, UserId, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<UserId> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var email = new Email(command.Email);
        var user = new User(UserId.New(), command.Name, email);
        
        await _repository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return user.Id;
    }
}
```

### API Endpoint

```csharp
public class CreateUserEndpoint : EndpointBase<CreateUserRequest, ApiResponse<UserId>>
{
    public CreateUserEndpoint(IMediator mediator, ILogger<CreateUserEndpoint> logger) 
        : base(mediator, logger) { }

    public override async Task<IResult> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        return await SafeExecuteAsync(async () =>
        {
            var command = new CreateUserCommand(request.Name, request.Email);
            var userId = await Mediator.SendAsync(command, cancellationToken);
            return Success(userId, "User created successfully");
        }, "CreateUser");
    }
}
```

## ⚙️ Configuration

### appsettings.json

```json
{
  "Api": {
    "Title": "My Microservice API",
    "Version": "1.0",
    "EnableSwaggerInProduction": false
  },
  "Authentication": {
    "Jwt": {
      "Enabled": true,
      "SecretKey": "your-secret-key",
      "Issuer": "your-issuer",
      "Audience": "your-audience"
    },
    "ApiKey": {
      "Enabled": true,
      "HeaderName": "X-API-Key"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MyDb;Trusted_Connection=true;TrustServerCertificate=true"
  },
  "Caching": {
    "Redis": {
      "ConnectionString": "localhost:6379",
      "Database": 0
    }
  }
}
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/BuildingBlocks.Domain.Tests/
```

## 🛠️ Development Tools

- **StyleCop** for code style enforcement
- **SonarAnalyzer** for code quality
- **EditorConfig** for consistent formatting
- **Centralized Package Management** for version control
- **Custom MSBuild targets** for automation

## 📚 Additional Resources

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://domainlanguage.com/ddd/)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [.NET 8 Documentation](https://docs.microsoft.com/dotnet/)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please ensure your code follows the established patterns and includes appropriate tests.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Clean Architecture** by Robert C. Martin
- **Domain-Driven Design** by Eric Evans
- **.NET Community** for excellent patterns and practices

---

**VibeMicro BuildingBlocks** - Building the future, one microservice at a time! 🚀 