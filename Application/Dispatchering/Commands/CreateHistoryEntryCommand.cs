using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Commands
{
    public sealed record CreateHistoryEntryCommand(HistoryInput Input) : ICommand<HistoryView>;
}
