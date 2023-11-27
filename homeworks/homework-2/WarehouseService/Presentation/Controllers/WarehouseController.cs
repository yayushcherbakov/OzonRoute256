using Domain.Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace WarehouseService.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost("CreateProduct")]
    public int CreateProduct(Product product)
    {
        return _warehouseService.CreateProduct(product);
    }

    [HttpGet("GetProductById")]
    public Product GetProductById(int productId)
    {
        return _warehouseService.GetProductById(productId);
    }

    
    [HttpGet("GetProducts")]
    public GetProductsResult GetProducts([FromQuery] ProductQuery productQuery)
    {
        return _warehouseService.GetProducts(productQuery);
    }

    [HttpPut("UpdateProductPrice")]
    public void UpdateProductPrice(int productId, decimal newPrice)
    {
        _warehouseService.UpdateProductPrice(productId, newPrice);
    }
}