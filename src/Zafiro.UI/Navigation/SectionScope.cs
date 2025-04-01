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
        LoadInitial = ReactiveCommand.CreateFromTask(() => Navigator.Go(type));
        LoadInitial.Execute().Subscribe();
    }

    public ReactiveCommand<Unit,Result<Unit>> LoadInitial { get; }

    public void Dispose()
    {
        scope.Dispose();
    }

    public INavigator Navigator { get; }
}