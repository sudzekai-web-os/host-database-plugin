using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Commands
{
    public sealed record GetHistoryEntryCommand(long Id) : ICommand<HistoryView>;
}
