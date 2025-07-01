using FastEndpoints;
using ProductManagement.Data;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults but skip database configuration
// Comment out to avoid Aspire overriding our connection string
// builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails(options =>
{
    // Configure custom problem details for production
    options.CustomizeProblemDetails = (context) =>
    {
        // Don't expose internal details in production
        if (!builder.Environment.IsDevelopment())
        {
            context.ProblemDetails.Detail = "An error occurred while processing your request.";
            if (context.ProblemDetails.Extensions.ContainsKey("traceId"))
            {
                context.ProblemDetails.Extensions.Remove("traceId");
            }
        }
    };
});

// Add FastEndpoints with global validation configuration
builder.Services.AddFastEndpoints(o =>
{
    o.IncludeAbstractValidators = true;
});

// Configure JSON serialization globally
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Add comprehensive logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddDebug();
}

// Add health checks
builder.Services.AddHealthChecks();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:7076", "http://localhost:5170")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register repositories using PostgreSQL (manual connection string)
builder.Services.AddScoped<IProductRepository>(provider =>
{
    // Debug: Check all connection strings
    var config = provider.GetService<IConfiguration>();
    var allConnectionStrings = config.GetSection("ConnectionStrings").GetChildren();
    var logger = provider.GetService<ILogger<Program>>();
    
    logger?.LogInformation("=== CONNECTION STRING DEBUG ===");
    foreach (var cs in allConnectionStrings)
    {
        logger?.LogInformation("Found connection string '{Key}': {Value}", cs.Key, cs.Value);
    }
    
    var connectionString = builder.Configuration.GetConnectionString("productmanagement");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("PostgreSQL connection string 'productmanagement' not found.");
    }
    
    // Debug logging for connection string
    logger?.LogInformation("=== USING CONNECTION STRING ===");
    logger?.LogInformation("Final connection string: {ConnectionString}", connectionString);
    logger?.LogInformation("=== END DEBUG ===");
    
    var repoLogger = provider.GetService<ILogger<ProductRepository>>();
    return new ProductRepository(connectionString, repoLogger);
});

builder.Services.AddScoped<IDatabaseInitializer>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("productmanagement");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("PostgreSQL connection string 'productmanagement' not found.");
    }
    
    // Debug logging for connection string
    var consoleLogger = provider.GetService<ILogger<Program>>();
    consoleLogger?.LogInformation("Database initializer using connection string: {ConnectionString}", connectionString);
    
    var logger = provider.GetService<ILogger<DatabaseInitializer>>();
    return new DatabaseInitializer(connectionString, logger);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next.Invoke();
});

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowBlazorApp");

// Use FastEndpoints
app.UseFastEndpoints(c =>
{
    c.Errors.UseProblemDetails();
});

// Map health checks
app.MapHealthChecks("/health");

// Initialize database with error handling and logging
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    
    try
    {
        logger.LogInformation("Initializing database...");
        await dbInitializer.InitializeAsync();
        logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to initialize database");
        throw; // Re-throw to prevent app startup with uninitialized database
    }
}

app.MapDefaultEndpoints();

app.Run();

// Make Program class accessible for testing
public partial class Program { }
