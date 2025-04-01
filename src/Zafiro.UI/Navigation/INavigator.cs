using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using System.Reactive;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Navigation
{
    /// <summary>
    /// Interface for navigation services
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// Current content displayed by the navigator
        /// </summary>
        IObservable<object?> Content { get; }

        /// <summary>
        /// Command to navigate backwards
        /// </summary>
        IEnhancedCommand<Unit, Result<Unit>> Back { get; }

        /// <summary>
        /// Navigate to content produced by the factory
        /// </summary>
        /// <param name="factory">Function that creates the content</param>
        /// <returns>Operation result</returns>
        Task<Result<Unit>> Go(Func<object> factory);
        
        /// <summary>
        /// Navigate to a specific type
        /// </summary>
        /// <param name="type">Type to navigate to</param>
        /// <returns>Operation result</returns>
        Task<Result<Unit>> Go(Type type);
        
        /// <summary>
        /// Navigate to a specific type with a parameter
        /// </summary>
        /// <param name="type">Type to navigate to</param>
        /// <param name="parameter">Navigation parameter (can be null)</param>
        /// <returns>Operation result</returns>
        Task<Result<Unit>> Go(Type type, object? parameter);

        /// <summary>
        /// Navigate backwards
        /// </summary>
        /// <returns>Operation result</returns>
        Task<Result<Unit>> GoBack();
    }
}