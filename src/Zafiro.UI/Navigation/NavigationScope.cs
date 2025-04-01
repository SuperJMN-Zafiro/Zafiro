using Microsoft.Extensions.DependencyInjection;

namespace Zafiro.UI.Navigation;

public class NavigationScope : IDisposable
{
    private readonly IServiceScope scope;
    private readonly IServiceProvider serviceProvider;

    public NavigationScope(IServiceProvider rootProvider)
    {
        scope = rootProvider.CreateScope();
        // Guardamos una referencia al ServiceProvider del scope
        serviceProvider = scope.ServiceProvider;
    }

    public T Resolve<T>() where T : notnull
    {
        return serviceProvider.GetRequiredService<T>();
    }

    public IServiceProvider ServiceProvider => serviceProvider;

    public INavigator Navigator => serviceProvider.GetRequiredService<INavigator>();

    public void Dispose()
    {
        scope.Dispose();
    }
}