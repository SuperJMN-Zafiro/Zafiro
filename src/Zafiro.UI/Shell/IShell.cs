using Zafiro.UI.Navigation.Sections;

namespace Zafiro.UI.Shell;

public interface IShell
{
    public object Header { get; set; }
    IEnumerable<ISection> Sections { get; }
    IContentSection SelectedSection { get; set; }
    void GoToSection(string sectionName);
}