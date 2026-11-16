using DatabasePlugin.Application.Dto;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Commands
{
    public sealed record GetHistoryCommand(Page Page) : ICommand<HistoryView[]>;
}
