using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Services.History;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class GetHistoryCommandHandler(HistoryService service) : ICommandHandler<GetHistoryCommand, HistoryView[]>
    {
        public HistoryView[] Handle(GetHistoryCommand command)
            => service.GetHistory(command.Page);
    }
}
