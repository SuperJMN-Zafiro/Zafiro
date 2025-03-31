using System.Reactive;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Zafiro.UI.Navigation;

public sealed class NavigationHost : INavigationHost
{
    private readonly IServiceScope scope;

    public INavigator Navigator { get; }
    public ReactiveCommand<Unit, Result<Unit>> Create { get; }

    public NavigationHost(IServiceProvider parentProvider, Func<IServiceProvider, object> initialContentFactory)
    {
        // Create a scope to ensure that this NavigationViewModel and its dependencies are "local"
        scope = parentProvider.CreateScope();
        Navigator = scope.ServiceProvider.GetRequiredService<INavigator>();

        // Using the scope ensures that the initialContentFactory resolves its dependencies
        // (including INavigator) in the same context.
        Create = ReactiveCommand.CreateFromTask(() =>
            Navigator.Go(() => initialContentFactory(scope.ServiceProvider))
        );

        // Execute the command immediately to set the initial content
        Create.Execute().Subscribe();
    }

    public void Dispose() => scope.Dispose();
}