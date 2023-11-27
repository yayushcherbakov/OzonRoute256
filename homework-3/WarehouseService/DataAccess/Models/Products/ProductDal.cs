namespace DataAccess.Models.Products;

public sealed class ProductDal
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Weight { get; set; }
    public ProductTypeDal Type { get; set; }
    public DateTime CreationDate { get; set; }
    public int WarehouseId { get; set; }
}