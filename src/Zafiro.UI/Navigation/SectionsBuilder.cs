using System.Windows.Input;
using CSharpFunctionalExtensions;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.UI.Navigation;

public class SectionsBuilder(IServiceProvider provider)
{
    private readonly List<SectionBase> sections = new();
    
    private static SectionBase CreateSection<T>(string name, Maybe<object> icon, IServiceProvider provider, bool isPrimary) where T : notnull
    {
        return Sections.Section.Create(name, () => new SectionScope(provider, typeof(T)), icon, isPrimary);
    }
    
    public SectionsBuilder Add<T>(string name, Maybe<object> icon, bool isPrimary = true) where T : notnull
    {
        sections.Add(CreateSection<T>(name, icon, provider, isPrimary));
        return this;
    }

    public IEnumerable<SectionBase> Build()
    {
        return sections;
    }

    public SectionsBuilder Separator(bool isPrimary = true)
    {
        sections.Add(Sections.Section.Separator(isPrimary));
        return this;
    }
    
    public SectionsBuilder Command(string name, ICommand command, Maybe<object> icon, bool isPrimary = true)
    {
        sections.Add(Sections.Section.Command(name, command, icon, isPrimary));
        return this;
    }
}