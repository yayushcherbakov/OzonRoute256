using DemandCalculator.Models;

namespace DemandCalculator.Contracts;

public interface IConfigService
{
    public ParallelConfig GetParallelConfig();
}