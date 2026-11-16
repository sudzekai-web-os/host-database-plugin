using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Commands
{
    public sealed record UpdateUserCommand(long Id, UserInput Input, long CallerId) : ICommand<UserView>;
}
