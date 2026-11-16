using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class DeleteHistoryEntryHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/history/{id:long}";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Delete;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Удаление записи истории", ["database"], "Удаление записи истории")
        {
            Description = "Удаляет запись истории по идентификатору. Доступно администратору.",
            Responses = [
                new(200) { Schema = """{"type":"null"}""" },
                new(400), new(401), new(403), new(404)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<DeleteHistoryEntryCommand, object?>(new(GetId(request)));
    }
}
