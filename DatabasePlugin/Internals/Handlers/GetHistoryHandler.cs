using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class GetHistoryHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/history";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Список записей истории", ["database"], "Список записей истории")
        {
            Description = "Возвращает историю изменений от новых записей к старым. Поддерживает параметры limit и offset.",
            Parameters = [new("limit", "query", DatabaseJsonContext.Default.Int32, "Размер страницы"),
                new("offset", "query", DatabaseJsonContext.Default.Int32, "Смещение списка")],
            Responses = [
                new(200, DatabaseJsonContext.Default.HistoryViewArray),
                new(400), new(401), new(403)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<GetHistoryCommand, HistoryView[]>(new(GetPage(request)));
    }
}
