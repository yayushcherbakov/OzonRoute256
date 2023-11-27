using System.Text;
using DemandCalculator.Contracts;
using DemandCalculator.Models;
using DemandCalculator.Parsers;

namespace DemandCalculator.Services;

internal class ParallelDemandExecutor : IParallelDemandExecutor
{
    private StreamReader? _streamReader;

    private StreamWriter? _streamWriter;

    private readonly SemaphoreSlim _semaphoreSlimForReading = new(1, 1);

    private readonly SemaphoreSlim _semaphoreSlimForWriting = new(1, 1);

    private int _readCount;

    private int _calculatedCount;

    private int _writeCount;

    private StreamReader StreamReader => _streamReader ?? throw new NullReferenceException();

    private StreamWriter StreamWriter => _streamWriter ?? throw new NullReferenceException();

    private int Calculate(Statistics statistics, CancellationToken cancellationToken)
    {
        var result = 0;

        for (var i = 0; i < 1000000; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();

            result = (int)Math.Ceiling(statistics.Prediction - statistics.Stock);
        }

        Interlocked.Increment(ref _calculatedCount);

        return result > 0 ? result : 0;
    }

    private async Task<Statistics?> GetNextStatistics(CancellationToken cancellationToken)
    {
        try
        {
            await _semaphoreSlimForReading.WaitAsync(cancellationToken);

            var line = await StreamReader.ReadLineAsync(cancellationToken);

            if (line is null)
            {
                return null;
            }

            Interlocked.Increment(ref _readCount);

            return StatisticsParser.Parse(line);
        }
        finally
        {
            _semaphoreSlimForReading.Release();
        }
    }

    private async Task WriteResultLine(int productId, int demand, CancellationToken cancellationToken)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(productId);

        stringBuilder.Append(", ");

        stringBuilder.Append(demand);

        try
        {
            await _semaphoreSlimForWriting.WaitAsync(cancellationToken);

            await StreamWriter.WriteLineAsync(stringBuilder, cancellationToken);

            Interlocked.Increment(ref _writeCount);
        }
        finally
        {
            _semaphoreSlimForWriting.Release();
        }
    }

    public async Task Run
    (
        string inputDataPath,
        string outputDataPath,
        int maxThreadsCount,
        CancellationToken cancellationToken
    )
    {
        try
        {
            _streamReader = new StreamReader(inputDataPath);

            _streamWriter = new StreamWriter(outputDataPath, false);

            var tasks = new List<Task>(maxThreadsCount);

            for (var i = 0; i < maxThreadsCount; ++i)
            {
                tasks.Add(Task.Run(async () =>
                    {
                        while (await GetNextStatistics(cancellationToken) is { } statistics)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                            }

                            PrintProgress(_readCount, _calculatedCount, _writeCount);

                            var demand = Calculate(statistics, cancellationToken);

                            PrintProgress(_readCount, _calculatedCount, _writeCount);

                            await WriteResultLine(statistics.Id, demand, cancellationToken);

                            PrintProgress(_readCount, _calculatedCount, _writeCount);
                        }
                    },
                    cancellationToken
                ));
            }

            await Task.WhenAll(tasks);
        }
        finally
        {
            StreamReader.Dispose();

            await StreamWriter.DisposeAsync();
        }
    }

    private static void PrintProgress(int readCount, int calculatedCount, int writeCount)
    {
        Console.WriteLine
        (
            $"readCount: {readCount}, calculatedCount: {calculatedCount}, writeCount: {writeCount}"
        );
    }
}