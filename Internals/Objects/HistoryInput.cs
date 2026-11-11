namespace DatabasePlugin
{
    internal sealed record HistoryInput(long UserId, DateTimeOffset? DateTime, string? Action, string? Summary);
}
