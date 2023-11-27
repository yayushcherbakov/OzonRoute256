using System.Globalization;
using DataAccess.Constants;
using DataAccess.Contracts;
using DataAccess.Entities;
using DataAccess.Exceptions;

namespace DataAccess.Repositories;

internal sealed class SeasonsRepository : ISeasonsRepository
{
    public List<Season> GetSeasonsDataById(int productId)
    {
        return GetAllSeason()
            .Where(x => x.Id == productId)
            .ToList();
    }

    private static IEnumerable<Season> GetAllSeason()
    {
        var path = Path.Combine
        (
            AppDomain.CurrentDomain.BaseDirectory,
            PathConstants.DataDirectory,
            PathConstants.SeasonsFileName
        );

        return File.ReadAllLines(path)
            .Select(x =>
            {
                var cols = x.Split(CsvConstants.ColumnSeparator);

                try
                {
                    return new Season
                    (
                        int.Parse(cols[0]),
                        int.Parse(cols[1]),
                        double.Parse(cols[2], CultureInfo.InvariantCulture)
                    );
                }
                catch
                {
                    throw new ParseException(ErrorMessages.CanNotParseSeason);
                }
            });
    }
}