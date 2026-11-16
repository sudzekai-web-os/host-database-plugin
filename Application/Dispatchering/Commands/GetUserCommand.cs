using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Commands
{
    public sealed record GetUserCommand(long Id, ClientRole Role) : ICommand<UserView>;
}
