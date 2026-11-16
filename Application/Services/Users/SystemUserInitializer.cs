using DatabasePlugin.Application.Context;
using DatabasePlugin.Application.Repositories.Roles;
using DatabasePlugin.Application.Repositories.Users;
using Sudzekai.LinCtrl.Core;
using IHashService = Sudzekai.LinCtrl.Hosting.Application.IHashService;

namespace DatabasePlugin.Application.Services.Users
{
    internal sealed class SystemUserInitializer(
        IDbContext db, DatabaseSettings settings, IRoleRepository roles, IUserRepository users, IHashService passwords)
    {
        public void Initialize()
        {
            using var connection = db.GetConnection();
            using var transaction = connection.BeginTransaction();

            roles.EnsureAdministrator(connection, transaction);
            var user = users.GetSystemUser(connection, transaction, settings.Username);
            if (user == null)
            {
                users.CreateSystemUser(connection, transaction, settings.Username, passwords.Hash(settings.Password));
            }
            else if (!string.Equals(user.Username, settings.Username, StringComparison.Ordinal)
                || user.Role != ClientRole.Administrator || user.IsBlocked || !passwords.Verify(settings.Password, user.PasswordHash))
            {
                throw new InvalidOperationException("Системная учётная запись конфликтует с существующим пользователем.");
            }

            transaction.Commit();
        }
    }
}
