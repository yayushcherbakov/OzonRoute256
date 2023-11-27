using System.Globalization;
using DataAccess.Constants;
using DataAccess.Contracts;
using DataAccess.Entities;
using DataAccess.Exceptions;

namespace DataAccess.Repositories;

internal sealed class SalesRepository : ISalesRepository
{
    public List<Sale> GetSalesDataById(int productId)
    {
        return GetAllSales()
            .Where(x => x.ProductId == productId)
            .ToList();
    }

    private static IEnumerable<Sale> GetAllSales()
    {
        var path = Path.Combine
        (
            AppDomain.CurrentDomain.BaseDirectory,
            PathConstants.DataDirectory,
            PathConstants.SalesFileName
        );

        return File.ReadAllLines(path)
            .Select(x =>
            {
                var cols = x.Split(CsvConstants.ColumnSeparator);

                try
                {
                    return new Sale
                    (
                        int.Parse(cols[0]),
                        DateOnly.Parse(cols[1], CultureInfo.InvariantCulture),
                        int.Parse(cols[2]),
                        int.Parse(cols[3])
                    );
                }
                catch
                {
                    throw new ParseException(ErrorMessages.CanNotParseSale);
                }
            });
    }
}