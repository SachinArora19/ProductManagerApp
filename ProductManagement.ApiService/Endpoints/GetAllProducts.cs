using FastEndpoints;
using ProductManagement.Data;
using ProductManagement.Models;

namespace ProductManagement.ApiService.Endpoints;

public class GetAllProductsEndpoint : EndpointWithoutRequest<IEnumerable<Product>>
{
    public IProductRepository Repository { get; set; } = null!;

    public override void Configure()
    {
        Get("/products");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get all products";
            s.Description = "Retrieves all products from the database";
            s.Responses[200] = "List of products retrieved successfully";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var logger = Resolve<ILogger<GetAllProductsEndpoint>>();
        
        try
        {
            logger.LogInformation("Retrieving all products");
            
            var products = await Repository.GetAllAsync();
            
            logger.LogInformation("Retrieved {ProductCount} products", products.Count());
            await SendOkAsync(products, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all products");
            throw;
        }
    }
}
