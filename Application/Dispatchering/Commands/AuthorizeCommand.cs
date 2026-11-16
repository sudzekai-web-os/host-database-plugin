using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Commands
{
    public sealed record AuthorizeCommand(string? Subject, string[]? Roles, bool RequiresAdministrator) : ICommand<(long Id, ClientRole Role)>;
}
