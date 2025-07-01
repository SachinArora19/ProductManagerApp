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

### Required Software
Before running the application, ensure you have the following installed:

1. **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** - Required to build and run the application
2. **[Docker Desktop](https://www.docker.com/products/docker-desktop)** - For PostgreSQL database
3. **[Git](https://git-scm.com/downloads)** - To clone the repository

### Installation Commands

**Windows (PowerShell as Administrator):**
```powershell
# Install .NET 8.0 SDK
winget install Microsoft.DotNet.SDK.8

# Install Docker Desktop
winget install Docker.DockerDesktop

# Install Git (if not already installed)
winget install Git.Git

# Verify installations
dotnet --version
docker --version
git --version
```

**macOS (Terminal):**
```bash
# Install Homebrew (if not already installed)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Install .NET 8.0 SDK
brew install --cask dotnet-sdk

# Install Docker Desktop
brew install --cask docker

# Install Git (if not already installed)
brew install git

# Verify installations
dotnet --version
docker --version
git --version
```

**Linux (Ubuntu/Debian):**
```bash
# Install .NET 8.0 SDK
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# Install Docker
sudo apt-get install -y docker.io docker-compose
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -aG docker $USER

# Install Git (if not already installed)
sudo apt-get install -y git

# Verify installations (may need to log out/in for docker group)
dotnet --version
docker --version
git --version
```

## 🚀 Quick Start Guide (New Machine Setup)

### Method 1: Docker Compose Setup (Recommended - 5 Minutes)

This is the **easiest and fastest** way to get the application running on any new machine:

```bash
# 1. Clone the repository
git clone <your-repository-url>
cd BlazorPostgressDapper_TestApp

# 2. Start PostgreSQL database
docker-compose up -d postgres

# 3. Verify database is running
docker ps
# You should see a container named "productmanagement-postgres"

# 4. Restore .NET dependencies
dotnet restore

# 5. Run the application
dotnet run --project ProductManagement.AppHost

# 6. Open your browser and go to:
# - Aspire Dashboard: http://localhost:15888
# - Web Application: http://localhost:5042
# - API: http://localhost:5141
```

### Method 2: Full Docker Setup (No .NET Required)

If you prefer to run everything in Docker containers:

```bash
# 1. Clone the repository
git clone <your-repository-url>
cd BlazorPostgressDapper_TestApp

# 2. Run everything in Docker
docker-compose -f docker-compose.full.yml up --build

# 3. Access the application:
# - Web Application: http://localhost:5062
# - API: http://localhost:5595
# - Database: localhost:5432
```

### Method 3: Manual Setup (Development)

For development with local .NET installation:

```bash
# 1. Clone the repository
git clone <your-repository-url>
cd BlazorPostgressDapper_TestApp

# 2. Start PostgreSQL (choose one option):

# Option A: Using provided Docker Compose
docker-compose up -d postgres

# Option B: Using standalone Docker
docker run --name productmanagement-postgres \
  -e POSTGRES_DB=productmanagement \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:16

# 3. Restore dependencies and build
dotnet restore
dotnet build

# 4. Run the application
dotnet run --project ProductManagement.AppHost
```

## 🔍 Verification & Access

### Check Everything is Working

1. **Verify Database Connection:**
   ```bash
   # Test API database connection
   curl http://localhost:5141/test-connection
   # Should return: "Database connection successful!"
   ```

2. **Access Application URLs:**
   - **🎛️ Aspire Dashboard:** http://localhost:15888 (Service monitoring)
   - **🌐 Web Application:** http://localhost:5042 (Main ProductHub app)
   - **⚡ API Endpoints:** http://localhost:5141 (REST API)
   - **🗄️ Database Admin:** http://localhost:8080 (pgAdmin, if using docker-compose)

3. **Test Application Features:**
   - Navigate to Products page
   - Create a new product
   - Edit an existing product
   - Delete a product
   - Verify data persists after refresh

### Expected Ports
- **Web App**: 5042 (HTTP) / 7042 (HTTPS)
- **API Service**: 5141 (HTTP) / 7141 (HTTPS) 
- **Aspire Dashboard**: 15888
- **PostgreSQL**: 5432
- **pgAdmin**: 8080 (optional)

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

## �️ Troubleshooting

### Common Issues and Solutions

#### ❌ "Port already in use" Error
```bash
# Check what's using the port
netstat -an | findstr :5432    # Windows
netstat -an | grep :5432       # Linux/macOS

# Kill existing PostgreSQL processes
# Windows:
taskkill /f /im postgres.exe
# Linux/macOS:
sudo pkill postgres

# Or use different ports in docker-compose.yml
```

#### ❌ Docker Not Running
```bash
# Start Docker Desktop application
# Windows/macOS: Open Docker Desktop from Applications
# Linux: sudo systemctl start docker

# Verify Docker is running
docker info
```

#### ❌ Database Connection Failed
```bash
# Check PostgreSQL container status
docker-compose ps

# View PostgreSQL logs
docker-compose logs postgres

# Restart PostgreSQL container
docker-compose restart postgres

# Recreate PostgreSQL container (will lose data)
docker-compose down postgres
docker volume rm blazorpostgressdapper_testapp_postgres_data
docker-compose up -d postgres
```

#### ❌ Build Errors
```bash
# Clean and restore
dotnet clean
dotnet restore
dotnet build

# Clear NuGet cache if needed
dotnet nuget locals all --clear
```

#### ❌ Application Won't Start
```bash
# Check if all required tools are installed
dotnet --version  # Should be 8.0.x or higher
docker --version  # Should be 20.x or higher

# Try running components individually
docker-compose up -d postgres
dotnet run --project ProductManagement.ApiService
# In another terminal:
dotnet run --project ProductManagement.Web
```

### Getting Help
- Check the Aspire Dashboard at http://localhost:15888 for service status
- View application logs in the terminal where you ran `dotnet run`
- Test database connectivity: http://localhost:5141/test-connection

## 🐳 Docker Compose Files Explanation

### `docker-compose.yml` (Database Only)
- Starts PostgreSQL database
- Includes pgAdmin for database management
- Use with local .NET development

### `docker-compose.full.yml` (Complete Application)
- Starts PostgreSQL database
- Builds and runs API service in container  
- Builds and runs Web application in container
- Fully containerized deployment

## 🔧 Advanced Configuration

### Environment Variables
You can override settings using environment variables:

**Windows (Command Prompt):**
```cmd
set ConnectionStrings__productmanagement=Host=localhost;Port=5432;Database=productmanagement;Username=postgres;Password=postgres;
dotnet run --project ProductManagement.AppHost
```

**Linux/macOS (Terminal):**
```bash
export ConnectionStrings__productmanagement="Host=localhost;Port=5432;Database=productmanagement;Username=postgres;Password=postgres;"
dotnet run --project ProductManagement.AppHost
```

### Custom Database Configuration
Edit `ProductManagement.ApiService/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "productmanagement": "Host=your-host;Port=5432;Database=your-db;Username=your-user;Password=your-password;"
  }
}
```

### Production Deployment
For production environments:
```bash
# Set production environment
export ASPNETCORE_ENVIRONMENT=Production

# Use production connection string  
export ConnectionStrings__productmanagement="Host=prod-server;Database=productmanagement;Username=prod_user;Password=secure_password;"

# Build and run
dotnet publish -c Release
dotnet ProductManagement.AppHost.dll
```

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

## ✅ New Machine Setup Checklist

Follow this checklist to ensure everything is set up correctly on a new machine:

### Pre-Setup Checklist
```bash
# ✅ Step 1: Verify Prerequisites
[ ] .NET 8.0 SDK installed (dotnet --version)
[ ] Docker Desktop installed and running (docker --version)
[ ] Git installed (git --version)
[ ] Have repository URL ready

# ✅ Step 2: Clone and Setup
[ ] Repository cloned successfully
[ ] Navigated to project directory
[ ] Docker Desktop is running

# ✅ Step 3: Database Setup  
[ ] Run: docker-compose up -d postgres
[ ] Verify: docker ps shows postgres container running
[ ] Test: curl http://localhost:5432 (should connect)

# ✅ Step 4: Application Setup
[ ] Run: dotnet restore (no errors)
[ ] Run: dotnet build (successful build)
[ ] Run: dotnet run --project ProductManagement.AppHost

# ✅ Step 5: Verification
[ ] Aspire Dashboard loads: http://localhost:15888
[ ] Web Application loads: http://localhost:5042  
[ ] API responds: http://localhost:5141/test-connection
[ ] Can create/edit/delete products successfully

# ✅ Step 6: Optional - Database Admin
[ ] pgAdmin loads: http://localhost:8080
[ ] Login: admin@admin.com / admin
[ ] Can connect to PostgreSQL server
```

### Quick Commands Summary

```bash
# Complete setup in 4 commands:
git clone <repository-url> && cd BlazorPostgressDapper_TestApp
docker-compose up -d postgres
dotnet restore
dotnet run --project ProductManagement.AppHost

# Then open: http://localhost:5042
```

### First-Time User Guide

1. **📥 Download & Install Prerequisites**
   - Install .NET 8.0 SDK from Microsoft
   - Install Docker Desktop and start it
   - Clone this repository

2. **🐳 Start Database**
   - Open terminal in project folder
   - Run `docker-compose up -d postgres`
   - Wait for "database system is ready to accept connections"

3. **🚀 Run Application**
   - Run `dotnet restore` (downloads dependencies)
   - Run `dotnet run --project ProductManagement.AppHost`
   - Wait for "Application started" message

4. **🌐 Access Application**
   - Open browser to http://localhost:5042
   - Navigate to "Products" page
   - Try creating, editing, and deleting products

5. **🎉 Success!**
   - You now have a fully working Product Management application
   - Data persists between application restarts
   - All features are ready to use

## 💻 System Requirements

### Minimum Requirements
- **OS**: Windows 10/11, macOS 10.15+, or Linux (Ubuntu 18.04+)
- **RAM**: 4 GB minimum, 8 GB recommended
- **Storage**: 2 GB free space for tools and dependencies
- **Network**: Internet connection for downloading dependencies

### Supported Operating Systems
- ✅ **Windows 10/11** (PowerShell, Command Prompt, WSL2)
- ✅ **macOS** (Intel and Apple Silicon)
- ✅ **Linux** (Ubuntu, Debian, CentOS, RHEL, Fedora)
- ✅ **Docker Desktop** supported platforms

## 🚀 Deployment Options

### Local Development
- Use `docker-compose.yml` for database only
- Run .NET application locally with `dotnet run`
- Best for development and debugging

### Full Containerization  
- Use `docker-compose.full.yml` for everything in containers
- No local .NET installation required
- Good for consistent environments

### Cloud Deployment

**Azure Container Instances:**
```bash
# Build and push to Azure Container Registry
az acr build --registry myregistry --image productmanagement .
az container create --resource-group myRG --name productmanagement --image myregistry.azurecr.io/productmanagement
```

**AWS ECS:**
```bash
# Build and push to ECR
docker build -t productmanagement .
docker tag productmanagement:latest 123456789012.dkr.ecr.us-east-1.amazonaws.com/productmanagement:latest
docker push 123456789012.dkr.ecr.us-east-1.amazonaws.com/productmanagement:latest
```

**Google Cloud Run:**
```bash
# Build and deploy
gcloud builds submit --tag gcr.io/PROJECT-ID/productmanagement
gcloud run deploy --image gcr.io/PROJECT-ID/productmanagement --platform managed
```

## 🚦 Development Workflow

1. **Make Changes**: Modify code in any project
2. **Auto-Reload**: Aspire automatically restarts affected services  
3. **Test**: Run integration tests to verify functionality
4. **Debug**: Use Aspire dashboard to monitor service health
5. **Deploy**: Use Docker containers for consistent deployment

## 📈 Production Considerations

### Security
- Use proper connection strings with secrets management
- Implement authentication and authorization
- Configure HTTPS certificates
- Set up security headers and CORS policies

### Performance  
- Configure connection pooling for database
- Enable response caching where appropriate
- Use CDN for static assets
- Monitor application performance

### Monitoring
- Health checks are enabled via Aspire service defaults
- Structured logging configured throughout application
- Use application monitoring tools (Application Insights, etc.)
- Set up alerts for critical failures

### Database
- Use managed PostgreSQL service for production
- Configure automated backups
- Set up read replicas for scaling
- Plan for database migrations and versioning

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
