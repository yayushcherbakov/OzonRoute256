using System.Collections.Concurrent;
using DataAccess.Constants;
using DataAccess.Contracts;
using DataAccess.Exceptions;
using DataAccess.Models.Products;

namespace DataAccess.Repositories;

internal class ProductRepository : IProductRepository
{
    private readonly ConcurrentBag<ProductDal> _products;

    public ProductRepository()
    {
        _products = new ConcurrentBag<ProductDal>();
    }

    public int Add(ProductDal productDal)
    {
        productDal = new ProductDal()
        {
            Id = _products.Count,
            Name = productDal.Name,
            Price = productDal.Price,
            Weight = productDal.Weight,
            Type = productDal.Type,
            CreationDate = productDal.CreationDate,
            WarehouseId = productDal.WarehouseId
        };


        _products.Add(productDal);

        return productDal.Id;
    }

    public ProductDal Get(int productId)
    {
        var product = _products.SingleOrDefault(x => x.Id == productId);

        if (product is null)
        {
            throw new NotFoundException
            (
                string.Format
                (
                    ErrorMessages.ProductNotFount,
                    productId
                )
            );
        }

        return product;
    }

    public GetProductsResultDal Get(ProductQueryDal payload)
    {
        var total = _products.Count;

        IEnumerable<ProductDal> productsEnumerable = _products;

        if (payload.CreationDate is not null)
        {
            productsEnumerable = productsEnumerable.Where(x => x.CreationDate == payload.CreationDate);
        }

        if (payload.Type is not null)
        {
            productsEnumerable = productsEnumerable.Where(x => x.Type == payload.Type);
        }

        if (payload.WarehouseId is not null)
        {
            productsEnumerable = productsEnumerable.Where(x => x.WarehouseId == payload.WarehouseId);
        }

        var products = productsEnumerable
            .Skip((payload.PageNumber - 1) * payload.PageSize)
            .Take(payload.PageSize)
            .ToList();

        return new GetProductsResultDal(total, products);
    }

    public void Update(ProductDal productDal)
    {
        var oldProduct = Get(productDal.Id);

        oldProduct.Name = productDal.Name;
        oldProduct.Price = productDal.Price;
        oldProduct.Type = productDal.Type;
        oldProduct.Weight = productDal.Weight;
        oldProduct.CreationDate = productDal.CreationDate;
        oldProduct.WarehouseId = productDal.WarehouseId;
    }
}