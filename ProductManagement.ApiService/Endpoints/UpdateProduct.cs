using FastEndpoints;
using ProductManagement.Data;
using ProductManagement.Models;

namespace ProductManagement.ApiService.Endpoints;

public class UpdateProductRequestWithId
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class UpdateProductEndpoint : Endpoint<UpdateProductRequestWithId, Product>
{
    public IProductRepository Repository { get; set; } = null!;

    public override void Configure()
    {
        Put("/products/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Update an existing product";
            s.Description = "Updates an existing product in the database";
            s.Responses[200] = "Product updated successfully";
            s.Responses[404] = "Product not found";
            s.Responses[400] = "Invalid product data";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(UpdateProductRequestWithId req, CancellationToken ct)
    {
        var logger = Resolve<ILogger<UpdateProductEndpoint>>();
        
        try
        {
            logger.LogInformation("Updating product with ID: {ProductId}", req.Id);
            
            var updateRequest = new UpdateProductRequest
            {
                Name = req.Name,
                Price = req.Price,
                Description = req.Description
            };

            var product = await Repository.UpdateAsync(req.Id, updateRequest);
            
            if (product == null)
            {
                logger.LogWarning("Product not found for update with ID: {ProductId}", req.Id);
                await SendNotFoundAsync(ct);
                return;
            }

            logger.LogInformation("Product updated successfully with ID: {ProductId}", req.Id);
            await SendOkAsync(product, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating product with ID: {ProductId}", req.Id);
            throw;
        }
    }
}
