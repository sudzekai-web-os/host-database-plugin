using System.Net;
using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Repositories.Roles;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class GetRoleCommandHandler(IRoleRepository roles) : ICommandHandler<GetRoleCommand, RoleView>
    {
        public RoleView Handle(GetRoleCommand command)
            => roles.GetRole(command.Id) ?? throw new DatabaseRequestException(HttpStatusCode.NotFound, "Роль не найдена.");
    }
}
