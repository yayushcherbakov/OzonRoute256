using System.Text.Json;
using DemandCalculator.Constants;
using DemandCalculator.Contracts;
using DemandCalculator.Exceptions;
using DemandCalculator.Models;

namespace DemandCalculator.Services;

public class ConfigService : IConfigService
{
    public ParallelConfig GetParallelConfig()
    {
        var path = Path.Combine
        (
            AppDomain.CurrentDomain.BaseDirectory,
            PathConstants.ConfigsDirectory,
            PathConstants.ParallelConfigFileName
        );

        var serializedConfig = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ParallelConfig>(serializedConfig, options)
               ?? throw new NotFoundException(PathConstants.ParallelConfigFileName);
    }
}