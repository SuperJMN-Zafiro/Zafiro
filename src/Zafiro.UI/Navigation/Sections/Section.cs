using System.Reactive.Linq;
using System.Windows.Input;

namespace Zafiro.UI.Navigation.Sections;

public class Section
{
    public bool IsPrimary { get; init; } = true;

    // Defaults for reactive visibility and sorting
    public IObservable<bool> IsVisible { get; init; } = Observable.Return(true);
    public IObservable<int> SortOrder { get; init; } = Observable.Return(0);

    public static IContentSection Content<T>(string name, IObservable<T> getViewModel, object? icon, bool isPrimary = true) where T : class
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