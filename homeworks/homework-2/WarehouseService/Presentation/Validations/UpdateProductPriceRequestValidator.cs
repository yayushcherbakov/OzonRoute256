using FluentValidation;
using WarehouseGrpc;

namespace WarehouseService.Validations;

public class UpdateProductPriceRequestValidator : AbstractValidator<UpdateProductPriceRequest>
{
    public UpdateProductPriceRequestValidator()
    {
        RuleFor(product => product.ProductId)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

        RuleFor(product => product.NewPrice)
            .NotEmpty()
            .GreaterThan(0);
    }
}