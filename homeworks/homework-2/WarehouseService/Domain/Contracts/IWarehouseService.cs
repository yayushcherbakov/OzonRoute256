using Domain.Models;

namespace Domain.Contracts;

public interface IWarehouseService
{
    int CreateProduct(Product product);

    Product GetProductById(int productId);

    GetProductsResult GetProducts(ProductQuery productQueryDal);

    void UpdateProductPrice(int productId, decimal newPrice);
}