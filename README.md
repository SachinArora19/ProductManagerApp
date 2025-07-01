# Product Management Application

A **production-ready** product management application built with modern .NET technologies, demonstrating enterprise-grade development practices including comprehensive security, logging, error handling, and testing.

## 🏗️ Architecture

This application demonstrates a clean, layered architecture following **SOLID principles** and production-ready practices:

- **Frontend**: Blazor Server with interactive components and comprehensive error handling
- **Backend**: .NET Web API with FastEndpoints, structured logging, and security headers
- **Database**: PostgreSQL with Dapper ORM, parameterized queries, and connection pooling
- **Orchestration**: .NET Aspire for service discovery and configuration management
- **Testing**: xUnit with PostgreSQL for comprehensive integration tests (10 comprehensive tests)
- **Security**: Input validation, SQL injection prevention, security headers, and error sanitization
- **Monitoring**: Health checks, structured logging, and comprehensive error tracking

## 📋 Project Structure

```
ProductManagement/
├── ProductManagement.AppHost/          # .NET Aspire orchestration
├── ProductManagement.ApiService/       # Web API with FastEndpoints + production features
├── ProductManagement.Web/             # Blazor Server frontend with error handling
├── ProductManagement.Data/            # Data access layer with logging and error handling
├── ProductManagement.Models/          # Shared models and DTOs with validation
├── ProductManagement.ServiceDefaults/ # Aspire service defaults
├── ProductManagement.Tests/           # 10 comprehensive integration tests
└── PRODUCTION_READINESS.md            # Detailed production features documentation
```

## 🛡️ Production-Ready Features

### Security
- **HTTP Security Headers** (X-Content-Type-Options, X-Frame-Options, X-XSS-Protection)
- **Input Validation** with FluentValidation and comprehensive rules
- **SQL Injection Prevention** with parameterized queries
- **Error Sanitization** to prevent information disclosure

### Observability
- **Structured Logging** throughout all layers with Serilog-compatible format
- **Health Checks** endpoint for monitoring system health
- **Comprehensive Error Tracking** with context preservation
- **Performance Logging** for database operations and API calls

### Reliability
- **Global Exception Handling** with Problem Details RFC 7807 compliance
- **Circuit Breaker Pattern** considerations in API client
- **Graceful Degradation** when services are unavailable
- **Database Connection Resilience** with proper disposal and pooling

## 🚀 Features

### API Endpoints
- `GET /products` - Retrieve all products
- `GET /products/{id}` - Retrieve a single product by ID
- `POST /products` - Create a new product
- `PUT /products/{id}` - Update an existing product
- `DELETE /products/{id}` - Delete a product

### Web Interface
- **Product List View**: Display all products in a responsive card layout
- **Create Product**: Form to add new products with validation
- **Edit Product**: In-place editing with modal dialogs
- **Delete Product**: Confirmation-based product deletion
- **Real-time Updates**: Blazor Server provides real-time UI updates

### Testing
- **Integration Tests**: Comprehensive API testing with PostgreSQL test databases
- **Database Testing**: Isolated test database per test run using unique PostgreSQL databases
- **CRUD Testing**: Full workflow testing from creation to deletion
- **Validation Testing**: API validation and error handling tests
- **All tests pass**: 10/10 integration tests passing ✅

## 🛠️ Prerequisites

Before running the application, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for PostgreSQL development database)
- [.NET Aspire Workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling)

### Installing .NET Aspire Workload

```bash
dotnet workload install aspire
```

## ⚡ Quick Start

### 1. Clone and Navigate
```bash
git clone <repository-url>
cd ProductManagement
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Run with Aspire (Recommended)
```bash
dotnet run --project ProductManagement.AppHost
```

This will start:
- PostgreSQL database in a Docker container
- pgAdmin for database management
- API service on configured port
- Blazor web application on configured port
- Aspire dashboard for monitoring

### 4. Access the Application

After starting with Aspire, you'll see URLs in the console output:

- **Blazor Web App**: `https://localhost:7076` (or shown port)
- **API Service**: `https://localhost:7077` (or shown port)  
- **Aspire Dashboard**: `https://localhost:15888`
- **pgAdmin**: `https://localhost:8080` (admin@admin.com / admin)

## 🧪 Running Tests

### Integration Tests
Run the comprehensive integration tests that use PostgreSQL for realistic testing:

```bash
dotnet test ProductManagement.Tests
```

The tests will:
- Create isolated PostgreSQL test databases for each test
- Run all CRUD operations tests (10 tests total)
- Verify data persistence and business logic
- Validate API error handling and responses
- Clean up test databases after completion

**Test Results**: ✅ All 10 integration tests passing

