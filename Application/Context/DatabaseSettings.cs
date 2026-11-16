using Microsoft.Extensions.Configuration;

namespace DatabasePlugin.Application.Context
{
    internal sealed class DatabaseSettings(IConfiguration configuration)
    {
        public string Host { get; } = Required(configuration, "Host");

        public string Database { get; } = Required(configuration, "Database");

        public string Username { get; } = Required(configuration, "Username");

        public string Password { get; } = Required(configuration, "Password");

        private static string Required(IConfiguration configuration, string key)
            => configuration[$"LinCtrlDatabase:{key}"] is { Length: > 0 } value
                ? value
                : throw new InvalidOperationException($"Missing configuration: LinCtrlDatabase:{key}");
    }
}
