using DataAccess.Exceptions;
using DataAccess.Models.Products;
using DataAccess.Repositories;
using FluentAssertions;

namespace WarehouseServiceUnitTest.Tests;

public class ProductRepositoryTests
{
    [Theory]
    [MemberData(nameof(ValidProductsDalForAdd))]
    public void Add_Product_ShouldAddProductReturnProductId(ProductDal product, bool isProductAdded)
    {
        var repository = new ProductRepository();

        repository.Add(product);

        var response = repository.Get(product.Id);

        Assert.NotNull(response);

        Assert.Equal
        (
            isProductAdded,
            response.Id == product.Id &&
            response.Name == product.Name &&
            response.Price == product.Price &&
            response.Type == product.Type &&
            response.Weight == product.Weight &&
            response.CreationDate == product.CreationDate &&
            response.WarehouseId == product.WarehouseId
        );
    }

    public static IEnumerable<object[]> ValidProductsDalForAdd()
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
                ),
                WarehouseId = 10
            },
            true
        };

        yield return new object[]
        {
            new ProductDal()
            {
                Id = 0,
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
                ),
                WarehouseId = 15
            },
            true
        };

        yield return new object[]
        {
            new ProductDal()
            {
                Id = 0,
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
                ),
                WarehouseId = 10
            },
            true
        };
    }
    
    [Fact]
    public void Get_ProductNotExisted_ShouldThrowNotFoundException()
    {
        var repository = new ProductRepository();

        Assert.Throws<NotFoundException>(() => repository.Get(0));
    }

    [Theory]
    [MemberData(nameof(ValidProductsDalForGet))]
    public void Get_ValidProductId_ShouldReturnProductDal(ProductDal product, int productId)
    {
        var repository = new ProductRepository();

        repository.Add(product);

        var response = repository.Get(productId);

        Assert.NotNull(response);

        response.Id.Should().Be(product.Id);
        response.Name.Should().Be(product.Name);
        response.Price.Should().Be(product.Price);
        response.Type.Should().Be(product.Type);
        response.Weight.Should().Be(product.Weight);
        response.CreationDate.Should().Be(product.CreationDate);
        response.WarehouseId.Should().Be(product.WarehouseId);
    }

    public static IEnumerable<object[]> ValidProductsDalForGet()
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
            },
            0
        };

        yield return new object[]
        {
            new ProductDal()
            {
                Id = 0,
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
            },
            0
        };

        yield return new object[]
        {
            new ProductDal()
            {
                Id = 0,
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
            },
            0
        };
    }

    [Theory]
    [MemberData(nameof(ValidProductsDalForUpdate))]
    public void Update_Product_ShouldUpdateProductDal
    (
        ProductDal product,
        ProductDal updatedProduct,
        bool isUpdatedProduct
    )
    {
        var repository = new ProductRepository();

        repository.Add(product);

        var currentPrice = repository.Get(product.Id).Price;

        repository.Update(updatedProduct);

        var updatedPrice = repository.Get(product.Id).Price;

        var actual = currentPrice != updatedPrice;

        Assert.Equal(isUpdatedProduct, actual);
    }

    public static IEnumerable<object[]> ValidProductsDalForUpdate()
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
                ),
                WarehouseId = 10
            },
            new ProductDal()
            {
                Id = 0,
                Name = "Apple",
                Price = 1200m,
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
                ),
                WarehouseId = 10
            },
            true
        };

        yield return new object[]
        {
            new ProductDal()
            {
                Id = 0,
                Name = "Apple",
                Price = 1200m,
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
                ),
                WarehouseId = 10
            },
            new ProductDal()
            {
                Id = 0,
                Name = "Apple",
                Price = 1200m,
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
                ),
                WarehouseId = 10
            },
            false
        };

        yield return new object[]
        {
            new ProductDal()
            {
                Id = 0,
                Name = "Apple",
                Price = 1200m,
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
                ),
                WarehouseId = 10
            },
            new ProductDal()
            {
                Id = 0,
                Name = "Apple",
                Price = 1400m,
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
                ),
                WarehouseId = 10
            },
            true
        };
    }
}