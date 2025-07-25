# Using BuildingBlocks in Cursor for Other Projects

## 1. Generate & Package Building Blocks

### Generate Libraries Using Prompts
1. **Domain Library**: Use `01-Domain-Library-Prompt.md` 
2. **Application Library**: Use `02-Application-Library-Prompt.md`
3. **Infrastructure Library**: Use `03-Infrastructure-Library-Prompt.md` 
4. **API Library**: Use `04-API-Library-Prompt.md`

### Package as NuGet Packages
```xml
<!-- Directory.Build.props for all BuildingBlocks -->
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <PackageVersion>1.0.0</PackageVersion>
    <Authors>Your Team</Authors>
    <Company>Your Company</Company>
    <PackageProjectUrl>https://github.com/yourorg/buildingblocks</PackageProjectUrl>
  </PropertyGroup>
</Project>
```

## 2. Project Creation Workflows in Cursor

### **A. New Microservice Template**

#### Create Project Structure
```
NewMicroservice/
├── src/
│   ├── NewMicroservice.API/
│   ├── NewMicroservice.Application/
│   ├── NewMicroservice.Domain/
│   └── NewMicroservice.Infrastructure/
├── tests/
└── docker/
```

#### Reference BuildingBlocks
```xml
<!-- In each project file -->
<ItemGroup>
  <PackageReference Include="BuildingBlocks.Domain" Version="1.0.0" />
  <PackageReference Include="BuildingBlocks.Application" Version="1.0.0" />
  <PackageReference Include="BuildingBlocks.Infrastructure" Version="1.0.0" />
  <PackageReference Include="BuildingBlocks.API" Version="1.0.0" />
</ItemGroup>
```

### **B. Cursor AI Prompts for New Projects**

#### Domain Layer Generation
```prompt
Using BuildingBlocks.Domain as the foundation, create a [YourDomain] microservice domain layer with:

1. Entities: [Entity1], [Entity2], [Entity3]
2. Value Objects: [ValueObject1], [ValueObject2]
3. Domain Events: [Event1], [Event2]
4. Business Rules: [Rule1], [Rule2]
5. Repositories: I[Entity]Repository interfaces

Follow the patterns from BuildingBlocks.Domain:
- Use strongly-typed IDs
- Inherit from Entity<TId, TIdValue>
- Create domain events for significant business operations
- Implement business rules as separate classes
- Use value objects for complex properties

Generate the complete domain model with proper validation and business logic.
```

#### Application Layer Generation
```prompt
Using BuildingBlocks.Application patterns, create the application layer for [YourDomain] with:

1. Commands: Create[Entity], Update[Entity], Delete[Entity]
2. Queries: Get[Entity], List[Entity]s, Search[Entity]s
3. DTOs: [Entity]Dto, Create[Entity]Dto, Update[Entity]Dto
4. Validators: FluentValidation validators for all commands
5. Handlers: CQRS handlers using custom mediator

Follow BuildingBlocks.Application patterns:
- Inherit from CommandBase/QueryBase
- Use PagedQuery for list operations
- Implement proper mapping between domain and DTOs
- Add comprehensive validation
- Use ICommandHandler<T>, IQueryHandler<T,R> interfaces
- Implement HandleAsync() methods
- Include pipeline behaviors (logging, validation, performance)

Generate complete CQRS implementation with proper error handling.
```

#### Infrastructure Layer Generation
```prompt
Using BuildingBlocks.Infrastructure, create the infrastructure layer for [YourDomain] with:

1. DbContext: [YourDomain]DbContext with proper entity configurations
2. Repositories: Concrete implementations of domain repositories
3. Entity Configurations: EF Core entity type configurations
4. Migrations: Initial database migration
5. Services: Infrastructure services (email, file storage, etc.)

Follow BuildingBlocks.Infrastructure patterns:
- Inherit from DbContextBase
- Use Repository<TEntity, TId> base class
- Implement UnitOfWork pattern
- Add proper audit and soft delete interceptors
- Configure strongly-typed IDs properly

Generate complete data access layer with EF Core.
```

#### API Layer Generation
```prompt
Using BuildingBlocks.API, create the API layer for [YourDomain] with:

1. Controllers: [Entity]Controller with full CRUD operations
2. Endpoints: Minimal API endpoints as alternative
3. DTOs: API-specific request/response models
4. Validation: Request validation attributes
5. Documentation: OpenAPI/Swagger documentation

Follow BuildingBlocks.API patterns:
- Inherit from EndpointBase
- Use ApiResponse<T> for all responses
- Implement proper HTTP status codes
- Add comprehensive validation
- Include rate limiting and caching where appropriate

Generate complete API with proper error handling and documentation.
```

## 3. Cursor Code Generation Workflows

### **A. Entity Creation Workflow**
When creating a new entity, use this Cursor prompt:

