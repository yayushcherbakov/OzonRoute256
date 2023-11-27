using Domain.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class DemandController : ControllerBase
{
    private readonly IDemandService _demandService;

    public DemandController(IDemandService demandService)
    {
        _demandService = demandService;
    }

    [HttpGet("CalculateDemand")]
    public double CalculateDemand(int productId, int countDays)
    {
        return _demandService.CalculateDemand(productId, countDays);
    }

    [HttpGet("CalculateDemandByDate")]
    public double CalculateDemandByDate(int productId, int countDays, DateOnly date)
    {
        return _demandService.CalculateDemand(productId, countDays, date);
    }
}