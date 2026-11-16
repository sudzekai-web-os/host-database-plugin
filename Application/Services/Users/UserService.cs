using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Repositories.Users;
using Sudzekai.LinCtrl.Core;
using IHashService = Sudzekai.LinCtrl.Hosting.Application.IHashService;
using System.Net;

namespace DatabasePlugin.Application.Services.Users
{
    internal sealed class UserService(IUserRepository repository, IHashService passwords)
    {
        public UserView[] GetUsers(Page page, ClientRole role)
            => repository.GetUsers(page, role == ClientRole.Administrator);

        public UserView GetUser(long id, ClientRole role)
            => repository.GetUser(id, role == ClientRole.Administrator) ?? throw NotFound("Пользователь не найден.");

        public UserView CreateUser(UserInput input)
        {
            ValidateUser(input, requiresPassword: true);
            EnsureUnreservedUsername(input.Username);
            return repository.CreateUser(input, passwords.Hash(input.Password!));
        }

        public UserView UpdateUser(long id, UserInput input, long callerId)
        {
            EnsureMutableUser(id, callerId);
            ValidateUser(input, requiresPassword: false);
            EnsureUnreservedUsername(input.Username);
            var hash = input.Password == null ? null : passwords.Hash(input.Password);
            return repository.UpdateUser(id, input, hash) ?? throw NotFound("Пользователь не найден.");
        }

        public void BlockUser(long id, long callerId)
        {
            EnsureMutableUser(id, callerId);
            if (!repository.BlockUser(id))
                throw NotFound("Пользователь не найден.");
        }

        private static void ValidateUser(UserInput input, bool requiresPassword)
        {
            if (string.IsNullOrWhiteSpace(input.Username) || input.Username.Length > 191
                || !string.Equals(input.Username, input.Username.Trim(), StringComparison.Ordinal))
                throw BadRequest("Имя пользователя должно содержать от 1 до 191 символа без крайних пробелов.");

            if (input.Password is { Length: 0 or > 1024 } || (requiresPassword && input.Password == null))
                throw BadRequest("Пароль должен содержать от 1 до 1024 символов.");

            if ((ClientRole)input.RoleId is not (ClientRole.Administrator or ClientRole.User or ClientRole.Viewer))
                throw BadRequest("Указана неизвестная роль.");
        }

        private void EnsureUnreservedUsername(string? username)
        {
            if (repository.IsProtectedUsername(username))
                throw ProtectedUser();
        }

        private void EnsureMutableUser(long targetId, long callerId)
        {
            if (repository.IsProtectedUser(targetId))
                throw ProtectedUser();

            if (targetId == callerId)
                throw new DatabaseRequestException(HttpStatusCode.Forbidden, "Изменение собственной учётной записи запрещено.");
        }

        private static DatabaseRequestException ProtectedUser()
            => new(HttpStatusCode.Forbidden, "Системная учётная запись защищена от изменений.");

        private static DatabaseRequestException BadRequest(string message)
            => new(HttpStatusCode.BadRequest, message);

        private static DatabaseRequestException NotFound(string message)
            => new(HttpStatusCode.NotFound, message);
    }
}
