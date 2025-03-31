using System.Diagnostics.CodeAnalysis;

namespace Zafiro.UI.Navigation;

/// <summary>
/// Implementation of ITypeResolver and ITypeWithParametersResolver based on registered factories
/// </summary>
public class FactoryTypeResolver : ITypeResolver, ITypeWithParametersResolver
{
    private readonly Dictionary<Type, Func<object>> factories = new();
    private readonly Dictionary<Type, Dictionary<Type, Delegate>> parameterizedFactories = new();

    [return: NotNull]
    public T Resolve<T>() where T : notnull
    {
        if (factories.TryGetValue(typeof(T), out var factory))
        {
            return (T)factory();
        }

        throw new InvalidOperationException($"No factory has been registered for type {typeof(T).Name}");
    }

    [return: NotNull]
    public T Resolve<T, TParam>(TParam parameter) where T : notnull
    {
        if (parameterizedFactories.TryGetValue(typeof(T), out var paramFactories) &&
            paramFactories.TryGetValue(typeof(TParam), out var factory))
        {
            if (factory is Func<TParam, T> typedFactory)
            {
                return typedFactory(parameter);
            }
        }

        throw new InvalidOperationException($"No factory has been registered for type {typeof(T).Name} with parameter of type {typeof(TParam).Name}");
    }

    /// <summary>
    /// Registers a factory for a type without parameters
    /// </summary>
    /// <typeparam name="T">Type to create</typeparam>
    /// <param name="factory">Factory that creates instances</param>
    public void Register<T>(Func<T> factory) where T : notnull
    {
        factories[typeof(T)] = () => factory();
    }

    /// <summary>
    /// Registers a factory for a type with a parameter
    /// </summary>
    /// <typeparam name="T">Type to create</typeparam>
    /// <typeparam name="TParam">Parameter type</typeparam>
    /// <param name="factory">Factory that creates instances</param>
    public void Register<T, TParam>(Func<TParam, T> factory) where T : notnull
    {
        if (!parameterizedFactories.TryGetValue(typeof(T), out var paramFactories))
        {
            paramFactories = new Dictionary<Type, Delegate>();
            parameterizedFactories[typeof(T)] = paramFactories;
        }

        paramFactories[typeof(TParam)] = factory;

        // We also register a factory without parameters that uses a default value
        // This allows resolving the type even without parameters
        factories[typeof(T)] = () => factory(default);
    }
}