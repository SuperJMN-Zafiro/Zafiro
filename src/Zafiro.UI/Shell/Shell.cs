using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveUI.SourceGenerators;
using Zafiro.Reactive;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.UI.Shell;

[PublicAPI]
public partial class Shell : ReactiveObject, IShell
{
    [Reactive] private IContentSection? selectedSection;

    public Shell(ShellProperties shellProperties, IEnumerable<ISection> sections)
    {
        Sections = sections;

        Content = this.WhenAnyValue(x => x.SelectedSection)
            .WhereNotNull()
            .DistinctUntilChanged()
            .Select(ret => ret.Content)
            .Switch()
            .ReplayLastActive();
        
        ContentHeader = Content
            .WhereNotNull()
            .Select(section => shellProperties.GetHeader(section))
            .Switch()
            .ReplayLastActive();
        
        SelectedSection = Sections.OfType<IContentSection>().FirstOrDefault();
        Header = shellProperties.Header;
    }

    public IObservable<object?> Content { get; }
    public object Header { get; set; }
    public IObservable<object?> ContentHeader { get; }
    public IEnumerable<ISection> Sections { get; }
    
    public void GoToSection(string sectionName)
    {
        SelectedSection = Sections.OfType<IContentSection>().First(x => x.Name == sectionName);
    }
}