using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Metadata;

namespace DatabasePlugin
{
    internal static class DatabaseHandlerServicesExtensions
    {
        public static void AddDatabaseHandlers(this IServiceCollection services)
        {
            Add(services, HttpRequestMethod.Get, "version", "Версия плагина",
                "Возвращает версию плагина строкой. Доступно наблюдателю, пользователю и администратору.",
                (dispatcher, _, _) => dispatcher.Dispatch(new GetVersion()));

            Add(services, HttpRequestMethod.Get, "roles", "Список ролей",
                "Возвращает идентификаторы и названия всех ролей. Доступно наблюдателю и выше.",
                (dispatcher, _, _) => dispatcher.Dispatch(new GetRoles()));

            Add(services, HttpRequestMethod.Get, "roles/{id:long}", "Получение роли",
                "Возвращает роль по идентификатору. Доступно наблюдателю и выше.",
                (dispatcher, request, _) => dispatcher.Dispatch(new GetRole(GetId(request))), 404);

            Add(services, HttpRequestMethod.Get, "users", "Список пользователей",
                "Возвращает только идентификаторы и имена пользователей. Поддерживает параметры limit и offset.",
                (dispatcher, request, _) => dispatcher.Dispatch(new GetUsers(GetPage(request))));

            Add(services, HttpRequestMethod.Get, "users/{id:long}", "Получение пользователя",
                "Возвращает только идентификатор и имя пользователя. Доступно наблюдателю и выше.",
                (dispatcher, request, _) => dispatcher.Dispatch(new GetUser(GetId(request))), 404);

            Add(services, HttpRequestMethod.Post, "users", "Создание пользователя",
                "Создаёт пользователя. Тело JSON содержит username, password, roleId и необязательное isBlocked. Доступно администратору.",
                (dispatcher, request, _) => dispatcher.Dispatch(new CreateUser(ReadBody(request, DatabaseJsonContext.Default.UserInput))), 409);

            Add(services, HttpRequestMethod.Put, "users/{id:long}", "Обновление пользователя",
                "Обновляет другого пользователя. Тело JSON содержит username и roleId. Пропущенные password и isBlocked сохраняют текущие значения. Доступно администратору.",
                (dispatcher, request, caller) => dispatcher.Dispatch(new UpdateUser(GetId(request), ReadBody(request, DatabaseJsonContext.Default.UserInput)), caller), 404, 409);

            AddEmpty(services, HttpRequestMethod.Delete, "users/{id:long}", "Удаление пользователя",
                "Удаляет другого пользователя без связанных записей истории. Доступно администратору.",
                (dispatcher, request, caller) => dispatcher.Dispatch(new DeleteUser(GetId(request)), caller), 404, 409);

            Add(services, HttpRequestMethod.Get, "history", "Список записей истории",
                "Возвращает историю изменений от новых записей к старым. Поддерживает параметры limit и offset.",
                (dispatcher, request, _) => dispatcher.Dispatch(new GetHistory(GetPage(request))));

            Add(services, HttpRequestMethod.Get, "history/{id:long}", "Получение записи истории",
                "Возвращает запись истории по идентификатору. Доступно наблюдателю и выше.",
                (dispatcher, request, _) => dispatcher.Dispatch(new GetHistoryEntry(GetId(request))), 404);

            Add(services, HttpRequestMethod.Post, "history", "Создание записи истории",
                "Создаёт запись истории. Тело JSON содержит userId, action, summary и необязательное dateTime. Доступно администратору.",
                (dispatcher, request, _) => dispatcher.Dispatch(new CreateHistoryEntry(ReadBody(request, DatabaseJsonContext.Default.HistoryInput))), 409);

            AddEmpty(services, HttpRequestMethod.Put, "history/{id:long}", "Обновление записи истории",
                "Обновляет запись истории. Тело JSON содержит userId, action и summary. Пропущенное dateTime сохраняет текущее значение. Доступно администратору.",
                (dispatcher, request, _) => dispatcher.Dispatch(new UpdateHistoryEntry(GetId(request), ReadBody(request, DatabaseJsonContext.Default.HistoryInput))), 404, 409);

            AddEmpty(services, HttpRequestMethod.Delete, "history/{id:long}", "Удаление записи истории",
                "Удаляет запись истории по идентификатору. Доступно администратору.",
                (dispatcher, request, _) => dispatcher.Dispatch(new DeleteHistoryEntry(GetId(request))), 404);
        }

