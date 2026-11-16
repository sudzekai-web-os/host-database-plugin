using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class BlockUserHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/users/{id:long}";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Delete;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Блокировка пользователя", ["database"], "Блокировка пользователя")
        {
            Description = "Блокирует другого пользователя, сохраняя его запись и историю. Доступно администратору.",
            Responses = [
                new(200) { Schema = """{"type":"null"}""" },
                new(400), new(401), new(403), new(404)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<BlockUserCommand, object?>(new(GetId(request), caller.Id));
    }
}
