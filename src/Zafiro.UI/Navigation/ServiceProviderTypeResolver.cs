using System.Diagnostics.CodeAnalysis;

namespace Zafiro.UI.Navigation.Zafiro.UI.Navigation;

/// <summary>
/// Implementation of ITypeResolver and ITypeWithParametersResolver 
/// that uses an IServiceProvider to resolve types
/// </summary>
public class ServiceProviderTypeResolver : ITypeResolver, ITypeWithParametersResolver
{
    private readonly IServiceProvider serviceProvider;
    private readonly Dictionary<Type, Dictionary<Type, Delegate>> parameterizedFactories = new();

    public ServiceProviderTypeResolver(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    [return: NotNull]
    public T Resolve<T>() where T : notnull
    {
        var service = serviceProvider.GetService(typeof(T));
        if (service == null)
        {
            throw new InvalidOperationException($"Could not resolve type {typeof(T).FullName} from the service provider.");
        }
            
        return (T)service;
    }

    [return: NotNull]
    public T Resolve<T, TParam>(TParam parameter) where T : notnull
    {
        // First check if there's a registered factory for this type and parameter
        if (parameterizedFactories.TryGetValue(typeof(T), out var typeFactories) &&
            typeFactories.TryGetValue(typeof(TParam), out var factory))
        {
            if (factory is Func<TParam, T> typedFactory)
            {
                return typedFactory(parameter);
            }
            else if (factory is Func<IServiceProvider, TParam, T> serviceProviderFactory)
            {
                return serviceProviderFactory(serviceProvider, parameter);
            }
        }
            
        // If no factory is found, fall back to the DefaultTypeResolver's approach
        var defaultResolver = new DefaultTypeResolver(serviceProvider);
        return defaultResolver.Resolve<T, TParam>(parameter);
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