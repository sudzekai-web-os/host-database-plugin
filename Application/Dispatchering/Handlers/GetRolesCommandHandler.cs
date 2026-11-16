using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Repositories.Roles;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class GetRolesCommandHandler(IRoleRepository roles) : ICommandHandler<GetRolesCommand, RoleView[]>
    {
        public RoleView[] Handle(GetRolesCommand command)
            => roles.GetRoles();
    }
}
