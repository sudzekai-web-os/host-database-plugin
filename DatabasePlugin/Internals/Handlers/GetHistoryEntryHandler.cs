using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class GetHistoryEntryHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/history/{id:long}";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Получение записи истории", ["database"], "Получение записи истории")
        {
            Description = "Возвращает запись истории по идентификатору. Доступно наблюдателю и выше.",
            Responses = [
                new(200, DatabaseJsonContext.Default.HistoryView),
                new(400), new(401), new(403), new(404)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<GetHistoryEntryCommand, HistoryView>(new(GetId(request)));
    }
}
