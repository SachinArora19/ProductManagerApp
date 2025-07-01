using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using ProductManagement.Models;

namespace ProductManagement.Data;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;
    private readonly ILogger<ProductRepository>? _logger;

    public ProductRepository(string connectionString, ILogger<ProductRepository>? logger = null)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = @"
                SELECT id, name, price, description, created_at as CreatedAt, updated_at as UpdatedAt 
                FROM products 
                ORDER BY created_at DESC";
            
            _logger?.LogDebug("Executing GetAllAsync query");
            return await connection.QueryAsync<Product>(sql);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving all products from database");
            throw;
        }
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = @"
                SELECT id, name, price, description, created_at as CreatedAt, updated_at as UpdatedAt 
                FROM products 
                WHERE id = @Id";
            
            _logger?.LogDebug("Executing GetByIdAsync query for ID: {ProductId}", id);
            return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
            throw;
        }
    }

    public async Task<Product> CreateAsync(CreateProductRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO products (name, price, description, created_at) 
                VALUES (@Name, @Price, @Description, @CreatedAt) 
                RETURNING id, name, price, description, created_at as CreatedAt, updated_at as UpdatedAt";
            
            _logger?.LogDebug("Executing CreateAsync query for product: {ProductName}", request.Name);
            
            var product = await connection.QueryFirstAsync<Product>(sql, new 
            { 
                request.Name, 
                request.Price, 
                request.Description, 
                CreatedAt = DateTime.UtcNow 
            });
            
            _logger?.LogDebug("Product created successfully with ID: {ProductId}", product.Id);
            return product;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error creating product: {ProductName}", request.Name);
            throw;
        }
    }

    public async Task<Product?> UpdateAsync(int id, UpdateProductRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = @"
                UPDATE products 
                SET name = @Name, price = @Price, description = @Description, updated_at = @UpdatedAt 
                WHERE id = @Id 
                RETURNING id, name, price, description, created_at as CreatedAt, updated_at as UpdatedAt";
            
            _logger?.LogDebug("Executing UpdateAsync query for product ID: {ProductId}", id);
            
            var product = await connection.QueryFirstOrDefaultAsync<Product>(sql, new 
            { 
                Id = id,
                request.Name, 
                request.Price, 
                request.Description, 
                UpdatedAt = DateTime.UtcNow 
            });
            
            if (product != null)
            {
                _logger?.LogDebug("Product updated successfully with ID: {ProductId}", id);
            }
            
            return product;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating product with ID: {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            const string sql = "DELETE FROM products WHERE id = @Id";
            
            _logger?.LogDebug("Executing DeleteAsync query for product ID: {ProductId}", id);
            
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            var deleted = rowsAffected > 0;
            
            if (deleted)
            {
                _logger?.LogDebug("Product deleted successfully with ID: {ProductId}", id);
            }
            
            return deleted;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting product with ID: {ProductId}", id);
            throw;
        }
    }
}
