using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Commands
{
    public sealed record BlockUserCommand(long Id, long CallerId) : ICommand<object?>;
}
