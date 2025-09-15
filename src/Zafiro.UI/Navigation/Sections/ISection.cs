namespace Zafiro.UI.Navigation.Sections;

public interface ISection
{
    bool IsPrimary { get; init; }
    IObservable<bool> IsVisible { get; init; }
    IObservable<int> SortOrder { get; init; }
}