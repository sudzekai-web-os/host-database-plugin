namespace DatabasePlugin.Application.Dto
{
    public sealed record UserInput(string? Username, string? Password, int RoleId, bool? IsBlocked);
}
