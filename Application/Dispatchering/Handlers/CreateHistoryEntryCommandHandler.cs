using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Services.History;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class CreateHistoryEntryCommandHandler(HistoryService service) : ICommandHandler<CreateHistoryEntryCommand, HistoryView>
    {
        public HistoryView Handle(CreateHistoryEntryCommand command)
            => service.CreateHistoryEntry(command.Input);
    }
}
