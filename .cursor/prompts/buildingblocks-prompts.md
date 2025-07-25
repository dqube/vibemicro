# BuildingBlocks Cursor AI Prompts

## 🏗️ Project Creation Prompts

### New Microservice
```prompt
Create a new microservice called [SERVICE_NAME] using BuildingBlocks architecture:

Structure:
- [SERVICE_NAME].Domain (entities, value objects, domain events, business rules)
- [SERVICE_NAME].Application (CQRS commands/queries, DTOs, handlers, validators)
- [SERVICE_NAME].Infrastructure (repositories, EF Core, configurations)
- [SERVICE_NAME].API (controllers/minimal APIs, middleware, responses)

Domain Requirements:
- Main entity: [ENTITY_NAME] with properties [PROPERTY_LIST]
- Value objects: [VALUE_OBJECT_LIST]
- Business rules: [BUSINESS_RULE_LIST]

Follow all BuildingBlocks patterns:
✅ Strongly-typed IDs using readonly structs
✅ Domain events for state changes
✅ CQRS with custom mediator
✅ Repository pattern with UnitOfWork
✅ Clean API responses with ApiResponse<T>
✅ Comprehensive validation and error handling

Generate complete solution with project files, dependencies, and basic CRUD operations.
```

### Add New Entity
```prompt
Add a new entity [ENTITY_NAME] to the [SERVICE_NAME] microservice using BuildingBlocks patterns:

Requirements:
- Properties: [PROPERTY_LIST]
- Related entities: [RELATED_ENTITIES]
- Business rules: [BUSINESS_RULES]
- Domain events: [DOMAIN_EVENTS]

Generate:
1. Domain layer:
   - [ENTITY_NAME] entity with strongly-typed ID
   - [ENTITY_NAME]Id strongly-typed identifier
   - Value objects for complex properties
   - Domain events for entity lifecycle
   - Business rules for validation
   - I[ENTITY_NAME]Repository interface

2. Application layer:
   - Commands: Create, Update, Delete[ENTITY_NAME]
   - Queries: Get[ENTITY_NAME], Get[ENTITY_NAME]s
   - DTOs: [ENTITY_NAME]Dto, Create[ENTITY_NAME]Dto, Update[ENTITY_NAME]Dto
   - Command/Query handlers with proper error handling
   - FluentValidation validators

3. Infrastructure layer:
   - [ENTITY_NAME]Repository implementation
   - EF Core entity configuration
   - Database migration

4. API layer:
   - [ENTITY_NAME]Controller with full CRUD
   - Alternative minimal API endpoints
   - Proper HTTP status codes and responses
   - OpenAPI documentation

Follow BuildingBlocks patterns and ensure proper separation of concerns.
```

## 🔧 Feature Development Prompts

### Add Authentication Feature
```prompt
Add JWT authentication feature to [SERVICE_NAME] microservice using BuildingBlocks:

Requirements:
- JWT token-based authentication
- Role-based authorization
- User registration and login
- Password hashing and validation
- Token refresh functionality

Generate:
1. Domain updates:
   - User entity with authentication properties
   - Authentication-related value objects (Email, PasswordHash)
   - Authentication domain events
   - User business rules

2. Application layer:
   - Authentication commands (Register, Login, RefreshToken)
   - Authentication queries (GetCurrentUser)
   - JWT token service interface
   - Authentication handlers with validation

3. Infrastructure:
   - JWT token service implementation
   - User repository with authentication methods
   - Password hashing service

4. API layer:
   - Authentication controller/endpoints
   - JWT middleware configuration
   - Authorization policies
   - Security configuration

Use BuildingBlocks.API authentication patterns and ensure proper security practices.
```

### Add Caching Feature
```prompt
Add comprehensive caching to [SERVICE_NAME] microservice using BuildingBlocks:

Requirements:
- Redis distributed caching
- Response caching for read operations
- Cache invalidation on data changes
- Configurable cache policies

Implement:
1. Application layer:
   - Caching behavior for custom mediator pipeline
   - Cache key generation strategies
   - Cache invalidation commands

2. Infrastructure:
   - Redis cache service implementation
   - Cache configuration options
   - Distributed cache setup

3. API layer:
   - Response caching middleware
   - Cache headers configuration
   - Cache control attributes

Follow BuildingBlocks caching patterns and ensure optimal performance.
```

## 🧪 Testing Prompts

