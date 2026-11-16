using DatabasePlugin.Application.Context;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;
using System.Data.Common;

namespace DatabasePlugin.Application.Repositories.Roles
{
    internal sealed class RoleRepository(IDbContext db) : IRoleRepository
    {
        public RoleView[] GetRoles()
            => db.Query("SELECT id, name FROM roles ORDER BY id", ReadRole);

        public RoleView? GetRole(long id)
            => db.Query("SELECT id, name FROM roles WHERE id = @id", ReadRole, ("@id", id)).SingleOrDefault();

        public void EnsureAdministrator(DbConnection connection, DbTransaction transaction)
        {
            using var insert = db.CreateCommand(connection, """
                INSERT INTO roles (id, name) VALUES (@id, @name)
                ON DUPLICATE KEY UPDATE id = roles.id
                """, ("@id", (int)ClientRole.Administrator), ("@name", nameof(ClientRole.Administrator)));
            insert.Transaction = transaction;
            insert.ExecuteNonQuery();

            using var select = db.CreateCommand(connection, "SELECT name FROM roles WHERE id = @id",
                ("@id", (int)ClientRole.Administrator));
            select.Transaction = transaction;
            if (!string.Equals(select.ExecuteScalar() as string, nameof(ClientRole.Administrator), StringComparison.Ordinal))
                throw new InvalidOperationException("Роль администратора конфликтует с идентификатором роли LinCtrl.");
        }

        private static RoleView ReadRole(DbDataReader reader)
            => new(reader.GetInt32(0), reader.GetString(1));
    }
}
