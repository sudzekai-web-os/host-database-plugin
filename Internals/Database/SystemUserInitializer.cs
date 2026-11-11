using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting;

namespace DatabasePlugin
{
    internal sealed class SystemUserInitializer(DatabaseConnectionFactory connections, PasswordHashService passwords)
    {
        public void Initialize()
        {
            using var connection = connections.Open();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = """
                INSERT INTO roles (id, name) VALUES (@role, @roleName)
                ON DUPLICATE KEY UPDATE id = roles.id
                """;
            command.Parameters.AddWithValue("@role", (int)ClientRole.Administrator);
            command.Parameters.AddWithValue("@roleName", nameof(ClientRole.Administrator));
            command.ExecuteNonQuery();

            command.CommandText = "SELECT name FROM roles WHERE id = @role";
            if (!string.Equals(command.ExecuteScalar() as string, nameof(ClientRole.Administrator), StringComparison.Ordinal))
                throw new InvalidOperationException("The administrator role conflicts with the LinCtrl role identifier");

            command.Parameters.AddWithValue("@username", connections.Username);
            command.CommandText = """
                SELECT username, password_hash, role_id, is_blocked
                FROM users WHERE username = @username FOR UPDATE
                """;
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    if (!string.Equals(reader.GetString(0), connections.Username, StringComparison.Ordinal)
                        || reader.GetInt32(2) != (int)ClientRole.Administrator || reader.GetBoolean(3)
                        || !passwords.Verify(connections.Password, reader.GetString(1)))
                        throw new InvalidOperationException("The configured database account conflicts with an existing system user");

                    reader.Close();
                    transaction.Commit();
                    return;
                }
            }

            command.CommandText = """
                INSERT INTO users (username, password_hash, role_id, is_blocked)
                VALUES (@username, @hash, @role, FALSE)
                """;
            command.Parameters.AddWithValue("@hash", passwords.Hash(connections.Password));
            command.ExecuteNonQuery();
            transaction.Commit();
        }
    }
}
