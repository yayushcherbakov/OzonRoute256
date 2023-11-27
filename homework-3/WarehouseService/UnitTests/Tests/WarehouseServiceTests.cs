using AutoMapper;
using DataAccess.Contracts;
using DataAccess.Exceptions;
using DataAccess.Models.Products;
using Domain.Models;
using FluentAssertions;
using Moq;
using WarehouseServiceUnitTest.Helpers;
using DateTime = System.DateTime;

namespace WarehouseServiceUnitTest.Tests;

public class WarehouseServiceTests
{
    private readonly IMapper _mapper;

    private readonly Mock<IProductRepository> _repositoryMock;

    public WarehouseServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();

        _mapper = MapperHelper.GetMapper();
    }

    [Theory]
    [MemberData(nameof(ProductTypes))]
    public void CreateProduct_Product_ShouldAddProductAndReturnProductId(
        ProductType productType)
    {
        _repositoryMock
            .Setup(x => x.Add(It.Is<ProductDal>(y => y.Id == 0)))
            .Returns(0);

        var service = new Domain.Services.WarehouseService(_repositoryMock.Object, _mapper);

        var product = new Product
        (
            0,
            "soap",
            3m,
            35,
            productType,
            new DateTime
                (
                    2022,
                    5,
                    1,
                    8,
                    30,
                    52
                )
                .ToUniversalTime(),
            10
        );

        var productId = service.CreateProduct(product);

        Assert.Equal(product.Id, productId);
    }

    public static IEnumerable<object[]> ProductTypes()
    {
        return Enum.GetValues<ProductType>().Select(x => new object[] { x });
    }

    [Fact]
    public void GetProductById_ProductId_ShouldReturnProduct()
    {
        _repositoryMock
            .Setup(x => x.Get(It.Is<int>(y => y == 0)))
            .Returns(new ProductDal()
            {
                Id = 0,
                Name = "soap",
                Price = 3m,
                Weight = 35,
                Type = ProductTypeDal.HouseholdChemicals,
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

        var service = new Domain.Services.WarehouseService(_repositoryMock.Object, _mapper);

        var product = service.GetProductById(0);

        Assert.NotNull(product);

        product.Id.Should().Be(0);
        product.Name.Should().Be("soap");
        product.Price.Should().Be(3m);
        product.Weight.Should().Be(35);
        product.Type.Should().Be(ProductType.HouseholdChemicals);
        product.CreationDate.Should().Be(new DateTime(2022, 5, 1, 8, 30, 52).ToUniversalTime());
        product.WarehouseId.Should().Be(10);
    }

    [Fact]
    public void GetProductById_NotExistedProductId_ShouldThrowNotFoundException()
    {
        _repositoryMock
            .Setup(x => x.Get(It.Is<int>(y => y == 2)))
            .Throws(() => new NotFoundException("NotFound"));

        var service = new Domain.Services.WarehouseService(_repositoryMock.Object, _mapper);

        Assert.Throws<NotFoundException>(() => service.GetProductById(2));
    }

    [Fact]
    public void GetProducts_ProductQueryWithNullableProperties_ShouldReturnProductResult()
    {
        _repositoryMock
            .Setup(x => x.Get(It.Is<ProductQueryDal>(y => y.PageNumber == 1 && y.PageSize == 1)))
            .Returns(new GetProductsResultDal
            (
                1,
                new List<ProductDal>()
                {
                    new ProductDal()
                    {
                        Id = 0,
                        Name = "Apple",
                        Price = 999.99m,
                        Type = ProductTypeDal.Technique,
                        Weight = 50,
                        CreationDate = new DateTime(2008, 5, 1, 8, 30, 52).ToUniversalTime(),
                        WarehouseId = 10
                    }
                }
            ));

        var service = new Domain.Services.WarehouseService(_repositoryMock.Object, _mapper);

        var getProductsResult = service.GetProducts
        (
            new ProductQuery
            (
                1,
                1,
                null,
                null,
                null
            )
        );

        Assert.NotNull(getProductsResult);

        Assert.Equal(1, getProductsResult.Total);
    }

    [Fact]
    public void GetProducts_ProductQueryWithSeveralNullableProperties_ShouldReturnProductResult()
    {
        _repositoryMock
            .Setup(x => x.Get(It.Is<ProductQueryDal>(y => y.PageNumber == 1 && y.PageSize == 2)))
            .Returns(new GetProductsResultDal
            (
                2,
                new List<ProductDal>()
                {
                    new ProductDal()
                    {
                        Id = 0,
                        Name = "Apple",
                        Price = 999.99m,
                        Type = ProductTypeDal.Technique,
                        Weight = 50,
                        CreationDate = new DateTime(2008, 5, 1, 8, 30, 52).ToUniversalTime(),
                        WarehouseId = 15
                    },
                    new ProductDal()
                    {
                        Id = 1,
                        Name = "Phone",
                        Price = 500m,
                        Type = ProductTypeDal.Technique,
                        Weight = 200,
                        CreationDate = new DateTime(2008, 5, 1, 8, 30, 52).ToUniversalTime(),
                        WarehouseId = 15
                    }
                }
            ));

        var service = new Domain.Services.WarehouseService(_repositoryMock.Object, _mapper);

        var getProductsResult = service.GetProducts
        (
            new ProductQuery
            (
                1,
                2,
                ProductType.Technique,
                null,
                15
            )
        );

        Assert.NotNull(getProductsResult);

        Assert.Equal(2, getProductsResult.Total);
    }

    [Fact]
    public void GetProducts_ProductQueryWithAllProperties_ShouldReturnProductResult()
    {
        _repositoryMock
            .Setup(x => x.Get(It.Is<ProductQueryDal>(y => y.PageNumber == 1 && y.PageSize == 2)))
            .Returns(new GetProductsResultDal
            (
                2,
                new List<ProductDal>()
                {
                    new ProductDal()
                    {
                        Id = 0,
                        Name = "Apple",
                        Price = 999.99m,
                        Type = ProductTypeDal.Technique,
                        Weight = 50,
                        CreationDate = new DateTime(2008, 5, 1, 8, 30, 52).ToUniversalTime(),
                        WarehouseId = 15
                    },
                    new ProductDal()
                    {
                        Id = 1,
                        Name = "Phone",
                        Price = 500m,
                        Type = ProductTypeDal.Technique,
                        Weight = 200,
                        CreationDate = new DateTime(2008, 5, 1, 8, 30, 52).ToUniversalTime(),
                        WarehouseId = 15
                    }
                }
            ));


        var service = new Domain.Services.WarehouseService(_repositoryMock.Object, _mapper);

        var getProductsResult = service.GetProducts
        (
            new ProductQuery
            (
                1,
                2,
                ProductType.Technique,
                new DateTime(2000, 5, 1, 8, 30, 52).ToUniversalTime(),
                15
            )
        );

        Assert.NotNull(getProductsResult);

        Assert.Equal(2, getProductsResult.Total);
    }

    [Theory]
    [MemberData(nameof(ProductPrices))]
    public void UpdateProductPrice_ValidPrice_ShouldUpdateProduct(decimal newPrice)
    {
        var productDal = new ProductDal()
        {
            Id = 0,
            Name = "soap",
            Price = 3m,
            Weight = 35,
            Type = ProductTypeDal.HouseholdChemicals,
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
            .Setup(x => x.Get(It.Is<int>(y => y == 0)))
            .Returns(productDal);

        var service = new Domain.Services.WarehouseService(_repositoryMock.Object, _mapper);

        service.UpdateProductPrice(0, newPrice);

        _repositoryMock.Verify(x => x.Update(productDal));
    }

    public static IEnumerable<object[]> ProductPrices()
    {
        yield return new object[] { 25m };

        yield return new object[] { 555m };

        yield return new object[] { 9999m };
    }
}