        private static void Add<TResult>(IServiceCollection services, HttpRequestMethod method, string pattern,
            string summary, string description, Func<DatabaseDispatcher, HttpRequestDto, long, TResult> execute, params int[] extraStatuses)
        {
            var schema = DatabaseJsonContext.Default.Options.GetJsonSchemaAsNode(typeof(TResult),
                new JsonSchemaExporterOptions { TreatNullObliviousAsNonNullable = true });
            Add(services, method, pattern, summary, description, schema.ToJsonString(),
                (dispatcher, request, caller) => execute(dispatcher, request, caller), extraStatuses);
        }

        private static void AddEmpty(IServiceCollection services, HttpRequestMethod method, string pattern,
            string summary, string description, Action<DatabaseDispatcher, HttpRequestDto, long> execute, params int[] extraStatuses)
            => Add(services, method, pattern, summary, description, """{"type":"null"}""", (dispatcher, request, caller) =>
            {
                execute(dispatcher, request, caller);
                return null;
            }, extraStatuses);

        private static void Add(IServiceCollection services, HttpRequestMethod method, string pattern,
            string summary, string description, string responseDataSchema,
            Func<DatabaseDispatcher, HttpRequestDto, long, object?> execute, int[] extraStatuses)
        {
            var statusCode = method == HttpRequestMethod.Post ? 201 : 200;
            var acceptsBody = method is HttpRequestMethod.Post or HttpRequestMethod.Put;
            int[] statuses = acceptsBody ? [statusCode, 400, 401, 403, 413, 415, .. extraStatuses] : [statusCode, 400, 401, 403, .. extraStatuses];
            var queryParameters = method == HttpRequestMethod.Get && pattern is "users" or "history"
                ? new[] { "limit", "offset" }
                : null;
            var metadata = new RequestHandlerMetadata(method, $"database/{pattern}", summary, description, statuses,
                acceptsBody, queryParameters, Group: "database", ResponseDataSchema: responseDataSchema);
            services.AddTransient<IRequestHandler>(provider => new DatabaseRequestHandler(
                provider.GetRequiredService<DatabaseDispatcher>(), metadata, method != HttpRequestMethod.Get, execute));
        }

        private static long GetId(HttpRequestDto request)
            => long.TryParse(request.RouteValues.GetValueOrDefault("id"), NumberStyles.None, CultureInfo.InvariantCulture, out var id) && id > 0
                ? id
                : throw new DatabaseRequestException(HttpStatusCode.BadRequest, "Укажите положительный идентификатор.");

        private static Page GetPage(HttpRequestDto request)
        {
            var query = QueryHelpers.ParseQuery(request.QueryString);
            var limit = query.TryGetValue("limit", out var limitValue) ? ParsePageValue(limitValue.ToString(), 1, 200) : 50;
            var offset = query.TryGetValue("offset", out var offsetValue) ? ParsePageValue(offsetValue.ToString(), 0, int.MaxValue) : 0;
            return new(limit, offset);
        }

        private static int ParsePageValue(string value, int minimum, int maximum)
            => int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var number) && number >= minimum && number <= maximum
                ? number
                : throw new DatabaseRequestException(HttpStatusCode.BadRequest, "Некорректные параметры limit или offset.");

        private static T ReadBody<T>(HttpRequestDto request, JsonTypeInfo<T> typeInfo) where T : class
        {
            if (request.ContentType != ResponseContentType.ApplicationJson)
                throw new DatabaseRequestException(HttpStatusCode.UnsupportedMediaType, "Требуется Content-Type application/json.");
            if (request.Body.Length > 1024 * 1024)
                throw new DatabaseRequestException(HttpStatusCode.RequestEntityTooLarge, "Тело запроса превышает 1 МиБ.");

            return JsonSerializer.Deserialize(request.Body, typeInfo)
                ?? throw new DatabaseRequestException(HttpStatusCode.BadRequest, "Требуется объект JSON.");
        }
    }
}
