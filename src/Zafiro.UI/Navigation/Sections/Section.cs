namespace Zafiro.UI.Navigation.Sections;

public class Section : ReactiveObject, ISection
{
    public bool IsPrimary { get; init; } = true;
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}