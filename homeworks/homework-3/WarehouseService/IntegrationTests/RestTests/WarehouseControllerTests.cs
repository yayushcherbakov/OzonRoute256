using System.Net;
using System.Text;
using System.Text.Json;
using DataAccess.Contracts;
using DataAccess.Exceptions;
using DataAccess.Models.Products;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace WarehouseServiceIntegrationTest.RestTests;

public class WarehouseControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    private readonly Mock<IProductRepository> _repositoryMock;

    public WarehouseControllerTests(WebApplicationFactory<Program> factory)
    {
        _repositoryMock = new Mock<IProductRepository>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.Replace(ServiceDescriptor.Singleton<IProductRepository>(_ => _repositoryMock.Object));
            });
        });
    }

    [Fact]
    public async Task CreateProduct_ValidProduct_ShouldAddProductAndReturnProductId()
    {
        _repositoryMock
            .Setup(x => x.Add(It.Is<ProductDal>(y => y.Id == 0)))
            .Returns(0);
        
        var httpClient = _factory.CreateClient();

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new Product
            (
                0,
                "Apple",
                999.99m,
                50,
                ProductType.Groceries,
                new DateTime(2008, 5, 1, 8, 30, 52).ToUniversalTime(),
                10
            )),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PostAsync("/Warehouse/CreateProduct", jsonContent);

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(jsonResponse);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CreateProduct_InvalidProductName_ShouldReturnBadRequest()
    {
        var httpClient = _factory.CreateClient();

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new Product
            (
                0,
                "Ap",
                999.99m,
                50,
                ProductType.Groceries,
                new DateTime(2008, 5, 1, 8, 30, 52).ToUniversalTime(),
                10
            )),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PostAsync("/Warehouse/CreateProduct", jsonContent);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_NegativeProductPrice_ShouldReturnBadRequest()
    {
        var httpClient = _factory.CreateClient();

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new Product
            (
                0,
                "Ap",
                -999.99m,
                50,
                ProductType.Groceries,
                new DateTime(2008, 5, 1, 8, 30, 52).ToUniversalTime(),
                10
            )),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PostAsync("/Warehouse/CreateProduct", jsonContent);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ValidProductId_ShouldReturnProduct()
    {
        _repositoryMock
            .Setup(x => x.Get(It.Is<int>(y => y == 0)))
            .Returns(new ProductDal()
            {
                Id = 0,
                Name = "soap",
                Price = 3m,
                Weight = 35,
                Type = ProductTypeDal.General,
                CreationDate = new DateTime
                    (
                        2022,
                        5,
                        1,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime(),
                WarehouseId = 10
            });
        
        var httpClient = _factory.CreateClient();

        var response = await httpClient.GetAsync("/Warehouse/GetProductById?productId=0");

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(jsonResponse);

        result?.Id.Should().Be(0);
    }

    [Fact]
    public async Task GetProductById_NegativeProductId_ShouldReturnInternalServerError()
    {
        _repositoryMock
            .Setup(x => x.Get(It.Is<int>(y => y == -1)))
            .Throws(() => new NotFoundException("Product not found"));
        
        var httpClient = _factory.CreateClient();

        var response = await httpClient.GetAsync("/Warehouse/GetProductById?productId=-1");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_NotExistedProductId_ShouldReturnInternalServerError()
    {
        _repositoryMock
            .Setup(x => x.Get(It.Is<int>(y => y == 777)))
            .Throws(() => new NotFoundException("Product not found"));
        
        var httpClient = _factory.CreateClient();

        var response = await httpClient.GetAsync("/Warehouse/GetProductById?productId=777");
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_ProductQueryWithoutAdditionalParameters_ShouldReturnProductResult()
    {
        _repositoryMock
            .Setup(x => x.Get(It.Is<ProductQueryDal>(y => y.PageNumber == 1)))
            .Returns(new GetProductsResultDal(1, new List<ProductDal>()
            {
                new ProductDal()
                {
                    Id = 0,
                    Name = "Apple",
                    Price = 999.99m,
                    Type = ProductTypeDal.Technique,
                    Weight = 50,
                    CreationDate = new DateTime
                        (
                            2008,
                            5,
                            1,
                            8,
                            30,
                            52
                        )
                        .ToUniversalTime(),
                    WarehouseId = 10
                }
            }));
        
        var httpClient = _factory.CreateClient();

        var response = await httpClient.GetAsync("/Warehouse/GetProducts?PageNumber=1&PageSize=1");

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<GetProductsResult>(jsonResponse);

        result?.Total.Should().Be(1);

        var product = result?.Products.Single();

        product?.Id.Should().Be(0);
    }

    [Fact]
    public async Task GetProducts_ProductQueryWithProductTypeParameter_ShouldReturnProductResult()
    {
        _repositoryMock
            .Setup(x => x.Get(It.Is<ProductQueryDal>(y => y.PageNumber == 1)))
            .Returns(new GetProductsResultDal(1, new List<ProductDal>()
            {
                new ProductDal()
                {
                    Id = 0,
                    Name = "Apple",
                    Price = 999.99m,
                    Type = ProductTypeDal.Technique,
                    Weight = 50,
                    CreationDate = new DateTime
                        (
                            2008,
                            5,
                            1,
                            8,
                            30,
                            52
                        )
                        .ToUniversalTime(),
                    WarehouseId = 10
                }
            }));
        
        var httpClient = _factory.CreateClient();

        var response =
            await httpClient.GetAsync("/Warehouse/GetProducts?PageNumber=1&PageSize=1&WarehouseId=10");

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<GetProductsResult>(jsonResponse);

        result?.Total.Should().Be(1);

        var product = result?.Products.Single();

        product?.Id.Should().Be(0);
    }

    [Fact]
    public async Task GetProducts_ProductQueryWithWarehouseIdParameter_ShouldReturnProductResult()
    {
        _repositoryMock
            .Setup(x => x.Get(It.Is<ProductQueryDal>(y => y.PageNumber == 1)))
            .Returns(new GetProductsResultDal(1, new List<ProductDal>()
            {
                new ProductDal()
                {
                    Id = 0,
                    Name = "Apple",
                    Price = 999.99m,
                    Type = ProductTypeDal.Technique,
                    Weight = 50,
                    CreationDate = new DateTime
                        (
                            2008,
                            5,
                            1,
                            8,
                            30,
                            52
                        )
                        .ToUniversalTime(),
                    WarehouseId = 10
                }
            }));
        
        var httpClient = _factory.CreateClient();

        var response =
            await httpClient.GetAsync("/Warehouse/GetProducts?PageNumber=1&PageSize=1&ProductType=Groceries");

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<GetProductsResult>(jsonResponse);

        result?.Total.Should().Be(1);

        var product = result?.Products.Single();

        product?.Id.Should().Be(0);
    }

    [Fact]
    public async Task UpdateProductPrice_ValidProductId_ShouldReturnSuccessStatusCode()
    {
        var productDal = new ProductDal()
        {
            Id = 1,
            Name = "soap",
            Price = 3m,
            Weight = 35,
            Type = ProductTypeDal.General,
            CreationDate = new DateTime
                (
                    2022,
                    5,
                    1,
                    8,
                    30,
                    52
                )
                .ToUniversalTime(),
            WarehouseId = 10
        };

        _repositoryMock
            .Setup(x => x.Get(It.Is<int>(y => y == 1)))
            .Returns(productDal);

        _repositoryMock
            .Setup(x => x.Update(It.Is<ProductDal>(y => y.Id == 1)));
        
        var httpClient = _factory.CreateClient();

        var response = await httpClient.PutAsync("/Warehouse/UpdateProductPrice?productId=1&newPrice=25", null);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateProductPrice_NotExistedProductId_ShouldReturnInternalServerError()
    {
        var httpClient = _factory.CreateClient();

        var response = await httpClient.PutAsync("/Warehouse/UpdateProductPrice?productId=999&newPrice=55", null);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProductPrice_NegativeProductId_ShouldReturnInternalServerError()
    {
        var httpClient = _factory.CreateClient();

        var response = await httpClient.PutAsync("/Warehouse/UpdateProductPrice?productId=-1&newPrice=55", null);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}