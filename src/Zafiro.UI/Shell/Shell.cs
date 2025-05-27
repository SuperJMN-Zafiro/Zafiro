using System.Reactive.Linq;
using ReactiveUI.SourceGenerators;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.UI.Shell;

public partial class Shell : ReactiveObject, IShell
{
    [Reactive] private IContentSection? selectedSection;

    public Shell(ShellProperties shellProperties, IEnumerable<ISection> sections)
    {
        Sections = sections;
        CurrentContent = this.WhenAnyObservable(x => x.SelectedSection!.Content);

        ContentHeader = CurrentContent
            .WhereNotNull()
            .Select(section => shellProperties.GetHeader(section))
            .Switch();

        SelectedSection = Sections.OfType<IContentSection>().FirstOrDefault();
        Header = shellProperties.Header;
    }

    public IObservable<object?> CurrentContent { get; }

    public object Header { get; set; }
    public IObservable<object> ContentHeader { get; }
    public IEnumerable<ISection> Sections { get; }

    public void GoToSection(string sectionName)
    {
        SelectedSection = Sections.OfType<IContentSection>().First(x => x.Name == sectionName);
    }
}