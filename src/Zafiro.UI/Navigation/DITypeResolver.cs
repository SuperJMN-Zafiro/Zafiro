using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Zafiro.UI.Navigation
{
    /// <summary>
    /// Implementation of ITypeResolver and ITypeWithParametersResolver using Microsoft.Extensions.DependencyInjection
    /// </summary>
    public class DITypeResolver : ITypeResolver, ITypeWithParametersResolver
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Dictionary<Type, Dictionary<Type, Delegate>> factories = new();

        public DITypeResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [return: NotNull]
        public T Resolve<T>() where T : notnull
        {
            return serviceProvider.GetRequiredService<T>();
        }

        [return: NotNull]
        public T Resolve<T, TParam>(TParam parameter) where T : notnull
        {
            // Look for a registered factory for this type and parameter
            if (factories.TryGetValue(typeof(T), out var paramFactories) &&
                paramFactories.TryGetValue(typeof(TParam), out var factory))
            {
                if (factory is Func<IServiceProvider, TParam, T> typedFactory)
                {
                    return typedFactory(serviceProvider, parameter);
                }
            }

            // If there's no registered factory, we try to resolve using IServiceProvider
            // This can work if a type with a factory that handles the parameters has been registered
            return serviceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Registers a factory to create instances of a type with a parameter
        /// </summary>
        /// <typeparam name="T">Type to create</typeparam>
        /// <typeparam name="TParam">Parameter type</typeparam>
        /// <param name="factory">Factory that creates instances</param>
        public void RegisterFactory<T, TParam>(Func<IServiceProvider, TParam, T> factory) where T : notnull
        {
            if (!factories.TryGetValue(typeof(T), out var paramFactories))
            {
                paramFactories = new Dictionary<Type, Delegate>();
                factories[typeof(T)] = paramFactories;
            }

            paramFactories[typeof(TParam)] = factory;
        }
    }
}