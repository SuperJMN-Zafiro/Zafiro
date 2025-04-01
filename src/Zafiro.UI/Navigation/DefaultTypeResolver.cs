using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Zafiro.UI.Navigation;

/// <summary>
/// Default implementation of ITypeResolver and ITypeWithParametersResolver
/// that uses reflection to create instances with parameters
/// </summary>
public class DefaultTypeResolver : ITypeResolver, ITypeWithParametersResolver
{
    private readonly IServiceProvider serviceProvider;
    private readonly Dictionary<Type, Dictionary<Type, Delegate>> parameterizedFactories = new();

    public DefaultTypeResolver(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    [return: NotNull]
    public T Resolve<T>() where T : notnull
    {
        var type = typeof(T);
            
        // Try to create the instance using the service provider
        if (serviceProvider != null)
        {
            var resolved = serviceProvider.GetService(type);
            if (resolved != null)
            {
                return (T)resolved;
            }
        }
            
        // If we couldn't resolve it, try to create an instance
        try
        {
            return (T)Activator.CreateInstance(type);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create an instance of {type.FullName}", ex);
        }
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
        }
            
        // Try to find a constructor that accepts the parameter
        var type = typeof(T);
        var paramType = typeof(TParam);
            
        // Get all constructors from the most specific to the most general
        var constructors = type.GetConstructors();
            
        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();
                
            // Look for a constructor with one parameter of the correct type
            if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(paramType))
            {
                return (T)constructor.Invoke(new object[] { parameter });
            }
                
            // Look for constructors where we might be able to resolve all parameters except one
            if (parameters.Length > 1)
            {
                bool canResolve = true;
                bool hasParamSlot = false;
                var args = new object[parameters.Length];
                    
                for (int i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                        
                    // If the parameter is assignable from TParam and we haven't used it yet
                    if (param.ParameterType.IsAssignableFrom(paramType) && !hasParamSlot)
                    {
                        args[i] = parameter;
                        hasParamSlot = true;
                    }
                    // Otherwise try to resolve it from the service provider
                    else if (serviceProvider != null)
                    {
                        var service = serviceProvider.GetService(param.ParameterType);
                        if (service != null)
                        {
                            args[i] = service;
                        }
                        else
                        {
                            canResolve = false;
                            break;
                        }
                    }
                    else
                    {
                        canResolve = false;
                        break;
                    }
                }
                    
                if (canResolve && hasParamSlot)
                {
                    return (T)constructor.Invoke(args);
                }
            }
        }
            
        // If no suitable constructor was found, try to create a default instance
        // and set a property with the parameter
        var instance = Resolve<T>();
            
        // Look for a property that matches the parameter type
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.PropertyType.IsAssignableFrom(paramType) && property.CanWrite)
            {
                property.SetValue(instance, parameter);
                return instance;
            }
        }
            
        // If we couldn't find a property, just return the default instance
        return instance;
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
}