using DemandCalculator.Constants;
using DemandCalculator.Contracts;
using DemandCalculator.Services;

namespace DemandCalculator;

public static class Program
{
    private static async Task Main()
    {
        IParallelDemandExecutor parallelDemandExecutor = new ParallelDemandExecutor();

        IConfigService configService = new ConfigService();

        var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;

        var config = configService.GetParallelConfig();

        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(config.CancelAfterInSeconds));

        var dataFolderPath = Path.Combine
        (
            AppDomain.CurrentDomain.BaseDirectory,
            PathConstants.DataDirectory
        );

        var inputDataPath = Path.Combine
        (
            dataFolderPath,
            PathConstants.InputDataFileName
        );

        var outputDataPath = Path.Combine
        (
            dataFolderPath,
            PathConstants.OutputDataFileName
        );

        await parallelDemandExecutor.Run
        (
            inputDataPath,
            outputDataPath,
            config.MaxThreadsCount,
            cancellationToken
        );
    }
}