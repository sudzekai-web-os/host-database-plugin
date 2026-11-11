using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    internal sealed record UserAccess(ClientRole Role, bool IsBlocked);
}
