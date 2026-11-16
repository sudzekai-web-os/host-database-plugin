using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Repositories.Users
{
    internal sealed record SystemUser(string Username, string PasswordHash, ClientRole Role, bool IsBlocked);
}
