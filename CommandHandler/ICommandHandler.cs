using System.Threading.Tasks;


namespace CommandHandler
{
    public interface ICommandHandler
    {
        Task HandleAsync(ICommand command);
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }
}