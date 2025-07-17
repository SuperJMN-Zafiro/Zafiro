using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Zafiro.UI.Navigation;
using Serilog;
using System.Threading.Tasks;

namespace Zafiro.Tests.UI;

public class NavigatorBookmarkTests
{
    private class Page
    {
    }

    private class AnotherPage
    {
    }

    [Fact]
    public async Task GoBackToBookmark_removes_intermediate_history()
    {
        var services = new ServiceCollection();
        services.AddTransient<Page>();
        services.AddTransient<AnotherPage>();
        var provider = services.BuildServiceProvider();
        var navigator = new Navigator(provider, Maybe<ILogger>.None);

        await navigator.Go(typeof(Page));
        var bookmark = navigator.CreateBookmark();
        await navigator.Go(typeof(AnotherPage));
        await navigator.Go(typeof(Page));

        await navigator.GoBackTo(bookmark);

        var current = await navigator.Content.FirstAsync();
        Assert.IsType<Page>(current);
        var result = await navigator.GoBack();
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Named_bookmark_can_be_used_and_removed()
    {
        var services = new ServiceCollection();
        services.AddTransient<Page>();
        services.AddTransient<AnotherPage>();
        var provider = services.BuildServiceProvider();
        var navigator = new Navigator(provider, Maybe<ILogger>.None);

        await navigator.Go(typeof(Page));
        navigator.CreateBookmark("wizard");
        await navigator.Go(typeof(AnotherPage));
        await navigator.Go(typeof(Page));

        await navigator.GoBackTo("wizard");

        var result = await navigator.GoBackTo("wizard");
        Assert.True(result.IsFailure);
    }
}
