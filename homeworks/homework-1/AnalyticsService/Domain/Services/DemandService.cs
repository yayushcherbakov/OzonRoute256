using DataAccess.Contracts;
using Domain.Constants;
using Domain.Contracts;

namespace Domain.Services;

public class DemandService : IDemandService
{
    private readonly ISalesRepository _salesRepository;
    private readonly ISalesPredictionService _salesPredictionService;

    public DemandService(ISalesRepository salesRepository, ISalesPredictionService salesPredictionService)
    {
        _salesRepository = salesRepository;
        _salesPredictionService = salesPredictionService;
    }

    public int CalculateDemand(int productId, int countDays)
    {
        return CalculateDemand(productId, countDays, DateOnly.FromDateTime(DateTime.UtcNow));
    }

    public int CalculateDemand(int productId, int countDays, DateOnly date)
    {
        if (date < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new InvalidDataException(ErrorMessages.DateMustBeGreaterThanOrEqualToCurrentDate);
        }

        var currentProductInStock = _salesRepository.GetSalesDataById(productId)
            .OrderBy(x => x.Date)
            .Select(x => x.Stock)
            .Last();

        var stockDownsidePrediction = _salesPredictionService.CalculateSalesPrediction
        (
            productId,
            (date.ToDateTime(new TimeOnly(0)) - DateTime.UtcNow).Days,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var predictProductInStock = currentProductInStock - stockDownsidePrediction;

        if (predictProductInStock < 0)
        {
            predictProductInStock = 0;
        }

        var salesPrediction = _salesPredictionService.CalculateSalesPrediction(productId, countDays, date);

        return salesPrediction - predictProductInStock;
    }
}