using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.DbApi;
public interface IConnectionFactory
{
    DbConnection CreateConnection();
}

public class PostgresConnectionFactory : IConnectionFactory
{
    private string _connectionString;

    public PostgresConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}
