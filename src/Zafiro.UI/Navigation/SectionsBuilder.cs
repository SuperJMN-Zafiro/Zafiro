using System.Reactive.Linq;
using System.Windows.Input;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.UI.Navigation;

public class SectionsBuilder(IServiceProvider provider)
{
    private readonly List<ISection> sections = new();

    private static ISection CreateSection<T>(string name, IServiceProvider provider, object? icon = null, bool isPrimary = true) where T : class
    {
        // Section content is no longer responsible for hosting a SectionScope.
        // We only use the section metadata and its RootType; content is unused here.
        var contentSection = Section.Content(name, Observable.Empty<T>(), icon, isPrimary);
        return contentSection;
    }

    public SectionsBuilder Add<T>(string name, object? icon = null, bool isPrimary = true) where T : class
    {
        sections.Add(CreateSection<T>(name, provider, icon, isPrimary));
        return this;
    }

    public IEnumerable<ISection> Build()
    {
        return sections;
    }

    public SectionsBuilder Separator(bool isPrimary = true)
    {
        sections.Add(Section.Separator(isPrimary));
        return this;
    }

    public SectionsBuilder Command(string name, ICommand command, object? icon, bool isPrimary = true)
    {
        sections.Add(Section.Command(name, command, icon, isPrimary));
        return this;
    }

    public SectionsBuilder Command(string name, Func<IServiceProvider, ICommand> createCommand, object? icon, bool isPrimary = true)
    {
        sections.Add(Section.Command(name, createCommand(provider), icon, isPrimary));
        return this;
    }
}