# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy project files
COPY ProductManagement.sln .
COPY ProductManagement.Models/*.csproj ./ProductManagement.Models/
COPY ProductManagement.Data/*.csproj ./ProductManagement.Data/
COPY ProductManagement.ApiService/*.csproj ./ProductManagement.ApiService/
COPY ProductManagement.Web/*.csproj ./ProductManagement.Web/
COPY ProductManagement.ServiceDefaults/*.csproj ./ProductManagement.ServiceDefaults/
COPY ProductManagement.AppHost/*.csproj ./ProductManagement.AppHost/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build the application
RUN dotnet publish ProductManagement.Web -c Release -o /app/web
RUN dotnet publish ProductManagement.ApiService -c Release -o /app/api

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
