using DatabasePlugin.Application.Dispatchering.Commands;

namespace DatabasePlugin.Application.Dispatchering.Dispatchers
{
    public interface IDispatcher
    {
        TResult Execute<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;
    }
}
