using System.Windows.Input;

namespace Zafiro.UI.Navigation.Sections;

public class Section
{
    public bool IsPrimary { get; init; } = true;
    
    public static IContentSection Content<T>(string name, Func<T> getViewModel, object? icon, bool isPrimary = true)
    {
        return new ContentSection<T>(name, getViewModel, icon)
        {
            IsPrimary = isPrimary,
        };
    }
    
    public static ICommandSection Command(string name, ICommand command, object? icon, bool isPrimary = true)
    {
        return new CommandSection(name, command, icon)
        {
            IsPrimary = isPrimary,
        };
    }
    
    public static ISectionSeparator Separator(bool isPrimary = true)
    {
        return new SectionSeparator
        {
            IsPrimary = isPrimary,
        };
    }
}