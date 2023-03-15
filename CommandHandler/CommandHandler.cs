using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace CommandHandler
{
    /// <inheritdoc cref="ICommandHandler"/>
    public class CommandHandler : ICommandHandler
    {
        private readonly Func<Type, object> _getHandler;

        public CommandHandler(Func<Type, object> getHandler)
        {
            _getHandler = getHandler;
        }

        public async Task HandleAsync(ICommand command)
        {
            var commandHandlerType = GetCommandHandlerType(command);
            var handler = _getHandler(commandHandlerType);
            await InvokeHandler(handler, command);
        }

        private Type GetCommandHandlerType(ICommand command)
        {
            return typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        }

        private async Task InvokeHandler(object handler, ICommand command)
        {
            var handlerMethod = handler.GetType().GetMethods()
                .Single(method => IsHandleMethod(method, command.GetType()));
            var task = (Task)handlerMethod.Invoke(handler, new object[] { command });
            await task.ConfigureAwait(false);
        }

        private bool IsHandleMethod(MethodInfo method, Type commandType)
        {
            if (method.Name != nameof(ICommandHandler.HandleAsync)
                || method.ReturnType != typeof(Task))
            {
                return false;
            }

            var parameters = method.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == commandType;
        }
    }
}