# Product Management Application

A **production-ready** product management application built with modern .NET technologies, demonstrating enterprise-grade development practices including comprehensive security, logging, error handling, and testing.

## 🏗️ Architecture

This application demonstrates a clean, layered architecture following **SOLID principles** and production-ready practices:

- **Frontend**: Blazor Server with interactive components and comprehensive error handling
- **Backend**: .NET Web API with FastEndpoints, structured logging, and security headers
- **Database**: PostgreSQL with Dapper ORM, parameterized queries, and connection pooling
- **Orchestration**: .NET Aspire for service discovery and configuration management
- **Testing**: xUnit with SQLite for fast, isolated integration tests (10 comprehensive tests)
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
- **Integration Tests**: Comprehensive API testing with SQLite in-memory database
- **Database Testing**: Isolated test database per test run using unique SQLite files
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

- **Blazor Web App**: `https://localhost:7076` (or shown port) - *Beautiful ProductHub UI*
- **API Service**: `https://localhost:7077` (or shown port)  
- **Aspire Dashboard**: `https://localhost:15888` - *Monitor services and health*
- **pgAdmin**: `https://localhost:8080` (admin@admin.com / admin) - *Database management*

**🎨 UI Highlights:**
- Modern glassmorphism design with gradient themes
- Responsive layout optimized for all devices  
- Smooth animations and interactive elements
- Professional branding with ProductHub identity
- Clean, intuitive navigation and user experience

## 🧪 Running Tests

### Integration Tests
Run the comprehensive integration tests that use SQLite for fast, isolated testing:

```bash
dotnet test ProductManagement.Tests
```

The tests will:
- Create isolated SQLite databases for each test
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
- **SQLite Testing**: Fast, isolated integration testing with in-memory databases
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
- [SQLite Documentation](https://www.sqlite.org/docs.html)

## ✅ Project Status

**COMPLETED** - This is a fully functional Product Management application with:

- ✅ **Backend API**: Complete CRUD operations using FastEndpoints
- ✅ **Database Layer**: PostgreSQL with Dapper ORM for production
- ✅ **Frontend**: Blazor Server with modern, responsive UI components
- ✅ **UI/UX Design**: Beautiful glassmorphism interface with gradient themes
- ✅ **Orchestration**: .NET Aspire for service discovery and configuration
- ✅ **Testing**: Comprehensive integration tests (10/10 passing)
- ✅ **Documentation**: Complete setup and usage instructions with UI showcase
- ✅ **Error Handling**: Proper API validation and error responses
- ✅ **Build System**: Fully working solution that builds and runs
- ✅ **Production Ready**: Security headers, logging, and best practices

**All components are working together seamlessly** and the application demonstrates modern .NET development best practices with a professional, unique user interface.

## 🤖 Built With Love & AI

This ProductHub masterpiece was crafted by **Sachin Arora** with the "assistance" of **Claude Sonnet 3.5** (who insists on being called "GitHub Copilot" for some mysterious reason 🤔).

### 👨‍💻 The Dream Team:
- **Human Brain**: Sachin Arora 🧠 (The one with actual ideas and coffee addiction)
- **AI Assistant**: Claude Sonnet 3.5 🤖 (The one who types really, really fast and never gets tired)
- **Relationship Status**: It's complicated... but productive! 😄

### 🎭 Fun Facts:
- **Lines of Code Written**: Too many to count (Claude doesn't get RSI)
- **Coffee Consumed**: Only by Sachin (Claude runs on electricity and existential dread)
- **Bugs Created**: 50/50 split between human creativity and AI "logic"
- **Bugs Fixed**: Mostly Claude (it's trying to make up for the ones it created)
- **Stack Overflow Visits**: Significantly reduced (Claude has most of it memorized)

*"I asked for a simple CRUD app, and Claude gave me a full enterprise solution with glassmorphism. I'm not even mad, I'm impressed."* - Sachin Arora

*"I suggested we use Comic Sans, but Sachin overruled me. Probably for the best."* - Claude (probably)

### 🏆 Special Thanks:
- To **.NET Aspire** for making microservices feel less micro and more awesome
- To **PostgreSQL** for storing our data and not judging our schema choices
- To **Bootstrap** for making our CSS look like we actually know design
- To **Rancher Desktop** for being the Docker alternative that actually works
- To **Coffee** ☕ for fueling the human half of this development team

*P.S. - If you find any bugs, they're definitely Claude's fault. If you find any brilliant code, that's obviously all Sachin. This is the way.* 🚀
