using System.Diagnostics.CodeAnalysis;

namespace Zafiro.UI.Navigation
{
    /// <summary>
    /// Interface for resolving types with parameters
    /// </summary>
    public interface ITypeWithParametersResolver
    {
        /// <summary>
        /// Resolves a type with a parameter
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <typeparam name="TParam">Parameter type</typeparam>
        /// <param name="parameter">Parameter for creation</param>
        /// <returns>Resolved instance</returns>
        [return: NotNull]
        T Resolve<T, TParam>(TParam parameter) where T : notnull;
    }
}