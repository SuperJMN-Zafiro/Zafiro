using System.Windows.Input;

namespace Zafiro.UI.Navigation;

public class SectionsBuilder(IServiceProvider provider)
{
    private readonly List<Sections.Section> sections = new();

    private static Sections.Section CreateSection<T>(string name, object? icon, IServiceProvider provider, bool isPrimary) where T : notnull
    {
        return Sections.Section.Content(name, () => new SectionScope(provider, typeof(T)), icon, isPrimary);
    }
    
    public SectionsBuilder Add<T>(string name, object? icon, bool isPrimary = true) where T : notnull
    {
        sections.Add(CreateSection<T>(name, icon, provider, isPrimary));
        return this;
    }

    public IEnumerable<Sections.Section> Build()
    {
        return sections;
    }

    public SectionsBuilder Separator(bool isPrimary = true)
    {
        sections.Add(Sections.Section.Separator(isPrimary));
        return this;
    }
    
    public SectionsBuilder Command(string name, ICommand command, object? icon, bool isPrimary = true)
    {
        sections.Add(Sections.Section.Command(name, command, icon, isPrimary));
        return this;
    }
}