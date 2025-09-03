using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace Zafiro.UI.Navigation;

public sealed class SectionScope : ISectionScope
{
    private readonly IServiceScope scope;

    public SectionScope(IServiceProvider provider, Type type)
    {
        scope = provider.CreateScope();
        Navigator = scope.ServiceProvider.GetRequiredService<INavigator>();

        // Initialize command is idempotent: it runs only once successfully per section scope
        LoadInitial = ReactiveCommand.CreateFromTask(
            async () =>
            {
                var result = await Navigator.Go(type);
                if (result.IsSuccess)
                {
                    IsInitialized = true;
                }

                return result;
            },
            Observable.Defer(() => Observable.Return(!IsInitialized))
        );
    }

    public ReactiveCommand<Unit, Result<Unit>> LoadInitial { get; }

    public bool IsInitialized { get; private set; }

    public void Dispose()
    {
        scope.Dispose();
    }

    public INavigator Navigator { get; }
}