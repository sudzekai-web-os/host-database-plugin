using System.Data.Common;

namespace DatabasePlugin.Application.Context
{
    internal interface IDbContext
    {
        DbConnection GetConnection();

        DbCommand CreateCommand(DbConnection connection, string sql, params (string Name, object? Value)[] parameters);

        T[] Query<T>(string sql, Func<DbDataReader, T> map, params (string Name, object? Value)[] parameters);

        int Execute(string sql, params (string Name, object? Value)[] parameters);

        long Insert(string sql, params (string Name, object? Value)[] parameters);
    }
}
