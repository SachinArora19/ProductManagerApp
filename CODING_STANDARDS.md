# Product Management Application - Coding Standards

## Project Overview
This is a full-stack Product Management application demonstrating modern .NET development practices with:
- **Blazor Server** for the frontend UI
- **FastEndpoints** for clean, minimal APIs
- **PostgreSQL** database with **Dapper** ORM
- **.NET Aspire** for orchestration and service discovery
- **SQLite testing** for fast, isolated integration tests (replaces TestContainers for better Windows compatibility)

## Code Style and Patterns

### General Guidelines
- Use **minimal APIs** pattern with FastEndpoints
- Follow **clean architecture** principles with separated concerns
- Implement **async/await** patterns consistently
- Use **dependency injection** for all services
- Apply **SOLID principles** throughout the codebase

### Database Access
- Use **Dapper** for data access, not Entity Framework
- Write **raw SQL queries** for database operations
- Implement **repository pattern** for data access abstraction
- Use **PostgreSQL-specific** features when beneficial
- Handle **database initialization** and **migrations** explicitly

### API Development
- Use **FastEndpoints** instead of traditional controllers
- Implement **proper HTTP status codes** (200, 201, 404, 400, etc.)
- Apply **data validation** using attributes and FastEndpoints validation
- Use **DTOs** for request/response models separate from domain models
- Implement **CORS** policies for Blazor integration

### Frontend Development
- Build **Blazor Server** components with **@rendermode InteractiveServer**
- Use **Bootstrap 5** for responsive design and styling
- Implement **real-time updates** using Blazor's built-in capabilities
- Handle **form validation** with DataAnnotations and Blazor forms
- Use **modal dialogs** for create/edit operations
- Implement **proper error handling** with user-friendly messages

### Testing
- Write **integration tests** using SQLite for fast, isolated testing
- Test **complete CRUD workflows** end-to-end
- Use **WebApplicationFactory** for API testing
- Implement **proper test isolation** with separate database instances per test
- Test **error scenarios** and **edge cases**

### Aspire Configuration
- Use **.NET Aspire** for service orchestration
- Implement **service discovery** between projects
- Configure **PostgreSQL** as a hosted service in Aspire
- Use **connection string** injection from Aspire configuration
- Leverage **health checks** and **monitoring** capabilities

## Project Structure Conventions

```
ProductManagement.Models/     # Shared DTOs and domain models
ProductManagement.Data/       # Repository interfaces and Dapper implementations  
ProductManagement.ApiService/ # FastEndpoints API with Program.cs configuration
ProductManagement.Web/       # Blazor Server with Pages and Components
ProductManagement.Tests/     # Integration tests with SQLite
ProductManagement.AppHost/   # Aspire orchestration and service configuration
```

## Naming Conventions
- Use **PascalCase** for public members and types
- Use **camelCase** for private fields and local variables
- Prefix interfaces with **I** (e.g., `IProductRepository`)
- Use **descriptive names** for endpoints (e.g., `GetAllProductsEndpoint`)
- Name test methods with **Given_When_Then** or **Action_Scenario_Result** patterns

## Security Considerations
- Implement **input validation** on all endpoints
- Use **parameterized queries** to prevent SQL injection
- Apply **CORS policies** appropriately
- Handle **sensitive data** properly (avoid logging secrets)
- Implement **proper error handling** without exposing internals

## Performance Guidelines
- Use **async/await** for all I/O operations
- Implement **efficient SQL queries** with proper indexing considerations
- Use **connection pooling** with PostgreSQL
- Apply **caching strategies** where appropriate
- Optimize **Blazor component** rendering with proper lifecycle management

## Common Patterns to Follow

### Repository Implementation
```csharp
public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;
    
    public async Task<Product> CreateAsync(CreateProductRequest request)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        const string sql = "INSERT INTO products (name, price, description) VALUES (@Name, @Price, @Description) RETURNING *";
        return await connection.QueryFirstAsync<Product>(sql, request);
    }
}
```

### FastEndpoints Pattern
```csharp
public class CreateProductEndpoint : Endpoint<CreateProductRequest, Product>
{
    public IProductRepository Repository { get; set; } = null!;

    public override void Configure()
    {
        Post("/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var product = await Repository.CreateAsync(req);
        await SendCreatedAtAsync<GetProductByIdEndpoint>(new { id = product.Id }, product, cancellation: ct);
    }
}
```

### Blazor Component Pattern
```razor
@rendermode InteractiveServer
@inject IProductApiClient ProductApiClient

<div class="card">
    <div class="card-body">
        <h5 class="card-title">@Product.Name</h5>
        <p class="card-text">@Product.Description</p>
        <p class="card-text"><strong>$@Product.Price.ToString("F2")</strong></p>
    </div>
</div>
```

When working on this project, always follow these patterns and guidelines to maintain consistency with the existing codebase.
