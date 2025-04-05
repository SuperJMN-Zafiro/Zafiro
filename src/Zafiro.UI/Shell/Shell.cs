using System.Reactive.Linq;
using ReactiveUI.SourceGenerators;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.UI.Shell;

public partial class Shell : ReactiveObject, IShell
{
    [Reactive] private IContentSection? selectedSection;

    public Shell(IEnumerable<Navigation.Sections.Section> sections)
    {
        Sections = sections;
        CurrentContent = this.WhenAnyValue(x => x.SelectedSection)
            .WhereNotNull()
            .Select(section => section.GetViewModel());
        SelectedSection = Sections.OfType<IContentSection>().FirstOrDefault();
    }

    public IEnumerable<Navigation.Sections.Section> Sections { get; }
    public IObservable<object?> CurrentContent { get; }

    public void GoToSection(string sectionName)
    {
        SelectedSection = Sections.OfType<IContentSection>().First(x => x.Name == sectionName);
    }
}