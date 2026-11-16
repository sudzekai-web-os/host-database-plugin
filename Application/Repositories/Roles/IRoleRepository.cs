using DatabasePlugin.Application.Dto;
using System.Data.Common;

namespace DatabasePlugin.Application.Repositories.Roles
{
    internal interface IRoleRepository
    {
        RoleView[] GetRoles();

        RoleView? GetRole(long id);

        void EnsureAdministrator(DbConnection connection, DbTransaction transaction);
    }
}
