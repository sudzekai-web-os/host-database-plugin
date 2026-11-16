using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Services.Users;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class CreateUserCommandHandler(UserService service) : ICommandHandler<CreateUserCommand, UserView>
    {
        public UserView Handle(CreateUserCommand command)
            => service.CreateUser(command.Input);
    }
}
