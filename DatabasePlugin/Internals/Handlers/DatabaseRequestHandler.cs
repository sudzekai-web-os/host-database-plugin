using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dto;
using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using System.Text.Json.Serialization.Metadata;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Assembly.Extensions;
using Sudzekai.LinCtrl.Core;
using System.Net;
using System.Text.Json;

namespace DatabasePlugin
{
    internal abstract class DatabaseRequestHandler(IDispatcher dispatcher) : IRequestHandler
    {
        public abstract string Pattern { get; }

        public abstract HttpRequestMethod HttpMethod { get; }

        public abstract HandlerMetadata Metadata { get; }

        public HttpResponseDto Handle(HttpRequestDto request)
        {
            try
            {
                var subject = request.Claims.TryGetValue("sub", out var subjects) && subjects.Length == 1
                    ? subjects[0]
                    : null;
                var caller = dispatcher.Execute<AuthorizeCommand, (long Id, ClientRole Role)>(new(subject, request.Claims.GetValueOrDefault("role"), HttpMethod != HttpRequestMethod.Get));
                var result = Execute(dispatcher, request, caller);
                var statusCode = HttpMethod == HttpRequestMethod.Post ? HttpStatusCode.Created : HttpStatusCode.OK;
                return result == null
                    ? HttpResponseDto.Create(statusCode).As(ResponseContentType.ApplicationJson)
                    : HttpResponseDto.Create(statusCode).AsJson(result);
            }
            catch (DatabaseRequestException ex)
            {
                return HttpResponseDto.CreateError(ex.StatusCode, (int)ex.StatusCode, ex.Message);
            }
            catch (JsonException)
            {
                return HttpResponseDto.CreateError(HttpStatusCode.BadRequest, 400, "Некорректное тело JSON.");
            }
        }

        protected abstract object? Execute(IDispatcher dispatcher, HttpRequestDto request, (long Id, ClientRole Role) caller);

        protected static long GetId(HttpRequestDto request)
            => long.TryParse(request.RouteValues.GetValueOrDefault("id"), NumberStyles.None, CultureInfo.InvariantCulture, out var id) && id > 0
                ? id
                : throw new DatabaseRequestException(HttpStatusCode.BadRequest, "Укажите положительный идентификатор.");

        protected static Page GetPage(HttpRequestDto request)
        {
            var query = QueryHelpers.ParseQuery(request.QueryString);
            var limit = query.TryGetValue("limit", out var limitValue) ? ParsePageValue(limitValue.ToString(), 1, 200) : 50;
            var offset = query.TryGetValue("offset", out var offsetValue) ? ParsePageValue(offsetValue.ToString(), 0, int.MaxValue) : 0;
            return new(limit, offset);
        }

        protected static int ParsePageValue(string value, int minimum, int maximum)
            => int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var number) && number >= minimum && number <= maximum
                ? number
                : throw new DatabaseRequestException(HttpStatusCode.BadRequest, "Некорректные параметры limit или offset.");

        protected static T ReadBody<T>(HttpRequestDto request, JsonTypeInfo<T> typeInfo) where T : class
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
