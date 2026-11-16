namespace DatabasePlugin.Application.Dto
{
    public sealed record HistoryView(long Id, long UserId, DateTime DateTime, string Action, string Summary);
}
