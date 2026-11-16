using DatabasePlugin.Application.Dispatchering.Commands;

namespace DatabasePlugin.Application.Dispatchering.Handlers
{
    internal sealed class GetVersionCommandHandler : ICommandHandler<GetVersionCommand, string>
    {
        public string Handle(GetVersionCommand command)
            => typeof(GetVersionCommandHandler).Assembly.GetName().Version!.ToString(3);
    }
}
