using FastEndpoints;
using FluentValidation;
using ProductManagement.ApiService.Endpoints;

namespace ProductManagement.ApiService.Validators;

public class UpdateProductValidator : Validator<UpdateProductRequestWithId>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(100)
            .WithMessage("Product name cannot exceed 100 characters")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Product name cannot be empty or whitespace");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than 0")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Product price cannot exceed $999,999.99");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Product description cannot exceed 500 characters");
    }
}
