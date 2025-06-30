using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using ProductManagement.Models;

namespace ProductManagement.Data;

/// <summary>
/// SQLite implementation of IProductRepository for testing purposes
/// </summary>
public class SqliteProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public SqliteProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("productmanagement") ??
                           throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        const string sql = "SELECT id, name, price, description, created_date as CreatedAt, updated_date as UpdatedAt FROM products ORDER BY created_date DESC";
        return await connection.QueryAsync<Product>(sql);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        const string sql = "SELECT id, name, price, description, created_date as CreatedAt, updated_date as UpdatedAt FROM products WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task<Product> CreateAsync(CreateProductRequest request)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        const string sql = @"
            INSERT INTO products (name, price, description, created_date, updated_date) 
            VALUES (@Name, @Price, @Description, @CreatedDate, NULL);
            SELECT id, name, price, description, created_date as CreatedAt, updated_date as UpdatedAt FROM products WHERE id = last_insert_rowid();";
        
        return await connection.QueryFirstAsync<Product>(sql, new
        {
            request.Name,
            request.Price,
            request.Description,
            CreatedDate = DateTime.UtcNow.ToString("O")
        });
    }

    public async Task<Product?> UpdateAsync(int id, UpdateProductRequest request)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        const string sql = @"
            UPDATE products 
            SET name = @Name, price = @Price, description = @Description, updated_date = @UpdatedDate 
            WHERE id = @Id;
            SELECT id, name, price, description, created_date as CreatedAt, updated_date as UpdatedAt FROM products WHERE id = @Id;";
        
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new
        {
            Id = id,
            request.Name,
            request.Price,
            request.Description,
            UpdatedDate = DateTime.UtcNow.ToString("O")
        });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        const string sql = "DELETE FROM products WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        
        return rowsAffected > 0;
    }
}
