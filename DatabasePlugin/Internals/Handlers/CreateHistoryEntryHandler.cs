using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class CreateHistoryEntryHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/history";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Post;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Создание записи истории", ["database"], "Создание записи истории")
        {
            Description = "Создаёт запись истории. Тело JSON содержит userId, action, summary и необязательное dateTime. Доступно администратору.",
            RequestBody = new(DatabaseJsonContext.Default.HistoryInput),
            Responses = [
                new(201, DatabaseJsonContext.Default.HistoryView),
                new(400), new(401), new(403), new(413), new(415), new(409)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<CreateHistoryEntryCommand, HistoryView>(new(ReadBody(request, DatabaseJsonContext.Default.HistoryInput)));
    }
}
