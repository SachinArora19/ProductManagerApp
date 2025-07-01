using FastEndpoints;
using ProductManagement.Data;

namespace ProductManagement.ApiService.Endpoints;

public class DeleteProductRequest
{
    public int Id { get; set; }
}

public class DeleteProductEndpoint : Endpoint<DeleteProductRequest>
{
    public IProductRepository Repository { get; set; } = null!;

    public override void Configure()
    {
        Delete("/products/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Delete a product";
            s.Description = "Deletes a product from the database";
            s.Responses[204] = "Product deleted successfully";
            s.Responses[404] = "Product not found";
            s.Responses[400] = "Invalid product ID";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(DeleteProductRequest req, CancellationToken ct)
    {
        var logger = Resolve<ILogger<DeleteProductEndpoint>>();
        
        try
        {
            if (req.Id <= 0)
            {
                logger.LogWarning("Invalid product ID for deletion: {ProductId}", req.Id);
                await SendErrorsAsync(400, ct);
                return;
            }

            logger.LogInformation("Deleting product with ID: {ProductId}", req.Id);
            
            var deleted = await Repository.DeleteAsync(req.Id);
            
            if (!deleted)
            {
                logger.LogWarning("Product not found for deletion with ID: {ProductId}", req.Id);
                await SendNotFoundAsync(ct);
                return;
            }

            logger.LogInformation("Product deleted successfully with ID: {ProductId}", req.Id);
            await SendNoContentAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting product with ID: {ProductId}", req.Id);
            throw;
        }
    }
}
