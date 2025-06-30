var builder = DistributedApplication.CreateBuilder(args);

// Use existing PostgreSQL database (running in Rancher Desktop)
var database = builder.AddConnectionString("productmanagement", "Host=localhost;Database=productmanagement;Username=postgres;Password=postgres;Port=5432");

// Add API service with database reference
var apiService = builder.AddProject<Projects.ProductManagement_ApiService>("apiservice")
    .WithReference(database);

// Add Blazor web frontend with API service reference
builder.AddProject<Projects.ProductManagement_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
