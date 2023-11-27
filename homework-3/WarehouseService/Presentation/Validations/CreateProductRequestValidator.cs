using FluentValidation;
using WarehouseGrpc;

namespace WarehouseService.Validations;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
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