namespace DatabasePlugin
{
    internal sealed record HistoryView(long Id, long UserId, DateTime DateTime, string Action, string Summary);
}
