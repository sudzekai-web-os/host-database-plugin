using System.Net;

namespace DatabasePlugin.Application.Dto
{
    public sealed class DatabaseRequestException(HttpStatusCode statusCode, string message) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; } = statusCode;
    }
}
