using DataAccess.Contracts;
using Domain.Constants;
using Domain.Contracts;
using Domain.Exceptions;

namespace Domain.Services;

public class AdsService : IAdsService
{
    private readonly ISalesRepository _salesRepository;

    public AdsService(ISalesRepository salesRepository)
    {
        _salesRepository = salesRepository;
    }

    public double CalculateAds(int productId)
    {
        var salesDataById = _salesRepository.GetSalesDataById(productId)
            .Where(x => x.Stock > 0)
            .ToList();

        if (salesDataById.Count == 0)
        {
            throw new NotFoundException(string.Format(ErrorMessages.SalesNotFound, productId));
        }

        double totalSales = salesDataById.Sum(x => x.Sales);

        var daysInStock = salesDataById.Count;

        return totalSales / daysInStock;
    }
}