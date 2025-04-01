using System;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Navigation;

namespace Zafiro.UI.Navigation
{
    /// <summary>
    /// Hosts a navigation context with its own scope
    /// </summary>
    public sealed class NavigationHost : INavigationHost
    {
        private readonly IDisposable scope;

        /// <summary>
        /// Navigator associated with this host
        /// </summary>
        public INavigator Navigator { get; }
        
        /// <summary>
        /// Command to create and navigate to the initial content
        /// </summary>
        public ReactiveCommand<Unit, Result<Unit>> Create { get; }

        /// <summary>
        /// Creates a new navigation host with an initial content factory
        /// </summary>
        /// <param name="typeResolverFactory">Factory to create a type resolver for this scope</param>
        /// <param name="initialContentFactory">Factory to create the initial content</param>
        public NavigationHost(Func<ITypeResolver> typeResolverFactory, Func<ITypeResolver, object> initialContentFactory)
        {
            var typeResolver1 =
                // Create the resolver for this scope
                typeResolverFactory();

            // If the resolver is also a scope, keep track of it for disposal
            if (typeResolver1 is IDisposable disposableResolver)
            {
                scope = disposableResolver;
            }
            
            // Try to get the navigator from the scoped resolver
            if (typeResolver1 is ScopedTypeResolver scopedResolver)
            {
                var existingNavigator = scopedResolver.ServiceProvider.GetService(typeof(INavigator)) as INavigator;
                Navigator = existingNavigator ?? new Navigator(typeResolver1);
            }
            else
            {
                // Fallback: create a new navigator
                Navigator = new Navigator(typeResolver1);
            }

            // Set up initial navigation
            Create = ReactiveCommand.CreateFromTask(() =>
            {
                return Navigator.Go(() => initialContentFactory(typeResolver1));
            });

            // Execute initial navigation
            Create.Execute().Subscribe();
        }
        
        /// <summary>
        /// Disposes the navigation host and its associated scope
        /// </summary>
        public void Dispose()
        {
            scope?.Dispose();
            
            // If Navigator implements IDisposable, dispose it too
            if (Navigator is IDisposable disposableNavigator)
            {
                disposableNavigator.Dispose();
            }
        }
    }
}