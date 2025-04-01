using CSharpFunctionalExtensions;

namespace Zafiro.UI.Navigation
{
    /// <summary>
    /// Extension methods for INavigator
    /// </summary>
    public static class NavigatorExtensions
    {
        /// <summary>
        /// Navigate to a specific type
        /// </summary>
        /// <typeparam name="T">Type to navigate to</typeparam>
        /// <param name="navigator">The navigator</param>
        /// <returns>Operation result</returns>
        public static Task<Result<Unit>> Go<T>(this INavigator navigator) where T : notnull
        {
            return navigator.Go(typeof(T));
        }

        /// <summary>
        /// Navigate to a specific type with a parameter
        /// </summary>
        /// <typeparam name="T">Type to navigate to</typeparam>
        /// <typeparam name="TParam">Parameter type</typeparam>
        /// <param name="navigator">The navigator</param>
        /// <param name="parameter">Navigation parameter</param>
        /// <returns>Operation result</returns>
        public static Task<Result<Unit>> Go<T, TParam>(this INavigator navigator, TParam parameter) where T : notnull
        {
            return navigator.Go(typeof(T), parameter);
        }

        /// <summary>
        /// Navigate to a specific type with a nullable parameter
        /// </summary>
        /// <typeparam name="T">Type to navigate to</typeparam>
        /// <param name="navigator">The navigator</param>
        /// <param name="parameter">Optional navigation parameter</param>
        /// <returns>Operation result</returns>
        public static Task<Result<Unit>> Go<T>(this INavigator navigator, object? parameter) where T : notnull
        {
            return navigator.Go(typeof(T), parameter);
        }
    }
}