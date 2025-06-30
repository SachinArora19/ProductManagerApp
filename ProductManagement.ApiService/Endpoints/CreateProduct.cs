using FastEndpoints;
using ProductManagement.Data;
using ProductManagement.Models;

namespace ProductManagement.ApiService.Endpoints;

public class CreateProductEndpoint : Endpoint<CreateProductRequest, Product>
{
    public IProductRepository Repository { get; set; } = null!;

    public override void Configure()
    {
        Post("/products");
        AllowAnonymous();
        
        Summary(s =>
        {
            s.Summary = "Create a new product";
            s.Description = "Creates a new product in the database";
            s.Responses[201] = "Product created successfully";
            s.Responses[400] = "Invalid product data";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var logger = Resolve<ILogger<CreateProductEndpoint>>();
        
        try
        {
            logger.LogInformation("Creating new product with name: {ProductName}", req.Name);
            
            var product = await Repository.CreateAsync(req);
            
            logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);
            await SendCreatedAtAsync<GetProductByIdEndpoint>(new { id = product.Id }, product, cancellation: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product with name: {ProductName}", req.Name);
            
            // Let the global exception handler deal with this
            throw;
        }
    }
}
