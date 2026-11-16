using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Services.History;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class DeleteHistoryEntryCommandHandler(HistoryService service) : ICommandHandler<DeleteHistoryEntryCommand, object?>
    {
        public object? Handle(DeleteHistoryEntryCommand command)
        {
            service.DeleteHistoryEntry(command.Id);
            return null;
        }
    }
}
