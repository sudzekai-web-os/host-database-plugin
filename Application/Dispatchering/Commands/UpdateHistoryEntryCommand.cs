using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Commands
{
    public sealed record UpdateHistoryEntryCommand(long Id, HistoryInput Input) : ICommand<object?>;
}
