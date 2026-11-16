using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class GetRolesHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/roles";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Список ролей", ["database"], "Список ролей")
        {
            Description = "Возвращает идентификаторы и названия всех ролей. Доступно наблюдателю и выше.",
            Responses = [
                new(200, DatabaseJsonContext.Default.RoleViewArray),
                new(400), new(401), new(403)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<GetRolesCommand, RoleView[]>(new());
    }
}
