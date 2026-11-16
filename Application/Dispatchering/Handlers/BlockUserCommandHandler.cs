using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Services.Users;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class BlockUserCommandHandler(UserService service) : ICommandHandler<BlockUserCommand, object?>
    {
        public object? Handle(BlockUserCommand command)
        {
            service.BlockUser(command.Id, command.CallerId);
            return null;
        }
    }
}
