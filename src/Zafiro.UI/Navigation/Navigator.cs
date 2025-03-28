using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Navigation;

public partial class Navigator : ReactiveObject, INavigator
{
    private readonly Maybe<ITypeResolver> resolver;
    private readonly ObservableStack<Func<INavigator, Task<object>>> stack = new();
    
    public Navigator(Maybe<ITypeResolver> resolver)
    {
        this.resolver = resolver;
        Back = EnhancedCommand.Create(ReactiveCommand.CreateFromTask(async () => await GoBack(), stack.Count.Select(b => b > 1)));
    }

    private async Task GoBack()
    {
        if (stack.Count.Value <= 1)
        {
            return;
        }

        stack.Pop();
        var previous = stack.Top.Value;
        Content = await previous(this);
    }

    public IEnhancedCommand Back { get; }

    public async Task Go(Func<INavigator, Task<object>> target)
    {
        stack.Push(target);
        Content = await target(this);
    }
    
    public async Task Go(Func<Task<object>> target)
    {
        stack.Push(async _ => await target());
        Content = await target();
    }

    public Task Go(Func<INavigator, object> target)
    {
        return Go(nav => Task.FromResult(target(nav)));
    }

    public Task Go<T>()
    {
        return resolver.Execute(r => Go(() => r.Resolve<T>()));
    }

    public Task Go(Func<object> target)
    {
        return Go(() => Task.FromResult(target()));
    }

    [Reactive]
    private object? content;
}