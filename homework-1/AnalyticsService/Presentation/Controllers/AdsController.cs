using Domain.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AdsController : ControllerBase
{
    private readonly IAdsService _adsService;

    public AdsController(IAdsService adsService)
    {
        _adsService = adsService;
    }

    [HttpGet("CalculateAds")]
    public double CalculateAds(int productId)
    {
        return _adsService.CalculateAds(productId);
    }
}