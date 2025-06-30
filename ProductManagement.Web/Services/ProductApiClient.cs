using ProductManagement.Models;
using System.Net;
using System.Text.Json;

namespace ProductManagement.Web.Services;

public interface IProductApiClient
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(CreateProductRequest request);
    Task<Product?> UpdateAsync(int id, UpdateProductRequest request);
    Task<bool> DeleteAsync(int id);
}

public class ProductApiClient : IProductApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<ProductApiClient> _logger;

    public ProductApiClient(HttpClient httpClient, ILogger<ProductApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all products from API");
            
            var response = await _httpClient.GetAsync("/products");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<Product[]>(json, _jsonOptions) ?? Array.Empty<Product>();
                
                _logger.LogInformation("Successfully retrieved {ProductCount} products", products.Length);
                return products;
            }
            
            _logger.LogWarning("Failed to retrieve products. Status: {StatusCode}", response.StatusCode);
            await LogErrorResponse(response);
            return Array.Empty<Product>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all products from API");
            throw new ApplicationException("Failed to retrieve products from the server", ex);
        }
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Fetching product with ID: {ProductId}", id);
            
            var response = await _httpClient.GetAsync($"/products/{id}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", id);
                return null;
            }
                
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(json, _jsonOptions);
                
                _logger.LogInformation("Successfully retrieved product with ID: {ProductId}", id);
                return product;
            }
            
            _logger.LogWarning("Failed to retrieve product {ProductId}. Status: {StatusCode}", id, response.StatusCode);
            await LogErrorResponse(response);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product with ID: {ProductId}", id);
            throw new ApplicationException($"Failed to retrieve product {id} from the server", ex);
        }
    }

    public async Task<Product> CreateAsync(CreateProductRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new product: {ProductName}", request.Name);
            
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/products", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(responseJson, _jsonOptions)!;
                
                _logger.LogInformation("Successfully created product with ID: {ProductId}", product.Id);
                return product;
            }
            
            _logger.LogWarning("Failed to create product. Status: {StatusCode}", response.StatusCode);
            await LogErrorResponse(response);
            throw new ApplicationException("Failed to create product");
        }
        catch (Exception ex) when (!(ex is ApplicationException))
        {
            _logger.LogError(ex, "Error creating product: {ProductName}", request.Name);
            throw new ApplicationException("Failed to create product on the server", ex);
        }
    }

    public async Task<Product?> UpdateAsync(int id, UpdateProductRequest request)
    {
        try
        {
            _logger.LogInformation("Updating product with ID: {ProductId}", id);
            
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/products/{id}", content);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Product not found for update with ID: {ProductId}", id);
                return null;
            }
                
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(responseJson, _jsonOptions);
                
                _logger.LogInformation("Successfully updated product with ID: {ProductId}", id);
                return product;
            }
            
            _logger.LogWarning("Failed to update product {ProductId}. Status: {StatusCode}", id, response.StatusCode);
            await LogErrorResponse(response);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
            throw new ApplicationException($"Failed to update product {id} on the server", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting product with ID: {ProductId}", id);
            
            var response = await _httpClient.DeleteAsync($"/products/{id}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Product not found for deletion with ID: {ProductId}", id);
                return false;
            }
                
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully deleted product with ID: {ProductId}", id);
                return true;
            }
            
            _logger.LogWarning("Failed to delete product {ProductId}. Status: {StatusCode}", id, response.StatusCode);
            await LogErrorResponse(response);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
            throw new ApplicationException($"Failed to delete product {id} from the server", ex);
        }
    }

    private async Task LogErrorResponse(HttpResponseMessage response)
    {
        try
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("API Error Response: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read error response content");
        }
    }
}