```prompt
Create a new entity [EntityName] for the [YourDomain] microservice using BuildingBlocks patterns:

1. **Domain Entity**: 
   - Strongly-typed ID: [EntityName]Id
   - Value objects for complex properties
   - Domain events for state changes
   - Business rules validation

2. **Application Layer**:
   - Commands: Create, Update, Delete
   - Queries: GetById, GetPaged, Search
   - DTOs with proper mapping
   - Validators using FluentValidation

3. **Infrastructure**:
   - Repository interface and implementation
   - EF Core entity configuration
   - Database migration

4. **API Layer**:
   - Controller with full CRUD
   - Minimal API endpoints
   - Proper response models
   - OpenAPI documentation

Generate all layers following the established patterns and include comprehensive error handling.
```

### **B. Feature Development Workflow**
For adding new features:

```prompt
Add [FeatureName] feature to [YourDomain] microservice using BuildingBlocks:

Business Requirements:
- [Requirement 1]
- [Requirement 2]
- [Requirement 3]

Generate:
1. Domain changes (entities, value objects, events, rules)
2. Application commands/queries with handlers
3. Infrastructure updates (repositories, configurations)
4. API endpoints with proper documentation
5. Integration tests

Follow all BuildingBlocks patterns and ensure proper error handling, validation, and logging.
```

## 4. Cursor Workspace Configuration

### **A. Create .cursor/ Configuration**
```json
// .cursor/settings.json
{
  "buildingBlocks.templatePath": "./templates",
  "buildingBlocks.snippetsPath": "./snippets",
  "codeGeneration.patterns": [
    "Domain-First",
    "CQRS",
    "Clean Architecture"
  ]
}
```

### **B. Code Snippets for Common Patterns**
Create snippets for BuildingBlocks patterns:

```json
// .cursor/snippets/domain.json
{
  "Strongly Typed ID": {
    "prefix": "stid",
    "body": [
      "public readonly struct ${1:EntityName}Id : IStronglyTypedId<Guid>",
      "{",
      "    public Guid Value { get; }",
      "    ",
      "    public ${1:EntityName}Id(Guid value) => Value = value;",
      "    ",
      "    public static ${1:EntityName}Id New() => new(Guid.NewGuid());",
      "    public static ${1:EntityName}Id Empty => new(Guid.Empty);",
      "    ",
      "    public static implicit operator Guid(${1:EntityName}Id id) => id.Value;",
      "    public static explicit operator ${1:EntityName}Id(Guid value) => new(value);",
      "}"
    ]
  }
}
```

### **C. Project Templates**
Create project templates for common scenarios:

```
Templates/
├── Microservice/
│   ├── template.json
│   └── content/
├── Feature/
│   ├── template.json
│   └── content/
└── Entity/
    ├── template.json
    └── content/
```

## 5. Development Workflow Examples

### **A. Creating a New Microservice**
1. **Use Cursor AI**: "Create a new microservice for [Domain] using BuildingBlocks"
2. **Generate Structure**: Domain → Application → Infrastructure → API
3. **Configure Dependencies**: Add BuildingBlocks NuGet references
4. **Implement Features**: Use feature-specific prompts
5. **Add Tests**: Generate tests using BuildingBlocks test patterns

### **B. Adding New Features**
1. **Domain First**: Start with domain modeling using Cursor AI
2. **Application Layer**: Generate CQRS handlers and DTOs
3. **Infrastructure**: Update repositories and configurations
4. **API Layer**: Add endpoints with proper validation
5. **Testing**: Generate integration and unit tests

### **C. Code Review with AI**
Use Cursor AI to review code against BuildingBlocks patterns:

```prompt
Review this code for compliance with BuildingBlocks patterns:
- Domain: Proper entity design, value objects, domain events
- Application: CQRS implementation, validation, error handling
- Infrastructure: Repository pattern, EF configuration
- API: Response formats, error handling, documentation

Suggest improvements and identify any pattern violations.
```

## 6. Advanced Cursor Integration

### **A. Custom Code Actions**
Create custom code actions for common BuildingBlocks operations:
- Generate entity with all layers
- Add CQRS command/query
- Create repository implementation
- Generate API endpoint

### **B. Automated Refactoring**
Use Cursor AI for:
- Converting primitive properties to value objects
- Adding domain events to entities
- Implementing missing validation
- Updating API responses to use ApiResponse<T>

### **C. Documentation Generation**
Automatically generate:
- API documentation from endpoints
- Domain model documentation
- Architecture decision records
- Migration guides

## 7. Best Practices for Cursor Usage

### **A. Prompt Engineering**
- Always reference BuildingBlocks patterns
- Provide specific requirements and constraints
- Ask for complete implementations with tests
- Request adherence to coding standards

### **B. Code Organization**
- Keep BuildingBlocks as separate solution
- Use consistent naming conventions
- Maintain clear separation of concerns
- Document deviations from patterns

### **C. Continuous Integration**
- Automate BuildingBlocks updates
- Validate pattern compliance in CI/CD
- Generate and publish NuGet packages
- Maintain backward compatibility

This approach transforms your BuildingBlocks into a powerful development accelerator within Cursor, enabling rapid creation of consistent, well-architected microservices. 