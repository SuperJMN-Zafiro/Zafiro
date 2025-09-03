namespace Zafiro.UI.Shell;

public interface ISectionActions
{
    IObservable<string> GoToSectionRequests { get; }
    void RequestGoToSection(string sectionName);
}