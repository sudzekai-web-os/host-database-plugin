using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Commands
{
    public sealed record CreateUserCommand(UserInput Input) : ICommand<UserView>;
}
