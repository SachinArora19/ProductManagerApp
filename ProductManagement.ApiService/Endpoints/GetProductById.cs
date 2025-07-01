using FastEndpoints;
using ProductManagement.Data;
using ProductManagement.Models;

namespace ProductManagement.ApiService.Endpoints;

public class GetProductByIdRequest
{
    public int Id { get; set; }
}

public class GetProductByIdEndpoint : Endpoint<GetProductByIdRequest, Product>
{
    public IProductRepository Repository { get; set; } = null!;

    public override void Configure()
    {
        Get("/products/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get product by ID";
            s.Description = "Retrieves a specific product by its ID";
            s.Responses[200] = "Product retrieved successfully";
            s.Responses[404] = "Product not found";
            s.Responses[400] = "Invalid product ID";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(GetProductByIdRequest req, CancellationToken ct)
    {
        var logger = Resolve<ILogger<GetProductByIdEndpoint>>();
        
        try
        {
            if (req.Id <= 0)
            {
                logger.LogWarning("Invalid product ID requested: {ProductId}", req.Id);
                await SendErrorsAsync(400, ct);
                return;
            }

            logger.LogInformation("Retrieving product with ID: {ProductId}", req.Id);
            
            var product = await Repository.GetByIdAsync(req.Id);
            
            if (product == null)
            {
                logger.LogWarning("Product not found with ID: {ProductId}", req.Id);
                await SendNotFoundAsync(ct);
                return;
            }

            logger.LogInformation("Product retrieved successfully with ID: {ProductId}", req.Id);
            await SendOkAsync(product, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving product with ID: {ProductId}", req.Id);
            throw;
        }
    }
}
