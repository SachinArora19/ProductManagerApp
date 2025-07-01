using FastEndpoints;
using Npgsql;
using Dapper;

namespace ProductManagement.ApiService.Endpoints;

public class TestConnection : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("/test-connection");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            var connectionString = Config.GetConnectionString("productmanagement");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                await SendAsync("Connection string is null or empty", 500, ct);
                return;
            }

            Logger.LogInformation("Testing connection with: {ConnectionString}", connectionString);

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(ct);
            
            var result = await connection.QueryFirstAsync<int>("SELECT 1");
            await connection.CloseAsync();
            
            await SendOkAsync($"Connection successful! Test query returned: {result}", ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Connection test failed");
            await SendAsync($"Connection failed: {ex.Message}", 500, ct);
        }
    }
}
