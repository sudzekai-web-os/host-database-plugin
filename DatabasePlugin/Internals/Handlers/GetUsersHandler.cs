using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class GetUsersHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/users";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Список пользователей", ["database"], "Список пользователей")
        {
            Description = "Возвращает идентификаторы и имена пользователей, для администратора также роли и признак блокировки. Поддерживает limit и offset.",
            Parameters = [new("limit", "query", DatabaseJsonContext.Default.Int32, "Размер страницы"),
                new("offset", "query", DatabaseJsonContext.Default.Int32, "Смещение списка")],
            Responses = [
                new(200, DatabaseJsonContext.Default.UserViewArray),
                new(400), new(401), new(403)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<GetUsersCommand, UserView[]>(new(GetPage(request), caller.Role));
    }
}
