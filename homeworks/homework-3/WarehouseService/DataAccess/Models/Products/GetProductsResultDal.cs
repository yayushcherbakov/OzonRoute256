namespace DataAccess.Models.Products;

public sealed record GetProductsResultDal
(
    int Total,
    List<ProductDal> Products
);