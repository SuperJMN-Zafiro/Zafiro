using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Zafiro.UI.Navigation
{
    /// <summary>
    /// Implementation of ITypeResolver and ITypeWithParametersResolver 
    /// that uses an IServiceProvider to resolve types
    /// </summary>
    public class ServiceProviderTypeResolver : ITypeWithParametersResolver
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Dictionary<Type, Dictionary<Type, Delegate>> parameterizedFactories = new();

        public ServiceProviderTypeResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Resolves a type
        /// </summary>
        /// <param name="type">Type to resolve</param>
        /// <returns>Resolved instance</returns>
        public object Resolve(Type type)
        {
            var service = serviceProvider.GetService(type);
            if (service == null)
            {
                throw new InvalidOperationException($"Could not resolve type {type.FullName} from the service provider.");
            }
            
            return service;
        }

        /// <summary>
        /// Resolves a type with a parameter
        /// </summary>
        /// <param name="type">Type to resolve</param>
        /// <param name="parameter">Parameter for creation</param>
        /// <returns>Resolved instance</returns>
        public object Resolve(Type type, object parameter)
        {
            var paramType = parameter.GetType();
            
            // First check if there's a registered factory for this type and parameter
            if (parameterizedFactories.TryGetValue(type, out var typeFactories) &&
                typeFactories.TryGetValue(paramType, out var factory))
            {
                // Use reflection to call the factory with the parameter
                try
                {
                    // For factories that take just the parameter
                    var factoryMethod = factory.Method;
                    if (factoryMethod.GetParameters().Length == 1)
                    {
                        return factory.DynamicInvoke(parameter);
                    }
                    // For factories that take the service provider and the parameter
                    else if (factoryMethod.GetParameters().Length == 2)
                    {
                        return factory.DynamicInvoke(serviceProvider, parameter);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to invoke factory for type {type.FullName} with parameter of type {paramType.FullName}", ex);
                }
            }
            
            // If no factory is found, fall back to the DefaultTypeResolver's approach
            var defaultResolver = new DefaultTypeResolver(serviceProvider);
            return defaultResolver.Resolve(type, parameter);
        }

        /// <summary>
        /// Generic convenience method for type resolution
        /// </summary>
        [return: NotNull]
        public T Resolve<T>() where T : notnull
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        /// Generic convenience method for type resolution with parameter
        /// </summary>
        [return: NotNull]
        public T Resolve<T, TParam>(TParam parameter) where T : notnull
        {
            return (T)Resolve(typeof(T), parameter);
        }

        /// <summary>
        /// Registers a factory for creating instances of a type with a parameter
        /// </summary>
        /// <typeparam name="T">Type to create</typeparam>
        /// <typeparam name="TParam">Parameter type</typeparam>
        /// <param name="factory">Factory function</param>
        public void RegisterFactory<T, TParam>(Func<TParam, T> factory) where T : notnull
        {
            if (!parameterizedFactories.TryGetValue(typeof(T), out var typeFactories))
            {
                typeFactories = new Dictionary<Type, Delegate>();
                parameterizedFactories[typeof(T)] = typeFactories;
            }
            
            typeFactories[typeof(TParam)] = factory;
        }

        /// <summary>
        /// Registers a factory that receives the service provider and a parameter
        /// </summary>
        /// <typeparam name="T">Type to create</typeparam>
        /// <typeparam name="TParam">Parameter type</typeparam>
        /// <param name="factory">Factory function that receives the service provider and a parameter</param>
        public void RegisterFactory<T, TParam>(Func<IServiceProvider, TParam, T> factory) where T : notnull
        {
            if (!parameterizedFactories.TryGetValue(typeof(T), out var typeFactories))
            {
                typeFactories = new Dictionary<Type, Delegate>();
                parameterizedFactories[typeof(T)] = typeFactories;
            }
            
            typeFactories[typeof(TParam)] = factory;
        }
    }
}