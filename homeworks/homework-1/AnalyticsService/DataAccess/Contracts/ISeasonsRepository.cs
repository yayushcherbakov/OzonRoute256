using DataAccess.Entities;

namespace DataAccess.Contracts;

public interface ISeasonsRepository
{
    public List<Season> GetSeasonsDataById(int productId);
}