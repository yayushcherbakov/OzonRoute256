using DemandCalculator.Exceptions;
using DemandCalculator.Models;

namespace DemandCalculator.Parsers;

public static class StatisticsParser
{
    public static Statistics Parse(string line)
    {
        try
        {
            var elements = line.Split(',');

            return new Statistics
            (
                int.Parse(elements[0]),
                double.Parse(elements[1]),
                int.Parse(elements[2])
            );
        }
        catch (Exception ex)
        {
            throw new StatisticsParseException(ex.Message);
        }
    }
}