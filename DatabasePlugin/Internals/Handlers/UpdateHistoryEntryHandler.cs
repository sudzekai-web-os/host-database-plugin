using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class UpdateHistoryEntryHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/history/{id:long}";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Put;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Обновление записи истории", ["database"], "Обновление записи истории")
        {
            Description = "Обновляет запись истории. Тело JSON содержит userId, action и summary. Пропущенное dateTime сохраняет текущее значение. Доступно администратору.",
            RequestBody = new(DatabaseJsonContext.Default.HistoryInput),
            Responses = [
                new(200) { Schema = """{"type":"null"}""" },
                new(400), new(401), new(403), new(413), new(415), new(404), new(409)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<UpdateHistoryEntryCommand, object?>(new(GetId(request), ReadBody(request, DatabaseJsonContext.Default.HistoryInput)));
    }
}
