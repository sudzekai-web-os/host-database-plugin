using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting;
using System.Net;
using System.Text;

namespace DatabasePlugin
{
    internal sealed class DatabaseDispatcher(DatabaseRepository repository, PasswordHashService passwords)
    {
        public long Authorize(string? subject, string[]? roles, bool requiresAdministrator)
        {
            if (!long.TryParse(subject, out var userId) || userId <= 0)
                throw new DatabaseRequestException(HttpStatusCode.Unauthorized, "Требуется действующий токен.");

            var access = repository.GetUserAccess(userId);
            if (access is null || access.IsBlocked)
                throw new DatabaseRequestException(HttpStatusCode.Unauthorized, "Учётная запись недоступна.");

            if (access.Role is not (ClientRole.Administrator or ClientRole.User or ClientRole.Viewer)
                || roles == null || !roles.Contains(access.Role.ToString(), StringComparer.Ordinal)
                || (requiresAdministrator && access.Role != ClientRole.Administrator))
                throw new DatabaseRequestException(HttpStatusCode.Forbidden, "Недостаточно прав.");

            return userId;
        }

        public string Dispatch(GetVersion query)
            => typeof(DatabaseDispatcher).Assembly.GetName().Version!.ToString(3);

        public UserView[] Dispatch(GetUsers query)
            => repository.GetUsers(query.Page);

        public UserView Dispatch(GetUser query)
            => repository.GetUser(query.Id) ?? throw NotFound("Пользователь не найден.");

        public UserView Dispatch(CreateUser command)
        {
            ValidateUser(command.Input, requiresPassword: true);
            return repository.CreateUser(command.Input, passwords.Hash(command.Input.Password!));
        }

        public UserView Dispatch(UpdateUser command, long callerId)
        {
            EnsureOtherUser(command.Id, callerId);
            ValidateUser(command.Input, requiresPassword: false);
            var hash = command.Input.Password == null ? null : passwords.Hash(command.Input.Password);
            return repository.UpdateUser(command.Id, command.Input, hash) ?? throw NotFound("Пользователь не найден.");
        }

        public void Dispatch(DeleteUser command, long callerId)
        {
            EnsureOtherUser(command.Id, callerId);
            if (!repository.DeleteUser(command.Id))
                throw NotFound("Пользователь не найден.");
        }

        public RoleView[] Dispatch(GetRoles query)
            => repository.GetRoles();

        public RoleView Dispatch(GetRole query)
            => repository.GetRole(query.Id) ?? throw NotFound("Роль не найдена.");

        public HistoryView[] Dispatch(GetHistory query)
            => repository.GetHistory(query.Page);

        public HistoryView Dispatch(GetHistoryEntry query)
            => repository.GetHistoryEntry(query.Id) ?? throw NotFound("Запись истории не найдена.");

        public HistoryView Dispatch(CreateHistoryEntry command)
        {
            ValidateHistory(command.Input);
            return repository.CreateHistoryEntry(command.Input, command.Input.DateTime?.UtcDateTime ?? DateTime.UtcNow);
        }

        public void Dispatch(UpdateHistoryEntry command)
        {
            ValidateHistory(command.Input);
            if (!repository.UpdateHistoryEntry(command.Id, command.Input))
                throw NotFound("Запись истории не найдена.");
        }

        public void Dispatch(DeleteHistoryEntry command)
        {
            if (!repository.DeleteHistoryEntry(command.Id))
                throw NotFound("Запись истории не найдена.");
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

        private static void ValidateHistory(HistoryInput input)
        {
            if (input.UserId <= 0 || string.IsNullOrWhiteSpace(input.Action) || input.Action.Length > 100
                || string.IsNullOrWhiteSpace(input.Summary) || Encoding.UTF8.GetByteCount(input.Summary) > 65535
                || input.DateTime?.UtcDateTime.Year < 1000)
                throw BadRequest("Укажите пользователя, действие до 100 символов и непустое описание до 65 535 байт.");
        }

        private static void EnsureOtherUser(long targetId, long callerId)
        {
            if (targetId == callerId)
                throw new DatabaseRequestException(HttpStatusCode.Forbidden, "Изменение собственной учётной записи запрещено.");
        }

        private static DatabaseRequestException BadRequest(string message)
            => new(HttpStatusCode.BadRequest, message);

        private static DatabaseRequestException NotFound(string message)
            => new(HttpStatusCode.NotFound, message);
    }
}
