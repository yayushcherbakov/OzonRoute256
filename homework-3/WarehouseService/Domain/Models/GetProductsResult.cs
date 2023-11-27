namespace Domain.Models;

public sealed record GetProductsResult
(
    int Total,
    List<Product> Products
);