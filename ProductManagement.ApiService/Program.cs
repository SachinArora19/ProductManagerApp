/*
 * ProductHub API Service
 * 
 * Crafted with ❤️ by Sachin Arora
 * With "minimal" help from Claude Sonnet 3.5 🤖
 * (Claude insisted on adding glassmorphism to everything - even the API responses!)
 * 
 * Warning: Contains traces of over-engineering and excessive use of gradients
 */

using FastEndpoints;
using ProductManagement.Data;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

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

// Configure PostgreSQL using external database (not Aspire-managed)
// We're using an external PostgreSQL instance, so we'll rely on connection strings only

// Register repositories using PostgreSQL
builder.Services.AddScoped<IProductRepository>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("productmanagement");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("PostgreSQL connection string 'productmanagement' not found.");
    }
    var logger = provider.GetService<ILogger<ProductRepository>>();
    return new ProductRepository(connectionString, logger);
});

builder.Services.AddScoped<IDatabaseInitializer>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("productmanagement");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("PostgreSQL connection string 'productmanagement' not found.");
    }
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
