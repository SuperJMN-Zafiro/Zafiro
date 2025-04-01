namespace Zafiro.UI.Navigation;

/// <summary>
/// Interface for resolving types with parameters
/// </summary>
public interface ITypeWithParametersResolver : ITypeResolver
{
    /// <summary>
    /// Resolves a type with a parameter
    /// </summary>
    /// <param name="type">Type to resolve</param>
    /// <param name="parameter">Parameter for creation</param>
    /// <returns>Resolved instance</returns>
    object Resolve(Type type, object parameter);
}