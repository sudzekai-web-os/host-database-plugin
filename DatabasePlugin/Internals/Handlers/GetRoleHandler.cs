using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class GetRoleHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/roles/{id:long}";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Получение роли", ["database"], "Получение роли")
        {
            Description = "Возвращает роль по идентификатору. Доступно наблюдателю и выше.",
            Responses = [
                new(200, DatabaseJsonContext.Default.RoleView),
                new(400), new(401), new(403), new(404)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<GetRoleCommand, RoleView>(new(GetId(request)));
    }
}
