using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Services.Users;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class UpdateUserCommandHandler(UserService service) : ICommandHandler<UpdateUserCommand, UserView>
    {
        public UserView Handle(UpdateUserCommand command)
            => service.UpdateUser(command.Id, command.Input, command.CallerId);
    }
}
