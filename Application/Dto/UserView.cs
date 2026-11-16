using System.Text.Json.Serialization;

namespace DatabasePlugin.Application.Dto
{
    public sealed record UserView(long Id, string Username,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? RoleId = null,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? RoleName = null,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] bool? IsBlocked = null);
}
