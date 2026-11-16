using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Services.Users;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class GetUsersCommandHandler(UserService service) : ICommandHandler<GetUsersCommand, UserView[]>
    {
        public UserView[] Handle(GetUsersCommand command)
            => service.GetUsers(command.Page, command.Role);
    }
}
