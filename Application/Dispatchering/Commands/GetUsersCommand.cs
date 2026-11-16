using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Commands
{
    public sealed record GetUsersCommand(Page Page, ClientRole Role) : ICommand<UserView[]>;
}
