using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace DatabasePlugin
{
    internal sealed class DatabaseConnectionFactory(IConfiguration configuration)
    {
        public string Username { get; } = Required(configuration, "Username");
        public string Password { get; } = Required(configuration, "Password");

        private readonly string _connectionString = new MySqlConnectionStringBuilder
        {
            Server = Required(configuration, "Host"),
            Database = Required(configuration, "Database"),
            UserID = Required(configuration, "Username"),
            Password = Required(configuration, "Password"),
            SslMode = MySqlSslMode.Required
        }.ConnectionString;

        public MySqlConnection Open()
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

        private static string Required(IConfiguration configuration, string key)
            => configuration[$"LinCtrlDatabase:{key}"] is { Length: > 0 } value
                ? value
                : throw new InvalidOperationException($"Missing configuration: LinCtrlDatabase:{key}");
    }
}
