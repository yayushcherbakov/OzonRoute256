namespace DataAccess.Models.Products;

public sealed class ProductQueryDal
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public ProductTypeDal? Type { get; set; }
    public DateTime? CreationDate { get; set; }
    public int? WarehouseId { get; set; }
}