# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy project files (including Tests project to satisfy solution file)
COPY ProductManagement.sln .
COPY ProductManagement.Models/*.csproj ./ProductManagement.Models/
COPY ProductManagement.Data/*.csproj ./ProductManagement.Data/
COPY ProductManagement.ApiService/*.csproj ./ProductManagement.ApiService/
COPY ProductManagement.Web/*.csproj ./ProductManagement.Web/
COPY ProductManagement.ServiceDefaults/*.csproj ./ProductManagement.ServiceDefaults/
COPY ProductManagement.AppHost/*.csproj ./ProductManagement.AppHost/
COPY ProductManagement.Tests/*.csproj ./ProductManagement.Tests/

# Restore dependencies for production projects only (exclude tests)
RUN dotnet restore ProductManagement.ApiService
RUN dotnet restore ProductManagement.Web

# Copy source code
COPY . .

# Build the application (production projects only)
RUN dotnet publish ProductManagement.Web -c Release -o /app/web --no-restore
RUN dotnet publish ProductManagement.ApiService -c Release -o /app/api --no-restore

# Runtime stage for Web
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS web
WORKDIR /app
COPY --from=build /app/web .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProductManagement.Web.dll"]

# Runtime stage for API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS api
WORKDIR /app
COPY --from=build /app/api .
EXPOSE 8081
ENTRYPOINT ["dotnet", "ProductManagement.ApiService.dll"]
