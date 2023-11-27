namespace Domain.Contracts;

public interface IDemandService
{
    int CalculateDemand(int productId, int countDays);
    
    int CalculateDemand(int productId, int countDays, DateOnly date);
}