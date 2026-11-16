using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Services.Users;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class GetUserCommandHandler(UserService service) : ICommandHandler<GetUserCommand, UserView>
    {
        public UserView Handle(GetUserCommand command)
            => service.GetUser(command.Id, command.Role);
    }
}
