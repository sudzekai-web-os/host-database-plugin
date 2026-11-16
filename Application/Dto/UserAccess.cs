using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dto
{
    public sealed record UserAccess(ClientRole Role, bool IsBlocked);
}
