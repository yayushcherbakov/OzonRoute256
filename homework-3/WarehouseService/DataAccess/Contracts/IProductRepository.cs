using DataAccess.Models.Products;

namespace DataAccess.Contracts;

public interface IProductRepository
{
    public int Add(ProductDal productDal);

    public ProductDal Get(int productId);

    public GetProductsResultDal Get(ProductQueryDal payload);

    public void Update(ProductDal productDal);
}