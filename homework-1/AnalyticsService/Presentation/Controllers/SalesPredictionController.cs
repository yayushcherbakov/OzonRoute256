using Domain.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class SalesPredictionController : ControllerBase
{
    private readonly ISalesPredictionService _salesPredictionService;

    public SalesPredictionController(ISalesPredictionService salesPredictionService)
    {
        _salesPredictionService = salesPredictionService;
    }

    [HttpGet("CalculateSalesPrediction")]
    public int CalculateSalesPrediction(int productId, int countDays)
    {
        return _salesPredictionService.CalculateSalesPrediction(productId, countDays);
    }
}