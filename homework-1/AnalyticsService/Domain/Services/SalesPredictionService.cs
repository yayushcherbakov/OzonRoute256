using DataAccess.Contracts;
using Domain.Constants;
using Domain.Contracts;
using Domain.Exceptions;

namespace Domain.Services;

public class SalesPredictionService : ISalesPredictionService
{
    private readonly ISeasonsRepository _seasonsRepository;
    private readonly IAdsService _adsService;

    public SalesPredictionService(ISeasonsRepository seasonsRepository, IAdsService adsService)
    {
        _seasonsRepository = seasonsRepository;
        _adsService = adsService;
    }

    public int CalculateSalesPrediction(int productId, int countDays)
    {
        return CalculateSalesPrediction(productId, countDays, DateOnly.FromDateTime(DateTime.UtcNow));
    }

    public int CalculateSalesPrediction(int productId, int countDays, DateOnly date)
    {
        var productSeason = _seasonsRepository.GetSeasonsDataById(productId);

        var ads = _adsService.CalculateAds(productId);

        var salesPrediction = 0.0;

        for (var i = 0; i < countDays; ++i)
        {
            var currentMonth = date.AddDays(i).Month;

            try
            {
                var currentCoefficient = productSeason
                    .Where(x => x.Month == currentMonth)
                    .Select(x => x.Coefficient)
                    .Single();

                salesPrediction += ads * currentCoefficient;
            }
            catch
            {
                throw new NotFoundException(string.Format(ErrorMessages.SeasonsNotFound, productId, currentMonth));
            }
        }

        return (int)Math.Ceiling(salesPrediction);
    }
}