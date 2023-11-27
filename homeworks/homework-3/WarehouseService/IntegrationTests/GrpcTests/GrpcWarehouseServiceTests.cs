using DataAccess.Contracts;
using DataAccess.Models.Products;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using WarehouseGrpc;

namespace WarehouseServiceIntegrationTest.GrpcTests;

public class GrpcWarehouseServiceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private readonly Mock<IProductRepository> _repositoryMock;

    public GrpcWarehouseServiceTests(WebApplicationFactory<Program> factory)
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
    public void CreateProduct_ValidProduct_ShouldAddProductAndReturnProductId()
    {
        _repositoryMock
            .Setup(x => x.Add(It.Is<ProductDal>(y => y.Id == 0)))
            .Returns(0);

        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        var response = grpcClient.CreateProduct(new CreateProductRequest()
        {
            Name = "Cola",
            Price = 25,
            Weight = 2,
            Type = GrpcProductType.Groceries,
            CreationDate = Timestamp.FromDateTime
            (
                new DateTime
                    (
                        2022,
                        5,
                        1,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime()
            ),
            WarehouseId = 10
        });

        response.Should().NotBeNull();
        response.ProductId.Should().Be(0);
    }

    [Fact]
    public void CreateProduct_InvalidProductName_ShouldThrowRpcException()
    {
        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        var requestModel = new CreateProductRequest()
        {
            Name = "Co",
            Price = 25,
            Weight = 2,
            Type = GrpcProductType.Groceries,
            CreationDate = Timestamp.FromDateTime
            (
                new DateTime
                    (
                        2022,
                        5,
                        1,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime()
            ),
            WarehouseId = 10
        };

        Assert.Throws<Grpc.Core.RpcException>(() => grpcClient.CreateProduct(requestModel));
    }

    [Fact]
    public void CreateProduct_NegativeProductPrice_ShouldThrowRpcException()
    {
        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        var requestModel = new CreateProductRequest()
        {
            Name = "Co",
            Price = -25,
            Weight = 2,
            Type = GrpcProductType.Groceries,
            CreationDate = Timestamp.FromDateTime
            (
                new DateTime
                    (
                        2022,
                        5,
                        1,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime()
            ),
            WarehouseId = 10
        };

        Assert.Throws<Grpc.Core.RpcException>(() => grpcClient.CreateProduct(requestModel));
    }

    [Fact]
    public void GetProductById_ValidProductId_ShouldReturnProduct()
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

        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        var getProductByIdResponse = grpcClient.GetProductById(new GetProductByIdRequest()
        {
            ProductId = 0
        });

        getProductByIdResponse.Should().NotBeNull();
        getProductByIdResponse.Id.Should().Be(0);
    }

    [Fact]
    public void GetProductById_NegativeProductId_ShouldThrowRpcException()
    {
        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        var requestModel = new GetProductByIdRequest()
        {
            ProductId = -1
        };

        Assert.Throws<Grpc.Core.RpcException>(() => grpcClient.GetProductById(requestModel));
    }

    [Fact]
    public void GetProductById_NotExistedProductId_ShouldThrowRpcException()
    {
        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        var requestModel = new GetProductByIdRequest()
        {
            ProductId = 999
        };

        Assert.Throws<Grpc.Core.RpcException>(() => grpcClient.GetProductById(requestModel));
    }

    [Fact]
    public void GetProducts_ProductQueryWithoutAdditionalParameters_ShouldReturnProductResult()
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

        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        var getProductsResponse = grpcClient.GetProducts(new GetProductsRequest()
        {
            PageNumber = 1,
            PageSize = 2,
            Type = null,
            CreationDate = null,
            WarehouseId = null
        });

        getProductsResponse.Should().NotBeNull();
        getProductsResponse.Total.Should().Be(1);
        getProductsResponse.Products.Count.Should().Be(1);
    }

    [Fact]
    public void GetProducts_ProductQueryWithProductTypeParameter_ShouldReturnProductResult()
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

        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        var getProductsResponse = grpcClient.GetProducts(new GetProductsRequest()
        {
            PageNumber = 1,
            PageSize = 2,
            Type = new NullableGrpcProductType()
            {
                HasValue = true,
                Value = GrpcProductType.Technique
            },
            CreationDate = null,
            WarehouseId = null
        });

        getProductsResponse.Should().NotBeNull();
        getProductsResponse.Total.Should().Be(1);
        getProductsResponse.Products.Count.Should().Be(1);
    }

    [Fact]
    public void GetProducts_ProductQueryWithWarehouseIdParameter_ShouldReturnProductResult()
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

        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        var getProductsResponse = grpcClient.GetProducts(new GetProductsRequest()
        {
            PageNumber = 1,
            PageSize = 2,
            Type = new NullableGrpcProductType()
            {
                HasValue = true,
                Value = GrpcProductType.Technique
            },
            CreationDate = null,
            WarehouseId = 10
        });

        getProductsResponse.Should().NotBeNull();
        getProductsResponse.Total.Should().Be(1);
        getProductsResponse.Products.Count.Should().Be(1);
    }

    [Fact]
    public void UpdateProductPrice_ValidProductId_ShouldReturnSuccessStatusCode()
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


        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        grpcClient.UpdateProductPrice(new UpdateProductPriceRequest()
        {
            ProductId = 1,
            NewPrice = 55
        });

        _repositoryMock.Verify(x => x.Update(productDal));
    }

    [Fact]
    public void UpdateProductPrice_NotExistedProductId_ShouldThrowRpcException()
    {
        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        var request = new UpdateProductPriceRequest()
        {
            ProductId = 99999,
            NewPrice = 55
        };

        Assert.Throws<Grpc.Core.RpcException>(() => grpcClient.UpdateProductPrice(request));
    }

    [Fact]
    public void UpdateProductPrice_NegativeProductPrice_ShouldThrowRpcException()
    {
        var clientWebApp = _factory.CreateClient();

        var channel = GrpcChannel.ForAddress
        (
            clientWebApp.BaseAddress!,
            new GrpcChannelOptions()
            {
                HttpClient = clientWebApp
            }
        );

        var grpcClient = new ProtoWarehouseService.ProtoWarehouseServiceClient(channel);

        var request = new UpdateProductPriceRequest()
        {
            ProductId = 1,
            NewPrice = -777
        };

        Assert.Throws<Grpc.Core.RpcException>(() => grpcClient.UpdateProductPrice(request));
    }
}