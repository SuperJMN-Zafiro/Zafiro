using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Zafiro.UI.Navigation.Sections;
using static Zafiro.UI.Navigation.Sections.Section;

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

public class SectionsBuilder(IServiceProvider provider)
{
    private readonly List<SectionBase> sections = new();
    
    private static SectionBase CreateSection<T>(string name, IServiceProvider provider) where T : notnull
    {
        return Create(name, () => new SectionScope(provider, typeof(T)));
    }
    
    public SectionsBuilder Add<T>(string name)
    {
        sections.Add(CreateSection<T>(name, provider));
        return this;
    }

    public IEnumerable<SectionBase> Build()
    {
        return sections;
    }
}