using DatabasePlugin.Application.Context;
using DatabasePlugin.Application.Dto;
using System.Data.Common;

namespace DatabasePlugin.Application.Repositories.History
{
    internal sealed class HistoryRepository(IDbContext db) : IHistoryRepository
    {
        public HistoryView[] GetHistory(Page page)
            => db.Query("""
                SELECT id, user_id, datetime, action, summary FROM change_history
                ORDER BY id DESC LIMIT @limit OFFSET @offset
                """, ReadHistory, ("@limit", page.Limit), ("@offset", page.Offset));

        public HistoryView? GetHistoryEntry(long id)
            => db.Query("SELECT id, user_id, datetime, action, summary FROM change_history WHERE id = @id",
                ReadHistory, ("@id", id)).SingleOrDefault();

        public HistoryView CreateHistoryEntry(HistoryInput input, DateTime dateTime)
        {
            var id = db.Insert(
                """
                INSERT INTO change_history (user_id, datetime, action, summary)
                VALUES (@userId, @datetime, @action, @summary)
                """, ("@userId", input.UserId), ("@datetime", dateTime),
                ("@action", input.Action), ("@summary", input.Summary));
            return new(id, input.UserId, dateTime, input.Action!, input.Summary!);
        }

        public bool UpdateHistoryEntry(long id, HistoryInput input)
            => db.Execute("""
                UPDATE change_history SET user_id = @userId, datetime = COALESCE(@datetime, datetime),
                    action = @action, summary = @summary
                WHERE id = @id
                """, ("@id", id), ("@userId", input.UserId), ("@datetime", input.DateTime?.UtcDateTime),
                ("@action", input.Action), ("@summary", input.Summary)) != 0;

        public bool DeleteHistoryEntry(long id)
            => db.Execute("DELETE FROM change_history WHERE id = @id", ("@id", id)) != 0;

        private static HistoryView ReadHistory(DbDataReader reader)
            => new(reader.GetInt64(0), reader.GetInt64(1), DateTime.SpecifyKind(reader.GetDateTime(2), DateTimeKind.Utc),
                reader.GetString(3), reader.GetString(4));
    }
}
