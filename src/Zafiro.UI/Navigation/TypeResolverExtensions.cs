using System.Diagnostics.CodeAnalysis;

namespace Zafiro.UI.Navigation.Zafiro.UI.Navigation;

/// <summary>
/// Extension methods for ITypeResolver
/// </summary>
public static class TypeResolverExtensions
{
    /// <summary>
    /// Resolves a type
    /// </summary>
    /// <typeparam name="T">Type to resolve</typeparam>
    /// <param name="resolver">The resolver</param>
    /// <returns>Resolved instance</returns>
    [return: NotNull]
    public static T Resolve<T>(this ITypeResolver resolver) where T : notnull
    {
        var instance = resolver.Resolve(typeof(T));
        if (instance == null)
        {
            throw new InvalidOperationException($"Failed to resolve type {typeof(T).FullName}");
        }
            
        return (T)instance;
    }

    /// <summary>
    /// Resolves a type with a parameter
    /// </summary>
    /// <typeparam name="T">Type to resolve</typeparam>
    /// <typeparam name="TParam">Parameter type</typeparam>
    /// <param name="resolver">The resolver</param>
    /// <param name="parameter">Parameter for creation</param>
    /// <returns>Resolved instance</returns>
    [return: NotNull]
    public static T Resolve<T, TParam>(this ITypeWithParametersResolver resolver, TParam parameter) where T : notnull
    {
        var instance = resolver.Resolve(typeof(T), parameter);
        if (instance == null)
        {
            throw new InvalidOperationException($"Failed to resolve type {typeof(T).FullName} with parameter of type {typeof(TParam).FullName}");
        }
            
        return (T)instance;
    }
}