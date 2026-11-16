using DatabasePlugin.Application.Dto;
using System.Net;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace DatabasePlugin.Application.Context
{
    internal sealed class AppDbContext(DatabaseSettings settings) : IDbContext
    {
        private readonly string _connectionString = new MySqlConnectionStringBuilder
        {
            Server = settings.Host,
            Database = settings.Database,
            UserID = settings.Username,
            Password = settings.Password,
            SslMode = MySqlSslMode.Required
        }.ConnectionString;

        public DbConnection GetConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            try
            {
                connection.Open();
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        public DbCommand CreateCommand(DbConnection connection, string sql, params (string Name, object? Value)[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            foreach (var (name, value) in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = name;
                parameter.Value = value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }

            return command;
        }

        public T[] Query<T>(string sql, Func<DbDataReader, T> map, params (string Name, object? Value)[] parameters)
        {
            using var connection = GetConnection();
            using var command = CreateCommand(connection, sql, parameters);
            using var reader = command.ExecuteReader();
            var result = new List<T>();
            while (reader.Read())
                result.Add(map(reader));

            return [.. result];
        }

        public int Execute(string sql, params (string Name, object? Value)[] parameters)
        {
            using var connection = GetConnection();
            using var command = CreateCommand(connection, sql, parameters);
            return ExecuteCommand(command);
        }

        public long Insert(string sql, params (string Name, object? Value)[] parameters)
        {
            using var connection = GetConnection();
            using var command = (MySqlCommand)CreateCommand(connection, sql, parameters);
            ExecuteCommand(command);
            return command.LastInsertedId;
        }

        private static int ExecuteCommand(DbCommand command)
        {
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (MySqlException ex) when (FindConstraintError(ex) is { } error)
            {
                var message = error.Number switch
                {
                    1062 => "Запись с таким уникальным значением уже существует.",
                    1217 or 1451 => "Запись используется другими данными.",
                    1216 or 1452 => "Связанная запись не найдена.",
                    _ => "Операция нарушает ограничение целостности данных."
                };
                throw new DatabaseRequestException(HttpStatusCode.Conflict, message);
            }
        }

        private static MySqlException? FindConstraintError(MySqlException exception)
        {
            for (Exception? current = exception; current != null; current = current.InnerException)
            {
                if (current is MySqlException error
                    && (error.Number is 1062 or 1216 or 1217 or 1451 or 1452 || error.SqlState == "23000"))
                    return error;
            }

            return null;
        }
    }
}
