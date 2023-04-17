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
    DbConnection CreateWriteConnection();

    DbConnection CreateReadConnection();
}

public class PostgresConnectionFactory : IConnectionFactory
{
    private readonly string _writeConnectionString;
    private readonly string _readConnectionString;

    public PostgresConnectionFactory(string writeConnectionString, string readConnectionString)
    {
        _writeConnectionString = writeConnectionString;
        _readConnectionString = readConnectionString;
    }

    public DbConnection CreateWriteConnection() => new NpgsqlConnection(_writeConnectionString);
    public DbConnection CreateReadConnection() => new NpgsqlConnection(_readConnectionString);
}
