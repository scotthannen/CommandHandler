using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandHandler;

namespace UnitTestProject1
{
    [TestClass]
    public class CommandHandlerTests
    {
        [TestMethod]
        public async Task CommandHandler_Invokes_Correct_Handler()
        {
            var services = new ServiceCollection();
            services
                .AddCommandHandling()
                .AddHandlersFromAssemblyContainingType<AddNumberToListCommand>();

            var serviceProvider = services.BuildServiceProvider();
            var commandHandler = serviceProvider.GetRequiredService<ICommandHandler>();

            var list = new List<int>();
            var command = new AddNumberToListCommand(list, 1);

            // This is the non-generic ICommandHandler interface
            await commandHandler.HandleAsync(command);
            Assert.IsTrue(list.Contains(1));

            
        }
    }

    public class AddNumberToListCommand : ICommand
    {
        public AddNumberToListCommand(List<int> listOfNumbers, int numberToAdd)
        {
            ListOfNumbers = listOfNumbers;
            NumberToAdd = numberToAdd;
        }

        public List<int> ListOfNumbers { get; }
        public int NumberToAdd { get; }
    }

    public class AddNumberToListHandler : ICommandHandler<AddNumberToListCommand>
    {
        public Task HandleAsync(AddNumberToListCommand command)
        {
            command.ListOfNumbers.Add(command.NumberToAdd);
            return Task.CompletedTask;
        }
    }
}