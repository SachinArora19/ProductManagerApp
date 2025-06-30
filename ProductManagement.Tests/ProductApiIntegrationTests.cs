using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ProductManagement.Models;
using ProductManagement.Data;
using System.Text.Json;
using System.Text;

namespace ProductManagement.Tests;

public class ProductApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _testDbName;

    public ProductApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _testDbName = $"test_{Guid.NewGuid():N}.db";
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Use unique SQLite database for each test instance
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:productmanagement"] = $"Data Source={_testDbName}"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Replace PostgreSQL repository with SQLite for testing
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IProductRepository));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                
                var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDatabaseInitializer));
                if (dbDescriptor != null)
                {
                    services.Remove(dbDescriptor);
                }
                
                services.AddScoped<IProductRepository, SqliteProductRepository>();
                services.AddScoped<IDatabaseInitializer, SqliteDatabaseInitializer>();
            });
        });

        _client = _factory.CreateClient();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        // Initialize the database
        InitializeDatabase().Wait();
    }

    private async Task InitializeDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        await dbInitializer.InitializeAsync();
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
        
        // Clean up test database
        if (File.Exists(_testDbName))
        {
            try
            {
                File.Delete(_testDbName);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [Fact]
    public async Task GetAllProducts_ReturnsInitialSeedData()
    {
        // Act
        var response = await _client.GetAsync("/products");

        // Assert
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<Product[]>(json, _jsonOptions);

        Assert.NotNull(products);
        Assert.True(products.Length >= 3); // We seed 3 products
        Assert.Contains(products, p => p.Name == "Sample Product 1");
    }

    [Fact]
    public async Task GetProductById_ExistingProduct_ReturnsProduct()
    {
        // Arrange - First get all products to find an existing ID
        var allProductsResponse = await _client.GetAsync("/products");
        var allProductsJson = await allProductsResponse.Content.ReadAsStringAsync();
        var allProducts = JsonSerializer.Deserialize<Product[]>(allProductsJson, _jsonOptions);
        var existingProduct = allProducts!.First();

        // Act
        var response = await _client.GetAsync($"/products/{existingProduct.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<Product>(json, _jsonOptions);

        Assert.NotNull(product);
        Assert.Equal(existingProduct.Id, product.Id);
        Assert.Equal(existingProduct.Name, product.Name);
    }

    [Fact]
    public async Task GetProductById_NonExistentProduct_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/products/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ValidData_ReturnsCreatedProduct()
    {
        // Arrange
        var createRequest = new CreateProductRequest
        {
            Name = "Test Product",
            Price = 29.99m,
            Description = "A test product for integration testing"
        };

        var json = JsonSerializer.Serialize(createRequest, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/products", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseJson = await response.Content.ReadAsStringAsync();
        var createdProduct = JsonSerializer.Deserialize<Product>(responseJson, _jsonOptions);

        Assert.NotNull(createdProduct);
        Assert.True(createdProduct.Id > 0);
        Assert.Equal(createRequest.Name, createdProduct.Name);
        Assert.Equal(createRequest.Price, createdProduct.Price);
        Assert.Equal(createRequest.Description, createdProduct.Description);
        Assert.True(createdProduct.CreatedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task CreateProduct_InvalidData_ReturnsBadRequest()
    {
        // Arrange - Empty name should be invalid
        var createRequest = new CreateProductRequest
        {
            Name = "", // Invalid - required field
            Price = 29.99m,
            Description = "A test product"
        };

        var json = JsonSerializer.Serialize(createRequest, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/products", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_ExistingProduct_ReturnsUpdatedProduct()
    {
        // Arrange - First create a product
        var createRequest = new CreateProductRequest
        {
            Name = "Original Product",
            Price = 19.99m,
            Description = "Original description"
        };

        var createJson = JsonSerializer.Serialize(createRequest, _jsonOptions);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/products", createContent);
        var createResponseJson = await createResponse.Content.ReadAsStringAsync();
        var createdProduct = JsonSerializer.Deserialize<Product>(createResponseJson, _jsonOptions);

        // Prepare update request
        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated Product",
            Price = 39.99m,
            Description = "Updated description"
        };

        var updateJson = JsonSerializer.Serialize(updateRequest, _jsonOptions);
        var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/products/{createdProduct!.Id}", updateContent);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        var updatedProduct = JsonSerializer.Deserialize<Product>(responseJson, _jsonOptions);

        Assert.NotNull(updatedProduct);
        Assert.Equal(createdProduct.Id, updatedProduct.Id);
        Assert.Equal(updateRequest.Name, updatedProduct.Name);
        Assert.Equal(updateRequest.Price, updatedProduct.Price);
        Assert.Equal(updateRequest.Description, updatedProduct.Description);
        Assert.NotNull(updatedProduct.UpdatedAt);
    }

    [Fact]
    public async Task UpdateProduct_NonExistentProduct_ReturnsNotFound()
    {
        // Arrange
        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated Product",
            Price = 39.99m,
            Description = "Updated description"
        };

        var json = JsonSerializer.Serialize(updateRequest, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/products/99999", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_ExistingProduct_ReturnsNoContent()
    {
        // Arrange - First create a product
        var createRequest = new CreateProductRequest
        {
            Name = "Product to Delete",
            Price = 19.99m,
            Description = "This product will be deleted"
        };

        var createJson = JsonSerializer.Serialize(createRequest, _jsonOptions);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/products", createContent);
        var createResponseJson = await createResponse.Content.ReadAsStringAsync();
        var createdProduct = JsonSerializer.Deserialize<Product>(createResponseJson, _jsonOptions);

        // Act
        var response = await _client.DeleteAsync($"/products/{createdProduct!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the product is actually deleted
        var getResponse = await _client.GetAsync($"/products/{createdProduct.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_NonExistentProduct_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/products/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task FullCrudWorkflow_CreateUpdateDelete_WorksCorrectly()
    {
        // Create
        var createRequest = new CreateProductRequest
        {
            Name = "Workflow Test Product",
            Price = 25.99m,
            Description = "Testing full CRUD workflow"
        };

        var createJson = JsonSerializer.Serialize(createRequest, _jsonOptions);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/products", createContent);
        var createdProduct = JsonSerializer.Deserialize<Product>(
            await createResponse.Content.ReadAsStringAsync(), _jsonOptions);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createdProduct);

        // Read
        var getResponse = await _client.GetAsync($"/products/{createdProduct.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // Update
        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated Workflow Product",
            Price = 35.99m,
            Description = "Updated workflow description"
        };

        var updateJson = JsonSerializer.Serialize(updateRequest, _jsonOptions);
        var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");
        var updateResponse = await _client.PutAsync($"/products/{createdProduct.Id}", updateContent);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // Delete
        var deleteResponse = await _client.DeleteAsync($"/products/{createdProduct.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify deletion
        var finalGetResponse = await _client.GetAsync($"/products/{createdProduct.Id}");
        Assert.Equal(HttpStatusCode.NotFound, finalGetResponse.StatusCode);
    }
}
