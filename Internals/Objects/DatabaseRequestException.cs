using System.Net;

namespace DatabasePlugin
{
    internal sealed class DatabaseRequestException(HttpStatusCode statusCode, string message) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; } = statusCode;
    }
}
