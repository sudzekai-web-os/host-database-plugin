using MySql.Data.MySqlClient;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Assembly.Extensions;
using Sudzekai.LinCtrl.Core;
using System.Net;
using System.Text.Json;

namespace DatabasePlugin
{
    internal sealed class DatabaseRequestHandler(
        DatabaseDispatcher dispatcher,
        RequestHandlerMetadata metadata,
        bool requiresAdministrator,
        Func<DatabaseDispatcher, HttpRequestDto, long, object?> execute) : IRequestHandler
    {
        public string Pattern => metadata.Pattern;
        public HttpRequestMethod HttpMethod => metadata.HttpMethod;
        public bool IsFiltering => true;
        public RequestHandlerMetadata Metadata => metadata;

        public HttpResponseDto Handle(HttpRequestDto request)
        {
            try
            {
                var subject = request.Claims.TryGetValue("sub", out var subjects) && subjects.Length == 1
                    ? subjects[0]
                    : null;
                var callerId = dispatcher.Authorize(subject, request.Claims.GetValueOrDefault("role"), requiresAdministrator);
                var result = execute(dispatcher, request, callerId);
                var statusCode = HttpMethod == HttpRequestMethod.Post ? HttpStatusCode.Created : HttpStatusCode.OK;
                return result == null ? HttpResponseDto.Create(statusCode) : HttpResponseDto.CreateJson(statusCode, result);
            }
            catch (DatabaseRequestException ex)
            {
                return HttpResponseDto.CreateError(ex.StatusCode, (int)ex.StatusCode, ex.Message);
            }
            catch (JsonException)
            {
                return HttpResponseDto.CreateError(HttpStatusCode.BadRequest, 400, "Некорректное тело JSON.");
            }
            catch (MySqlException ex) when (FindConstraintError(ex) is { } error)
            {
                var message = error.Number switch
                {
                    1062 => "Запись с таким уникальным значением уже существует.",
                    1217 or 1451 => "Запись используется другими данными.",
                    1216 or 1452 => "Связанная запись не найдена.",
                    _ => "Операция нарушает ограничение целостности данных."
                };
                return HttpResponseDto.CreateError(HttpStatusCode.Conflict, 409, message);
            }
        }

        private static MySqlException? FindConstraintError(MySqlException exception)
        {
            for (Exception? current = exception; current != null; current = current.InnerException)
            {
                if (current is MySqlException error
                    && (error.Number is 1062 or 1216 or 1217 or 1451 or 1452 || error.SqlState == "23000"))
                    return error;
            }

            return null;
        }
    }
}
