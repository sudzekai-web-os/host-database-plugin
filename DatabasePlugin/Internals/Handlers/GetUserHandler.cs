using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class GetUserHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/users/{id:long}";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Получение пользователя", ["database"], "Получение пользователя")
        {
            Description = "Возвращает идентификатор и имя пользователя, для администратора также роль и признак блокировки.",
            Responses = [
                new(200, DatabaseJsonContext.Default.UserView),
                new(400), new(401), new(403), new(404)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<GetUserCommand, UserView>(new(GetId(request), caller.Role));
    }
}
