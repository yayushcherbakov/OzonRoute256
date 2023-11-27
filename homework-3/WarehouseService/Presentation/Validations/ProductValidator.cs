using Domain.Models;
using FluentValidation;

namespace WarehouseService.Validations;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty()
            .Length(3, 30);

        RuleFor(product => product.Price)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(product => product.Weight)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(product => product.Type)
            .NotEmpty();

        RuleFor(product => product.CreationDate);

        RuleFor(product => product.WarehouseId)
            .InclusiveBetween(default, int.MaxValue);
    }
}