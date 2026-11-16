using DatabasePlugin.Application.Context;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;
using System.Data.Common;

namespace DatabasePlugin.Application.Repositories.Users
{
    internal sealed class UserRepository(IDbContext db, DatabaseSettings settings) : IUserRepository
    {
        private const string _protectedUsername = "linctrl_user";

        public bool IsProtectedUsername(string? username)
            => string.Equals(username, _protectedUsername, StringComparison.OrdinalIgnoreCase)
                || string.Equals(username, settings.Username, StringComparison.OrdinalIgnoreCase);

        public UserAccess? GetUserAccess(long id)
            => db.Query("SELECT role_id, is_blocked FROM users WHERE id = @id",
                reader => new UserAccess((ClientRole)reader.GetInt32(0), reader.GetBoolean(1)), ("@id", id)).SingleOrDefault();

        private const string _usersWithRole = """
            SELECT u.id, u.username, u.role_id, r.name, u.is_blocked
            FROM users u JOIN roles r ON r.id = u.role_id
            """;

        public UserView[] GetUsers(Page page, bool includeRole)
            => db.Query(_usersWithRole + " ORDER BY u.id LIMIT @limit OFFSET @offset",
                reader => ReadUser(reader, includeRole), ("@limit", page.Limit), ("@offset", page.Offset));

        public UserView? GetUser(long id, bool includeRole)
            => db.Query(_usersWithRole + " WHERE u.id = @id", reader => ReadUser(reader, includeRole), ("@id", id)).SingleOrDefault();

        public bool IsProtectedUser(long id)
            => db.Query("SELECT username IN (@protected, @system) FROM users WHERE id = @id",
                reader => reader.GetBoolean(0), ("@id", id), ("@protected", _protectedUsername),
                ("@system", settings.Username)).SingleOrDefault();

        public UserView CreateUser(UserInput input, string passwordHash)
        {
            var id = db.Insert(
                """
                INSERT INTO users (username, password_hash, role_id, is_blocked)
                VALUES (@username, @passwordHash, @roleId, @isBlocked)
                """,
                ("@username", input.Username), ("@passwordHash", passwordHash),
                ("@roleId", input.RoleId), ("@isBlocked", input.IsBlocked ?? false));
            return new(id, input.Username!);
        }

        public UserView? UpdateUser(long id, UserInput input, string? passwordHash)
        {
            var count = db.Execute("""
                UPDATE users SET username = @username, password_hash = COALESCE(@passwordHash, password_hash),
                    role_id = @roleId, is_blocked = COALESCE(@isBlocked, is_blocked)
                WHERE id = @id AND username NOT IN (@protected, @system)
                """,
                ("@id", id), ("@username", input.Username), ("@passwordHash", passwordHash),
                ("@protected", _protectedUsername), ("@system", settings.Username),
                ("@roleId", input.RoleId), ("@isBlocked", input.IsBlocked));
            return count == 0 ? null : new(id, input.Username!);
        }

        public bool BlockUser(long id)
            => db.Execute("UPDATE users SET is_blocked = TRUE WHERE id = @id AND username NOT IN (@protected, @system)",
                ("@id", id), ("@protected", _protectedUsername), ("@system", settings.Username)) != 0;

        public SystemUser? GetSystemUser(DbConnection connection, DbTransaction transaction, string username)
        {
            using var command = db.CreateCommand(connection, """
                SELECT username, password_hash, role_id, is_blocked
                FROM users WHERE username = @username FOR UPDATE
                """, ("@username", username));
            command.Transaction = transaction;

            using var reader = command.ExecuteReader();
            return reader.Read()
                ? new(reader.GetString(0), reader.GetString(1), (ClientRole)reader.GetInt32(2), reader.GetBoolean(3))
                : null;
        }

        public void CreateSystemUser(DbConnection connection, DbTransaction transaction, string username, string passwordHash)
        {
            using var command = db.CreateCommand(connection, """
                INSERT INTO users (username, password_hash, role_id, is_blocked)
                VALUES (@username, @hash, @role, FALSE)
                """, ("@username", username), ("@hash", passwordHash), ("@role", (int)ClientRole.Administrator));
            command.Transaction = transaction;
            command.ExecuteNonQuery();
        }

        private static UserView ReadUser(DbDataReader reader, bool includeRole)
            => includeRole
                ? new(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(3), reader.GetBoolean(4))
                : new(reader.GetInt64(0), reader.GetString(1));
    }
}
