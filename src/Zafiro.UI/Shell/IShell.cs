using Zafiro.UI.Navigation.Sections;

namespace Zafiro.UI.Shell;

public interface IShell
{
    IEnumerable<Navigation.Sections.Section> Sections { get; }
    IContentSection SelectedSection { get; set; }
    void GoToSection(string sectionName);
}