using System.Transactions;
using Npgsql;
using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Configurations;
using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Contracts;

namespace Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Repositories;

public abstract class PgRepository : IPgRepository
{
    private readonly DalOptions _dalSettings;

    protected const int DefaultTimeoutInSeconds = 5;

    protected PgRepository(DalOptions dalSettings)
    {
        _dalSettings = dalSettings;
    }

    protected async Task<NpgsqlConnection> GetConnection()
    {
        if (Transaction.Current is not null &&
            Transaction.Current.TransactionInformation.Status is TransactionStatus.Aborted)
        {
            throw new TransactionAbortedException();
        }

        var connection = new NpgsqlConnection(_dalSettings.PostgresConnectionString);
        await connection.OpenAsync();

        // Due to in-process migrations
        connection.ReloadTypes();

        return connection;
    }
}
