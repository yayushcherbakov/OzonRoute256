using DataAccess.Models.Products;
using Domain.Models;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using WarehouseGrpc;
using WarehouseServiceUnitTest.Helpers;

namespace WarehouseServiceUnitTest.Tests;

public class MappingTests
{
    [Theory]
    [MemberData(nameof(ValidProducts))]
    public void Map_Product_ShouldReturnProductDal(Product product)
    {
        var mapper = MapperHelper.GetMapper();

        var productDal = mapper.Map<ProductDal>(product);

        productDal.Should().NotBeNull();

        productDal.Id.Should().Be(product.Id);
        productDal.Name.Should().Be(product.Name);
        productDal.Price.Should().Be(product.Price);
        productDal.Type.Should().Be((ProductTypeDal)product.Type);
        productDal.Weight.Should().Be(product.Weight);
        productDal.CreationDate.Should().Be(product.CreationDate);
        productDal.WarehouseId.Should().Be(product.WarehouseId);
    }

    public static IEnumerable<object[]> ValidProducts()
    {
        yield return new object[]
        {
            new Product
            (
                0,
                "Apple",
                999.99m,
                50,
                ProductType.Groceries,
                new DateTime
                    (
                        2008,
                        5,
                        1,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime(),
                10
            )
        };

        yield return new object[]
        {
            new Product
            (
                1,
                "Milk",
                25m,
                1000,
                ProductType.Groceries,
                new DateTime
                    (
                        2023,
                        5,
                        10,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime(),
                15
            )
        };

        yield return new object[]
        {
            new Product
            (
                3,
                "soap",
                3m,
                35,
                ProductType.HouseholdChemicals,
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
            )
        };
    }

    [Theory]
    [MemberData(nameof(ValidProductsDal))]
    public void Map_ProductDal_ShouldReturnProduct(ProductDal productDal)
    {
        var mapper = MapperHelper.GetMapper();

        var product = mapper.Map<Product>(productDal);

        product.Should().NotBeNull();

        product.Id.Should().Be(productDal.Id);
        product.Name.Should().Be(productDal.Name);
        product.Price.Should().Be(productDal.Price);
        product.Type.Should().Be((ProductType)productDal.Type);
        product.Weight.Should().Be(productDal.Weight);
        product.CreationDate.Should().Be(productDal.CreationDate);
        product.WarehouseId.Should().Be(productDal.WarehouseId);
    }

    public static IEnumerable<object[]> ValidProductsDal()
    {
        yield return new object[]
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
        };

        yield return new object[]
        {
            new ProductDal()
            {
                Id = 1,
                Name = "Milk",
                Price = 25m,
                Type = ProductTypeDal.Groceries,
                Weight = 1000,
                CreationDate = new DateTime
                    (
                        2023,
                        5,
                        10,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime(),
                WarehouseId = 15
            }
        };

        yield return new object[]
        {
            new ProductDal()
            {
                Id = 3,
                Name = "soap",
                Price = 3m,
                Type = ProductTypeDal.HouseholdChemicals,
                Weight = 35,
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
            }
        };
    }

    [Theory]
    [MemberData(nameof(ValidProductQueries))]
    public void Map_ProductQuery_ShouldReturnProductQueryDal(ProductQuery productQuery)
    {
        var mapper = MapperHelper.GetMapper();

        var productQueryDal = mapper.Map<ProductQueryDal>(productQuery);

        productQueryDal.Should().NotBeNull();

        productQueryDal.PageNumber.Should().Be(productQuery.PageNumber);
        productQueryDal.PageSize.Should().Be(productQuery.PageSize);
        productQueryDal.Type.Should().Be((ProductTypeDal?)productQuery.Type);
        productQueryDal.CreationDate.Should().Be(productQuery.CreationDate);
        productQueryDal.WarehouseId.Should().Be(productQuery.WarehouseId);
    }

    public static IEnumerable<object[]> ValidProductQueries()
    {
        yield return new object[]
        {
            new ProductQuery
            (
                1,
                5,
                ProductType.General,
                new DateTime
                    (
                        2008,
                        5,
                        1,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime(),
                25
            )
        };

        yield return new object[]
        {
            new ProductQuery
            (
                3,
                10,
                ProductType.Technique,
                null,
                25
            )
        };

        yield return new object[]
        {
            new ProductQuery
            (
                1,
                5,
                null,
                null,
                null
            )
        };
    }

    [Theory]
    [MemberData(nameof(ValidGetProductsResultDal))]
    public void Map_GetProductsResultDal_ShouldReturnGetProductsResult(GetProductsResultDal getProductsResultDal)
    {
        var mapper = MapperHelper.GetMapper();

        var productsResult = mapper.Map<GetProductsResult>(getProductsResultDal);

        productsResult.Should().NotBeNull();

        productsResult.Total.Should().Be(getProductsResultDal.Total);

        productsResult.Products.Count.Should().Be(getProductsResultDal.Products.Count);

        foreach (var product in productsResult.Products)
        {
            var dalProduct = getProductsResultDal.Products.SingleOrDefault(x => x.Id == product.Id);

            dalProduct.Should().NotBeNull();

            dalProduct?.Id.Should().Be(product.Id);
            dalProduct?.Name.Should().Be(product.Name);
            dalProduct?.Price.Should().Be(product.Price);
            dalProduct?.Type.Should().Be((ProductTypeDal)product.Type);
            dalProduct?.Weight.Should().Be(product.Weight);
            dalProduct?.CreationDate.Should().Be(product.CreationDate);
            dalProduct?.WarehouseId.Should().Be(product.WarehouseId);
        }
    }

    public static IEnumerable<object[]> ValidGetProductsResultDal()
    {
        yield return new object[]
        {
            new GetProductsResultDal
            (
                0,
                new List<ProductDal>()
            )
        };

        yield return new object[]
        {
            new GetProductsResultDal
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
                }
            )
        };

        yield return new object[]
        {
            new GetProductsResultDal
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
                    },

                    new ProductDal()
                    {
                        Id = 1,
                        Name = "Milk",
                        Price = 25m,
                        Type = ProductTypeDal.Groceries,
                        Weight = 1000,
                        CreationDate = new DateTime
                            (
                                2023,
                                5,
                                10,
                                8,
                                30,
                                52
                            )
                            .ToUniversalTime(),
                        WarehouseId = 15
                    }
                }
            )
        };
    }

    [Theory]
    [MemberData(nameof(ValidCreateProductRequests))]
    public void Map_CreateProductRequest_ShouldReturnProduct(CreateProductRequest createProductRequest)
    {
        var mapper = MapperHelper.GetMapper();

        var product = mapper.Map<Product>(createProductRequest);

        product.Should().NotBeNull();

        product.Id.Should().Be(default);
        product.Name.Should().Be(createProductRequest.Name);
        product.Price.Should().Be((decimal)createProductRequest.Price);
        product.Type.Should().Be((ProductType)createProductRequest.Type);
        product.Weight.Should().Be(createProductRequest.Weight);
        product.CreationDate.Should().Be(createProductRequest.CreationDate.ToDateTime());
        product.WarehouseId.Should().Be(createProductRequest.WarehouseId);
    }

    public static IEnumerable<object[]> ValidCreateProductRequests()
    {
        yield return new object[]
        {
            new CreateProductRequest()
            {
                Name = "Apple",
                Price = 999.99,
                Weight = 50,
                Type = GrpcProductType.Groceries,
                CreationDate = new DateTime
                    (
                        2008,
                        5,
                        1,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime().ToTimestamp(),
                WarehouseId = 10
            }
        };

        yield return new object[]
        {
            new CreateProductRequest()
            {
                Name = "Milk",
                Price = 25,
                Weight = 1000,
                Type = GrpcProductType.Groceries,
                CreationDate = new DateTime
                    (
                        2023,
                        5,
                        10,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime().ToTimestamp(),
                WarehouseId = 15
            }
        };

        yield return new object[]
        {
            new CreateProductRequest()
            {
                Name = "soap",
                Price = 3,
                Weight = 35,
                Type = GrpcProductType.HouseholdChemicals,
                CreationDate = new DateTime
                    (
                        2022,
                        5,
                        1,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime().ToTimestamp(),
                WarehouseId = 10
            }
        };
    }

    [Theory]
    [MemberData(nameof(ValidProductsForGetProductByIdResponses))]
    public void Map_Product_ShouldReturnGetProductByIdResponse(Product product)
    {
        var mapper = MapperHelper.GetMapper();

        var getProductByIdResponse = mapper.Map<GetProductByIdResponse>(product);

        getProductByIdResponse.Should().NotBeNull();

        getProductByIdResponse.Id.Should().Be(product.Id);
        getProductByIdResponse.Name.Should().Be(product.Name);
        getProductByIdResponse.Price.Should().Be((double)product.Price);
        getProductByIdResponse.Type.Should().Be((GrpcProductType)product.Type);
        getProductByIdResponse.Weight.Should().Be(product.Weight);
        getProductByIdResponse.CreationDate.Should().Be(Timestamp.FromDateTime(product.CreationDate));
        getProductByIdResponse.WarehouseId.Should().Be(product.WarehouseId);
    }

    public static IEnumerable<object[]> ValidProductsForGetProductByIdResponses()
    {
        yield return new object[]
        {
            new Product
            (
                0,
                "Apple",
                999.99m,
                50,
                ProductType.Groceries,
                new DateTime
                    (
                        2008,
                        5,
                        1,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime(),
                10
            )
        };

        yield return new object[]
        {
            new Product
            (
                1,
                "Milk",
                25m,
                1000,
                ProductType.Groceries,
                new DateTime
                    (
                        2023,
                        5,
                        10,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime(),
                15
            )
        };

        yield return new object[]
        {
            new Product
            (
                3,
                "soap",
                3m,
                35,
                ProductType.HouseholdChemicals,
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
            )
        };
    }

    [Theory]
    [MemberData(nameof(ValidGetProductsRequests))]
    public void Map_GetProductsRequest_ShouldReturnProductQuery(GetProductsRequest getProductsRequest)
    {
        var mapper = MapperHelper.GetMapper();

        var productQuery = mapper.Map<ProductQuery>(getProductsRequest);

        productQuery.Should().NotBeNull();

        productQuery.PageNumber.Should().Be(getProductsRequest.PageNumber);
        productQuery.PageSize.Should().Be(getProductsRequest.PageSize);
        productQuery.Type.Should().Be
        (
            getProductsRequest.Type.HasValue
                ? (ProductType?)getProductsRequest.Type.Value
                : null
        );
        productQuery.CreationDate.Should().Be
        (
            getProductsRequest.CreationDate.HasValue
                ? getProductsRequest.CreationDate.Value.ToDateTime().ToUniversalTime()
                : null
        );
        productQuery.WarehouseId.Should().Be(getProductsRequest.WarehouseId);
    }

    public static IEnumerable<object[]> ValidGetProductsRequests()
    {
        yield return new object[]
        {
            new GetProductsRequest()
            {
                PageNumber = 1,
                PageSize = 1,
                Type = new NullableGrpcProductType()
                {
                    HasValue = false,
                    Value = GrpcProductType.General
                },
                CreationDate = new NullableDateTime()
                {
                    HasValue = false,
                    Value = new DateTime
                        (
                            2008,
                            5,
                            1,
                            8,
                            30,
                            52
                        )
                        .ToUniversalTime().ToTimestamp(),
                },
                WarehouseId = null
            }
        };

        yield return new object[]
        {
            new GetProductsRequest()
            {
                PageNumber = 2,
                PageSize = 2,
                Type = new NullableGrpcProductType()
                {
                    HasValue = false,
                    Value = GrpcProductType.General
                },
                CreationDate = new NullableDateTime()
                {
                    HasValue = false,
                    Value = new DateTime
                        (
                            2008,
                            5,
                            1,
                            8,
                            30,
                            52
                        )
                        .ToUniversalTime().ToTimestamp(),
                },
                WarehouseId = null
            }
        };

        yield return new object[]
        {
            new GetProductsRequest()
            {
                PageNumber = 3,
                PageSize = 5,
                Type = new NullableGrpcProductType()
                {
                    HasValue = true,
                    Value = GrpcProductType.General
                },
                CreationDate = new NullableDateTime()
                {
                    HasValue = true,
                    Value = new DateTime
                        (
                            2008,
                            5,
                            1,
                            8,
                            30,
                            52
                        )
                        .ToUniversalTime().ToTimestamp(),
                },
                WarehouseId = 5
            }
        };
    }

    [Theory]
    [MemberData(nameof(ValidProductsForGrpc))]
    public void Map_Product_ShouldReturnGrpcProduct(Product product)
    {
        var mapper = MapperHelper.GetMapper();

        var grpcProduct = mapper.Map<GrpcProduct>(product);

        grpcProduct.Should().NotBeNull();

        grpcProduct.Id.Should().Be(product.Id);
        grpcProduct.Name.Should().Be(product.Name);
        grpcProduct.Price.Should().Be((double)product.Price);
        grpcProduct.Type.Should().Be((GrpcProductType)product.Type);
        grpcProduct.Weight.Should().Be(product.Weight);
        grpcProduct.CreationDate.Should()
            .Be(Timestamp.FromDateTime(product.CreationDate));
        grpcProduct.WarehouseId.Should().Be(product.WarehouseId);
    }

    public static IEnumerable<object[]> ValidProductsForGrpc()
    {
        yield return new object[]
        {
            new Product
            (
                0,
                "Apple",
                999.99m,
                50,
                ProductType.Groceries,
                new DateTime
                    (
                        2008,
                        5,
                        1,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime(),
                10
            )
        };

        yield return new object[]
        {
            new Product
            (
                1,
                "Milk",
                25m,
                1000,
                ProductType.Groceries,
                new DateTime
                    (
                        2023,
                        5,
                        10,
                        8,
                        30,
                        52
                    )
                    .ToUniversalTime(),
                15
            )
        };

        yield return new object[]
        {
            new Product
            (
                3,
                "soap",
                3m,
                35,
                ProductType.HouseholdChemicals,
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
            )
        };
    }

    [Theory]
    [MemberData(nameof(ValidGetProductResults))]
    public void Map_GetProductsResult_ShouldReturnGetProductsResponse(GetProductsResult getProductsResult)
    {
        var mapper = MapperHelper.GetMapper();

        var productsResponse = mapper.Map<GetProductsResponse>(getProductsResult);

        productsResponse.Should().NotBeNull();

        productsResponse.Total.Should().Be(getProductsResult.Total);

        productsResponse.Products.Count.Should().Be(getProductsResult.Products.Count);

        foreach (var item in productsResponse.Products)
        {
            var product = getProductsResult.Products.SingleOrDefault(x => x.Id == item.Id);

            product.Should().NotBeNull();

            product?.Id.Should().Be(item.Id);
            product?.Name.Should().Be(item.Name);
            product?.Price.Should().Be((decimal)item.Price);
            product?.Type.Should().Be((ProductType?)item.Type);
            product?.Weight.Should().Be(item.Weight);
            product?.CreationDate.Should().Be(item.CreationDate.ToDateTime());
            product?.WarehouseId.Should().Be(item.WarehouseId);
        }
    }

    public static IEnumerable<object[]> ValidGetProductResults()
    {
        yield return new object[]
        {
            new GetProductsResult
            (
                0,
                new List<Product>()
            )
        };

        yield return new object[]
        {
            new GetProductsResult
            (
                1,
                new List<Product>()
                {
                    new Product
                    (
                        0,
                        "Apple",
                        999.99m,
                        50,
                        ProductType.Technique,
                        new DateTime
                            (
                                2008,
                                5,
                                1,
                                8,
                                30,
                                52
                            )
                            .ToUniversalTime(),
                        10
                    )
                }
            )
        };

        yield return new object[]
        {
            new GetProductsResult
            (
                2,
                new List<Product>()
                {
                    new Product
                    (
                        0,
                        "Apple",
                        999.99m,
                        50,
                        ProductType.Technique,
                        new DateTime
                            (
                                2008,
                                5,
                                1,
                                8,
                                30,
                                52
                            )
                            .ToUniversalTime(),
                        10
                    ),

                    new Product
                    (
                        1,
                        "Milk",
                        25m,
                        1000,
                        ProductType.Groceries,
                        new DateTime
                            (
                                2023,
                                5,
                                10,
                                8,
                                30,
                                52
                            )
                            .ToUniversalTime(),
                        15
                    )
                }
            )
        };
    }
}