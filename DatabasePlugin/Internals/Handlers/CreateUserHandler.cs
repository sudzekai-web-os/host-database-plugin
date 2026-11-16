using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class CreateUserHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/users";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Post;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Создание пользователя", ["database"], "Создание пользователя")
        {
            Description = "Создаёт пользователя. Тело JSON содержит username, password, roleId и необязательное isBlocked. Доступно администратору.",
            RequestBody = new(DatabaseJsonContext.Default.UserInput),
            Responses = [
                new(201, DatabaseJsonContext.Default.UserView),
                new(400), new(401), new(403), new(413), new(415), new(409)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<CreateUserCommand, UserView>(new(ReadBody(request, DatabaseJsonContext.Default.UserInput)));
    }
}
