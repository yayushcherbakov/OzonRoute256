namespace Domain.Models;

public sealed record Product
(
    int Id,
    string Name,
    decimal Price,
    int Weight,
    ProductType Type,
    DateTime CreationDate,
    int WarehouseId
);