using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Services.History;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class GetHistoryEntryCommandHandler(HistoryService service) : ICommandHandler<GetHistoryEntryCommand, HistoryView>
    {
        public HistoryView Handle(GetHistoryEntryCommand command)
            => service.GetHistoryEntry(command.Id);
    }
}
