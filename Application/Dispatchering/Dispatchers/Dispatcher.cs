using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace DatabasePlugin.Application.Dispatchering.Dispatchers
{
    internal sealed class Dispatcher(IServiceProvider services) : IDispatcher
    {
        public TResult Execute<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
            => services.GetRequiredService<ICommandHandler<TCommand, TResult>>().Handle(command);
    }
}
