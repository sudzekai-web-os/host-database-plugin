using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Services.Authorization;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class AuthorizeCommandHandler(AuthorizationService service) : ICommandHandler<AuthorizeCommand, (long Id, ClientRole Role)>
    {
        public (long Id, ClientRole Role) Handle(AuthorizeCommand command)
            => service.Authorize(command.Subject, command.Roles, command.RequiresAdministrator);
    }
}
