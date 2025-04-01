using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.UI.Navigation;

public static class AddNavigation
{
    public static IServiceCollection RegisterSections(this IServiceCollection serviceCollection, Action<SectionsBuilder> configure)
    {
        serviceCollection.AddScoped<INavigator>(provider => new Navigator(provider, Maybe<ILogger>.None));
            
        return serviceCollection.AddSingleton<IEnumerable<SectionBase>>(provider =>
        {
            var sectionsBuilder = new SectionsBuilder(provider);
            configure(sectionsBuilder);
            return sectionsBuilder.Build();
        });
    }
}