namespace DataAccess.Entities;

public sealed record Sale(int ProductId, DateOnly Date, int Sales, int Stock);