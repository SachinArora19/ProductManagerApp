var builder = DistributedApplication.CreateBuilder(args);

// Use existing PostgreSQL instead of creating a new one
// Comment out Aspire's PostgreSQL to avoid conflicts
// var postgres = builder.AddPostgres("postgres")
//     .WithDataVolume()
//     .WithPgAdmin();
// var database = postgres.AddDatabase("productmanagement");

// Add API service without database reference (it will use connection string from appsettings)
var apiService = builder.AddProject<Projects.ProductManagement_ApiService>("apiservice");

// Add Blazor web frontend with API service reference
builder.AddProject<Projects.ProductManagement_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
