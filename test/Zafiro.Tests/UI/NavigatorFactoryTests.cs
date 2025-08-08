using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Zafiro.UI.Navigation;
using Serilog;
using System.Threading.Tasks;

namespace Zafiro.Tests.UI;

public class NavigatorFactoryTests
{
    private class Page
    {
        public int Id { get; }
        public Page(int id) => Id = id;
    }

    private class AnotherPage
    {
    }

    [Fact]
    public async Task GoBack_uses_factory_to_create_new_instance()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var navigator = new Navigator(provider, Maybe<ILogger>.None, null);

        var counter = 0;
        Page? first = null;
        await navigator.Go(() =>
        {
            var page = new Page(++counter);
            first ??= page;
            return page;
        });

        await navigator.Go(() => new AnotherPage());
        await navigator.GoBack();

        var current = await navigator.Content.FirstAsync();
        var second = Assert.IsType<Page>(current);
        Assert.NotSame(first, second);
        Assert.Equal(2, counter);
    }
}
