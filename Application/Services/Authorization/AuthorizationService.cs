using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Repositories.Users;
using Sudzekai.LinCtrl.Core;
using System.Net;

namespace DatabasePlugin.Application.Services.Authorization
{
    internal sealed class AuthorizationService(IUserRepository repository)
    {
        public (long Id, ClientRole Role) Authorize(string? subject, string[]? roles, bool requiresAdministrator)
        {
            if (!long.TryParse(subject, out var userId) || userId <= 0)
                throw new DatabaseRequestException(HttpStatusCode.Unauthorized, "Требуется действующий токен.");

            var access = repository.GetUserAccess(userId);
            if (access is null)
                throw new DatabaseRequestException(HttpStatusCode.Unauthorized, "Учётная запись недоступна.");
            if (access.IsBlocked)
                throw new DatabaseRequestException(HttpStatusCode.Forbidden, "Учётная запись заблокирована.");

            if (access.Role is not (ClientRole.Administrator or ClientRole.User or ClientRole.Viewer)
                || roles == null || !roles.Contains(access.Role.ToString(), StringComparer.Ordinal)
                || (requiresAdministrator && access.Role != ClientRole.Administrator))
                throw new DatabaseRequestException(HttpStatusCode.Forbidden, "Недостаточно прав.");

            return (userId, access.Role);
        }
    }
}
