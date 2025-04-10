using Zafiro.UI.Navigation.Sections;

namespace Zafiro.UI.Shell;

public interface IShell
{
    IEnumerable<ISection> Sections { get; }
    IContentSection SelectedSection { get; set; }
    void GoToSection(string sectionName);
}