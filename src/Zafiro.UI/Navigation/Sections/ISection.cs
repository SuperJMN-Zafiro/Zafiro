namespace Zafiro.UI.Navigation.Sections;

public interface ISection
{
    bool IsPrimary { get; init; }
    bool IsVisible { get; set; }
    int SortOrder { get; set; }
}