using System;

namespace Zafiro.UI.Navigation
{
    /// <summary>
    /// Interface for resolving types
    /// </summary>
    public interface ITypeResolver
    {
        /// <summary>
        /// Resolves a type
        /// </summary>
        /// <param name="type">Type to resolve</param>
        /// <returns>Resolved instance</returns>
        object Resolve(Type type);
    }
}