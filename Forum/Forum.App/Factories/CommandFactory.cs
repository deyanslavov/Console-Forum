namespace Forum.App.Factories
{
	using Contracts;
    using System;
    using System.Linq;
    using System.Reflection;

    public class CommandFactory : ICommandFactory
	{
        private IServiceProvider serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ICommand CreateCommand(string commandName)
		{
            Type commandType;
            if (commandName.Contains("Command"))
            {
                commandType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(c => c.Name == commandName);
            }
            else
            {
                commandType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(c => c.Name == commandName + "Command");
            }
            
            if (commandType == null)
            {
                throw new InvalidOperationException("Command not found!");
            }

            if (!typeof(ICommand).IsAssignableFrom(commandType))
            {
                throw new InvalidOperationException($"{commandType} is not a command!");
            }

            ParameterInfo[] ctorParams = commandType.GetConstructors().First().GetParameters();
            object[] args = new object[ctorParams.Length];

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = this.serviceProvider.GetService(ctorParams[i].ParameterType);
            }

            return (ICommand)Activator.CreateInstance(commandType, args);
		}
	}
}