### Generate Tests
```prompt
Generate comprehensive tests for [ENTITY_NAME] in [SERVICE_NAME] microservice using BuildingBlocks:

Generate:
1. Unit Tests:
   - Domain entity tests (business logic, validation)
   - Value object tests (validation, equality)
   - Business rule tests
   - Command/Query handler tests with mocks

2. Integration Tests:
   - Repository tests with in-memory database
   - API endpoint tests with TestClient
   - End-to-end workflow tests

3. Test Infrastructure:
   - Test builders for entities and DTOs
   - Mock setups for dependencies
   - Test data factories
   - Custom assertions

Use xUnit, FluentAssertions, Moq, and follow BuildingBlocks testing patterns.
Include both positive and negative test scenarios.
```

## 🔄 Refactoring Prompts

### Convert to Value Objects
```prompt
Refactor [ENTITY_NAME] in [SERVICE_NAME] to use value objects for primitive properties:

Current primitive properties to convert:
- [PROPERTY_LIST]

For each property:
1. Create value object using SingleValueObject<T> pattern
2. Add validation in ValidateValue method
3. Update entity to use value object
4. Update DTOs and mappings
5. Update database configuration
6. Create migration for changes

Ensure all BuildingBlocks patterns are followed and existing functionality is preserved.
```

### Add Domain Events
```prompt
Add domain events to [ENTITY_NAME] in [SERVICE_NAME] microservice:

Events to implement:
- [ENTITY_NAME]Created
- [ENTITY_NAME]Updated
- [ENTITY_NAME]Deleted
- [CUSTOM_EVENTS]

For each event:
1. Create domain event record inheriting from DomainEventBase
2. Update entity to raise events on state changes
3. Create event handlers in application layer
4. Add integration event publishing if needed
5. Update tests to verify event raising

Follow BuildingBlocks domain event patterns and ensure proper event handling.
```

## 📊 Performance Optimization Prompts

### Optimize Queries
```prompt
Optimize queries in [SERVICE_NAME] microservice using BuildingBlocks patterns:

Current performance issues:
- [ISSUE_LIST]

Optimize:
1. Repository queries:
   - Add proper includes for related data
   - Implement projection queries
   - Add pagination where missing
   - Optimize database indexes

2. Application layer:
   - Add query-specific DTOs for projections
   - Implement caching for frequent queries
   - Add query result caching

3. API layer:
   - Add response compression
   - Implement conditional requests (ETags)
   - Add rate limiting for expensive operations

Follow BuildingBlocks performance patterns and maintain clean architecture.
```

## 📝 Documentation Prompts

### Generate Documentation
```prompt
Generate comprehensive documentation for [SERVICE_NAME] microservice:

Generate:
1. API Documentation:
   - OpenAPI/Swagger documentation
   - Endpoint descriptions and examples
   - Request/Response schemas
   - Error response documentation

2. Architecture Documentation:
   - Domain model documentation
   - CQRS flow diagrams
   - Database schema documentation
   - Dependency diagrams

3. Developer Documentation:
   - Getting started guide
   - Development setup instructions
   - Testing guidelines
   - Deployment instructions

4. Code Documentation:
   - XML documentation for all public APIs
   - Inline comments for complex business logic
   - README files for each project

Ensure documentation follows BuildingBlocks conventions and is developer-friendly.
```

## 🔍 Code Review Prompts

### Review Code Quality
```prompt
Review [FILE_NAME] for BuildingBlocks pattern compliance:

Check for:
✅ Proper domain modeling (entities, value objects, domain events)
✅ CQRS implementation (commands, queries, handlers)
✅ Repository pattern usage
✅ Error handling and validation
✅ Separation of concerns
✅ Naming conventions
✅ XML documentation
✅ Test coverage

Identify:
❌ Pattern violations
❌ Code smells
❌ Performance issues
❌ Security concerns
❌ Missing validation
❌ Inadequate error handling

Provide specific recommendations for improvements following BuildingBlocks best practices.
```

## 🚀 Deployment Prompts

### Generate Deployment Configuration
```prompt
Generate deployment configuration for [SERVICE_NAME] microservice:

Generate:
1. Docker configuration:
   - Multi-stage Dockerfile
   - Docker Compose for local development
   - Health check configuration

2. Kubernetes manifests:
   - Deployment configuration
   - Service and Ingress
   - ConfigMaps and Secrets
   - Health check probes

3. CI/CD pipeline:
   - Build and test stages
   - Security scanning
   - Deployment to environments
   - Rollback procedures

4. Configuration:
   - Environment-specific appsettings
   - Connection strings management
   - Logging configuration
   - Monitoring setup

Follow cloud-native best practices and BuildingBlocks configuration patterns.
```

## 💡 Usage Tips

1. **Replace placeholders** like [SERVICE_NAME], [ENTITY_NAME] with actual values
2. **Customize requirements** based on your specific business needs
3. **Run prompts incrementally** - start with domain, then application, etc.
4. **Review generated code** and adjust to match your coding standards
5. **Test thoroughly** after each generation phase
6. **Document deviations** from standard BuildingBlocks patterns 