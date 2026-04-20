using System.Data;
using Npgsql;

namespace Infrastructure.Persistence.Db;

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseOptions _databaseOptions;

    public NpgsqlConnectionFactory(DatabaseOptions databaseOptions)
    {
        _databaseOptions = databaseOptions;
    }

    public IDbConnection CreateOpenConnection()
    {
        if (string.IsNullOrWhiteSpace(_databaseOptions.ConnectionString))
        {
            throw new InvalidOperationException("ConnectionString is not configured.");
        }

        var connection = new NpgsqlConnection(_databaseOptions.ConnectionString);
        connection.Open();
        return connection;
    }
}
