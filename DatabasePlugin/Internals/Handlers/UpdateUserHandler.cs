using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed class UpdateUserHandler(IDispatcher dispatcher)
        : DatabaseRequestHandler(dispatcher)
    {
        public override string Pattern => "database/users/{id:long}";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Put;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Обновление пользователя", ["database"], "Обновление пользователя")
        {
            Description = "Обновляет другого пользователя. Тело JSON содержит username и roleId. Пропущенные password и isBlocked сохраняют текущие значения. Доступно администратору.",
            RequestBody = new(DatabaseJsonContext.Default.UserInput),
            Responses = [
                new(200, DatabaseJsonContext.Default.UserView),
                new(400), new(401), new(403), new(413), new(415), new(404), new(409)
            ]
        };

        protected override object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller)
            => dispatcher.Execute<UpdateUserCommand, UserView>(new(GetId(request), ReadBody(request, DatabaseJsonContext.Default.UserInput), caller.Id));
    }
}
