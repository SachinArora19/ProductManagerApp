using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace ProductManagement.Data;

public class DatabaseInitializer : IDatabaseInitializer
{
    protected readonly string _connectionString;
    protected readonly ILogger<DatabaseInitializer>? _logger;

    public DatabaseInitializer(string connectionString, ILogger<DatabaseInitializer>? logger = null)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public virtual async Task InitializeAsync()
    {
        try
        {
            _logger?.LogInformation("Starting database initialization");
            
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string createTableSql = @"
                CREATE TABLE IF NOT EXISTS products (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    price DECIMAL(10,2) NOT NULL CHECK (price > 0),
                    description VARCHAR(500),
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP
                );
                
                CREATE INDEX IF NOT EXISTS idx_products_name ON products(name);
                CREATE INDEX IF NOT EXISTS idx_products_created_at ON products(created_at);";

            await connection.ExecuteAsync(createTableSql);
            _logger?.LogInformation("Database tables and indexes created successfully");

            // Seed some sample data if table is empty
            const string countSql = "SELECT COUNT(*) FROM products";
            var count = await connection.QueryFirstAsync<int>(countSql);

            if (count == 0)
            {
                _logger?.LogInformation("Seeding initial product data");
                
                const string seedSql = @"
                    INSERT INTO products (name, price, description) VALUES 
                    ('Sample Product 1', 10.99, 'A sample product for testing.'),
                    ('Sample Product 2', 25.50, 'Another sample product.'),
                    ('Sample Product 3', 5.00, 'Third sample product.')";

                await connection.ExecuteAsync(seedSql);
                _logger?.LogInformation("Initial product data seeded successfully");
            }
            else
            {
                _logger?.LogInformation("Skipping data seeding - {ProductCount} products already exist", count);
            }
            
            _logger?.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Database initialization failed");
            throw;
        }
    }
}
