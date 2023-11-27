using DataAccess.Entities;

namespace DataAccess.Contracts;

public interface ISalesRepository
{
    public List<Sale> GetSalesDataById(int productId);
}