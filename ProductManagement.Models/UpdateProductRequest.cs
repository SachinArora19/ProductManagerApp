using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models;

public class UpdateProductRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
}
