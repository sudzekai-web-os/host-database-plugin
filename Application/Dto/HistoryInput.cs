namespace DatabasePlugin.Application.Dto
{
    public sealed record HistoryInput(long UserId, DateTimeOffset? DateTime, string? Action, string? Summary);
}
