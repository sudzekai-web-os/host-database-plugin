namespace DatabasePlugin
{
    internal sealed record UserInput(string? Username, string? Password, int RoleId, bool? IsBlocked);
}
