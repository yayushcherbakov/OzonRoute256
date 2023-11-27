namespace Domain.Models;

public sealed record ProductQuery
(
    int PageNumber,
    int PageSize,
    ProductType? Type,
    DateTime? CreationDate,
    int? WarehouseId
);