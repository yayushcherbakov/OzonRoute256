namespace DemandCalculator.Contracts;

public interface IParallelDemandExecutor
{
    public Task Run
    (
        string inputDataPath,
        string outputDataPath,
        int maxThreadsCount,
        CancellationToken cancellationToken
    );
}