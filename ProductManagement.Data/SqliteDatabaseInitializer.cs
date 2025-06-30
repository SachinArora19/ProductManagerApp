using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ProductManagement.Data;

/// <summary>
/// SQLite implementation of DatabaseInitializer for testing purposes
/// </summary>
public class SqliteDatabaseInitializer : DatabaseInitializer
{
    public SqliteDatabaseInitializer(IConfiguration configuration) 
        : base(configuration.GetConnectionString("productmanagement") ?? 
               throw new ArgumentNullException(nameof(configuration))) { }

    public override async Task InitializeAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        // Create the products table
        const string createTableSql = @"
            CREATE TABLE IF NOT EXISTS products (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                price REAL NOT NULL,
                description TEXT,
                created_date TEXT NOT NULL,
                updated_date TEXT
            );";

        await connection.ExecuteAsync(createTableSql);

        // Check if we need to seed data
        const string countSql = "SELECT COUNT(*) FROM products";
        var count = await connection.QueryFirstAsync<int>(countSql);

        if (count == 0)
        {
            // Seed some initial data
            const string seedSql = @"
                INSERT INTO products (name, price, description, created_date) VALUES
                ('Sample Product 1', 19.99, 'A sample product for testing', @CreatedDate),
                ('Sample Product 2', 29.99, 'Another sample product', @CreatedDate),
                ('Sample Product 3', 39.99, 'Third sample product', @CreatedDate);";

            await connection.ExecuteAsync(seedSql, new { CreatedDate = DateTime.UtcNow.ToString("O") });
        }
    }
}
