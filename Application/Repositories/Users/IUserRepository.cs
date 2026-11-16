using DatabasePlugin.Application.Dto;
using System.Data.Common;

namespace DatabasePlugin.Application.Repositories.Users
{
    internal interface IUserRepository
    {
        bool IsProtectedUsername(string? username);

        bool IsProtectedUser(long id);

        UserAccess? GetUserAccess(long id);

        UserView[] GetUsers(Page page, bool includeRole);

        UserView? GetUser(long id, bool includeRole);

        UserView CreateUser(UserInput input, string passwordHash);

        UserView? UpdateUser(long id, UserInput input, string? passwordHash);

        bool BlockUser(long id);

        SystemUser? GetSystemUser(DbConnection connection, DbTransaction transaction, string username);

        void CreateSystemUser(DbConnection connection, DbTransaction transaction, string username, string passwordHash);
    }
}