### Individual Test Categories
```bash
# Run specific test methods
dotnet test --filter "FullCrudWorkflow"

# Run with minimal output
dotnet test -v minimal

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"
```

## 🐳 Manual Database Setup (Alternative)

If you prefer to run PostgreSQL manually instead of using Aspire:

### Using Docker
```bash
docker run --name postgres-productmanagement \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=productmanagement \
  -p 5432:5432 \
  -d postgres:15
```

### Update Connection String
Update `appsettings.json` in `ProductManagement.ApiService`:
```json
{
  "ConnectionStrings": {
    "productmanagement": "Host=localhost;Database=productmanagement;Username=postgres;Password=postgres"
  }
}
```

### Run Services Individually
```bash
# Start API Service
dotnet run --project ProductManagement.ApiService

# Start Web Application (in another terminal)
dotnet run --project ProductManagement.Web
```

## 🔧 Configuration

### Database Configuration
The application uses PostgreSQL with automatic database initialization. The database schema and seed data are created automatically on startup.

### CORS Configuration
The API is configured to allow requests from the Blazor application with appropriate CORS policies.

### Service Discovery
.NET Aspire handles service discovery between the Blazor app and API service automatically.

## 📊 Database Schema

```sql
CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    description VARCHAR(500),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP
);
```

## 🎯 Key Technologies Demonstrated

- **FastEndpoints**: Clean, minimal API endpoints with automatic validation
- **Dapper**: Lightweight ORM with raw SQL control
- **Blazor Server**: Real-time, component-based UI
- **.NET Aspire**: Modern application orchestration and service discovery
- **PostgreSQL Testing**: Comprehensive integration testing with isolated test databases
- **PostgreSQL**: Production-ready relational database

## 🚦 Development Workflow

1. **Make Changes**: Modify code in any project
2. **Auto-Reload**: Aspire automatically restarts affected services
3. **Test**: Run integration tests to verify functionality
4. **Debug**: Use Aspire dashboard to monitor service health

## 📈 Production Considerations

For production deployment, consider:

- **Environment Configuration**: Use proper connection strings and secrets
- **Health Checks**: Enabled via Aspire service defaults
- **Logging**: Configured through Aspire and .NET logging
- **Security**: Implement authentication and authorization
- **Database Migrations**: Consider using Entity Framework migrations or database migration tools

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Add comprehensive tests
4. Update documentation
5. Submit a pull request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Useful Links

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [FastEndpoints Documentation](https://fast-endpoints.com/)
- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

## ✅ Project Status

**COMPLETED** - This is a fully functional Product Management application with:

- ✅ **Backend API**: Complete CRUD operations using FastEndpoints
- ✅ **Database Layer**: PostgreSQL with Dapper ORM for production
- ✅ **Frontend**: Blazor Server with interactive UI components
- ✅ **Orchestration**: .NET Aspire for service discovery and configuration
- ✅ **Testing**: Comprehensive integration tests (10/10 passing)
- ✅ **Documentation**: Complete setup and usage instructions
- ✅ **Error Handling**: Proper API validation and error responses
- ✅ **Build System**: Fully working solution that builds and runs

**All components are working together seamlessly** and the application demonstrates modern .NET development best practices.

## 🎉 Fun Credits & Acknowledgments

This ProductHub application was crafted with:
- 🧠 **Countless brain cells** sacrificing themselves for clean code
- ☕ **Industrial amounts of caffeine** (coffee count: lost track after 47)
- 🍕 **Pizza-powered late-night coding sessions** because proper nutrition is overrated
- 🐛 **Bugs that became features** (and features that became bugs)
- 💻 **Stack Overflow visits** (roughly 342,891 times... but who's counting?)
- 🎵 **Lo-fi hip hop playlists** for maximum coding concentration
- 🐘 **Rubber duck debugging sessions** (our duck prefers PostgreSQL for its robustness)
- 😴 **Sleep deprivation** carefully balanced with determination
- 🌟 **Pure developer magic** and a sprinkle of "it works on my machine"

**Special thanks to:**
- The .NET team for making Aspire awesome 🚀
- PostgreSQL elephants for storing our data reliably 🐘
- Bootstrap for making our UI not look like it's from 1995 💄
- Blazor for making server-side rendering cool again ⚡
- FastEndpoints for keeping our APIs fast and our endpoints... fast 🏃‍♂️
- Our future selves who will have to maintain this code 😅

*Built with love, determination, and an unhealthy relationship with syntax highlighting.* ❤️

---

*"Code is like humor. When you have to explain it, it's bad." - Cory House*  
*"This code doesn't need explanation... we hope." - ProductHub Team* 😉
