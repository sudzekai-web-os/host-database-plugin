using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Repositories.History;
using System.Net;
using System.Text;

namespace DatabasePlugin.Application.Services.History
{
    internal sealed class HistoryService(IHistoryRepository repository)
    {
        public HistoryView[] GetHistory(Page page)
            => repository.GetHistory(page);

        public HistoryView GetHistoryEntry(long id)
            => repository.GetHistoryEntry(id) ?? throw NotFound("Запись истории не найдена.");

        public HistoryView CreateHistoryEntry(HistoryInput input)
        {
            ValidateHistory(input);
            return repository.CreateHistoryEntry(input, input.DateTime?.UtcDateTime ?? DateTime.UtcNow);
        }

        public void UpdateHistoryEntry(long id, HistoryInput input)
        {
            ValidateHistory(input);
            if (!repository.UpdateHistoryEntry(id, input))
                throw NotFound("Запись истории не найдена.");
        }

        public void DeleteHistoryEntry(long id)
        {
            if (!repository.DeleteHistoryEntry(id))
                throw NotFound("Запись истории не найдена.");
        }

        private static void ValidateHistory(HistoryInput input)
        {
            if (input.UserId <= 0 || string.IsNullOrWhiteSpace(input.Action) || input.Action.Length > 100
                || string.IsNullOrWhiteSpace(input.Summary) || Encoding.UTF8.GetByteCount(input.Summary) > 65535
                || input.DateTime?.UtcDateTime.Year < 1000)
                throw BadRequest("Укажите пользователя, действие до 100 символов и непустое описание до 65 535 байт.");
        }

        private static DatabaseRequestException BadRequest(string message)
            => new(HttpStatusCode.BadRequest, message);

        private static DatabaseRequestException NotFound(string message)
            => new(HttpStatusCode.NotFound, message);
    }
}
