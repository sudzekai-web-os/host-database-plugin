using MySql.Data.MySqlClient;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class DatabaseRepository(DatabaseConnectionFactory connections)
    {
        public UserAccess? GetUserAccess(long id)
            => Query("SELECT role_id, is_blocked FROM users WHERE id = @id",
                reader => new UserAccess((ClientRole)reader.GetInt32(0), reader.GetBoolean(1)), ("@id", id)).SingleOrDefault();

        public UserView[] GetUsers(Page page)
            => Query("SELECT id, username FROM users ORDER BY id LIMIT @limit OFFSET @offset",
                ReadUser, ("@limit", page.Limit), ("@offset", page.Offset));

        public UserView? GetUser(long id)
            => Query("SELECT id, username FROM users WHERE id = @id", ReadUser, ("@id", id)).SingleOrDefault();

        public UserView CreateUser(UserInput input, string passwordHash)
        {
            using var connection = connections.Open();
            using var command = CreateCommand(connection,
                """
                INSERT INTO users (username, password_hash, role_id, is_blocked)
                VALUES (@username, @passwordHash, @roleId, @isBlocked)
                """,
                ("@username", input.Username), ("@passwordHash", passwordHash),
                ("@roleId", input.RoleId), ("@isBlocked", input.IsBlocked ?? false));
            command.ExecuteNonQuery();
            return new(command.LastInsertedId, input.Username!);
        }

        public UserView? UpdateUser(long id, UserInput input, string? passwordHash)
        {
            var count = Execute("""
                UPDATE users SET username = @username, password_hash = COALESCE(@passwordHash, password_hash),
                    role_id = @roleId, is_blocked = COALESCE(@isBlocked, is_blocked)
                WHERE id = @id
                """,
                ("@id", id), ("@username", input.Username), ("@passwordHash", passwordHash),
                ("@roleId", input.RoleId), ("@isBlocked", input.IsBlocked));
            return count == 0 ? null : new(id, input.Username!);
        }

        public bool DeleteUser(long id)
            => Execute("DELETE FROM users WHERE id = @id", ("@id", id)) != 0;

        public RoleView[] GetRoles()
            => Query("SELECT id, name FROM roles ORDER BY id", ReadRole);

        public RoleView? GetRole(long id)
            => Query("SELECT id, name FROM roles WHERE id = @id", ReadRole, ("@id", id)).SingleOrDefault();

        public HistoryView[] GetHistory(Page page)
            => Query("""
                SELECT id, user_id, datetime, action, summary FROM change_history
                ORDER BY id DESC LIMIT @limit OFFSET @offset
                """, ReadHistory, ("@limit", page.Limit), ("@offset", page.Offset));

        public HistoryView? GetHistoryEntry(long id)
            => Query("SELECT id, user_id, datetime, action, summary FROM change_history WHERE id = @id",
                ReadHistory, ("@id", id)).SingleOrDefault();

        public HistoryView CreateHistoryEntry(HistoryInput input, DateTime dateTime)
        {
            using var connection = connections.Open();
            using var command = CreateCommand(connection,
                """
                INSERT INTO change_history (user_id, datetime, action, summary)
                VALUES (@userId, @datetime, @action, @summary)
                """, ("@userId", input.UserId), ("@datetime", dateTime),
                ("@action", input.Action), ("@summary", input.Summary));
            command.ExecuteNonQuery();
            return new(command.LastInsertedId, input.UserId, dateTime, input.Action!, input.Summary!);
        }

        public bool UpdateHistoryEntry(long id, HistoryInput input)
            => Execute("""
                UPDATE change_history SET user_id = @userId, datetime = COALESCE(@datetime, datetime),
                    action = @action, summary = @summary
                WHERE id = @id
                """, ("@id", id), ("@userId", input.UserId), ("@datetime", input.DateTime?.UtcDateTime),
                ("@action", input.Action), ("@summary", input.Summary)) != 0;

        public bool DeleteHistoryEntry(long id)
            => Execute("DELETE FROM change_history WHERE id = @id", ("@id", id)) != 0;

        private T[] Query<T>(string sql, Func<MySqlDataReader, T> map, params (string Name, object? Value)[] parameters)
        {
            using var connection = connections.Open();
            using var command = CreateCommand(connection, sql, parameters);
            using var reader = command.ExecuteReader();
            var result = new List<T>();
            while (reader.Read())
                result.Add(map(reader));
            return [.. result];
        }

        private int Execute(string sql, params (string Name, object? Value)[] parameters)
        {
            using var connection = connections.Open();
            using var command = CreateCommand(connection, sql, parameters);
            return command.ExecuteNonQuery();
        }

        private static MySqlCommand CreateCommand(MySqlConnection connection, string sql,
            params (string Name, object? Value)[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            foreach (var (name, value) in parameters)
                command.Parameters.AddWithValue(name, value ?? DBNull.Value);
            return command;
        }

        private static UserView ReadUser(MySqlDataReader reader)
            => new(reader.GetInt64(0), reader.GetString(1));

        private static RoleView ReadRole(MySqlDataReader reader)
            => new(reader.GetInt32(0), reader.GetString(1));

        private static HistoryView ReadHistory(MySqlDataReader reader)
            => new(reader.GetInt64(0), reader.GetInt64(1), DateTime.SpecifyKind(reader.GetDateTime(2), DateTimeKind.Utc),
                reader.GetString(3), reader.GetString(4));
    }
}
