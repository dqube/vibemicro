# BuildingBlocks Architecture Structure

## Overview
This document outlines the complete architecture and file structure of the BuildingBlocks library - a comprehensive foundation for .NET microservices following Domain-Driven Design (DDD), Clean Architecture, and CQRS patterns.

## 📋 Table of Contents
- [🏗️ Domain Layer](#-domain-layer)
- [⚙️ Application Layer](#-application-layer)
- [🔧 Infrastructure Layer](#-infrastructure-layer)
- [🌐 API Layer](#-api-layer)

---

## 🏗️ Domain Layer
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
└── InvalidOperationDomainException.cs   # Invalid operations
```

### 📏 Business Rules
*Domain validation and business logic*
```
├── IBusinessRule.cs            # Rule interface
├── BusinessRuleBase.cs         # Base implementation
└── CompositeBusinessRule.cs    # Rule composition
```

### 🔧 Common
*Shared value objects and utilities*
```
├── Money.cs                    # Money with currency
├── DateRange.cs               # Date range value object
├── Address.cs                 # Address value object
├── Email.cs                   # Email value object
└── PhoneNumber.cs             # Phone value object
```

### 🛠️ Extensions
*Domain utility methods*
```
└── DomainExtensions.cs         # Domain object extensions
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
├── OutboxBackgroundService.cs # Outbox pattern background service
└── InboxBackgroundService.cs  # Inbox pattern background service
```

### ✅ Validation
*Input validation framework*
```
├── IValidator.cs              # Validator interface
├── IValidationRule.cs         # Validation rule interface
├── ValidationResult.cs        # Validation result
├── ValidationError.cs         # Validation error details
├── CompositeValidator.cs      # Composite validation
└── ValidatorBase.cs           # Base validator implementation
```

### 💾 Caching
*Application-level caching abstractions*
```
├── ICacheService.cs           # Cache service interface
├── ICacheKey.cs               # Cache key interface
├── CacheKey.cs                # Cache key implementation
├── CacheSettings.cs           # Cache configuration
└── CachePolicy.cs             # Cache policies
```

### 📨 Messaging
*Message bus and event bus abstractions*
```
├── IMessageBus.cs             # Message bus interface
├── IEventBus.cs               # Event bus interface
├── IMessageHandler.cs         # Message handler interface
├── IMessagePublisher.cs       # Message publisher interface
└── MessageMetadata.cs         # Message metadata
```

### 📊 DTOs
*Data Transfer Objects*
```
├── BaseDto.cs                 # Base DTO
├── AuditableDto.cs           # Auditable DTO
└── PagedDto.cs               # Paged DTO
```

### 🗺️ Mapping
*Object mapping abstractions*
```
├── IMapper.cs                 # Mapper interface
├── IMappingProfile.cs         # Mapping profile interface
└── MapperBase.cs              # Base mapper implementation
```

### 🔐 Security
*Security context and user information*
```
├── ICurrentUserService.cs     # Current user service
├── IPermissionService.cs      # Permission service
├── UserContext.cs             # User context
└── SecurityContext.cs         # Security context
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
└── IdempotencyOptions.cs      # Idempotency configuration
```

### 🛠️ Extensions
*Application layer extensions*
```
├── ServiceCollectionExtensions.cs # DI registration
├── ApplicationExtensions.cs        # Application extensions
└── MediatorExtensions.cs           # Mediator extensions
```

---

## 🔧 Infrastructure Layer
**BuildingBlocks.Infrastructure** - External concerns and infrastructure implementations.

### 🗄️ Data
*Data access and persistence*

#### Repositories
```
├── Repository.cs              # Generic repository implementation
├── ReadOnlyRepository.cs      # Read-only repository implementation
└── RepositoryBase.cs          # Base repository functionality
```

#### Unit of Work
```
├── UnitOfWork.cs              # Unit of work implementation
└── IDbTransaction.cs          # Database transaction interface
```

#### Context
```
├── IDbContext.cs              # Database context interface
├── ApplicationDbContext.cs    # Application database context
├── DbContextBase.cs           # Base database context
└── IDbContextFactory.cs       # Database context factory
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
├── ICacheService.cs           # Cache service interface
├── MemoryCacheService.cs      # In-memory cache implementation
├── DistributedCacheService.cs # Distributed cache implementation
├── RedisCacheService.cs       # Redis cache implementation
├── CacheKeyGenerator.cs       # Cache key generation
└── CacheConfiguration.cs      # Cache configuration
```

### 📨 Messaging
*Message bus and event bus implementations*

#### Message Bus
```
├── IMessageBus.cs             # Message bus interface
├── InMemoryMessageBus.cs      # In-memory message bus
├── ServiceBusMessageBus.cs    # Azure Service Bus implementation
└── RabbitMQMessageBus.cs      # RabbitMQ implementation
```

#### Event Bus
```
├── IEventBus.cs               # Event bus interface
├── InMemoryEventBus.cs        # In-memory event bus
├── ServiceBusEventBus.cs      # Azure Service Bus event bus
└── RabbitMQEventBus.cs        # RabbitMQ event bus
```

#### Publishers
```
├── IMessagePublisher.cs       # Message publisher interface
├── MessagePublisherBase.cs    # Base message publisher
├── ServiceBusPublisher.cs     # Azure Service Bus publisher
└── RabbitMQPublisher.cs       # RabbitMQ publisher
```

#### Subscribers
```
├── IMessageSubscriber.cs      # Message subscriber interface
├── MessageSubscriberBase.cs   # Base message subscriber
├── ServiceBusSubscriber.cs    # Azure Service Bus subscriber
└── RabbitMQSubscriber.cs      # RabbitMQ subscriber
```

#### Serialization
```
├── IMessageSerializer.cs      # Message serializer interface
├── JsonMessageSerializer.cs   # JSON message serializer
└── BinaryMessageSerializer.cs # Binary message serializer
```

#### Configuration
```
├── MessageBusConfiguration.cs # Message bus configuration
├── ServiceBusConfiguration.cs # Azure Service Bus configuration
└── RabbitMQConfiguration.cs   # RabbitMQ configuration
```

### 📝 Logging
*Logging and observability*
```
├── ILoggerService.cs          # Logger service interface
├── LoggerService.cs           # Logger service implementation
├── OpenTelemetry/             # OpenTelemetry integration
├── Structured/                # Structured logging
└── [Additional logging components]
```

### 🔐 Authentication
*Authentication providers*
```
├── JWT/                       # JWT authentication
├── OAuth/                     # OAuth authentication
├── ApiKey/                    # API key authentication
└── Identity/                  # Identity management
```

### 🛡️ Authorization
*Authorization services*
```
├── IAuthorizationService.cs   # Authorization service
├── AuthorizationService.cs    # Authorization implementation
├── Policies/                  # Authorization policies
├── Handlers/                  # Authorization handlers
└── Requirements/              # Authorization requirements
```

### 💾 Storage
*File and blob storage*
```
├── Files/                     # File storage services
├── Blobs/                     # Blob storage services
└── Documents/                 # Document storage services
```

### 📞 Communication
*Communication services*
```
├── Email/                     # Email services
├── SMS/                       # SMS services
├── Push/                      # Push notification services
└── Notifications/             # General notification services
```

### 📊 Monitoring
*Health checks and monitoring*
```
├── Health/                    # Health check services
├── Metrics/                   # Metrics collection
├── Tracing/                   # Distributed tracing
└── Performance/               # Performance monitoring
```

### ⚙️ Background Services
*Background task processing*
```
├── IBackgroundTaskService.cs  # Background task service
├── BackgroundTaskService.cs   # Background task implementation
├── Queues/                    # Background queues
├── Jobs/                      # Job scheduling
└── Workers/                   # Worker services
```

### 🌐 External
*External service integrations*
```
├── HttpClients/               # HTTP client services
├── APIs/                      # External API integrations
└── ThirdParty/                # Third-party integrations
```

### 🔒 Security
*Security implementations*
```
├── Encryption/                # Encryption services
├── Hashing/                   # Hashing services
├── KeyManagement/             # Key management
└── Secrets/                   # Secrets management
```

### 🗺️ Mapping
*Object mapping implementations*
```
├── AutoMapper/                # AutoMapper implementation
├── Mapster/                   # Mapster implementation
└── Manual/                    # Manual mapping
```

### ✅ Validation
*Validation implementations*
```
├── FluentValidation/          # FluentValidation implementation
├── DataAnnotations/           # Data annotations validation
└── Custom/                    # Custom validation
```

### 📄 Serialization
*Serialization services*
```
├── Json/                      # JSON serialization
├── Xml/                       # XML serialization
├── Binary/                    # Binary serialization
└── Csv/                       # CSV serialization
```

### 🔄 Idempotency
*Idempotency implementation*
```
├── IdempotencyEntity.cs       # Idempotency entity
├── IIdempotencyRepository.cs  # Idempotency repository
├── IdempotencyProcessor.cs    # Idempotency processor
├── IdempotencyMiddleware.cs   # Idempotency middleware
└── IdempotencyConfiguration.cs # Idempotency configuration
```

### ⚙️ Configuration
*Configuration management*
```
├── IConfigurationService.cs   # Configuration service
├── ConfigurationService.cs    # Configuration implementation
├── Settings/                  # Application settings
├── Providers/                 # Configuration providers
└── Validation/                # Configuration validation
```

### 🛠️ Extensions
*Infrastructure extensions*
```
├── ServiceCollectionExtensions.cs    # DI registration
├── ApplicationBuilderExtensions.cs   # Application builder extensions
├── HostBuilderExtensions.cs          # Host builder extensions
├── [Additional extensions...]
└── InfrastructureExtensions.cs       # General infrastructure extensions
```

---

## 🌐 API Layer
**BuildingBlocks.API** - Web API infrastructure, endpoints, and HTTP concerns.

### 🎯 Endpoints
*API endpoint definitions*

#### Base
```
├── EndpointBase.cs            # Base endpoint class
├── CrudEndpoints.cs           # CRUD endpoint patterns
└── QueryEndpoints.cs          # Query endpoint patterns
```

#### Extensions
```
├── EndpointRouteBuilderExtensions.cs # Route builder extensions
└── MinimalApiExtensions.cs           # Minimal API extensions
```

#### Conventions
```
├── ApiEndpointConvention.cs          # API endpoint conventions
└── VersioningEndpointConvention.cs   # Versioning conventions
```

### 🛡️ Middleware
*HTTP request processing pipeline*

#### Error Handling
```
├── GlobalExceptionMiddleware.cs # Global exception handling
├── ErrorResponse.cs            # Error response models
└── ProblemDetailsFactory.cs    # RFC 7807 problem details
```

#### Logging
```
├── RequestLoggingMiddleware.cs # Request/response logging
└── CorrelationIdMiddleware.cs  # Correlation ID handling
```

#### Security
```
├── SecurityHeadersMiddleware.cs # Security headers
└── RateLimitingMiddleware.cs    # Rate limiting
```

### 📊 Responses
*API response models*

#### Base
```
├── ApiResponse.cs             # Standard API response
├── PagedResponse.cs           # Paged response wrapper
└── ErrorResponse.cs           # Error response model
```

#### Builders
```
├── ApiResponseBuilder.cs      # Response builder
└── ErrorResponseBuilder.cs    # Error response builder
```

### 🔐 Authentication
*API authentication*
```
├── JWT/                       # JWT authentication for APIs
└── ApiKey/                    # API key authentication
```

### ✅ Validation
*Request validation*
```
├── Validators/                # Request validators
├── Extensions/                # Validation extensions
└── Results/                   # Validation results
```

### 📚 OpenAPI
*API documentation*
```
├── Configuration/             # OpenAPI configuration
├── Filters/                   # Swagger filters
└── Extensions/                # OpenAPI extensions
```

### 🔢 Versioning
*API versioning*
```
├── Extensions/                # Versioning extensions
└── Conventions/               # Versioning conventions
```

### 🏥 Health
*Health check endpoints*
```
├── Extensions/                # Health check extensions
└── Reporters/                 # Health reporters
```

### ⚙️ Configuration
*API configuration*
```
├── Options/                   # Configuration options
└── Extensions/                # Configuration extensions
```

### 🛠️ Extensions
*API layer extensions*
```
├── ApiExtensions.cs           # General API extensions
├── AuthenticationExtensions.cs # Authentication extensions
├── CorsExtensions.cs          # CORS extensions
├── [Additional extensions...]
└── ResponseExtensions.cs      # Response extensions
```

### 🔧 Utilities
*API utilities and helpers*
```
├── Helpers/                   # Helper classes
├── Constants/                 # API constants
└── Factories/                 # Factory classes
```

---

## 🎯 Key Features

### ✨ Domain Layer
- **Strongly-typed IDs** using readonly structs
- **Value Objects** with validation
- **Domain Events** for business state changes
- **Business Rules** encapsulation
- **Rich domain models** following DDD

### ⚙️ Application Layer
- **Custom Mediator** implementation (not MediatR)
- **CQRS** with separate commands and queries
- **Pipeline Behaviors** for cross-cutting concerns
- **Inbox/Outbox** patterns
- **Comprehensive validation**

### 🔧 Infrastructure Layer
- **Multiple storage** implementations
- **Message bus** abstractions
- **Caching** strategies
- **Authentication/Authorization** providers
- **Monitoring and logging** integration

### 🌐 API Layer
- **Minimal APIs** and Controllers
- **Standardized responses** with ApiResponse<T>
- **Comprehensive middleware** pipeline
- **OpenAPI documentation**
- **Versioning and health checks**

---

*This architecture provides a solid foundation for building scalable, maintainable microservices following modern .NET best practices.* 