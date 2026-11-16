using DatabasePlugin.Application.Dto;

namespace DatabasePlugin.Application.Repositories.History
{
    internal interface IHistoryRepository
    {
        HistoryView[] GetHistory(Page page);

        HistoryView? GetHistoryEntry(long id);

        HistoryView CreateHistoryEntry(HistoryInput input, DateTime dateTime);

        bool UpdateHistoryEntry(long id, HistoryInput input);

        bool DeleteHistoryEntry(long id);
    }
}
