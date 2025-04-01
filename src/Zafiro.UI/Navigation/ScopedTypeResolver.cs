using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Zafiro.UI.Navigation
{
    /// <summary>
    /// A type resolver that creates and maintains a DI scope
    /// </summary>
    public class ScopedTypeResolver : ITypeResolver, ITypeWithParametersResolver, IDisposable
    {
        private readonly IServiceScope scope;
        private readonly ServiceProviderTypeResolver resolver;
        private bool isDisposed;

        /// <summary>
        /// Creates a new scoped type resolver
        /// </summary>
        /// <param name="serviceProvider">The root service provider to create a scope from</param>
        public ScopedTypeResolver(IServiceProvider serviceProvider)
        {
            // Create a scope from the parent provider
            scope = serviceProvider.CreateScope();
            
            // Register this resolver in the scope's service provider
            if (scope.ServiceProvider is ServiceProviderExtensions.IServiceCollectionAccessor accessor)
            {
                // This is a hack to allow registering services after building the provider
                // It only works if the provider supports it
                accessor.ServiceCollection
                    .AddScoped<ITypeResolver>(provider => this)
                    .AddScoped<ITypeWithParametersResolver>(provider => this)
                    .AddScoped<INavigator>(provider => new Navigator(provider.GetRequiredService<ITypeResolver>()));
                
                // Rebuild the provider 
                accessor.RebuildProvider();
            }
            
            // Create the resolver that will delegate to the scope's provider
            resolver = new ServiceProviderTypeResolver(scope.ServiceProvider);
        }

        /// <summary>
        /// Resolves a type from the scope
        /// </summary>
        public object Resolve(Type type)
        {
            ThrowIfDisposed();
            return resolver.Resolve(type);
        }

        /// <summary>
        /// Resolves a type with a parameter from the scope
        /// </summary>
        public object Resolve(Type type, object parameter)
        {
            ThrowIfDisposed();
            return resolver.Resolve(type, parameter);
        }

        /// <summary>
        /// Generic convenience method for type resolution
        /// </summary>
        [return: NotNull]
        public T Resolve<T>() where T : notnull
        {
            ThrowIfDisposed();
            return resolver.Resolve<T>();
        }

        /// <summary>
        /// Generic convenience method for type resolution with parameter
        /// </summary>
        [return: NotNull]
        public T Resolve<T, TParam>(TParam parameter) where T : notnull
        {
            ThrowIfDisposed();
            return resolver.Resolve<T, TParam>(parameter);
        }

        /// <summary>
        /// Registers a factory for creating instances of a type with a parameter
        /// </summary>
        public void RegisterFactory<T, TParam>(Func<TParam, T> factory) where T : notnull
        {
            ThrowIfDisposed();
            resolver.RegisterFactory(factory);
        }

        /// <summary>
        /// Registers a factory that receives the service provider and a parameter
        /// </summary>
        public void RegisterFactory<T, TParam>(Func<IServiceProvider, TParam, T> factory) where T : notnull
        {
            ThrowIfDisposed();
            resolver.RegisterFactory(factory);
        }

        /// <summary>
        /// Gets the service provider from this scope
        /// </summary>
        public IServiceProvider ServiceProvider => scope.ServiceProvider;

        private void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(ScopedTypeResolver));
            }
        }

        /// <summary>
        /// Disposes the scope
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                scope.Dispose();
                isDisposed = true;
            }
        }
    }

    // Helper extensions for ServiceProvider
    public static class ServiceProviderExtensions
    {
        // Interface that could be implemented by a custom ServiceProvider
        // to allow adding services after building
        public interface IServiceCollectionAccessor
        {
            IServiceCollection ServiceCollection { get; }
            void RebuildProvider();
        }
    }
}