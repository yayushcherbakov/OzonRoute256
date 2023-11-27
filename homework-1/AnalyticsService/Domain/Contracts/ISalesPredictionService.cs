namespace Domain.Contracts;

public interface ISalesPredictionService
{
    int CalculateSalesPrediction(int productId, int countDays);

    int CalculateSalesPrediction(int productId, int countDays, DateOnly date);
}