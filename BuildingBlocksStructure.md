# BuildingBlocks Architecture Structure

## Overview
This document outlines the complete architecture and file structure of the BuildingBlocks library - a comprehensive foundation for .NET microservices following Domain-Driven Design (DDD), Clean Architecture, and CQRS patterns with centralized package management and modern build configuration.

## 📋 Table of Contents
- [🏗️ Build Configuration](#-build-configuration)
- [🔗 Centralized Package Management](#-centralized-package-management)
- [🏛️ Domain Layer](#-domain-layer)
- [⚙️ Application Layer](#-application-layer)
- [🔧 Infrastructure Layer](#-infrastructure-layer)
- [🌐 API Layer](#-api-layer)
- [🚀 Services Example](#-services-example)
- [📚 Documentation](#-documentation)

---

## 🏗️ Build Configuration
**Centralized build configuration across the entire solution**

### 📄 Root Configuration Files
```
├── Directory.Build.props       # Global properties and feature flags
├── Directory.Build.targets     # Centralized package management
├── Directory.Packages.props    # Central Package Management (CPM)
├── global.json                 # .NET SDK version
├── BuildingBlocks.ruleset      # Code analysis rules
├── coverlet.runsettings        # Code coverage settings
└── VibeMicro.sln              # Solution file
```

### 🎛️ Build Properties (Directory.Build.props)
```
Common Properties:
├── TargetFramework: net8.0
├── LangVersion: latest
├── ImplicitUsings: enable
├── Nullable: enable
├── TreatWarningsAsErrors: true
└── UseArtifactsOutput: true

Package Information:
├── Company: VibeMicro
├── Product: BuildingBlocks
├── Authors: VibeMicro Team
├── PackageLicenseExpression: MIT
└── RepositoryUrl: https://github.com/vibemicro/buildingblocks

Version Management:
├── VersionPrefix: 1.0.0
├── AssemblyVersion: 1.0.0.0
├── FileVersion: 1.0.0.0
└── InformationalVersion: Auto-generated

Source Code Analysis:
├── EnableNETAnalyzers: true
├── AnalysisLevel: latest
├── AnalysisMode: AllEnabledByDefault
└── RunAnalyzersDuringBuild: true

Project Type Detection:
├── IsApiProject: Auto-detected
├── IsInfrastructureProject: Auto-detected
├── IsDomainProject: Auto-detected
├── IsApplicationProject: Auto-detected
├── IsTestProject: Auto-detected
└── IsBenchmarkProject: Auto-detected
```

---

## 🔗 Centralized Package Management
**All package references managed centrally through build targets**

### 🎯 Feature Control Properties
*Enable/disable functionality via MSBuild properties*

```xml
<!-- Global Features -->
<IncludeValidation>true</IncludeValidation>
<IncludeSerialization>true</IncludeSerialization>
<IncludeHttpClient>true</IncludeHttpClient>
<IncludeSecurity>true</IncludeSecurity>

<!-- Infrastructure Features -->
<IncludeEntityFramework>true</IncludeEntityFramework>
<IncludeCaching>true</IncludeCaching>
<IncludeAuthentication>true</IncludeAuthentication>
<IncludeMapping>true</IncludeMapping>
<IncludeBackgroundServices>true</IncludeBackgroundServices>
<IncludeMessaging>true</IncludeMessaging>
<IncludeMonitoring>true</IncludeMonitoring>
<IncludeCloudStorage>true</IncludeCloudStorage>
<IncludeEmailServices>true</IncludeEmailServices>

<!-- API Features -->
<IncludeSwagger>true</IncludeSwagger>
<IncludeHealthChecks>true</IncludeHealthChecks>
<IncludeApiVersioning>true</IncludeApiVersioning>
<IncludeRateLimiting>true</IncludeRateLimiting>

<!-- Tooling Features -->
<IsSourceLinkSupported>true</IsSourceLinkSupported>
<UseMinVer>true</UseMinVer>
```

### 📦 Package Categories by Project Type

#### All Projects (Core)
- Microsoft.Extensions.DependencyInjection.Abstractions
- Microsoft.Extensions.Logging.Abstractions
- Microsoft.Extensions.Configuration.Abstractions
- System.ComponentModel.Annotations

#### Domain Projects
- Core packages only (minimal dependencies)

#### Application Projects  
- Core packages + Hosting abstractions
- Caching abstractions
- FluentValidation (if enabled)

#### Infrastructure Projects
- Entity Framework Core
- Caching (Memory, Redis)
- Authentication & Authorization
- Mapping (AutoMapper, Mapster)
- Serialization (JSON, Protobuf, MessagePack)
- Background Services (Hangfire)
- Messaging (Azure Service Bus, RabbitMQ)
- Monitoring (OpenTelemetry, Health Checks)
- Cloud Storage (Azure, AWS)
- Email Services (MailKit)
- HTTP Client with Polly

#### API Projects
- ASP.NET Core packages
- OpenAPI/Swagger
- Validation
- Rate Limiting
- Health Checks
- Security
- Monitoring

#### Test Projects
- xUnit framework
- Coverlet for code coverage
- Test SDK

---

## 🏛️ Domain Layer
**BuildingBlocks.Domain** - Core business logic and domain models following DDD principles.

### 📁 Entities
*Domain objects with identity and lifecycle*
```
├── Entity.cs                    # Base entity with strongly-typed ID
├── AggregateRoot.cs            # Aggregate root with domain events
├── IAuditableEntity.cs         # Audit fields interface  
└── ISoftDeletable.cs           # Soft deletion interface
```

### 💎 Value Objects
*Immutable objects defined by their attributes*
```
├── ValueObject.cs              # Base record for multi-property value objects
├── SingleValueObject.cs        # Base record for single-value wrappers
└── Enumeration.cs             # Rich enumeration pattern
```

### 🔑 Strongly Typed IDs
*Type-safe identifiers using readonly structs*
```
├── IStronglyTypedId.cs         # Base interface
├── StronglyTypedId.cs          # Base implementation
├── IntId.cs                    # Integer-based ID struct
├── LongId.cs                   # Long-based ID struct
├── GuidId.cs                   # GUID-based ID struct
└── StringId.cs                 # String-based ID struct
```

### 📢 Domain Events
*Business event system for domain state changes*
```
├── IDomainEvent.cs             # Event interface
├── IDomainEventDispatcher.cs   # Dispatcher interface
├── DomainEventDispatcher.cs    # Default implementation
├── DomainEventBase.cs          # Base record for events
└── IDomainEventHandler.cs      # Event handler interface
```

### 🗃️ Repository
*Data access abstractions*
```
├── IRepository.cs              # Generic repository
├── IReadOnlyRepository.cs      # Read-only operations
├── IUnitOfWork.cs             # Transaction boundary
└── RepositoryBase.cs          # Base implementation
```

### 🔍 Specifications
*Encapsulated query logic*
```
├── ISpecification.cs           # Specification interface
├── Specification.cs            # Base implementation
├── AndSpecification.cs         # Logical AND
├── OrSpecification.cs          # Logical OR
├── NotSpecification.cs         # Logical NOT
└── SpecificationEvaluator.cs   # EF Core evaluation
```

### ⚠️ Exceptions
*Domain-specific error handling*
```
├── DomainException.cs                    # Base domain exception
├── BusinessRuleValidationException.cs   # Rule violation
├── AggregateNotFoundException.cs        # Entity not found
├── ConcurrencyException.cs              # Optimistic concurrency
├── InvalidOperationDomainException.cs   # Invalid operations
└── InvariantViolationException.cs       # Invariant violations
```

### 📏 Business Rules
*Domain validation and business logic*
```
├── IBusinessRule.cs            # Rule interface
├── BusinessRuleBase.cs         # Base implementation
└── CompositeBusinessRule.cs    # Rule composition
```

### 🛡️ Guards
*Defensive programming utilities*
```
├── Guard.cs                    # Guard utility class
└── GuardExtensions.cs          # Guard extension methods
```

### 🔧 Common Value Objects
*Shared value objects and utilities*
```
├── Money.cs                    # Money with currency
├── Currency.cs                 # Currency value object
├── DateRange.cs               # Date range value object
├── Address.cs                 # Address value object
├── Email.cs                   # Email value object
├── PhoneNumber.cs             # Phone value object
├── Percentage.cs              # Percentage value object
└── Url.cs                     # URL value object
```

### ✅ Validation
*Domain validation framework*
```
├── IDomainValidator.cs         # Domain validator interface
├── DomainValidatorBase.cs      # Base domain validator
├── ValidationError.cs          # Validation error details
└── ValidationResult.cs         # Validation result
```

### 🔧 Services
*Domain services for complex business logic*
```
├── IDomainService.cs           # Domain service interface
└── DomainServiceBase.cs        # Base domain service
```

### 🛠️ Extensions
*Domain utility methods*
```
├── DomainExtensions.cs         # Domain object extensions
└── JsonExtensions.cs           # JSON serialization extensions
```

---

## ⚙️ Application Layer
**BuildingBlocks.Application** - Application services, CQRS implementation, and orchestration logic.

### 🎯 CQRS
*Command Query Responsibility Segregation implementation*

#### Commands
```
├── ICommand.cs                 # Command interfaces
├── ICommandHandler.cs          # Command handler interfaces
└── CommandBase.cs             # Base command implementation
```

#### Queries
```
├── IQuery.cs                  # Query interfaces
├── IQueryHandler.cs           # Query handler interfaces
├── QueryBase.cs               # Base query implementation
├── PagedQuery.cs              # Pagination support
├── PagedResult.cs             # Paged result wrapper
└── SortingQuery.cs            # Sorting support
```

#### Events
```
├── IEvent.cs                  # Application event interface
├── IEventHandler.cs           # Event handler interface
├── IIntegrationEvent.cs       # Integration event interface
├── IntegrationEventBase.cs    # Base integration event
└── DomainEventNotification.cs # Domain event notification
```

#### Messages
```
├── IMessage.cs                # Message interface
├── IStreamMessage.cs          # Stream message interface
├── MessageBase.cs             # Base message implementation
└── IMessageContext.cs         # Message context interface
```

#### Mediator
```
├── IMediator.cs               # Custom mediator interface
└── Mediator.cs                # Custom mediator implementation
```

### 🔄 Behaviors
*Cross-cutting concerns for request processing*
```
├── IPipelineBehavior.cs       # Pipeline behavior interface
├── LoggingBehavior.cs         # Request/response logging
├── ValidationBehavior.cs      # Input validation
├── CachingBehavior.cs         # Response caching
├── TransactionBehavior.cs     # Database transactions
├── PerformanceBehavior.cs     # Performance monitoring
└── RetryBehavior.cs           # Retry policies
```

### 🏢 Services
*Application-level services*
```
├── IApplicationService.cs     # Application service interface
├── ApplicationServiceBase.cs  # Base application service
├── IDomainEventService.cs     # Domain event service interface
├── DomainEventService.cs      # Domain event service implementation
├── IServiceContext.cs         # Service context interface
├── ServiceContext.cs          # Service context implementation
├── OutboxBackgroundService.cs # Outbox pattern background service
└── InboxBackgroundService.cs  # Inbox pattern background service
```

### ✅ Validation
*Input validation framework*
```
└── IValidator.cs              # Validator interface
```

### 💾 Caching
*Application-level caching abstractions*
```
└── ICacheService.cs           # Cache service interface
```

### 📨 Messaging
*Message bus abstractions*
```
└── IMessageBus.cs             # Message bus interface
```

### 📊 DTOs
*Data Transfer Objects*
```
└── BaseDto.cs                 # Base DTO
```

### 🔐 Security
*Security context and user information*
```
└── ICurrentUser.cs            # Current user service
```

### 📥 Inbox
*Inbox pattern implementation*
```
├── IInboxService.cs           # Inbox service interface
├── InboxMessage.cs            # Inbox message entity
├── InboxMessageStatus.cs      # Message status enumeration
├── IInboxProcessor.cs         # Inbox processor interface
└── InboxProcessor.cs          # Inbox processor implementation
```

### 📤 Outbox
*Outbox pattern implementation*
```
├── IOutboxService.cs          # Outbox service interface
├── OutboxMessage.cs           # Outbox message entity
├── OutboxMessageStatus.cs     # Message status enumeration
├── IOutboxProcessor.cs        # Outbox processor interface
└── OutboxProcessor.cs         # Outbox processor implementation
```

### 🚀 Dispatchers
*Message dispatching services*
```
├── ICommandDispatcher.cs      # Command dispatcher interface
├── CommandDispatcher.cs       # Command dispatcher implementation
├── IQueryDispatcher.cs        # Query dispatcher interface
├── QueryDispatcher.cs         # Query dispatcher implementation
├── IEventDispatcher.cs        # Event dispatcher interface
├── EventDispatcher.cs         # Event dispatcher implementation
├── IMessageDispatcher.cs      # Message dispatcher interface
└── MessageDispatcher.cs       # Message dispatcher implementation
```

### 🎭 Sagas
*Long-running business processes*
```
├── ISaga.cs                   # Saga interface
├── SagaBase.cs                # Base saga implementation
└── ISagaManager.cs            # Saga manager interface
```

### 🔄 Idempotency
*Idempotent operation support*
```
├── IIdempotencyService.cs     # Idempotency service interface
├── IdempotencyOptions.cs      # Idempotency configuration
└── IdempotencyRecord.cs       # Idempotency record
```

### 🛠️ Extensions
*Application layer extensions*
```
├── ServiceCollectionExtensions.cs # DI registration
└── QueryableExtensions.cs         # Queryable extensions
```

---

## 🔧 Infrastructure Layer
**BuildingBlocks.Infrastructure** - External concerns and infrastructure implementations.

### 🗄️ Data
*Data access and persistence*

#### Context
```
├── IDbContext.cs              # Database context interface
├── ApplicationDbContext.cs    # Application database context
├── DbContextBase.cs           # Base database context
└── IDbContextFactory.cs       # Database context factory
```

#### Repositories
```
├── Repository.cs              # Generic repository implementation
└── ReadOnlyRepository.cs      # Read-only repository implementation
```

#### Unit of Work
```
└── UnitOfWork.cs              # Unit of work implementation
```

#### Migrations
```
├── IMigrationRunner.cs        # Migration runner interface
└── MigrationRunner.cs         # Migration runner implementation
```

#### Seeding
```
├── IDataSeeder.cs             # Data seeder interface
├── DataSeederBase.cs          # Base data seeder
└── SeedDataExtensions.cs      # Seeding extensions
```

#### Interceptors
```
├── AuditInterceptor.cs        # Audit trail interceptor
├── DomainEventInterceptor.cs  # Domain event interceptor
└── SoftDeleteInterceptor.cs   # Soft delete interceptor
```

#### Configurations
```
├── EntityConfigurationBase.cs      # Base entity configuration
├── AuditableEntityConfiguration.cs # Auditable entity configuration
└── ValueObjectConfiguration.cs     # Value object configuration
```

### 💾 Caching
*Caching implementations*
```
├── DistributedCacheService.cs # Distributed cache implementation
├── InMemoryCacheService.cs    # In-memory cache implementation
├── MemoryCacheService.cs      # Memory cache implementation
├── RedisCacheService.cs       # Redis cache implementation
├── CacheKeyGenerator.cs       # Cache key generation
└── CacheConfiguration.cs      # Cache configuration
```

### 📨 Messaging
*Message bus implementations*

#### Message Bus
```
├── IMessageBus.cs             # Message bus interface
└── InMemoryMessageBus.cs      # In-memory message bus
```

#### Serialization
```
├── IMessageSerializer.cs      # Message serializer interface
├── JsonMessageSerializer.cs   # JSON message serializer
└── BinaryMessageSerializer.cs # Binary message serializer
```

#### Configuration
```
└── MessageBusConfiguration.cs # Message bus configuration
```

### 🔐 Security
*Security implementations*

#### Encryption
```
└── IEncryptionService.cs      # Encryption service interface
```

### 📄 Serialization
*Serialization services*

#### Json
```
└── JsonSerializationService.cs # JSON serialization service
```

### 🔄 Idempotency
*Idempotency implementation*
```
├── IdempotencyEntity.cs       # Idempotency entity
└── IIdempotencyRepository.cs  # Idempotency repository
```

### 🛠️ Extensions
*Infrastructure extensions*
```
└── ServiceCollectionExtensions.cs # DI registration
```

---

## 🌐 API Layer
**BuildingBlocks.API** - Web API infrastructure, endpoints, and HTTP concerns.

### 🎯 Endpoints
*API endpoint definitions*

#### Base
```
├── CrudEndpoints.cs           # CRUD endpoint patterns
└── QueryEndpoints.cs          # Query endpoint patterns
```

#### Extensions
```
└── MinimalApiExtensions.cs    # Minimal API extensions
```

### 🛡️ Middleware
*HTTP request processing pipeline*

#### Error Handling
```
├── GlobalExceptionMiddleware.cs # Global exception handling
├── ErrorResponse.cs            # Error response models
├── ProblemDetailsFactory.cs    # RFC 7807 problem details
└── ExceptionHandlingExtensions.cs # Exception handling extensions
```

#### Logging
```
├── RequestLoggingMiddleware.cs # Request/response logging
└── CorrelationIdMiddleware.cs  # Correlation ID handling
```

### 📊 Responses
*API response models*

#### Base
```
├── ApiResponse.cs             # Standard API response
└── PagedResponse.cs           # Paged response wrapper
```

#### Builders
```
└── ApiResponseBuilder.cs      # Response builder
```

### ⚙️ Configuration
*API configuration*

#### Options
```
└── ApiOptions.cs              # API configuration options
```

### 🔧 Utilities
*API utilities and helpers*

#### Constants
```
└── ApiConstants.cs            # API constants
```

### 🛠️ Extensions
*API layer extensions*
```
├── EndpointExtensions.cs            # Endpoint extensions
├── EndpointRouteBuilderExtensions.cs # Route builder extensions
├── ServiceCollectionExtensions.cs   # DI registration
└── WebApplicationExtensions.cs      # Web application extensions
```

---

## 🚀 Services Example
**AuthService** - Example microservice implementation using BuildingBlocks

### 📁 Structure
```
Services/
└── AuthService/
    ├── API/                    # API layer
    │   ├── Endpoints/          # API endpoints
    │   ├── Program.cs          # Application entry point
    │   └── appsettings.json    # Configuration
    ├── Application/            # Application layer
    │   ├── Commands/           # Command handlers
    │   ├── Queries/            # Query handlers
    │   ├── Events/             # Event handlers
    │   ├── Validation/         # Input validation
    │   ├── Behaviors/          # Custom behaviors
    │   ├── Services/           # Application services
    │   └── Caching/            # Cache keys
    ├── Domain/                 # Domain layer
    │   ├── Entities/           # Domain entities
    │   ├── ValueObjects/       # Value objects
    │   ├── StronglyTypedIds/   # Typed identifiers
    │   ├── DomainEvents/       # Domain events
    │   ├── Events/             # Integration events
    │   ├── BusinessRules/      # Business rules
    │   ├── Specifications/     # Query specifications
    │   ├── Repositories/       # Repository interfaces
    │   ├── Services/           # Domain service interfaces
    │   └── Exceptions/         # Domain exceptions
    └── Infrastructure/         # Infrastructure layer
        ├── Data/               # Data access
        ├── Repositories/       # Repository implementations
        ├── Services/           # Domain service implementations
        ├── Authentication/     # Auth implementations
        ├── Health/             # Health checks
        ├── Inbox/              # Inbox implementation
        └── Outbox/             # Outbox implementation
```

### 🎯 Key Features
- **Clean Architecture**: Clear separation of concerns
- **CQRS Pattern**: Command/Query separation
- **Domain Events**: Business state change notifications
- **Strongly Typed IDs**: Type-safe identifiers
- **Repository Pattern**: Data access abstraction
- **Specifications**: Encapsulated query logic
- **Inbox/Outbox**: Reliable messaging patterns

---

## 📚 Documentation
**Comprehensive documentation and guides**

### 📖 Main Documentation
```
├── README.md                           # Main project overview
├── BuildingBlocksStructure.md         # This file - architecture overview
├── BuildingBlocks-Usage-Guide.md      # Usage guide and examples
└── README-CentralizedPackageManagement.md # Package management guide
```

### 📋 Specialized Guides
```
├── README-MinimalAPIUsage.md          # Minimal API usage patterns
├── README-ProblemDetailsMiddleware.md # Error handling guide
├── README-ServiceStructure.md         # Service layer structure
└── README-StronglyTypedIdJsonConverters.md # JSON converter guide
```

### 📊 Analysis & Planning
```
└── Domain-Library-Gap-Analysis.md     # Gap analysis documentation
```

### 🏗️ Generation Prompts
```
GenerationPrompts/
└── BuildingBlocks/
    ├── 01-Domain-Library-Prompt.md      # Domain layer generation
    ├── 02-Application-Library-Prompt.md # Application layer generation
    ├── 03-Infrastructure-Library-Prompt.md # Infrastructure layer generation
    └── 04-API-Library-Prompt.md         # API layer generation
```

---

## 🎯 Key Architectural Features

### ✨ Build System
- **Centralized Package Management** - All packages managed through build targets
- **Feature Flags** - Enable/disable functionality per project
- **Automatic Project Detection** - Smart package inclusion based on project type
- **Modern .NET Configuration** - Latest SDK features and optimizations
- **Clean Project Files** - No package references in .csproj files

### 🏛️ Domain Layer
- **Strongly-typed IDs** using readonly structs with JSON converters
- **Value Objects** with validation and immutability
- **Domain Events** for business state changes
- **Business Rules** encapsulation with validation
- **Rich domain models** following DDD principles
- **Comprehensive exception handling**

### ⚙️ Application Layer
- **Custom Mediator** implementation (not MediatR dependency)
- **CQRS** with separate commands and queries
- **Pipeline Behaviors** for cross-cutting concerns
- **Inbox/Outbox** patterns for reliable messaging
- **Comprehensive validation** framework
- **Service context** for request handling

### 🔧 Infrastructure Layer
- **Multiple storage** implementations with abstractions
- **Message bus** abstractions with in-memory implementation
- **Caching** strategies (Memory, Distributed, Redis)
- **Authentication/Authorization** infrastructure
- **Monitoring and logging** integration
- **Entity Framework** with interceptors and configurations

### 🌐 API Layer
- **Minimal APIs** and endpoint patterns
- **Standardized responses** with ApiResponse<T>
- **Comprehensive middleware** pipeline
- **Global exception handling** with Problem Details
- **Request/response logging** with correlation IDs
- **Modular extension** system

### 📦 Package Management Benefits
1. **Consistency** - All projects use the same package versions
2. **Maintainability** - Update packages in one place
3. **Flexibility** - Enable/disable features per project
4. **Performance** - Only include packages you need
5. **Clean Code** - No package clutter in project files
6. **Smart Defaults** - Sensible package selection per project type

---

This architecture provides a solid, modern foundation for building scalable, maintainable microservices following current .NET best practices with intelligent package management and comprehensive abstractions for maximum flexibility and control. 