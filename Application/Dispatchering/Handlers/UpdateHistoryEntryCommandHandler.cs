using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Services.History;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class UpdateHistoryEntryCommandHandler(HistoryService service) : ICommandHandler<UpdateHistoryEntryCommand, object?>
    {
        public object? Handle(UpdateHistoryEntryCommand command)
        {
            service.UpdateHistoryEntry(command.Id, command.Input);
            return null;
        }
    }
}
