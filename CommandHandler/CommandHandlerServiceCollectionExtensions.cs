using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace CommandHandler
{
    public static class CommandHandlerServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandHandling(this IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler>(provider => new CommandHandler(handlerType =>
                {
                    return provider.GetRequiredService(handlerType);
                }
            ));
            return services;
        }

        public static IServiceCollection AddHandlersFromAssemblyContainingType<T>(this IServiceCollection services)
            where T : class
        {
            var assembly = typeof(T).Assembly;
            IEnumerable<Type> types = assembly.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface);
            foreach (Type type in types)
            {
                Type[] typeInterfaces = type.GetInterfaces();
                foreach (Type typeInterface in typeInterfaces)
                {
                    if (typeInterface.IsGenericType && typeInterface.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                    {
                        services.AddScoped(typeInterface, type);
                    }
                }
            }
            return services;
        }
    }
}